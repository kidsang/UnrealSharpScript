// ReSharper disable CppUEBlueprintCallableFunctionUnused
#pragma once
#include "CoreMinimal.h"
#include "Templates/SubclassOf.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "SsBindingTestEnum.h"
#include "SsBindingTestStruct.h"
#include "SsBindingTestInterface.h"
#include "SsBindingTestDelegate.h"
#include "SsBindingFunctionTest.generated.h"

/** Used to verify UFunction static export. */
UCLASS(BlueprintType)
class USsBindingFunctionTest : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

public:
	UFUNCTION(Category = "CSharp|Internal")
	static int8 FuncInt8(int8 InValue, int8& OutValue);

	UFUNCTION(Category = "CSharp|Internal")
	static int16 FuncInt16(int16 InValue, int16& OutValue);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static int32 FuncInt32(int32 InValue, int32& OutValue, int32 Default1 = 0, int32 Default2 = 1);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static int64 FuncInt64(int64 InValue, int64& OutValue, int64 Default1 = 0, int64 Default2 = 1);

	UFUNCTION(Category = "CSharp|UInternal")
	static uint8 FuncUInt8(uint8 InValue, uint8& OutValue);

	UFUNCTION(Category = "CSharp|UInternal")
	static uint16 FuncUInt16(uint16 InValue, uint16& OutValue);

	UFUNCTION(Category = "CSharp|UInternal")
	static uint32 FuncUInt32(uint32 InValue, uint32& OutValue);

	UFUNCTION(Category = "CSharp|UInternal")
	static uint64 FuncUInt64(uint64 InValue, uint64& OutValue);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static float FuncFloat(float InValue, float& OutValue, float Default1 = 0, float Default2 = 1.0f);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static double FuncDouble(double InValue, double& OutValue, double Default1 = 0, double Default2 = 1.0f);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static bool FuncBool(bool InValue, bool& OutValue, bool Default1 = false, bool Default2 = true);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static FString FuncString(const FString& InValue, FString& OutValue, const FString& Default1 = TEXT(""), const FString& Default2 = TEXT("123"));

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static FName FuncName(const FName& InValue, FName& OutValue, const FName& Default1 = TEXT(""), const FName& Default2 = TEXT("123"));

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static FText FuncText(const FText& InValue, FText& OutValue, const FText& Default1 = INVTEXT(""), const FText& Default2 = INVTEXT("123"));

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TEnumAsByte<ESsBindingTestEnum> FuncEnum(TEnumAsByte<ESsBindingTestEnum> InValue, TEnumAsByte<ESsBindingTestEnum>& OutValue, TEnumAsByte<ESsBindingTestEnum> Default1 = ESsBindingTestEnum::One, TEnumAsByte<ESsBindingTestEnum> Default2 = ESsBindingTestEnum::Two);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static ESsBindingTestLongEnum FuncLongEnum(ESsBindingTestLongEnum InValue, ESsBindingTestLongEnum& OutValue, ESsBindingTestLongEnum Default1 = ESsBindingTestLongEnum::One, ESsBindingTestLongEnum Default2 = ESsBindingTestLongEnum::Two);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TFieldPath<FProperty> FuncFieldPath(const TFieldPath<FProperty>& InValue, TFieldPath<FProperty>& OutValue);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TArray<FString> FuncStringArray(const TArray<FString>& InValue, TArray<FString>& OutValue);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TSet<FString> FuncStringSet(const TSet<FString>& InValue, TSet<FString>& OutValue);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TMap<FString, int> FuncStringIntMap(const TMap<FString, int>& InValue, TMap<FString, int>& OutValue);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static FSsBindingTestStruct FuncStruct(const FSsBindingTestStruct& InValue, FSsBindingTestStruct& OutValue, const FSsBindingTestStruct& Default = FSsBindingTestStruct());

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static FSsBindingTestBlittableStruct FuncBlittableStruct(const FSsBindingTestBlittableStruct& InValue, FSsBindingTestBlittableStruct& OutValue, const FSsBindingTestBlittableStruct& Default = FSsBindingTestBlittableStruct());

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static UObject* FuncObject(UObject* InValue, UObject*& OutValue, UObject* Default = nullptr);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TSoftObjectPtr<UObject> FuncSoftObjectPtr(const TSoftObjectPtr<UObject>& InValue, TSoftObjectPtr<UObject>& OutValue, const TSoftObjectPtr<UObject>& Default = nullptr);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TSubclassOf<UObject> FuncClass(const TSubclassOf<UObject>& InValue, TSubclassOf<UObject>& OutValue, const TSubclassOf<UObject>& Default = nullptr);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TSoftClassPtr<UObject> FuncSoftClassPtr(const TSoftClassPtr<UObject>& InValue, TSoftClassPtr<UObject>& OutValue, const TSoftClassPtr<UObject>& Default = nullptr);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static TScriptInterface<ISsBindingTestChildInterface> FuncInterface(const TScriptInterface<ISsBindingTestChildInterface>& InValue, TScriptInterface<ISsBindingTestChildInterface>& OutValue);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	static FSsBindingTestDelegate FuncDelegate(const FSsBindingTestDelegate& InValue);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta = (DeterminesOutputType = "InClass1"))
	static UActorComponent* FuncGenericRet(const TSubclassOf<UActorComponent>& InClass1, const TSubclassOf<USceneComponent>& InClass2, TArray<UActorComponent*>& Output1, TSet<UActorComponent*>& Output2, TMap<UActorComponent*, UActorComponent*>& Output3);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta = (DeterminesOutputType = "InClass2", DynamicOutputParam="Output1,Output2,Output3"))
	static UActorComponent* FuncGenericOut(const TSubclassOf<UActorComponent>& InClass1, const TSubclassOf<USceneComponent>& InClass2, TArray<UActorComponent*>& Output1, TSet<UActorComponent*>& Output2, TMap<UActorComponent*, UActorComponent*>& Output3);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta = (DeterminesOutputType = "InClass"))
	static TArray<UActorComponent*> FuncGenericRetArray(const TSubclassOf<UActorComponent>& InClass);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta = (DeterminesOutputType = "InClass"))
	static TSet<UActorComponent*> FuncGenericRetSet(const TSubclassOf<UActorComponent>& InClass);

	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal", meta = (DeterminesOutputType = "InClass"))
	static TMap<UActorComponent*, UActorComponent*> FuncGenericRetMap(const TSubclassOf<UActorComponent>& InClass);
};
