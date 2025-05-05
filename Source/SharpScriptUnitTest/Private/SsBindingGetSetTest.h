#pragma once
// ReSharper disable CppUEBlueprintCallableFunctionUnused
// ReSharper disable CppUEBlueprintImplementableEventNotImplemented
#include "CoreMinimal.h"
#include "UObject/Object.h"
#include "SsBindingTestEnum.h"
#include "SsBindingTestStruct.h"
#include "SsBindingTestInterface.h"
#include "SsBindingGetSetTest.generated.h"

/**
 * Class to test blueprint getter & setter.
 */
UCLASS(BlueprintType)
class USsBindingGetSetTest : public UObject
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, BlueprintGetter=GetBool, BlueprintSetter=SetBool, Category = "CSharp|Internal")
	bool Bool = false;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	bool GetBool() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetBool(bool InBool);

	UPROPERTY(EditAnywhere, BlueprintReadWrite, BlueprintGetter=GetBitfieldBool, BlueprintSetter=SetBitFieldBool, Category = "CSharp|Internal")
	uint8 BitfieldBool : 1;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	bool GetBitfieldBool() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetBitfieldBool(bool InBool);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetInt, BlueprintSetter=SetInt, Category = "CSharp|Internal")
	int32 Int = 0;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	int32 GetInt() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetInt(int32 InInt);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetFloat, BlueprintSetter=SetFloat, Category = "CSharp|Internal")
	double Float = 0;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	double GetFloat() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetFloat(double InFloat);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetEnum, BlueprintSetter=SetEnum, Category = "CSharp|Internal")
	TEnumAsByte<ESsBindingTestEnum> Enum = ESsBindingTestEnum::One;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	TEnumAsByte<ESsBindingTestEnum> GetEnum() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetEnum(TEnumAsByte<ESsBindingTestEnum> InEnum);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetString, BlueprintSetter=SetString, Category = "CSharp|Internal")
	FString String;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	const FString& GetString() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetString(const FString& InString);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetNameMember, BlueprintSetter=SetNameMember, Category = "CSharp|Internal")
	FName Name;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	FName GetNameMember() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetNameMember(const FName& InName);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetText, BlueprintSetter=SetText, Category = "CSharp|Internal")
	FText Text;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	const FText& GetText() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetText(const FText& InText);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetFieldPath, BlueprintSetter=SetFieldPath, Category = "CSharp|Internal")
	TFieldPath<FProperty> FieldPath;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	TFieldPath<FProperty> GetFieldPath() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetFieldPath(const TFieldPath<FProperty>& InFieldPath);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetStringArray, BlueprintSetter=SetStringArray, Category = "CSharp|Internal")
	TArray<FString> StringArray;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	const TArray<FString>& GetStringArray() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetStringArray(const TArray<FString>& InStringArray);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetStringSet, BlueprintSetter=SetStringSet, Category = "CSharp|Internal")
	TSet<FString> StringSet;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	const TSet<FString>& GetStringSet() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetStringSet(const TSet<FString>& InStringSet);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetStringIntMap, BlueprintSetter=SetStringIntMap, Category = "CSharp|Internal")
	TMap<FString, int32> StringIntMap;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	const TMap<FString, int32>& GetStringIntMap() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetStringIntMap(const TMap<FString, int32>& InStringIntMap);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetStruct, BlueprintSetter=SetStruct, Category = "CSharp|Internal")
	FSsBindingTestStruct Struct;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	const FSsBindingTestStruct& GetStruct() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetStruct(const FSsBindingTestStruct& InStruct);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetObject, BlueprintSetter=SetObject, Category = "CSharp|Internal")
	UObject* Object;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	UObject* GetObject() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetObject(UObject* InObject);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetClassMember, BlueprintSetter=SetClassMember, Category = "CSharp|Internal")
	TSubclassOf<UObject> Class;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	TSubclassOf<UObject> GetClassMember() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetClassMember(TSubclassOf<UObject> InClass);

	UPROPERTY(EditAnywhere, BlueprintReadWrite,  BlueprintGetter=GetInterface, BlueprintSetter=SetInterface, Category = "CSharp|Internal")
	TScriptInterface<ISsBindingTestChildInterface> Interface;

	UFUNCTION(BlueprintPure, Category = "CSharp|Internal")
	TScriptInterface<ISsBindingTestChildInterface> GetInterface() const;

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	void SetInterface(TScriptInterface<ISsBindingTestChildInterface> InInterface);
};
