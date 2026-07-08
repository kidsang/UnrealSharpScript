using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Incremental generator that turns C# enums annotated with <c>[UENUM]</c> into UE-bound types.
/// It emits a static <c>&lt;Enum&gt;NativeRef</c> class that registers the enum with UE at first
/// use (<c>SubclassingUtils.GenerateEnum</c>), mirroring the hand-written
/// SsTestGenEnumManual.generated.cs. Byte-backed enum properties then reference
/// <c>&lt;Enum&gt;NativeRef.NativeType</c> as their underlying <c>UEnum</c>.
/// </summary>
[Generator]
public sealed class UEnumGenerator : IIncrementalGenerator
{
	private const string UEnumAttributeFullName = "SharpScript.Subclassing.UENUMAttribute";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValuesProvider<INamedTypeSymbol?> enumDeclarations = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				UEnumAttributeFullName,
				predicate: static (node, _) => node is EnumDeclarationSyntax,
				transform: static (ctx, _) => ctx.TargetSymbol as INamedTypeSymbol)
			.Where(static symbol => symbol is not null);

		context.RegisterSourceOutput(enumDeclarations, static (spc, symbol) => Execute(spc, symbol!));
	}

	private static void Execute(SourceProductionContext context, INamedTypeSymbol enumSymbol)
	{
		// Skip enums that already carry a hand-written binding. The reference enum
		// (ESsTestGenEnumManual) ships a hand-authored *.generated.cs that already declares
		// the companion "<Enum>NativeRef" type; generating again would duplicate it.
		if (HasHandWrittenBinding(enumSymbol))
		{
			return;
		}

		// Validate: the native subclassing path only supports byte-backed enums
		// (FByteProperty + UEnum). Reject other underlying types with a diagnostic.
		if (enumSymbol.EnumUnderlyingType?.SpecialType != SpecialType.System_Byte)
		{
			context.ReportDiagnostic(Diagnostic.Create(
				Diagnostics.EnumMustBeByteBacked,
				enumSymbol.Locations.FirstOrDefault(),
				enumSymbol.Name));
			return;
		}

		EnumModel model = new()
		{
			Namespace = enumSymbol.ContainingNamespace.IsGlobalNamespace
				? ""
				: enumSymbol.ContainingNamespace.ToDisplayString(),
			EnumName = enumSymbol.Name,
			UnrealName = NamingUtils.StripTypePrefix(enumSymbol.Name, 'E'),
			IsFlags = HasFlagsAttribute(enumSymbol),
		};

		// Carry the [UENUM] specifiers + metadata through to the model. The generator only transports
		// these values; the C++ layer is solely responsible for interpreting/expanding them.
		ParseEnumAttribute(enumSymbol, model);

		foreach (ISymbol member in enumSymbol.GetMembers())
		{
			if (member is IFieldSymbol { IsStatic: true, ConstantValue: not null } fieldSymbol)
			{
				model.Values.Add(new EnumValueModel { Name = fieldSymbol.Name });
			}
		}

		string source = EnumEmitter.Emit(model);
		context.AddSource(model.HintName, SourceText.From(source, Encoding.UTF8));
	}

	/// <summary>
	/// Reads the <c>[UENUM]</c> attribute off the enum symbol and copies its specifier bits and
	/// metadata (DisplayName / Category / Meta) into the model. Purely a transport step: the raw
	/// <c>EnumSpecs</c> bit set is OR-folded into <see cref="EnumModel.SpecifierNames"/> and the
	/// name/value metadata is collected verbatim; no bit is interpreted here.
	/// </summary>
	private static void ParseEnumAttribute(INamedTypeSymbol enumSymbol, EnumModel model)
	{
		AttributeData? attr = enumSymbol.GetAttributes().FirstOrDefault(
			a => a.AttributeClass?.ToDisplayString() == UEnumAttributeFullName);
		if (attr == null)
		{
			return;
		}

		AttributeParsing.CollectSpecifierNames(attr, model.SpecifierNames);
		AttributeParsing.CollectMetadata(attr, model.Metadata);
	}

	/// <summary>
	/// True when the enum carries the standard <c>[System.Flags]</c> attribute. Such an enum
	/// is generated as a UE bitmask enum (EEnumFlags::Flags).
	/// </summary>
	private static bool HasFlagsAttribute(INamedTypeSymbol enumSymbol)
	{
		foreach (AttributeData attr in enumSymbol.GetAttributes())
		{
			if (attr.AttributeClass is { Name: "FlagsAttribute", ContainingNamespace.Name: "System" })
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Detects whether the enum already has a hand-written binding. The hand-authored
	/// reference *.generated.cs files declare the companion "&lt;Enum&gt;NativeRef" type in
	/// the same namespace; if it already exists, the generator must not emit a second copy.
	/// </summary>
	private static bool HasHandWrittenBinding(INamedTypeSymbol enumSymbol)
	{
		string nativeRefName = $"{enumSymbol.Name}NativeRef";
		INamespaceSymbol ns = enumSymbol.ContainingNamespace;
		return ns.GetTypeMembers(nativeRefName).Any();
	}
}
