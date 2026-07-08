#pragma once
#include "CoreMinimal.h"
#include "Engine/UserDefinedStruct.h"
#include "SsGeneratedStruct.generated.h"

struct FSsPropertyDef;
struct FSsStructDef;

/**
 * An Unreal struct that was generated from a C# type.
 */
UCLASS()
class USsGeneratedStruct : public UUserDefinedStruct
{
	GENERATED_BODY()

public:
	/**
	 * Generate a new unreal struct from given infos.
	 * @param StructDef The struct definition bundle (name, properties, specifiers, metadata).
	 * @return Newly generated struct if success, otherwise nullptr.
	 */
	static USsGeneratedStruct* GenerateStruct(const FSsStructDef& StructDef);

	/** Called by struct builder, generate default instance of this struct. */
	void GenerateDefaultInstance();
};
