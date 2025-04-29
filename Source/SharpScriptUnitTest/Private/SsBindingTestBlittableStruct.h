#pragma once
#include "CoreMinimal.h"
#include "SsBindingTestBlittableStruct.generated.h"

/**
 * 测试Blittable类型结构体直接导出为C# ref struct。
 */
USTRUCT(BlueprintType)
struct FSsBindingTestBlittableStruct
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	int X;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	int Y;
};

inline uint32 GetTypeHash(const FSsBindingTestBlittableStruct& value)
{
	return HashCombine(GetTypeHash(value.X), GetTypeHash(value.Y));
}
