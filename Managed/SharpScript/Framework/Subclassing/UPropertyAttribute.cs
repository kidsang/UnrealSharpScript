namespace SharpScript.Subclassing;

/// <summary>
/// Engine-agnostic UPROPERTY specifier bits supplied by the C# layer.
/// <br/> See: SharpScript/Private/Subclassing/SsPropertySpecifiers.h ESsPropertySpecifier 
/// </summary>
[Flags]
public enum PropSpecs : UInt64
{
	// /// <summary>
	// /// This property is const and should be exported as const.
	// /// </summary>
	// Const = 1 << 0,

	/// <summary>
	/// Property should be loaded/saved to ini file as permanent profile.
	/// </summary>
	Config = 1 << 1,

	/// <summary>
	/// Same as above but load config from base class, not subclass.
	/// </summary>
	GlobalConfig = 1 << 2,

	/// <summary>
	/// Property is transient: shouldn't be saved, zero-filled at load time.
	/// </summary>
	Transient = 1 << 3,

	/// <summary>
	/// Property should always be reset to the default value during any type of duplication (copy/paste, binary duplication, etc.)
	/// </summary>
	DuplicateTransient = 1 << 4,

	// /// <summary>
	// /// Property should always be reset to the default value unless it's being duplicated for a PIE session - deprecated, use NonPIEDuplicateTransient instead
	// /// </summary>
	// NonPieTransient = 1 << 5,

	/// <summary>
	/// Property should always be reset to the default value unless it's being duplicated for a PIE session
	/// </summary>
	NonPieDuplicateTransient = 1 << 6,

	/// <summary>
	/// Object property can be exported with it's owner.
	Export = 1 << 7,

	/// <summary>
	/// Hide clear button in the editor.
	/// </summary>
	NoClear = 1 << 8,

	/// <summary>
	/// Indicates that elements of an array can be modified, but its size cannot be changed.
	/// </summary>
	EditFixedSize = 1 << 9,

	// /// <summary>
	// /// Property is relevant to network replication.
	// /// </summary>
	// Replicated = 1 << 10,

	// /// <summary>
	// /// Skip replication (only for struct members and parameters in service request functions).
	// /// </summary>
	// NotReplicated = 1 << 11,

	/// <summary>
	/// Interpolatable property for use with cinematics. Always user-settable in the editor.
	/// </summary>
	Interp = 1 << 12,

	/// <summary>
	/// Property isn't transacted.
	/// </summary>
	NonTransactional = 1 << 13,

	/// <summary>
	/// Property is a component reference. Implies EditInline and Export.
	/// </summary>
	Instanced = 1 << 14,

	// /// <summary>
	// ///MC Delegates only.  Property should be exposed for assigning in blueprints.
	// /// </summary>
	// BlueprintAssignable = 1 << 15,

	/// <summary>
	/// Properties appear visible by default in a details panel
	/// </summary>
	SimpleDisplay = 1 << 16,

	/// <summary>
	///  Properties are in the advanced dropdown in a details panel
	/// </summary>
	AdvancedDisplay = 1 << 17,

	/// <summary>
	/// Indicates that this property can be edited by property windows in the editor
	/// </summary>
	EditAnywhere = 1 << 18,

	/// <summary>
	/// Indicates that this property can be edited by property windows, but only on instances, not on archetypes
	/// </summary>
	EditInstanceOnly = 1 << 19,

	/// <summary>
	/// Indicates that this property can be edited by property windows, but only on archetypes
	/// </summary>
	EditDefaultsOnly = 1 << 20,

	/// <summary>
	/// Indicates that this property is visible in property windows, but cannot be edited at all
	/// </summary>
	VisibleAnywhere = 1 << 21,

	/// <summary>
	/// Indicates that this property is only visible in property windows for instances, not for archetypes, and cannot be edited
	/// </summary>
	VisibleInstanceOnly = 1 << 22,

	/// <summary>
	/// Indicates that this property is only visible in property windows for archetypes, and cannot be edited
	/// </summary>
	VisibleDefaultsOnly = 1 << 23,

	/// <summary>
	/// This property can be read by blueprints, but not modified.
	/// </summary>
	BlueprintReadOnly = 1 << 24,

	/// <summary>
	/// This property can be read or written from a blueprint.
	/// </summary>
	BlueprintReadWrite = 1 << 25,

	/// <summary>
	/// The AssetRegistrySearchable keyword indicates that this property and it's value will be automatically added
	/// to the asset registry for any asset class instances containing this as a member variable.  It is not legal
	/// to use on struct properties or parameters.
	/// </summary>
	AssetRegistrySearchable = 1 << 26,

	/// <summary>
	/// Property should be serialized for save games.
	/// This is only checked for game-specific archives with ArIsSaveGame set
	/// </summary>
	SaveGame = 1 << 27,

	// /// <summary>
	// /// MC Delegates only.  Property should be exposed for calling in blueprint code
	// /// </summary>
	// BlueprintCallable = 1 << 28,

	// /// <summary>
	// /// MC Delegates only. This delegate accepts (only in blueprint) only events with BlueprintAuthorityOnly.
	// /// </summary>
	// BlueprintAuthorityOnly = 1 << 29,

	/// <summary>
	/// Property shouldn't be exported to text format (e.g. copy/paste)
	/// </summary>
	TextExportTransient = 1 << 30,

	/// <summary>
	/// Property shouldn't be serialized, can still be exported to text
	/// </summary>
	SkipSerialization = 1ul << 31,
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
// ReSharper disable once InconsistentNaming
public class UPROPERTYAttribute : Attribute
{
	public UPROPERTYAttribute(params PropSpecs[] specifiers)
	{
		foreach (var specifier in specifiers)
		{
			Specifiers |= specifier;
		}
	}

	public PropSpecs Specifiers { get; }

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
