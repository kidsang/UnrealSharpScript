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
			if (member is not IPropertySymbol propertySymbol)
			{
				continue;
			}

			if (!SymbolUtils.HasUPropertyAttribute(propertySymbol))
			{
				continue;
			}

			PropertyModel? prop = PropertyClassifier.Classify(propertySymbol, context.ReportDiagnostic);
			if (prop != null)
			{
				model.Properties.Add(prop);
			}
		}

		string source = ClassEmitter.Emit(model);
		context.AddSource(model.HintName, SourceText.From(source, Encoding.UTF8));
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
