#pragma once
#include "CoreMinimal.h"
#include "SsBindingTestBlittableStruct.generated.h"

/**
 * Test exporting Blittable type struct directly as C# ref struct.
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
