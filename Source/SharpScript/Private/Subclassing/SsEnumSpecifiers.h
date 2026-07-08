#pragma once
#include "CoreMinimal.h"

struct FSsMetaDataEntry;

/**
 * Engine-agnostic UENUM specifier bits supplied by the C# layer.
 * <br/> See: SharpScript/Framework/Subclassing/UEnumAttribute.cs EnumSpecs
 */
enum class ESsEnumSpecifier : uint64
{
	/** Exposes this enum as a type that can be used for variables in blueprints. */
	BlueprintType = 1llu << 0,
};

ENUM_CLASS_FLAGS(ESsEnumSpecifier);

/**
 * Expands the engine-agnostic UENUM specifier bits + metadata onto a freshly built generated enum.
 */
class FSsEnumSpecifiers
{
public:
	/**
	 * Apply the given specifier bits and metadata entries to Enum.
	 * @param Enum The generated enum being built (its metadata is mutated in place).
	 * @param SpecifierBits The raw C# EnumSpecs bit set to expand.
	 * @param MetaEntries Free-form metadata key/value entries to write verbatim (may be null).
	 * @param MetaCount Count of MetaEntries.
	 */
	static void Apply(UEnum* Enum, uint64 SpecifierBits, const FSsMetaDataEntry* MetaEntries, int MetaCount);
};
