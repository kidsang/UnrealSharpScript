#pragma once
#include "CoreMinimal.h"
#include "Templates/SubclassOf.h"
#include "SsBindingTestEnum.h"
#include "SsBindingTestBlittableStruct.h"
#include "SsBindingTestInterface.h"
#include "SsBindingTestDelegate.h"
#include "SsBindingMapTest.generated.h"

/** Structure for testing nested dictionary access */
USTRUCT(BlueprintType)
struct FSsBindingMapTestInnerStruct
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<int, int> IntIntMap;
};

/** Structure for testing various types of dictionaries */
USTRUCT(BlueprintType)
struct FSsBindingMapTestStruct
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<FString, FText> StringTextMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<int, bool> IntBoolMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<TEnumAsByte<ESsBindingTestEnum>, TEnumAsByte<ESsBindingTestEnum>> EnumMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<ESsBindingTestLongEnum, ESsBindingTestLongEnum> LongEnumMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<int, FSsBindingMapTestInnerStruct> IntStructMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<FSsBindingTestBlittableStruct, FSsBindingTestBlittableStruct> BlittableStructMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<UObject*, UObject*> ObjectMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<TSoftObjectPtr<UObject>, TSoftObjectPtr<UObject>> SoftObjectPtrMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<TSubclassOf<UObject>, TSubclassOf<UObject>> ClassMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<TSoftClassPtr<UObject>, TSoftClassPtr<UObject>> SoftClassPtrMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<int, TScriptInterface<ISsBindingTestChildInterface>> IntInterfaceMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<int, FSsBindingTestDelegate> IntDelegateMap;
};
