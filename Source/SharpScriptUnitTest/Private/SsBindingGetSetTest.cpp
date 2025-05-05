#include <SsBindingGetSetTest.h>

bool USsBindingGetSetTest::GetBool() const
{
	return Bool;
}

void USsBindingGetSetTest::SetBool(bool InBool)
{
	Bool = InBool;
}

bool USsBindingGetSetTest::GetBitfieldBool() const
{
	return BitfieldBool;
}

void USsBindingGetSetTest::SetBitfieldBool(bool InBool)
{
	BitfieldBool = InBool;
}

int32 USsBindingGetSetTest::GetInt() const
{
	return Int;
}

void USsBindingGetSetTest::SetInt(int32 InInt)
{
	Int = InInt;
}

double USsBindingGetSetTest::GetFloat() const
{
	return Float;
}

void USsBindingGetSetTest::SetFloat(double InFloat)
{
	Float = InFloat;
}

TEnumAsByte<ESsBindingTestEnum> USsBindingGetSetTest::GetEnum() const
{
	return Enum;
}

void USsBindingGetSetTest::SetEnum(TEnumAsByte<ESsBindingTestEnum> InEnum)
{
	Enum = InEnum;
}

const FString& USsBindingGetSetTest::GetString() const
{
	return String;
}

void USsBindingGetSetTest::SetString(const FString& InString)
{
	String = InString;
}

FName USsBindingGetSetTest::GetNameMember() const
{
	return Name;
}

void USsBindingGetSetTest::SetNameMember(const FName& InName)
{
	Name = InName;
}

const FText& USsBindingGetSetTest::GetText() const
{
	return Text;
}

void USsBindingGetSetTest::SetText(const FText& InText)
{
	Text = InText;
}

TFieldPath<FProperty> USsBindingGetSetTest::GetFieldPath() const
{
	return FieldPath;
}

void USsBindingGetSetTest::SetFieldPath(const TFieldPath<FProperty>& InFieldPath)
{
	FieldPath = InFieldPath;
}

const TArray<FString>& USsBindingGetSetTest::GetStringArray() const
{
	return StringArray;
}

void USsBindingGetSetTest::SetStringArray(const TArray<FString>& InStringArray)
{
	StringArray = InStringArray;
}

const TSet<FString>& USsBindingGetSetTest::GetStringSet() const
{
	return StringSet;
}

void USsBindingGetSetTest::SetStringSet(const TSet<FString>& InStringSet)
{
	StringSet = InStringSet;
}

const TMap<FString, int32>& USsBindingGetSetTest::GetStringIntMap() const
{
	return StringIntMap;
}

void USsBindingGetSetTest::SetStringIntMap(const TMap<FString, int32>& InStringIntMap)
{
	StringIntMap = InStringIntMap;
}

const FSsBindingTestStruct& USsBindingGetSetTest::GetStruct() const
{
	return Struct;
}

void USsBindingGetSetTest::SetStruct(const FSsBindingTestStruct& InStruct)
{
	Struct = InStruct;
}

UObject* USsBindingGetSetTest::GetObject() const
{
	return Object;
}

void USsBindingGetSetTest::SetObject(UObject* InObject)
{
	Object = InObject;
}

TSubclassOf<UObject> USsBindingGetSetTest::GetClassMember() const
{
	return Class;
}

void USsBindingGetSetTest::SetClassMember(TSubclassOf<UObject> InClass)
{
	Class = InClass;
}

TScriptInterface<ISsBindingTestChildInterface> USsBindingGetSetTest::GetInterface() const
{
	return Interface;
}

void USsBindingGetSetTest::SetInterface(TScriptInterface<ISsBindingTestChildInterface> InInterface)
{
	Interface = InInterface;
}
