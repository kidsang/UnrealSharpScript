#pragma once
#include "CoreMinimal.h"
#include "Templates/SubclassOf.h"
#include "SsBindingTestEnum.h"
#include "SsBindingTestBlittableStruct.h"
#include "SsBindingTestInterface.h"
#include "SsBindingTestDelegate.h"
#include "SsBindingArrayTest.generated.h"

/** Structure for testing nested array access */
USTRUCT(BlueprintType)
struct FSsBindingArrayTestInnerStruct
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<int> IntArray;
};

/** Structure for testing arrays of various types */
USTRUCT(BlueprintType)
struct FSsBindingArrayTestStruct
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
	TArray<TEnumAsByte<ESsBindingTestEnum>> EnumArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<ESsBindingTestLongEnum> LongEnumArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<FSsBindingArrayTestInnerStruct> StructArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<FSsBindingTestBlittableStruct> BlittableStructArray;

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
	TArray<TScriptInterface<ISsBindingTestChildInterface>> InterfaceArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<FSsBindingTestDelegate> DelegateArray;
};
