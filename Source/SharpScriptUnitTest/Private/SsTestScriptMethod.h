// ReSharper disable CppUEBlueprintCallableFunctionUnused
#pragma once
#include "CoreMinimal.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "SsTestScriptMethod.generated.h"

USTRUCT(BlueprintType)
struct FSsTestNumericStruct
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	int X;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	int Y;
};

/**
 * Test struct extension methods exported through "ScriptMethod" metadata.
 */
UCLASS()
class USsTestScriptMethod : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:
	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta=(ScriptMethod="ToString"))
	static FString NumericStructToString(const FSsTestNumericStruct& InStruct);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta=(ScriptMethod))
	static void TestDefaultValue(FSsTestNumericStruct& InStruct, const FVector& InVector = FVector::ForwardVector, const FVector& InVector2 = FVector::ZeroVector);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta=(ScriptMethod, BlueprintAutocast))
	static FIntPoint NumericStructToIntPoint(const FSsTestNumericStruct& InStruct);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta=(ScriptMethod, ScriptOperator="=="))
	static bool Equals(const FSsTestNumericStruct& Lhs, const FSsTestNumericStruct& Rhs);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta=(ScriptMethod, ScriptOperator="+"))
	static FSsTestNumericStruct AddInt(const FSsTestNumericStruct& Lhs, int Rhs);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta=(ScriptMethod, ScriptOperator="-"))
	static FSsTestNumericStruct SubtractInt(const FSsTestNumericStruct& Lhs, int Rhs);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta=(ScriptMethod, ScriptOperator="*"))
	static FSsTestNumericStruct MultiplyInt(const FSsTestNumericStruct& Lhs, int Rhs);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta=(ScriptMethod, ScriptOperator="/"))
	static FSsTestNumericStruct DivideInt(const FSsTestNumericStruct& Lhs, int Rhs);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta=(ScriptMethod, ScriptOperator="neg"))
	static FSsTestNumericStruct Negate(const FSsTestNumericStruct& Lhs);
};
