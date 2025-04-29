#include "SsSetInterop.h"

int USsSetInterop::GetNum(const FSetProperty* SetProp, const void* SetPtr)
{
	FScriptSetHelper Helper(SetProp, SetPtr);
	return Helper.Num();
}

int USsSetInterop::GetMaxIndex(const FSetProperty* SetProp, const void* SetPtr)
{
	FScriptSetHelper Helper(SetProp, SetPtr);
	return Helper.GetMaxIndex();
}

int USsSetInterop::FindElementIndexFromHash(const FSetProperty* SetProp, const void* SetPtr, const void* elemPtr)
{
	FScriptSetHelper Helper(SetProp, SetPtr);
	return Helper.FindElementIndexFromHash(elemPtr);
}

int USsSetInterop::IsValidIndex(const FSetProperty* SetProp, const void* SetPtr, int Index)
{
	FScriptSetHelper Helper(SetProp, SetPtr);
	bool bResult = Helper.IsValidIndex(Index);
	return bResult ? 1 : 0;
}

void* USsSetInterop::GetElementPtr(const FSetProperty* SetProp, const void* SetPtr, int Index)
{
	FScriptSetHelper Helper(SetProp, SetPtr);
	return Helper.GetElementPtr(Index);
}

void USsSetInterop::AddElement(const FSetProperty* SetProp, const void* SetPtr, const void* elemPtr)
{
	FScriptSetHelper Helper(SetProp, SetPtr);
	Helper.AddElement(elemPtr);
}

void USsSetInterop::RemoveAt(const FSetProperty* SetProp, const void* SetPtr, int Index)
{
	FScriptSetHelper Helper(SetProp, SetPtr);
	Helper.RemoveAt(Index);
}

void USsSetInterop::EmptyElements(const FSetProperty* SetProp, const void* SetPtr, int Slack)
{
	FScriptSetHelper Helper(SetProp, SetPtr);
	Helper.EmptyElements(Slack);
}

void USsSetInterop::Rehash(const FSetProperty* SetProp, const void* SetPtr)
{
	FScriptSetHelper Helper(SetProp, SetPtr);
	Helper.Rehash();
}

void* USsSetInterop::AddDefaultValueAndGetPtr(const FSetProperty* SetProp, const void* SetPtr)
{
	FScriptSetHelper Helper(SetProp, SetPtr);
	int Index = Helper.AddDefaultValue_Invalid_NeedsRehash();
	return Helper.GetElementPtr(Index);
}

void USsSetInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&GetNum, TEXT("SetInterop.GetNum"));
	BindNativeCallbackFunc(&GetMaxIndex, TEXT("SetInterop.GetMaxIndex"));
	BindNativeCallbackFunc(&FindElementIndexFromHash, TEXT("SetInterop.FindElementIndexFromHash"));
	BindNativeCallbackFunc(&IsValidIndex, TEXT("SetInterop.IsValidIndex"));
	BindNativeCallbackFunc(&GetElementPtr, TEXT("SetInterop.GetElementPtr"));
	BindNativeCallbackFunc(&AddElement, TEXT("SetInterop.AddElement"));
	BindNativeCallbackFunc(&RemoveAt, TEXT("SetInterop.RemoveAt"));
	BindNativeCallbackFunc(&EmptyElements, TEXT("SetInterop.EmptyElements"));
	BindNativeCallbackFunc(&Rehash, TEXT("SetInterop.Rehash"));
	BindNativeCallbackFunc(&AddDefaultValueAndGetPtr, TEXT("SetInterop.AddDefaultValueAndGetPtr"));
}
