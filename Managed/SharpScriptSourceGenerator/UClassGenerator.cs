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

	/// <summary>Fully-qualified display format with the <c>global::</c> prefix, used for the base-class name.</summary>
	private static readonly SymbolDisplayFormat FullyQualifiedFormat = new(
		globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
		typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

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
			// Fully-qualified (global::) so ClassDef.SuperClass resolves regardless of the base type's namespace
			// (e.g. a C++ binding base in UnrealEngine.<Module> vs the subclass in its own namespace).
			SuperClass = classSymbol.BaseType?.ToDisplayString(FullyQualifiedFormat) ?? "global::UnrealEngine.CoreUObject.UObject",
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
			IsOverride = method.IsOverride,
		};

		// Carry the [UFUNCTION] specifiers + metadata through to the model. The generator only transports
		// these values; the C++ layer (FSsFunctionSpecifiers) is solely responsible for expanding them.
		ParseFunctionAttribute(method, func);

		// Validate BlueprintEvent constraints up front (mirrors UHT's BlueprintNativeEvent checks, restricted
		// to specifiers this layer actually supports). On conflict we abort the whole function so no broken
		// dispatch/interceptor glue is emitted.
		if (func.IsBlueprintEvent && !ValidateBlueprintEvent(method, func, report))
		{
			return null;
		}

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
	/// Validates a BlueprintEvent method against the constraints this layer supports (a subset of UHT's
	/// BlueprintNativeEvent rules, limited to specifiers that are actually enabled):
	/// Reports one diagnostic per violation and returns <c>false</c> if any was found.
	/// </summary>
	private static bool ValidateBlueprintEvent(IMethodSymbol method, FunctionModel func, Action<Diagnostic> report)
	{
		bool valid = true;

		if (func.IsStatic)
		{
			report(Diagnostic.Create(
				Diagnostics.BlueprintEventCannotBeStatic,
				method.Locations.FirstOrDefault(),
				method.Name));
			valid = false;
		}

		if (func.SpecifierNames.Contains("Exec"))
		{
			report(Diagnostic.Create(
				Diagnostics.BlueprintEventConflictsWithExec,
				method.Locations.FirstOrDefault(),
				method.Name));
			valid = false;
		}

		// If a same-named base event glue exists but this method is not 'override', it silently hides the base
		// virtual glue (CS0108) and the generated Invoke_ would collide. Require 'override' (SS1010).
		if (!method.IsOverride && FindBaseEventGlueOwner(method) is { } baseOwner)
		{
			report(Diagnostic.Create(
				Diagnostics.BlueprintEventMustOverrideBase,
				method.Locations.FirstOrDefault(),
				method.Name,
				baseOwner));
			valid = false;
		}

		return valid;
	}

	/// <summary>
	/// Walks the base-type chain looking for an accessible, same-named method that carries the binding-glue
	/// marker <c>[BlueprintEventGlue]</c> (i.e. a C++ BlueprintEvent exposed via generated glue). Returns the
	/// declaring type's display name when found, otherwise null. Used to require <c>override</c> (SS1010).
	/// </summary>
	private static string? FindBaseEventGlueOwner(IMethodSymbol method)
	{
		for (INamedTypeSymbol? baseType = method.ContainingType.BaseType; baseType != null; baseType = baseType.BaseType)
		{
			foreach (ISymbol member in baseType.GetMembers(method.Name))
			{
				if (member is IMethodSymbol { MethodKind: MethodKind.Ordinary } baseMethod
					&& baseMethod.GetAttributes().Any(a => a.AttributeClass?.Name == "BlueprintEventGlueAttribute"))
				{
					return baseType.ToDisplayString();
				}
			}
		}
		return null;
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
		return AttributeParsing.ValidateMutuallyExclusive(
			model.SpecifierNames,
			"ClassSpecs",
			MutuallyExclusiveSpecifierGroups,
			joined => context.ReportDiagnostic(Diagnostic.Create(
				Diagnostics.MutuallyExclusiveSpecifiers,
				location,
				model.ClassName,
				joined)));
	}

	/// <summary>
	/// Reads the <c>[UCLASS]</c> attribute off the class symbol and copies its specifier bits and
	/// metadata (DisplayName / Category / Meta) into the model. Purely a transport step: the raw
	/// <c>ClassSpecs</c> bit set is OR-folded into <see cref="ClassModel.SpecifierNames"/> and the
	/// name/value metadata is collected verbatim; no bit is interpreted here. <c>Config</c> is handled
	/// specially — it maps to ClassConfigName, not metadata.
	/// </summary>
	private static void ParseClassAttribute(INamedTypeSymbol classSymbol, ClassModel model)
	{
		AttributeData? attr = classSymbol.GetAttributes().FirstOrDefault(
			a => a.AttributeClass?.ToDisplayString() == UClassAttributeFullName);
		if (attr == null)
		{
			return;
		}

		AttributeParsing.CollectSpecifierNames(attr, model.SpecifierNames);
		AttributeParsing.CollectMetadata(attr, model.Metadata);

		// Config is UCLASS-only and is NOT metadata: it maps to ClassConfigName on the C++ side.
		foreach (KeyValuePair<string, TypedConstant> named in attr.NamedArguments)
		{
			if (named.Key == "Config")
			{
				model.ConfigName = named.Value.Value as string;
			}
		}
	}

	/// <summary>
	/// Reads the <c>[UFUNCTION]</c> attribute off the method symbol and copies its specifier bits and
	/// metadata (DisplayName / Category / Meta) into the model. Purely a transport step: the raw
	/// <c>FuncSpecs</c> bit set is OR-folded into <see cref="FunctionModel.SpecifierNames"/> and the
	/// name/value metadata is collected verbatim; no bit is interpreted here (the C++ layer expands them).
	/// </summary>
	private static void ParseFunctionAttribute(IMethodSymbol method, FunctionModel func)
	{
		AttributeData? attr = method.GetAttributes().FirstOrDefault(
			a => a.AttributeClass?.Name == "UFUNCTIONAttribute");
		if (attr == null)
		{
			return;
		}

		AttributeParsing.CollectSpecifierNames(attr, func.SpecifierNames);
		AttributeParsing.CollectMetadata(attr, func.Metadata);

		// A BlueprintEvent needs the virtual-dispatch entry + call-site interception treatment
		// (the C++ layer expands the actual EFunctionFlags; here we only remember that it is an event).
		func.IsBlueprintEvent = func.SpecifierNames.Contains("BlueprintEvent");
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
