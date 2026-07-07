namespace SharpScript.Subclassing;

[Flags]
public enum ClassSpecs : UInt64
{
	/// <summary>
	/// Exposes this class as a type that can be used for variables in blueprints. This is inherited by subclasses unless overridden.
	/// </summary>
	BlueprintType = 1 << 0,

	/// <summary>
	/// Prevents this class from being used for variables in blueprints. This is inherited by subclasses unless overridden.
	/// </summary>
	NotBlueprintType = 1 << 1,

	/// <summary>
	/// Exposes this class as an acceptable base class for creating blueprints. The default is NotBlueprintable, unless inherited otherwise. This is inherited by subclasses.
	/// </summary>
	Blueprintable = 1 << 2,

	/// <summary>
	/// Specifies that this class is *NOT* an acceptable base class for creating blueprints. The default is NotBlueprintable, unless inherited otherwise. This is inherited by subclasses.
	/// </summary>
	NotBlueprintable = 1 << 3,

	/// <summary>
	/// This class cannot be placed in the editor (it cancels out an inherited placeable flag).
	/// </summary>
	NotPlaceable = 1 << 4,

	/// <summary>
	/// All instances of this class are considered "instanced". Instanced classes (components) are duplicated upon construction. This flag is inherited by subclasses.
	/// </summary>
	DefaultToInstanced = 1 << 5,

	/// <summary>
	/// All properties and functions in this class are const and should be exported as const. This flag is inherited by subclasses.
	/// </summary>
	Const = 1 << 6,

	/// <summary>
	/// Class is abstract and can't be instantiated directly.
	/// </summary>
	Abstract = 1 << 7,

	/// <summary>
	/// This class is deprecated and objects of this class won't be saved when serializing. This flag is inherited by subclasses.
	/// </summary>
	Deprecated = 1 << 8,

	/// <summary>
	/// This class can't be saved; null it out at save time. This flag is inherited by subclasses.
	/// </summary>
	Transient = 1 << 9,

	/// <summary>
	/// This class should be saved normally (it cancels out an inherited transient flag).
	/// </summary>
	NonTransient = 1 << 10,

	/// <summary>
	/// Handle object configuration on a per-object basis, rather than per-class.
	/// </summary>
	PerObjectConfig = 1 << 11,

	/// <summary>
	/// Save object configuration only to Default INIs, never to local INIs. Must be combined with "Config"
	/// </summary>
	DefaultConfig = 1 << 12,

	/// <summary>
	/// Class settings are saved to [AppData]/..../Blah.ini (as opposed to "DefaultConfig")
	/// </summary>
	GlobalUserConfig = 1 << 13,

	/// <summary>
	/// Indicates that the config settings for this class will be saved to Project/User*.ini (similar to "GlobalUserConfig")
	/// </summary>
	ProjectUserConfig = 1 << 14,

	/// <summary>
	/// Indicates that object configuration will not check against ini base/defaults when serialized
	/// </summary>
	ConfigDoNotCheckDefaults = 1 << 15,

	/// <summary>
	/// Class can be constructed from editinline New button.
	/// </summary>
	EditInlineNew = 1 << 16,

	/// <summary>
	/// Class can't be constructed from editinline New button.
	/// </summary>
	NotEditInlineNew = 1 << 17,

	/// <summary>
	/// Class not shown in editor drop down for class selection.
	/// </summary>
	HideDropdown = 1 << 18,
}

[AttributeUsage(AttributeTargets.Class)]
// ReSharper disable once InconsistentNaming
public class UCLASSAttribute : Attribute
{
	public UCLASSAttribute(params ClassSpecs[] specifiers)
	{
		foreach (var specifier in specifiers)
		{
			Specifiers |= specifier;
		}
	}

	public ClassSpecs Specifiers { get; }

	/// <summary>
	/// The name to display for this class, property, or function instead of auto-generating it from the name.
	/// </summary>
	public string? DisplayName;

	/// <summary>
	/// Specifies the category of the function when displayed in blueprint editing tools.
	/// </summary>
	public string? Category;

	/// <summary>
	/// Load object configuration at construction time.
	/// </summary>
	public string? Config;

	/// <summary>
	/// Free-form metadata entries applied to the generated class, each in the form "Key=Value"
	/// (a bare "Key" is treated as "Key=true").
	/// </summary>
	public string[]? Meta;
}