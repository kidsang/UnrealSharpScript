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

	/// <summary>
	/// A [UENUM] used an underlying type other than <c>byte</c>. The native subclassing
	/// path only supports byte-backed enums (FByteProperty + UEnum).
	/// </summary>
	public static readonly DiagnosticDescriptor EnumMustBeByteBacked = new(
		id: "SS1005",
		title: "UENUM must be byte-backed",
		messageFormat: "UENUM '{0}' must have an underlying type of 'byte' to be bound by the SharpScript generator",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	/// <summary>
	/// A [UCLASS] declares mutually exclusive class specifiers (e.g. Transient and NonTransient).
	/// These cannot be applied together on the same class.
	/// </summary>
	public static readonly DiagnosticDescriptor MutuallyExclusiveSpecifiers = new(
		id: "SS1006",
		title: "Mutually exclusive class specifiers",
		messageFormat: "UCLASS '{0}' has mutually exclusive specifiers: {1}. Only one may be used at a time.",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	/// <summary>
	/// A [UPROPERTY] declares mutually exclusive property specifiers, e.g. more than one
	/// edit/visibility specifier (EditAnywhere / VisibleAnywhere / ...) or both BlueprintReadOnly
	/// and BlueprintReadWrite. These cannot be applied together on the same property (mirrors UHT).
	/// </summary>
	public static readonly DiagnosticDescriptor MutuallyExclusivePropertySpecifiers = new(
		id: "SS1007",
		title: "Mutually exclusive property specifiers",
		messageFormat: "UPROPERTY '{0}' has mutually exclusive specifiers: {1}. Only one may be used at a time.",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	/// <summary>
	/// A <c>[UFUNCTION(FuncSpecs.BlueprintEvent)]</c> method was declared <c>static</c>. A BlueprintEvent is
	/// dispatched virtually on an object instance (its source calls are intercepted and forwarded to an
	/// instance <c>Invoke_&lt;Name&gt;</c> entry that runs ProcessEvent), so it cannot be static.
	/// </summary>
	public static readonly DiagnosticDescriptor BlueprintEventCannotBeStatic = new(
		id: "SS1008",
		title: "BlueprintEvent cannot be static",
		messageFormat: "UFUNCTION '{0}' is a BlueprintEvent and cannot be static; declare it as an instance method",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	/// <summary>
	/// A <c>[UFUNCTION]</c> combined <c>BlueprintEvent</c> with <c>Exec</c>. Mirrors UHT, where an
	/// executable-console function and a blueprint-overridable event are mutually exclusive roles.
	/// </summary>
	public static readonly DiagnosticDescriptor BlueprintEventConflictsWithExec = new(
		id: "SS1009",
		title: "BlueprintEvent conflicts with Exec",
		messageFormat: "UFUNCTION '{0}' cannot be both BlueprintEvent and Exec",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	/// <summary>
	/// A <c>[UFUNCTION(FuncSpecs.BlueprintEvent)]</c> method has the same name as an accessible base-class
	/// event (e.g. a C++ <c>BlueprintEvent</c> exposed via binding glue) but was not declared <c>override</c>.
	/// Without <c>override</c> the method silently hides the base virtual glue (CS0108): the CLR keeps the base
	/// body, and the generated <c>Invoke_&lt;Name&gt;</c> would collide. Declaring it <c>override</c> is required
	/// so the runtime duplicate-super path and virtual dispatch work.
	/// </summary>
	public static readonly DiagnosticDescriptor BlueprintEventMustOverrideBase = new(
		id: "SS1010",
		title: "BlueprintEvent overriding a base event must use 'override'",
		messageFormat: "UFUNCTION '{0}' is a BlueprintEvent that hides base event '{1}.{0}'; declare it 'override' to correctly override the base",
		category: Category,
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);
}
