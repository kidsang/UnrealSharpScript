#include "SsTestObject.h"
#include "SsCommon.h"

USsTestObject::USsTestObject()
	: BitfieldBoolA(false)
	, BitfieldBoolB(false)
	, Struct()
	, BlittableStruct()
	, Object(nullptr)
{
}

int32 USsTestObject::FuncBlueprintNative_Implementation(const int32 InValue) const
{
	return InValue;
}

void USsTestObject::FuncBlueprintNativeRef_Implementation(FSsTestStruct& InOutStruct) const
{
}

int32 USsTestObject::CallFuncBlueprintImplementable(const int32 InValue) const
{
	return FuncBlueprintImplementable(InValue);
}

int32 USsTestObject::CallFuncBlueprintNative(const int32 InValue) const
{
	return FuncBlueprintNative(InValue);
}

void USsTestObject::CallFuncBlueprintNativeRef(FSsTestStruct& InOutStruct) const
{
	return FuncBlueprintNativeRef(InOutStruct);
}

void USsTestObject::FuncTakingSsTestStruct(const FSsTestStruct& InStruct) const
{
}

void USsTestObject::LegacyFuncTakingSsTestStruct(const FSsTestStruct& InStruct) const
{
	FuncTakingSsTestStruct(InStruct);
}

void USsTestObject::FuncTakingSsTestStructDefault(const FSsTestStruct& InStruct)
{
}

int32 USsTestObject::FuncTakingSsTestDelegate(const FSsTestDelegate& InDelegate, const int32 InValue) const
{
	return InDelegate.IsBound() ? InDelegate.Execute(InValue) : INDEX_NONE;
}

void USsTestObject::FuncTakingFieldPath(const TFieldPath<FProperty>& InFieldPath)
{
	FieldPath = InFieldPath;
}

void USsTestObject::MulticastDelegatePropertyCallback(FString InStr)
{
	String = InStr;
}

TArray<int32> USsTestObject::ReturnArray()
{
	TArray<int32> TmpArray;
	TmpArray.Add(10);
	return TmpArray;
}

TSet<int32> USsTestObject::ReturnSet()
{
	TSet<int32> TmpSet;
	TmpSet.Add(10);
	return TmpSet;
}

TMap<int32, bool> USsTestObject::ReturnMap()
{
	TMap<int32, bool> TmpMap;
	TmpMap.Add(10, true);
	return TmpMap;
}

TFieldPath<FProperty> USsTestObject::ReturnFieldPath()
{
	return TFieldPath<FProperty>(USsTestObject::StaticClass()->FindPropertyByName(TEXT("FieldPath")));
}

void USsTestObject::EmitScriptError()
{
	FFrame::KismetExecutionMessage(TEXT("EmitScriptError was called"), ELogVerbosity::Error);
}

void USsTestObject::EmitScriptWarning()
{
	FFrame::KismetExecutionMessage(TEXT("EmitScriptWarning was called"), ELogVerbosity::Warning);
}

int32 USsTestObject::GetConstantValue()
{
	return 10;
}

void USsTestObject::SetInt(int InValue)
{
	Int = InValue;
}

int USsTestObject::GetInt() const
{
	return Int;
}

int32 USsTestObject::FuncInterface(const int32 InValue) const
{
	return InValue;
}

int32 USsTestObject::FuncInterfaceChild(const int32 InValue) const
{
	return InValue;
}

int32 USsTestObject::FuncInterfaceOther(const int32 InValue) const
{
	return InValue;
}
