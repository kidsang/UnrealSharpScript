#include "SsMapInterop.h"

int USsMapInterop::GetNum(const FMapProperty* MapProp, const void* MapPtr)
{
	FScriptMapHelper Helper(MapProp, MapPtr);
	return Helper.Num();
}

int USsMapInterop::GetMaxIndex(const FMapProperty* MapProp, const void* MapPtr)
{
	FScriptMapHelper Helper(MapProp, MapPtr);
	return Helper.GetMaxIndex();
}

int USsMapInterop::IsValidIndex(const FMapProperty* MapProp, const void* MapPtr, int Index)
{
	FScriptMapHelper Helper(MapProp, MapPtr);
	bool bResult = Helper.IsValidIndex(Index);
	return bResult ? 1 : 0;
}

int USsMapInterop::GetPairPtr(const FMapProperty* MapProp, const void* MapPtr, int Index, void*& OutKeyPtr, void*& OutValPtr)
{
	FScriptMapHelper Helper(MapProp, MapPtr);
	uint8* PairPtr = Helper.GetPairPtr(Index);
	if (!PairPtr)
	{
		OutKeyPtr = nullptr;
		OutValPtr = nullptr;
		return 0;
	}

	OutKeyPtr = PairPtr + MapProp->KeyProp->GetOffset_ForInternal();
	OutValPtr = PairPtr + MapProp->ValueProp->GetOffset_ForInternal();
	return 1;
}

int USsMapInterop::FindMapIndexWithKey(const FMapProperty* MapProp, const void* MapPtr, const void* Key)
{
	FScriptMapHelper Helper(MapProp, MapPtr);
	return Helper.FindMapIndexWithKey(Key);
}

void USsMapInterop::EmptyValues(const FMapProperty* MapProp, const void* MapPtr, int Slack)
{
	FScriptMapHelper Helper(MapProp, MapPtr);
	Helper.EmptyValues(Slack);
}

void USsMapInterop::Rehash(const FMapProperty* MapProp, const void* MapPtr)
{
	FScriptMapHelper Helper(MapProp, MapPtr);
	Helper.Rehash();
}

void USsMapInterop::AddPair(const FMapProperty* MapProp, const void* MapPtr, const void* KeyPtr, const void* ValPtr)
{
	FScriptMapHelper Helper(MapProp, MapPtr);
	Helper.AddPair(KeyPtr, ValPtr);
}

void USsMapInterop::RemoveAt(const FMapProperty* MapProp, const void* MapPtr, int Index)
{
	FScriptMapHelper Helper(MapProp, MapPtr);
	Helper.RemoveAt(Index);
}

void USsMapInterop::AddDefaultValueAndGetPair(const FMapProperty* MapProp, const void* MapPtr, void*& OutKeyPtr, void*& OutValPtr)
{
	FScriptMapHelper Helper(MapProp, MapPtr);
	int Index = Helper.AddDefaultValue_Invalid_NeedsRehash();
	uint8* PairPtr = Helper.GetPairPtr(Index);
	check(PairPtr);
	OutKeyPtr = PairPtr + MapProp->KeyProp->GetOffset_ForInternal();
	OutValPtr = PairPtr + MapProp->ValueProp->GetOffset_ForInternal();
}

void USsMapInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&GetNum, TEXT("MapInterop.GetNum"));
	BindNativeCallbackFunc(&GetMaxIndex, TEXT("MapInterop.GetMaxIndex"));
	BindNativeCallbackFunc(&IsValidIndex, TEXT("MapInterop.IsValidIndex"));
	BindNativeCallbackFunc(&GetPairPtr, TEXT("MapInterop.GetPairPtr"));
	BindNativeCallbackFunc(&FindMapIndexWithKey, TEXT("MapInterop.FindMapIndexWithKey"));
	BindNativeCallbackFunc(&EmptyValues, TEXT("MapInterop.EmptyValues"));
	BindNativeCallbackFunc(&Rehash, TEXT("MapInterop.Rehash"));
	BindNativeCallbackFunc(&AddPair, TEXT("MapInterop.AddPair"));
	BindNativeCallbackFunc(&RemoveAt, TEXT("MapInterop.RemoveAt"));
	BindNativeCallbackFunc(&AddDefaultValueAndGetPair, TEXT("MapInterop.AddDefaultValueAndGetPair"));
}
