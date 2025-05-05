#pragma once
// ReSharper disable CppUEBlueprintCallableFunctionUnused
// ReSharper disable CppUEBlueprintImplementableEventNotImplemented
#include "CoreMinimal.h"
#include "Templates/SubclassOf.h"
#include "SsBindingTestEnum.h"
#include "SsBindingTestStruct.h"
#include "SsBindingTestBlittableStruct.h"
#include "SsBindingTestDelegate.h"
#include "SsBindingTestInterface.h"
#include "SsBindingTestObject.generated.h"

UCLASS(BlueprintType)
class USsBindingTestObject : public UObject, public ISsBindingTestChildInterface, public ISsBindingTestOtherInterface
{
	GENERATED_BODY()

public:
	USsBindingTestObject();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	bool Bool = false;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	uint8 BitfieldBoolA : 1;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	uint8 BitfieldBoolB : 1;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	int32 Int = 0;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	double Float = 0;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TEnumAsByte<ESsBindingTestEnum> Enum = ESsBindingTestEnum::One;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	ESsBindingTestLongEnum LongEnum = ESsBindingTestLongEnum::One;

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
	FSsBindingTestDelegate Delegate;

	UPROPERTY(EditAnywhere, BlueprintAssignable, Category = "CSharp|Internal")
	FSsBindingTestMulticastDelegate MulticastDelegate;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	FSsBindingTestStruct Struct;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	TArray<FSsBindingTestStruct> StructArray;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	FSsBindingTestBlittableStruct BlittableStruct;

	// UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")
	// FSsTestChildStruct ChildStruct;

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
	TScriptInterface<ISsBindingTestChildInterface> Interface;

	UPROPERTY(EditInstanceOnly, Category = "CSharp|Internal")
	bool BoolInstanceOnly = false;

	UPROPERTY(EditDefaultsOnly, Category = "CSharp|Internal")
	bool BoolDefaultsOnly = false;

	UFUNCTION(BlueprintImplementableEvent, Category = "CSharp|Internal")
	int32 FuncBlueprintImplementable(const int32 InValue) const;

	UFUNCTION(BlueprintNativeEvent, Category = "CSharp|Internal")
	int32 FuncBlueprintNative(const int32 InValue) const;

	UFUNCTION(BlueprintNativeEvent, Category = "CSharp|Internal")
	void FuncBlueprintNativeRef(UPARAM(ref) FSsBindingTestStruct& InOutStruct) const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	int32 CallFuncBlueprintImplementable(const int32 InValue) const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	int32 CallFuncBlueprintNative(const int32 InValue) const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void CallFuncBlueprintNativeRef(UPARAM(ref) FSsBindingTestStruct& InOutStruct) const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void FuncTakingSsTestStruct(const FSsBindingTestStruct& InStruct) const;

	// UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	// void FuncTakingSsTestChildStruct(const FSsTestChildStruct& InStruct) const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta=(DeprecatedFunction, DeprecationMessage="LegacyFuncTakingSsTestStruct is deprecated. Please use FuncTakingSsTestStruct instead."))
	void LegacyFuncTakingSsTestStruct(const FSsBindingTestStruct& InStruct) const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void FuncTakingSsTestStructDefault(const FSsBindingTestStruct& InStruct = FSsBindingTestStruct());

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	int32 FuncTakingSsBindingTestDelegate(const FSsBindingTestDelegate& InDelegate, const int32 InValue) const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void FuncTakingFieldPath(const TFieldPath<FProperty>& InFieldPath); // UHT couldn't parse any default value for the FieldPath.

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void MulticastDelegatePropertyCallback(FString InStr);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TArray<int32> ReturnArray();

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TSet<int32> ReturnSet();

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TMap<int32, bool> ReturnMap();

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TFieldPath<FProperty> ReturnFieldPath();

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static void EmitScriptError();

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static void EmitScriptWarning();

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal", meta=(ScriptConstant="ConstantValue"))
	static int32 GetConstantValue();

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetInt(int InValue);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	int GetInt() const;

	virtual int32 FuncInterface(const int32 InValue) const override;

	virtual int32 FuncInterfaceChild(const int32 InValue) const override;

	virtual int32 FuncInterfaceOther(const int32 InValue) const override;
};
