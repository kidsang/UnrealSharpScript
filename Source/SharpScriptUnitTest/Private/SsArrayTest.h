#pragma once
#include "CoreMinimal.h"
#include "Templates/SubclassOf.h"
#include "SsTestEnum.h"
#include "SsTestBlittableStruct.h"
#include "SsTestInterface.h"
#include "SsTestDelegate.h"
#include "SsArrayTest.generated.h"

/** Structure for testing nested array access */
USTRUCT(BlueprintType)
struct FSsArrayTestInnerStruct
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<int> IntArray;
};

/** Structure for testing arrays of various types */
USTRUCT(BlueprintType)
struct FSsArrayTestStruct
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<int> IntArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<bool> BoolArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<FString> StringArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<FText> TextArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<TEnumAsByte<ESsTestEnum>> EnumArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<ESsTestLongEnum> LongEnumArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<FSsArrayTestInnerStruct> StructArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<FSsTestBlittableStruct> BlittableStructArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<UObject*> ObjectArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<TSoftObjectPtr<UObject>> SoftObjectPtrArray;

	UPROPERTY(EditAnywhere, Category = "CSharp|Internal")
	TArray<TWeakObjectPtr<UObject>> WeakObjectPtrArray;

	UPROPERTY(EditAnywhere, Category = "CSharp|Internal")
	TArray<TLazyObjectPtr<UObject>> LazyObjectPtrArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<TSubclassOf<UObject>> ClassArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<TSoftClassPtr<UObject>> SoftClassPtrArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<TScriptInterface<ISsTestChildInterface>> InterfaceArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<FSsTestDelegate> DelegateArray;
};
