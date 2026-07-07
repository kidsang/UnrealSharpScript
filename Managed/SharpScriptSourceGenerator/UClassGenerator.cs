using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Incremental generator that turns C# classes annotated with <c>[UCLASS]</c> into UE-bound types.
/// </summary>
[Generator]
public sealed class UClassGenerator : IIncrementalGenerator
{
	private const string UClassAttributeFullName = "SharpScript.Subclassing.UCLASSAttribute";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValuesProvider<INamedTypeSymbol?> classDeclarations = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				UClassAttributeFullName,
				predicate: static (node, _) => node is ClassDeclarationSyntax,
				transform: static (ctx, _) => ctx.TargetSymbol as INamedTypeSymbol)
			.Where(static symbol => symbol is not null);

		context.RegisterSourceOutput(classDeclarations, static (spc, symbol) => Execute(spc, symbol!));
	}

	private static void Execute(SourceProductionContext context, INamedTypeSymbol classSymbol)
	{
		// Skip classes that already carry a hand-written binding partial. The reference
		// types (e.g. USsTestGenClassManual) ship a hand-authored *.generated.cs that
		// declares the static 'StaticClass' member; generating again would duplicate it.
		if (HasHandWrittenBinding(classSymbol))
		{
			return;
		}

		// Validate: must be partial.
		if (!SymbolUtils.IsPartial(classSymbol))
		{
			context.ReportDiagnostic(Diagnostic.Create(
				Diagnostics.ClassMustBePartial,
				classSymbol.Locations.FirstOrDefault(),
				classSymbol.Name));
			return;
		}

		// Validate: must derive from UObject.
		if (!SymbolUtils.IsUObjectDerived(classSymbol, includeSelf: false))
		{
			context.ReportDiagnostic(Diagnostic.Create(
				Diagnostics.ClassMustDeriveFromUObject,
				classSymbol.Locations.FirstOrDefault(),
				classSymbol.Name));
			return;
		}

		ClassModel model = new()
		{
			Namespace = classSymbol.ContainingNamespace.IsGlobalNamespace
				? ""
				: classSymbol.ContainingNamespace.ToDisplayString(),
			ClassName = classSymbol.Name,
			UnrealName = NamingUtils.StripTypePrefix(classSymbol.Name, 'U', 'A'),
			SuperClass = classSymbol.BaseType?.Name ?? "UObject",
		};

		// Carry the [UCLASS] specifiers + metadata through to the model. The generator only transports
		// these values; the C++ layer is solely responsible for interpreting/expanding them.
		ParseClassAttribute(classSymbol, model);

		// Validate mutually exclusive specifiers before proceeding.
		if (!ValidateClassSpecifiers(model, classSymbol.Locations.FirstOrDefault(), context))
		{
			return;
		}

		foreach (ISymbol member in classSymbol.GetMembers())
		{
			switch (member)
			{
				case IPropertySymbol propertySymbol when SymbolUtils.HasUPropertyAttribute(propertySymbol):
				{
					PropertyModel? prop = PropertyClassifier.Classify(propertySymbol, context.ReportDiagnostic);
					if (prop != null)
					{
						model.Properties.Add(prop);
					}
					break;
				}
				case IMethodSymbol { MethodKind: MethodKind.Ordinary } methodSymbol when SymbolUtils.HasUFunctionAttribute(methodSymbol):
				{
					FunctionModel? func = ClassifyFunction(methodSymbol, context.ReportDiagnostic);
					if (func != null)
					{
						model.Functions.Add(func);
					}
					break;
				}
			}
		}

		string source = ClassEmitter.Emit(model);
		context.AddSource(model.HintName, SourceText.From(source, Encoding.UTF8));
	}

	/// <summary>
	/// Classifies a <c>[UFUNCTION]</c> method into a <see cref="FunctionModel"/>. Each ref/out
	/// parameter becomes an out param, each by-value parameter an in param, and a non-void
	/// return type produces the synthetic "ReturnValue" param. A parameter (or return) whose
	/// type the subclassing path cannot express reports SS1001 and skips the whole function.
	/// </summary>
	private static FunctionModel? ClassifyFunction(IMethodSymbol method, Action<Diagnostic> report)
	{
		FunctionModel func = new()
		{
			Name = method.Name,
			IsStatic = method.IsStatic,
		};

		// Return value (if any) becomes the synthetic "ReturnValue" parameter.
		if (method.ReturnType.SpecialType != SpecialType.System_Void)
		{
			PropertyModel? retType = PropertyClassifier.ClassifyParam(method.ReturnType);
			if (retType == null)
			{
				report(Diagnostic.Create(
					Diagnostics.UnsupportedPropertyType,
					method.Locations.FirstOrDefault(),
					$"{method.Name} (return)",
					method.ReturnType.ToDisplayString()));
				return null;
			}
			func.ReturnParam = new FunctionParamModel
			{
				Name = "ReturnValue",
				Role = ParamRole.Return,
				Type = retType,
				DeclaredType = method.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
			};
		}

		foreach (IParameterSymbol param in method.Parameters)
		{
			PropertyModel? paramType = PropertyClassifier.ClassifyParam(param.Type);
			if (paramType == null)
			{
				report(Diagnostic.Create(
					Diagnostics.UnsupportedPropertyType,
					param.Locations.FirstOrDefault(),
					$"{method.Name}.{param.Name}",
					param.Type.ToDisplayString()));
				return null;
			}

			// 'out'/'ref' parameters are copied back after the call; everything else is input.
			ParamRole role = param.RefKind is RefKind.Out or RefKind.Ref ? ParamRole.Out : ParamRole.In;
			func.Parameters.Add(new FunctionParamModel
			{
				Name = param.Name,
				Role = role,
				Type = paramType,
				DeclaredType = param.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
			});
		}

		return func;
	}

	/// <summary>
	/// Groups of <c>ClassSpecs</c> members that are mutually exclusive: at most one member of each group may
	/// appear on a class. Applying two members of the same group is a programming error the C++ layer cannot
	/// resolve, so we surface it as SS1006 at compile time.
	/// </summary>
	private static readonly string[][] MutuallyExclusiveSpecifierGroups =
	[
		["BlueprintType", "NotBlueprintType"],
		["Blueprintable", "NotBlueprintable"],
		["Transient", "NonTransient"],
		["EditInlineNew", "NotEditInlineNew"],
		["DefaultConfig", "GlobalUserConfig", "ProjectUserConfig"]
	];

	/// <summary>
	/// Validates that the resolved <c>[UCLASS]</c> specifiers contain no mutually exclusive combinations
	/// (see <see cref="MutuallyExclusiveSpecifierGroups"/>). Reports one SS1006 diagnostic per offending group
	/// (listing every conflicting member present) at the class declaration <paramref name="location"/> and
	/// returns <c>false</c> if any conflict was found, so the caller can abort generation for this class.
	/// </summary>
	private static bool ValidateClassSpecifiers(ClassModel model, Location? location, SourceProductionContext context)
	{
		if (model.SpecifierNames.Count < 2)
		{
			return true;
		}

		// HashSet for O(1) membership checks; SpecifierNames holds the resolved single-bit member names.
		HashSet<string> present = new(model.SpecifierNames);
		bool valid = true;

		foreach (string[] group in MutuallyExclusiveSpecifierGroups)
		{
			// Collect every member of this exclusive group that the class actually declares.
			List<string> conflicting = new();
			foreach (string member in group)
			{
				if (present.Contains(member))
				{
					conflicting.Add(member);
				}
			}

			// Two or more members of the same group present => mutually exclusive violation.
			if (conflicting.Count >= 2)
			{
				string joined = string.Join(", ", conflicting.Select(name => $"ClassSpecs.{name}"));
				context.ReportDiagnostic(Diagnostic.Create(
					Diagnostics.MutuallyExclusiveSpecifiers,
					location,
					model.ClassName,
					joined));
				valid = false;
			}
		}

		return valid;
	}

	/// <summary>
	/// Reads the <c>[UCLASS]</c> attribute off the class symbol and copies its specifier bits and
	/// metadata (DisplayName / Category / Meta) into the model. Purely a transport step: the raw
	/// <c>ClassSpecs</c> bit set is OR-folded into <see cref="ClassModel.Specifiers"/> and the
	/// name/value metadata is collected verbatim; no bit is interpreted here.
	/// </summary>
	private static void ParseClassAttribute(INamedTypeSymbol classSymbol, ClassModel model)
	{
		AttributeData? attr = classSymbol.GetAttributes().FirstOrDefault(
			a => a.AttributeClass?.ToDisplayString() == UClassAttributeFullName);
		if (attr == null)
		{
			return;
		}

		// Constructor is UCLASSAttribute(params ClassSpecs[] specifiers): a single array-typed ctor arg.
		// Each element is a ClassSpecs enum constant; record its member name so we can emit
		// (ulong)(ClassSpecs.A | ClassSpecs.B) verbatim, matching the hand-written references.
		foreach (TypedConstant ctorArg in attr.ConstructorArguments)
		{
			if (ctorArg.Kind == TypedConstantKind.Array)
			{
				foreach (TypedConstant element in ctorArg.Values)
				{
					AddSpecifierName(model, element);
				}
			}
			else if (ctorArg.Kind == TypedConstantKind.Enum)
			{
				AddSpecifierName(model, ctorArg);
			}
		}

		// Named args: DisplayName / Category map to well-known metadata keys; Meta is free-form "Key=Value";
		// Config sets the ConfigName on the model directly (it is NOT metadata — it maps to ClassConfigName).
		foreach (KeyValuePair<string, TypedConstant> named in attr.NamedArguments)
		{
			switch (named.Key)
			{
				case "DisplayName":
					AddMetadataIfNonEmpty(model, "DisplayName", named.Value.Value as string);
					break;
				case "Category":
					AddMetadataIfNonEmpty(model, "Category", named.Value.Value as string);
					break;
				case "Config":
					model.ConfigName = named.Value.Value as string;
					break;
				case "Meta" when named.Value.Kind == TypedConstantKind.Array:
					foreach (TypedConstant entry in named.Value.Values)
					{
						AddMetaEntry(model, entry.Value as string);
					}
					break;
			}
		}
	}

	/// <summary>
	/// Resolves the enum member name(s) for a single ClassSpecs constant and records them on the model.
	/// Each attribute element is normally a single named flag; if the constant is a combined value we
	/// decompose it into the matching single-bit member names so the emitted expression stays readable.
	/// </summary>
	private static void AddSpecifierName(ClassModel model, TypedConstant specifier)
	{
		if (specifier.Type is not INamedTypeSymbol enumType || specifier.Value == null)
		{
			return;
		}

		ulong bits = Convert.ToUInt64(specifier.Value);
		if (bits == 0)
		{
			return;
		}

		// Walk the enum's declared members; collect the name of every single-bit member contained in
		// the value. This handles both the common single-flag case and any combined constant.
		foreach (ISymbol member in enumType.GetMembers())
		{
			if (member is not IFieldSymbol { HasConstantValue: true, ConstantValue: { } cv })
			{
				continue;
			}

			ulong memberBits = Convert.ToUInt64(cv);
			if (memberBits != 0 && (bits & memberBits) == memberBits)
			{
				if (!model.SpecifierNames.Contains(member.Name))
				{
					model.SpecifierNames.Add(member.Name);
				}
			}
		}
	}

	/// <summary>Adds a "Key=Value" (or bare "Key" =&gt; "true") metadata entry to the model, skipping blanks.</summary>
	private static void AddMetaEntry(ClassModel model, string? raw)
	{
		if (string.IsNullOrWhiteSpace(raw))
		{
			return;
		}

		int eq = raw!.IndexOf('=');
		if (eq < 0)
		{
			AddMetadataIfNonEmpty(model, raw.Trim(), "true");
		}
		else
		{
			string key = raw.Substring(0, eq).Trim();
			string value = raw.Substring(eq + 1);
			AddMetadataIfNonEmpty(model, key, value);
		}
	}

	/// <summary>Appends a metadata pair when the key is non-empty; a null value is normalized to "".</summary>
	private static void AddMetadataIfNonEmpty(ClassModel model, string key, string? value)
	{
		if (!string.IsNullOrEmpty(key))
		{
			model.Metadata.Add((key, value ?? ""));
		}
	}

	/// <summary>
	/// Detects whether the class already has a hand-written binding partial. Such a
	/// partial declares the static <c>StaticClass</c> member (see the *.generated.cs
	/// reference files). When present, the generator must not emit a second copy.
	/// </summary>
	private static bool HasHandWrittenBinding(INamedTypeSymbol classSymbol)
	{
		foreach (ISymbol member in classSymbol.GetMembers("StaticClass"))
		{
			if (member is IPropertySymbol { IsStatic: true })
			{
				return true;
			}
		}
		return false;
	}
}
