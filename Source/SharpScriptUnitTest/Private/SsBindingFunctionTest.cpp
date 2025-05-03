#include "SsBindingFunctionTest.h"
#include "Components/ActorComponent.h"
#include "Components/SceneComponent.h"

int8 USsBindingFunctionTest::FuncInt8(int8 InValue, int8& OutValue)
{
	OutValue = InValue;
	return OutValue;
}

int16 USsBindingFunctionTest::FuncInt16(int16 InValue, int16& OutValue)
{
	OutValue = InValue;
	return OutValue;
}

int32 USsBindingFunctionTest::FuncInt32(int32 InValue, int32& OutValue, int32 Default1, int32 Default2)
{
	OutValue = InValue;
	return OutValue;
}

int64 USsBindingFunctionTest::FuncInt64(int64 InValue, int64& OutValue, int64 Default1, int64 Default2)
{
	OutValue = InValue;
	return OutValue;
}

uint8 USsBindingFunctionTest::FuncUInt8(uint8 InValue, uint8& OutValue)
{
	OutValue = InValue;
	return OutValue;
}

uint16 USsBindingFunctionTest::FuncUInt16(uint16 InValue, uint16& OutValue)
{
	OutValue = InValue;
	return OutValue;
}

uint32 USsBindingFunctionTest::FuncUInt32(uint32 InValue, uint32& OutValue)
{
	OutValue = InValue;
	return OutValue;
}

uint64 USsBindingFunctionTest::FuncUInt64(uint64 InValue, uint64& OutValue)
{
	OutValue = InValue;
	return OutValue;
}

float USsBindingFunctionTest::FuncFloat(float InValue, float& OutValue, float Default1, float Default2)
{
	OutValue = InValue;
	return OutValue;
}

double USsBindingFunctionTest::FuncDouble(double InValue, double& OutValue, double Default1, double Default2)
{
	OutValue = InValue;
	return OutValue;
}

bool USsBindingFunctionTest::FuncBool(bool InValue, bool& OutValue, bool Default1, bool Default2)
{
	OutValue = InValue;
	return OutValue;
}

FString USsBindingFunctionTest::FuncString(const FString& InValue, FString& OutValue, const FString& Default1, const FString& Default2)
{
	OutValue = InValue;
	return OutValue;
}

FName USsBindingFunctionTest::FuncName(const FName& InValue, FName& OutValue, const FName& Default1, const FName& Default2)
{
	OutValue = InValue;
	return OutValue;
}

FText USsBindingFunctionTest::FuncText(const FText& InValue, FText& OutValue, const FText& Default1, const FText& Default2)
{
	OutValue = InValue;
	return OutValue;
}

TEnumAsByte<ESsBindingTestEnum> USsBindingFunctionTest::FuncEnum(TEnumAsByte<ESsBindingTestEnum> InValue, TEnumAsByte<ESsBindingTestEnum>& OutValue, TEnumAsByte<ESsBindingTestEnum> Default1, TEnumAsByte<ESsBindingTestEnum> Default2)
{
	OutValue = InValue;
	return OutValue;
}

ESsBindingTestLongEnum USsBindingFunctionTest::FuncLongEnum(ESsBindingTestLongEnum InValue, ESsBindingTestLongEnum& OutValue, ESsBindingTestLongEnum Default1, ESsBindingTestLongEnum Default2)
{
	OutValue = InValue;
	return OutValue;
}

TFieldPath<FProperty> USsBindingFunctionTest::FuncFieldPath(const TFieldPath<FProperty>& InValue, TFieldPath<FProperty>& OutValue)
{
	OutValue = InValue;
	return OutValue;
}

TArray<FString> USsBindingFunctionTest::FuncStringArray(const TArray<FString>& InValue, TArray<FString>& OutValue)
{
	OutValue = InValue;
	return OutValue;
}

TSet<FString> USsBindingFunctionTest::FuncStringSet(const TSet<FString>& InValue, TSet<FString>& OutValue)
{
	OutValue = InValue;
	return OutValue;
}

TMap<FString, int> USsBindingFunctionTest::FuncStringIntMap(const TMap<FString, int>& InValue, TMap<FString, int>& OutValue)
{
	OutValue = InValue;
	return OutValue;
}

