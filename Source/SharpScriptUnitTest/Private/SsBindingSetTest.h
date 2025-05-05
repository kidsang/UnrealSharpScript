#pragma once
#include "CoreMinimal.h"
#include "SsBindingTestBlittableStruct.h"
#include "SsBindingSetTest.generated.h"

/** A struct for testing various types of sets */
USTRUCT(BlueprintType)
struct FSsBindingSetTestStruct
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TSet<int> IntSet;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TSet<FSsBindingTestBlittableStruct> StructSet;
};
