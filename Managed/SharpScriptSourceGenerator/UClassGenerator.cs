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