FSsBindingTestStruct USsBindingFunctionTest::FuncStruct(const FSsBindingTestStruct& InValue, FSsBindingTestStruct& OutValue, const FSsBindingTestStruct& Default)
{
	OutValue = InValue;
	return OutValue;
}

FSsBindingTestBlittableStruct USsBindingFunctionTest::FuncBlittableStruct(const FSsBindingTestBlittableStruct& InValue, FSsBindingTestBlittableStruct& OutValue, const FSsBindingTestBlittableStruct& Default)
{
	OutValue = InValue;
	return OutValue;
}

UObject* USsBindingFunctionTest::FuncObject(UObject* InValue, UObject*& OutValue, UObject* Default)
{
	OutValue = InValue;
	return OutValue;
}

TSoftObjectPtr<UObject> USsBindingFunctionTest::FuncSoftObjectPtr(const TSoftObjectPtr<UObject>& InValue, TSoftObjectPtr<UObject>& OutValue, const TSoftObjectPtr<UObject>& Default)
{
	OutValue = InValue;
	return OutValue;
}

TSubclassOf<UObject> USsBindingFunctionTest::FuncClass(const TSubclassOf<UObject>& InValue, TSubclassOf<UObject>& OutValue, const TSubclassOf<UObject>& Default)
{
	OutValue = InValue;
	return OutValue;
}

TSoftClassPtr<UObject> USsBindingFunctionTest::FuncSoftClassPtr(const TSoftClassPtr<UObject>& InValue, TSoftClassPtr<UObject>& OutValue, const TSoftClassPtr<UObject>& Default)
{
	OutValue = InValue;
	return OutValue;
}

TScriptInterface<ISsBindingTestChildInterface> USsBindingFunctionTest::FuncInterface(const TScriptInterface<ISsBindingTestChildInterface>& InValue, TScriptInterface<ISsBindingTestChildInterface>& OutValue)
{
	OutValue = InValue;
	return OutValue;
}

FSsBindingTestDelegate USsBindingFunctionTest::FuncDelegate(const FSsBindingTestDelegate& InValue)
{
	return InValue;
}

UActorComponent* USsBindingFunctionTest::FuncGenericRet(const TSubclassOf<UActorComponent>& InClass1, const TSubclassOf<USceneComponent>& InClass2, TArray<UActorComponent*>& Output1, TSet<UActorComponent*>& Output2, TMap<UActorComponent*, UActorComponent*>& Output3)
{
	Output1.Add(InClass1.GetDefaultObject());
	Output2.Add(InClass1.GetDefaultObject());
	Output3.Add(InClass1.GetDefaultObject(), InClass1.GetDefaultObject());
	return InClass1.GetDefaultObject();
}

UActorComponent* USsBindingFunctionTest::FuncGenericOut(const TSubclassOf<UActorComponent>& InClass1, const TSubclassOf<USceneComponent>& InClass2, TArray<UActorComponent*>& Output1, TSet<UActorComponent*>& Output2, TMap<UActorComponent*, UActorComponent*>& Output3)
{
	Output1.Add(InClass2.GetDefaultObject());
	Output2.Add(InClass2.GetDefaultObject());
	Output3.Add(InClass2.GetDefaultObject(), InClass2.GetDefaultObject());
	return InClass2.GetDefaultObject();
}

TArray<UActorComponent*> USsBindingFunctionTest::FuncGenericRetArray(const TSubclassOf<UActorComponent>& InClass)
{
	TArray<UActorComponent*> Out;
	Out.Add(InClass.GetDefaultObject());
	return Out;
}

TSet<UActorComponent*> USsBindingFunctionTest::FuncGenericRetSet(const TSubclassOf<UActorComponent>& InClass)
{
	TSet<UActorComponent*> Out;
	Out.Add(InClass.GetDefaultObject());
	return Out;
}

TMap<UActorComponent*, UActorComponent*> USsBindingFunctionTest::FuncGenericRetMap(const TSubclassOf<UActorComponent>& InClass)
{
	TMap<UActorComponent*, UActorComponent*> Out;
	Out.Add(InClass.GetDefaultObject(), InClass.GetDefaultObject());
	return Out;
}
