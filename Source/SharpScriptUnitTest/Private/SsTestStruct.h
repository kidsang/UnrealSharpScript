#pragma once
#include "CoreMinimal.h"
#include "Templates/SubclassOf.h"
#include "SsTestEnum.h"
#include "SsTestInterface.h"
#include "SsArrayTest.h"
#include "SsNativeFuncExporter.h"
#include "SsTestStruct.generated.h"

/**
 * 用于测试导出到C#包装类的各种UStruct功能的结构体。
 */
USTRUCT(BlueprintType)
struct FSsTestStruct
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	bool Bool = false;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	uint8 BitfieldBoolA : 1;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	uint8 BitfieldBoolB : 1;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	int32 Int = 0;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	double Float = 0.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TEnumAsByte<ESsTestEnum> Enum = ESsTestEnum::One;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	ESsTestLongEnum LongEnum = ESsTestLongEnum::One;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	FString String;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	FName Name;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	FText Text;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TFieldPath<FProperty> FieldPath;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TFieldPath<FStructProperty> StructFieldPath;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<FString> StringArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TSet<FString> StringSet;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TMap<FString, int32> StringIntMap;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	FSsArrayTestInnerStruct Struct;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	UObject* Object;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TObjectPtr<UObject> ObjectPtr;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TSoftObjectPtr<UObject> SoftObjectPtr;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TWeakObjectPtr<UObject> WeakObjectPtr;

	UPROPERTY(EditAnywhere, Category = "CSharp|Internal")
	TLazyObjectPtr<UObject> LazyObjectPtr;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TSubclassOf<UObject> Class;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TObjectPtr<UClass> ClassPtr;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TSoftClassPtr<UObject> SoftClassPtr;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TScriptInterface<ISsTestChildInterface> Interface;

	UPROPERTY(meta=(DeprecatedProperty, DeprecationMessage="LegacyInt is deprecated. Please use Int instead."))
	int32 LegacyInt_DEPRECATED = 0;

	UPROPERTY(EditInstanceOnly, Category = "CSharp|Internal")
	bool BoolInstanceOnly = false;

	UPROPERTY(EditDefaultsOnly, Category = "CSharp|Internal")
	bool BoolDefaultsOnly = false;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "CSharp|Internal")
	mutable bool BoolMutable = false;
};

UCLASS()
class USsTestStructUtils : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static void* CreateNativeTestStructDefault();

	static void* CreateNativeTestStructCustom();

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
