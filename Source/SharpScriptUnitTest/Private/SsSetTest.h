#pragma once
#include "CoreMinimal.h"
#include "SsTestBlittableStruct.h"
#include "SsSetTest.generated.h"

/** 用于测试各种类型集合的结构体 */
USTRUCT(BlueprintType)
struct FSsSetTestStruct
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TSet<int> IntSet;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TSet<FSsTestBlittableStruct> StructSet;
};
