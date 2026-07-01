using Microsoft.CodeAnalysis;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Diagnostic descriptors emitted by the SharpScript source generator.
/// </summary>
internal static class Diagnostics
{
	private const string Category = "SharpScript";

	/// <summary>
	/// A [UPROPERTY] used a C# type the generator does not yet know how to bind.
	/// The property is skipped; the rest of the class still generates.
	/// </summary>
	public static readonly DiagnosticDescriptor UnsupportedPropertyType = new(
		id: "SS1001",
		title: "Unsupported UPROPERTY type",
		messageFormat: "UPROPERTY '{0}' has unsupported type '{1}'; it will be skipped by the SharpScript generator",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true);

	/// <summary>
	/// A [UCLASS] does not derive from UObject.
	/// </summary>
	public static readonly DiagnosticDescriptor ClassMustDeriveFromUObject = new(
		id: "SS1002",
		title: "UCLASS must derive from UObject",
		messageFormat: "UCLASS '{0}' must derive from UObject (directly or indirectly)",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	/// <summary>
	/// A [UCLASS] (or its containing types) was not declared 'partial'.
	/// </summary>
	public static readonly DiagnosticDescriptor ClassMustBePartial = new(
		id: "SS1003",
		title: "UCLASS must be partial",
		messageFormat: "UCLASS '{0}' must be declared 'partial' so the generator can extend it",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	/// <summary>
	/// A [USTRUCT] was not declared 'partial'.
	/// </summary>
	public static readonly DiagnosticDescriptor StructMustBePartial = new(
		id: "SS1004",
		title: "USTRUCT must be partial",
		messageFormat: "USTRUCT '{0}' must be declared 'partial' so the generator can extend it",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);
}
