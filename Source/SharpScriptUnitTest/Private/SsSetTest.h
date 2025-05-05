#pragma once
#include "CoreMinimal.h"
#include "SsTestBlittableStruct.h"
#include "SsSetTest.generated.h"

/** A struct for testing various types of sets */
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
