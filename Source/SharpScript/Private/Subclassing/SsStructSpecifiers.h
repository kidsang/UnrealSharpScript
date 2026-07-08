#pragma once
#include "CoreMinimal.h"

struct FSsMetaDataEntry;

/**
 * Engine-agnostic USTRUCT specifier bits supplied by the C# layer.
 * <br/> See: SharpScript/Framework/Subclassing/UStructAttribute.cs StructSpecs
 */
enum class ESsStructSpecifier : uint64
{
	/** Exposes this struct as a type that can be used for variables in blueprints. */
	BlueprintType = 1llu << 0,
};

ENUM_CLASS_FLAGS(ESsStructSpecifier);

/**
 * Expands the engine-agnostic USTRUCT specifier bits + metadata onto a freshly built generated struct.
 */
class FSsStructSpecifiers
{
public:
	/**
	 * Apply the given specifier bits and metadata entries to Struct.
	 * @param Struct The generated struct being built (its metadata is mutated in place).
	 * @param SpecifierBits The raw C# StructSpecs bit set to expand.
	 * @param MetaEntries Free-form metadata key/value entries to write verbatim (may be null).
	 * @param MetaCount Count of MetaEntries.
	 */
	static void Apply(UScriptStruct* Struct, uint64 SpecifierBits, const FSsMetaDataEntry* MetaEntries, int MetaCount);
};
