#include "SsTestStruct.h"
#include "SsTestObject.h"

void* USsTestStructUtils::CreateNativeTestStructDefault()
{
	return new FSsTestStruct();
}

void* USsTestStructUtils::CreateNativeTestStructCustom()
{
	FSsTestStruct* Instance = new FSsTestStruct();
	Instance->Bool = true;
	Instance->BitfieldBoolA = true;
	Instance->BitfieldBoolB = false;
	Instance->Int = 123;
	Instance->Float = 2;
	Instance->Enum = ESsTestEnum::Two;
	Instance->LongEnum = ESsTestLongEnum::Two;
	Instance->String = TEXT("String");
	Instance->Name = FName(TEXT("Name"));
	Instance->Text = FText::FromString(TEXT("Text"));
	Instance->FieldPath = TFieldPath<FProperty>(FSsTestStruct::StaticStruct()->FindPropertyByName(TEXT("StringArray")));
	Instance->StructFieldPath = TFieldPath<FStructProperty>(CastField<FStructProperty>(USsTestObject::StaticClass()->FindPropertyByName(TEXT("Struct"))));
	Instance->StringArray = {TEXT("String"), TEXT("Array")};
	Instance->StringSet = {TEXT("String"), TEXT("Set")};
	Instance->StringIntMap = {
		{TEXT("A"), 1},
		{TEXT("B"), 2},
	};
	Instance->Struct.IntArray = {1, 2, 3};
	return Instance;
}

void USsTestStructUtils::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&CreateNativeTestStructDefault, TEXT("WrapperStructTest.CreateNativeTestStructDefault"));
	BindNativeCallbackFunc(&CreateNativeTestStructCustom, TEXT("WrapperStructTest.CreateNativeTestStructCustom"));
}
