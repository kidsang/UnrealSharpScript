#include "SsBindingTestStruct.h"

void* USsBindingTestStructUtils::CreateNativeTestStructDefault()
{
	return new FSsBindingTestStruct();
}

void* USsBindingTestStructUtils::CreateNativeTestStructCustom()
{
	FSsBindingTestStruct* Instance = new FSsBindingTestStruct();
	Instance->Bool = true;
	Instance->BitfieldBoolA = true;
	Instance->BitfieldBoolB = false;
	Instance->Int = 123;
	Instance->Float = 2;
	Instance->String = TEXT("String");
	Instance->Name = FName(TEXT("Name"));
	Instance->Text = FText::FromString(TEXT("Text"));
	Instance->StringArray = {TEXT("String"), TEXT("Array")};
	Instance->StringSet = {TEXT("String"), TEXT("Set")};
	Instance->StringIntMap = {
		{TEXT("A"), 1},
		{TEXT("B"), 2},
	};
	Instance->Struct.IntArray = {1, 2, 3};
	return Instance;
}

void USsBindingTestStructUtils::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&CreateNativeTestStructDefault, TEXT("WrapperStructTest.CreateNativeTestStructDefault"));
	BindNativeCallbackFunc(&CreateNativeTestStructCustom, TEXT("WrapperStructTest.CreateNativeTestStructCustom"));
}
