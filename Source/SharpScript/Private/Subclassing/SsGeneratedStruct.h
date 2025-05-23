#pragma once
#include "CoreMinimal.h"
#include "Engine/UserDefinedStruct.h"
#include "SsGeneratedStruct.generated.h"

struct FSsPropertyDef;

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
	 * @param StructName Name of the new struct.
	 * @param PropertyDefines Array of property defines.
	 * @param PropertyCount Count of property array.
	 * @return Newly generated struct if success, otherwise nullptr.
	 */
	static USsGeneratedStruct* GenerateStruct(const FName& StructName, const FSsPropertyDef* PropertyDefines, int PropertyCount);

	/** Called by struct builder, generate default instance of this struct. */
	void GenerateDefaultInstance();
};
