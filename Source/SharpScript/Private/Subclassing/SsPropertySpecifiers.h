#pragma once
#include "CoreMinimal.h"

struct FSsMetaDataEntry;

/**
 * Engine-agnostic UPROPERTY specifier bits supplied by the C# layer.
 * <br/> See: SharpScript/Framework/Subclassing/UPropertyAttribute.cs PropSpecs
 */
enum class ESsPropertySpecifier : uint64
{
	// /** This property is const and should be exported as const. */
	// Const = 1llu << 0,

	/** Property should be loaded/saved to ini file as permanent profile. */
	Config = 1llu << 1,

	/** Same as Config but load config from base class, not subclass. */
	GlobalConfig = 1llu << 2,

	/** Property is transient: shouldn't be saved, zero-filled at load time. */
	Transient = 1llu << 3,

	/** Property should always be reset to the default value during any type of duplication. */
	DuplicateTransient = 1llu << 4,

	// /** Deprecated - use NonPieDuplicateTransient instead. Reset unless duplicated for a PIE session. */
	// NonPieTransient = 1llu << 5,

	/** Property should always be reset to the default value unless duplicated for a PIE session. */
	NonPieDuplicateTransient = 1llu << 6,

	/** Object property can be exported with its owner. */
	Export = 1llu << 7,

	/** Hide clear button in the editor. */
	NoClear = 1llu << 8,

	/** Elements of an array can be modified, but its size cannot be changed. */
	EditFixedSize = 1llu << 9,

	// /** Property is relevant to network replication. */
	// Replicated = 1llu << 10,

	// /** Skip replication (only for struct members and parameters in service request functions). */
	// NotReplicated = 1llu << 11,

	/** Interpolatable property for use with cinematics. Always user-settable in the editor. */
	Interp = 1llu << 12,

	/** Property isn't transacted. */
	NonTransactional = 1llu << 13,

	/** Property is a component reference. Implies EditInline and Export. */
	Instanced = 1llu << 14,

	// /** MC Delegates only.  Property should be exposed for assigning in blueprints. */
	// BlueprintAssignable = 1llu << 15,

	/** Properties appear visible by default in a details panel. */
	SimpleDisplay = 1llu << 16,

	/** Properties are in the advanced dropdown in a details panel. */
	AdvancedDisplay = 1llu << 17,

	/** This property can be edited by property windows in the editor. */
	EditAnywhere = 1llu << 18,

	/** Editable by property windows, but only on instances, not on archetypes. */
	EditInstanceOnly = 1llu << 19,

	/** Editable by property windows, but only on archetypes. */
	EditDefaultsOnly = 1llu << 20,

	/** Visible in property windows, but cannot be edited at all. */
	VisibleAnywhere = 1llu << 21,

	/** Visible in property windows for instances only, and cannot be edited. */
	VisibleInstanceOnly = 1llu << 22,

	/** Visible in property windows for archetypes only, and cannot be edited. */
	VisibleDefaultsOnly = 1llu << 23,

	/** This property can be read by blueprints, but not modified. */
	BlueprintReadOnly = 1llu << 24,

	/** This property can be read or written from a blueprint. */
	BlueprintReadWrite = 1llu << 25,

	/** Automatically added to the asset registry for asset class instances containing this member. */
	AssetRegistrySearchable = 1llu << 26,

	/** Property should be serialized for save games (only for archives with ArIsSaveGame set). */
	SaveGame = 1llu << 27,

	// /** MC Delegates only.  Property should be exposed for calling in blueprint code */
	// BlueprintCallable = 1 << 28,

	// /** MC Delegates only. This delegate accepts (only in blueprint) only events with BlueprintAuthorityOnly. */
	// BlueprintAuthorityOnly = 1 << 29,

	/** Property shouldn't be exported to text format (e.g. copy/paste). */
	TextExportTransient = 1llu << 30,

	/** Property shouldn't be serialized, can still be exported to text. */
	SkipSerialization = 1llu << 31,
};

ENUM_CLASS_FLAGS(ESsPropertySpecifier);

/**
 * Expands the engine-agnostic UPROPERTY specifier bits + metadata onto a freshly created property.
 */
class FSsPropertySpecifiers
{
public:
	/**
	 * Apply the given specifier bits and metadata entries to Prop.
	 * @param Prop The generated property being built (its PropertyFlags/metadata are mutated in place).
	 * @param SpecifierBits The raw C# PropSpecs bit set to expand.
	 * @param MetaEntries Free-form metadata key/value entries to write verbatim (may be null).
	 * @param MetaCount Count of MetaEntries.
	 */
	static void Apply(FProperty* Prop, uint64 SpecifierBits, const FSsMetaDataEntry* MetaEntries, int MetaCount);
};
