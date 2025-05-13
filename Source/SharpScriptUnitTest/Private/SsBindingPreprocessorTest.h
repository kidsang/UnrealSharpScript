#pragma once
#include "CoreMinimal.h"
#include "UObject/Object.h"
#include "SsBindingPreprocessorTest.generated.h"

/**
 * Class for preprocessor macros define test.
 */
UCLASS(BlueprintType)
class USsBindingPreprocessorTest : public UObject
{
	GENERATED_BODY()

public:
	#if WITH_EDITOR
	UFUNCTION(BlueprintPure, Category = "SsBindingPreprocessorTest")
	static int FuncWithEditor();
	#endif
};
