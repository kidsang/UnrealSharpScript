#pragma once
#include "CoreMinimal.h"
#include "UObject/Class.h"
#include "SsGeneratedEnum.generated.h"

struct FSsEnumValueDef;

/**
 * An Unreal enum that was generated from a C# type.
 */
UCLASS()
class USsGeneratedEnum : public UEnum
{
	GENERATED_BODY()

public:
	/**
	 * Generate a new unreal enum from given infos.
	 * @param EnumName Name of the new enum.
	 * @param ValueDefines Array of enum value defines.
	 * @param ValueCount Count of enum value array.
	 * @param bIsFlags Whether the C# enum was declared with [Flags]; sets EEnumFlags::Flags on the generated UEnum.
	 * @return Newly generated enum if success, otherwise nullptr.
	 */
	static USsGeneratedEnum* GenerateEnum(const FName& EnumName, const FSsEnumValueDef* ValueDefines, int ValueCount, bool bIsFlags);
};
