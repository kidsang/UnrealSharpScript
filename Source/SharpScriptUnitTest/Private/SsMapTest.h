#pragma once
#include "CoreMinimal.h"
#include "Templates/SubclassOf.h"
#include "SsTestEnum.h"
#include "SsTestBlittableStruct.h"
#include "SsTestInterface.h"
#include "SsTestDelegate.h"
#include "SsMapTest.generated.h"

/** 用于测试字典嵌套访问的结构体 */
USTRUCT(BlueprintType)
struct FSsMapTestInnerStruct
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<int, int> IntIntMap;
};

/** 用于测试各种类型字典的结构体 */
USTRUCT(BlueprintType)
struct FSsMapTestStruct
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<FString, FText> StringTextMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<int, bool> IntBoolMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<TEnumAsByte<ESsTestEnum>, TEnumAsByte<ESsTestEnum>> EnumMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<ESsTestLongEnum, ESsTestLongEnum> LongEnumMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<int, FSsMapTestInnerStruct> IntStructMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<FSsTestBlittableStruct, FSsTestBlittableStruct> BlittableStructMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<UObject*, UObject*> ObjectMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<TSoftObjectPtr<UObject>, TSoftObjectPtr<UObject>> SoftObjectPtrMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<TSubclassOf<UObject>, TSubclassOf<UObject>> ClassMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<TSoftClassPtr<UObject>, TSoftClassPtr<UObject>> SoftClassPtrMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<int, TScriptInterface<ISsTestChildInterface>> IntInterfaceMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<int, FSsTestDelegate> IntDelegateMap;
};
