namespace SharpScript.Subclassing;

/// <summary>
/// Engine-agnostic UENUM specifier bits supplied by the C# layer.
/// <br/> See: SharpScript/Private/Subclassing/SsEnumSpecifiers.h ESsEnumSpecifier
/// </summary>
[Flags]
public enum EnumSpecs : UInt64
{
	/// <summary>
	/// Exposes this enum as a type that can be used for variables in blueprints.
	/// </summary>
	BlueprintType = 1 << 0,
}

[AttributeUsage(AttributeTargets.Enum)]
// ReSharper disable once InconsistentNaming
public class UENUMAttribute : Attribute
{
	public UENUMAttribute(params EnumSpecs[] specifiers)
	{
		foreach (var specifier in specifiers)
		{
			Specifiers |= specifier;
		}
	}

	public EnumSpecs Specifiers { get; }

	/// <summary>
	/// The name to display for this enum instead of auto-generating it from the name.
	/// </summary>
	public string? DisplayName;

	/// <summary>
	/// Specifies the category of the enum when displayed in blueprint editing tools.
	/// </summary>
	public string? Category;

	/// <summary>
	/// Free-form metadata entries applied to the generated enum, each in the form "Key=Value"
	/// (a bare "Key" is treated as "Key=true").
	/// </summary>
	public string[]? Meta;
}
