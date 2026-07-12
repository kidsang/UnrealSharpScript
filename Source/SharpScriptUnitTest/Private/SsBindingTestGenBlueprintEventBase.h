#pragma once
// ReSharper disable CppUEBlueprintCallableFunctionUnused
// ReSharper disable CppUEBlueprintImplementableEventNotImplemented
#include "CoreMinimal.h"
#include "SsBindingTestGenBlueprintEventBase.generated.h"

/**
 * C++ base class used to validate that a C# subclass can override a C++ BlueprintEvent.
 */
UCLASS(BlueprintType)
class USsBindingTestGenBlueprintEventBase : public UObject
{
	GENERATED_BODY()

public:
	UFUNCTION(BlueprintNativeEvent, Category = "CSharp|Internal")
	int32 BaseNativeScore(int32 InValue) const;

	UFUNCTION(BlueprintImplementableEvent, Category = "CSharp|Internal")
	int32 BaseImplScore(int32 InValue) const;
};
