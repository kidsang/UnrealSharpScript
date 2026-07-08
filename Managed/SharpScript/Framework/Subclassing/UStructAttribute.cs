namespace SharpScript.Subclassing;

/// <summary>
/// Engine-agnostic USTRUCT specifier bits supplied by the C# layer.
/// <br/> See: SharpScript/Private/Subclassing/SsStructSpecifiers.h ESsStructSpecifier 
/// </summary>
[Flags]
public enum StructSpecs : UInt64
{
	/// <summary>
	/// Exposes this struct as a type that can be used for variables in blueprints
	/// </summary>
	BlueprintType = 1 << 0,
}

[AttributeUsage(AttributeTargets.Struct)]
// ReSharper disable once InconsistentNaming
public class USTRUCTAttribute : Attribute
{
	public USTRUCTAttribute(params StructSpecs[] specifiers)
	{
		foreach (var specifier in specifiers)
		{
			Specifiers |= specifier;
		}
	}

	public StructSpecs Specifiers { get; }

	/// <summary>
	/// The name to display for this class, property, or function instead of auto-generating it from the name.
	/// </summary>
	public string? DisplayName;

	/// <summary>
	/// Specifies the category of the function when displayed in blueprint editing tools.
	/// </summary>
	public string? Category;

	/// <summary>
	/// Free-form metadata entries applied to the generated class, each in the form "Key=Value"
	/// (a bare "Key" is treated as "Key=true").
	/// </summary>
	public string[]? Meta;
}
