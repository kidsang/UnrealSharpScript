#include "SsBindingTestObject.h"
#include "SsCommon.h"

USsBindingTestObject::USsBindingTestObject()
	: BitfieldBoolA(false)
	, BitfieldBoolB(false)
	, Struct()
	, BlittableStruct()
	, Object(nullptr)
{
}

int32 USsBindingTestObject::FuncBlueprintNative_Implementation(const int32 InValue) const
{
	return InValue;
}

void USsBindingTestObject::FuncBlueprintNativeRef_Implementation(FSsBindingTestStruct& InOutStruct) const
{
}

int32 USsBindingTestObject::CallFuncBlueprintImplementable(const int32 InValue) const
{
	return FuncBlueprintImplementable(InValue);
}

int32 USsBindingTestObject::CallFuncBlueprintNative(const int32 InValue) const
{
	return FuncBlueprintNative(InValue);
}

void USsBindingTestObject::CallFuncBlueprintNativeRef(FSsBindingTestStruct& InOutStruct) const
{
	return FuncBlueprintNativeRef(InOutStruct);
}

void USsBindingTestObject::FuncTakingSsTestStruct(const FSsBindingTestStruct& InStruct) const
{
}

void USsBindingTestObject::LegacyFuncTakingSsTestStruct(const FSsBindingTestStruct& InStruct) const
{
	FuncTakingSsTestStruct(InStruct);
}

void USsBindingTestObject::FuncTakingSsTestStructDefault(const FSsBindingTestStruct& InStruct)
{
}

int32 USsBindingTestObject::FuncTakingSsBindingTestDelegate(const FSsBindingTestDelegate& InDelegate, const int32 InValue) const
{
	return InDelegate.IsBound() ? InDelegate.Execute(InValue) : INDEX_NONE;
}

void USsBindingTestObject::FuncTakingFieldPath(const TFieldPath<FProperty>& InFieldPath)
{
	FieldPath = InFieldPath;
}

void USsBindingTestObject::MulticastDelegatePropertyCallback(FString InStr)
{
	String = InStr;
}

TArray<int32> USsBindingTestObject::ReturnArray()
{
	TArray<int32> TmpArray;
	TmpArray.Add(10);
	return TmpArray;
}

TSet<int32> USsBindingTestObject::ReturnSet()
{
	TSet<int32> TmpSet;
	TmpSet.Add(10);
	return TmpSet;
}

TMap<int32, bool> USsBindingTestObject::ReturnMap()
{
	TMap<int32, bool> TmpMap;
	TmpMap.Add(10, true);
	return TmpMap;
}

TFieldPath<FProperty> USsBindingTestObject::ReturnFieldPath()
{
	return TFieldPath<FProperty>(USsBindingTestObject::StaticClass()->FindPropertyByName(TEXT("FieldPath")));
}

void USsBindingTestObject::EmitScriptError()
{
	FFrame::KismetExecutionMessage(TEXT("EmitScriptError was called"), ELogVerbosity::Error);
}

void USsBindingTestObject::EmitScriptWarning()
{
	FFrame::KismetExecutionMessage(TEXT("EmitScriptWarning was called"), ELogVerbosity::Warning);
}

int32 USsBindingTestObject::GetConstantValue()
{
	return 10;
}

void USsBindingTestObject::SetInt(int InValue)
{
	Int = InValue;
}

int USsBindingTestObject::GetInt() const
{
	return Int;
}

int32 USsBindingTestObject::FuncInterface(const int32 InValue) const
{
	return InValue;
}

int32 USsBindingTestObject::FuncInterfaceChild(const int32 InValue) const
{
	return InValue;
}

int32 USsBindingTestObject::FuncInterfaceOther(const int32 InValue) const
{
	return InValue;
}
