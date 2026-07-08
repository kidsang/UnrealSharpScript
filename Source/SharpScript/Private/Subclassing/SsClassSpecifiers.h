#pragma once
#include "CoreMinimal.h"

struct FSsMetaDataEntry;

/**
 * Engine-agnostic UCLASS specifier bits supplied by the C# layer.
 * <br/> See: SharpScript/Framework/Subclassing/UClassAttribute.cs ClassSpecs
 */
enum class ESsClassSpecifier : uint64
{
	/** Exposes this class as a type that can be used for variables in blueprints. */
	BlueprintType = 1llu << 0,

	/** Prevents this class from being used for variables in blueprints. */
	NotBlueprintType = 1llu << 1,

	/** Exposes this class as an acceptable base class for creating blueprints. */
	Blueprintable = 1llu << 2,

	/** Specifies that this class is *NOT* an acceptable base class for creating blueprints. */
	NotBlueprintable = 1llu << 3,

	/** This class cannot be placed in the editor. */
	NotPlaceable = 1llu << 4,

	/** All instances of this class are considered "instanced". */
	DefaultToInstanced = 1llu << 5,

	/** All properties and functions in this class are const. */
	Const = 1llu << 6,

	/** Class is abstract and can't be instantiated directly. */
	Abstract = 1llu << 7,

	/** This class is deprecated and objects of this class won't be saved when serializing. */
	Deprecated = 1llu << 8,

	/** This class can't be saved; null it out at save time. */
	Transient = 1llu << 9,

	/** This class should be saved normally (it cancels out an inherited transient flag). */
	NonTransient = 1llu << 10,

	/** Handle object configuration on a per-object basis, rather than per-class. */
	PerObjectConfig = 1llu << 11,

	/** Save object configuration only to Default INIs, never to local INIs. Must be combined with Config. */
	DefaultConfig = 1llu << 12,

	/** Class settings are saved to [AppData]/..../Blah.ini (as opposed to DefaultConfig). */
	GlobalUserConfig = 1llu << 13,

	/** Indicates that the config settings for this class will be saved to Project/User*.ini. */
	ProjectUserConfig = 1llu << 14,

	/** Indicates that object configuration will not check against ini base/defaults when serialized. */
	ConfigDoNotCheckDefaults = 1llu << 15,

	/** Class can be constructed from the editinline New button. */
	EditInlineNew = 1llu << 16,

	/** Class can't be constructed from the editinline New button. */
	NotEditInlineNew = 1llu << 17,

	/** Class not shown in editor drop down for class selection. */
	HideDropdown = 1llu << 18,
};

ENUM_CLASS_FLAGS(ESsClassSpecifier);

/**
 * Expands the engine-agnostic UCLASS specifier bits + metadata onto a freshly built generated class.
 */
class FSsClassSpecifiers
{
public:
	/**
	 * Apply the given specifier bits and metadata entries to Class.
	 * @param Class The generated class being built (its ClassFlags/metadata are mutated in place).
	 * @param SpecifierBits The raw C# ClassSpecs bit set to expand.
	 * @param MetaEntries Free-form metadata key/value entries to write verbatim (may be null).
	 * @param MetaCount Count of MetaEntries.
	 */
	static void Apply(UClass* Class, uint64 SpecifierBits, const FSsMetaDataEntry* MetaEntries, int MetaCount);
};
