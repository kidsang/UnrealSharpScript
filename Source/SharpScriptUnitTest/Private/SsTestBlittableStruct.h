#pragma once
#include "CoreMinimal.h"
#include "SsTestBlittableStruct.generated.h"

/**
 * Test exporting a Blittable type struct directly as a C# ref struct.
 */
USTRUCT(BlueprintType)
struct FSsTestBlittableStruct
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	int X = 0;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	int Y = 0;
};

inline uint32 GetTypeHash(const FSsTestBlittableStruct& value)
{
	return HashCombine(GetTypeHash(value.X), GetTypeHash(value.Y));
}
