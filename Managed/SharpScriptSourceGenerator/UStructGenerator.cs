using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Incremental generator that turns C# structs annotated with <c>[USTRUCT]</c> into UE-bound types.
/// </summary>
[Generator]
public sealed class UStructGenerator : IIncrementalGenerator
{
	private const string UStructAttributeFullName = "SharpScript.Subclassing.USTRUCTAttribute";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValuesProvider<INamedTypeSymbol?> structDeclarations = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				UStructAttributeFullName,
				predicate: static (node, _) => node is StructDeclarationSyntax,
				transform: static (ctx, _) => ctx.TargetSymbol as INamedTypeSymbol)
			.Where(static symbol => symbol is not null);

		context.RegisterSourceOutput(structDeclarations, static (spc, symbol) => Execute(spc, symbol!));
	}

	private static void Execute(SourceProductionContext context, INamedTypeSymbol structSymbol)
	{
		// Skip structs that already carry a hand-written binding. The reference types
		// (e.g. FSsTestGenStructManual, FSsArrayTestInnerGenStructManual) ship a
		// hand-authored *.generated.cs that already declares the companion
		// "<Struct>NativeRef" type; generating again would duplicate it. Checked before
		// the partial validation so existing non-partial reference structs are not flagged.
		if (HasHandWrittenBinding(structSymbol))
		{
			return;
		}

		// Validate: must be partial (the generator emits a partial struct half).
		if (!SymbolUtils.IsPartial(structSymbol))
		{
			context.ReportDiagnostic(Diagnostic.Create(
				Diagnostics.StructMustBePartial,
				structSymbol.Locations.FirstOrDefault(),
				structSymbol.Name));
			return;
		}

		StructModel model = new()
		{
			Namespace = structSymbol.ContainingNamespace.IsGlobalNamespace
				? ""
				: structSymbol.ContainingNamespace.ToDisplayString(),
			StructName = structSymbol.Name,
			UnrealName = NamingUtils.StripTypePrefix(structSymbol.Name, 'F'),
		};

		foreach (ISymbol member in structSymbol.GetMembers())
		{
			if (member is not IFieldSymbol fieldSymbol || fieldSymbol.IsStatic || fieldSymbol.IsImplicitlyDeclared)
			{
				continue;
			}

			if (!SymbolUtils.HasUPropertyAttribute(fieldSymbol))
			{
				continue;
			}

			PropertyModel? prop = PropertyClassifier.ClassifyField(fieldSymbol, context.ReportDiagnostic);
			if (prop != null)
			{
				model.Properties.Add(prop);
			}
		}

		// A struct is blittable only when every bound field is a single blittable value.
		model.IsBlittable = model.Properties.All(p => p.IsBlittable);

		string source = StructEmitter.Emit(model);
		context.AddSource(model.HintName, SourceText.From(source, Encoding.UTF8));
	}

	/// <summary>
	/// Detects whether the struct already has a hand-written binding. The hand-authored
	/// reference *.generated.cs files declare the companion "&lt;Struct&gt;NativeRef" type in
	/// the same namespace; if it already exists, the generator must not emit a second copy.
	/// </summary>
	private static bool HasHandWrittenBinding(INamedTypeSymbol structSymbol)
	{
		string nativeRefName = $"{structSymbol.Name}NativeRef";
		INamespaceSymbol ns = structSymbol.ContainingNamespace;
		return ns.GetTypeMembers(nativeRefName).Any();
	}
}
