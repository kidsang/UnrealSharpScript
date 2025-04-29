#include "SsArrayInterop.h"

void USsArrayInterop::Destroy(FScriptArray* Instance)
{
	Instance->~FScriptArray();
}

void USsArrayInterop::EmptyValues(const FArrayProperty* ArrayProp, const FScriptArray* Instance)
{
	FScriptArrayHelper Helper(ArrayProp, Instance);
	Helper.EmptyValues();
}

void USsArrayInterop::EmptyAndAddValues(const FArrayProperty* ArrayProp, const FScriptArray* Instance, int Count)
{
	FScriptArrayHelper Helper(ArrayProp, Instance);
	Helper.EmptyAndAddValues(Count);
}

int32 USsArrayInterop::AddValue(const FArrayProperty* ArrayProp, const FScriptArray* Instance)
{
	FScriptArrayHelper Helper(ArrayProp, Instance);
	return Helper.AddValue();
}

void USsArrayInterop::InsertValue(const FArrayProperty* ArrayProp, const FScriptArray* Instance, int Index)
{
	FScriptArrayHelper Helper(ArrayProp, Instance);
	Helper.InsertValues(Index);
}

void USsArrayInterop::RemoveValue(const FArrayProperty* ArrayProp, const FScriptArray* Instance, int Index)
{
	FScriptArrayHelper Helper(ArrayProp, Instance);
	Helper.RemoveValues(Index);
}

void USsArrayInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&Destroy, TEXT("ArrayInterop.Destroy"));
	BindNativeCallbackFunc(&EmptyValues, TEXT("ArrayInterop.EmptyValues"));
	BindNativeCallbackFunc(&EmptyAndAddValues, TEXT("ArrayInterop.EmptyAndAddValues"));
	BindNativeCallbackFunc(&AddValue, TEXT("ArrayInterop.AddValue"));
	BindNativeCallbackFunc(&InsertValue, TEXT("ArrayInterop.InsertValue"));
	BindNativeCallbackFunc(&RemoveValue, TEXT("ArrayInterop.RemoveValue"));
}
