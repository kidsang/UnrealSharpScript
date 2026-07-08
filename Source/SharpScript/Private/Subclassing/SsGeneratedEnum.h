#pragma once
#include "CoreMinimal.h"
#include "UObject/Class.h"
#include "SsGeneratedEnum.generated.h"

struct FSsEnumValueDef;
struct FSsEnumDef;

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
	 * @param EnumDef The enum definition bundle (name, values, flags, specifiers, metadata).
	 * @return Newly generated enum if success, otherwise nullptr.
	 */
	static USsGeneratedEnum* GenerateEnum(const FSsEnumDef& EnumDef);
};
