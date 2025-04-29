#pragma once
#include "SsNativeFuncExporter.h"
#include "SsMapInterop.generated.h"

/**
 * Provides interop methods for TMap.
 */
UCLASS()
class USsMapInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static int GetNum(const FMapProperty* MapProp, const void* MapPtr);

	static int GetMaxIndex(const FMapProperty* MapProp, const void* MapPtr);

	static int IsValidIndex(const FMapProperty* MapProp, const void* MapPtr, int Index);

	static int GetPairPtr(const FMapProperty* MapProp, const void* MapPtr, int Index, void*& OutKeyPtr, void*& OutValPtr);

	static int FindMapIndexWithKey(const FMapProperty* MapProp, const void* MapPtr, const void* Key);

	static void EmptyValues(const FMapProperty* MapProp, const void* MapPtr, int Slack);

	static void Rehash(const FMapProperty* MapProp, const void* MapPtr);

	static void AddPair(const FMapProperty* MapProp, const void* MapPtr, const void* KeyPtr, const void* ValPtr);

	static void RemoveAt(const FMapProperty* MapProp, const void* MapPtr, int Index);

	static void AddDefaultValueAndGetPair(const FMapProperty* MapProp, const void* MapPtr, void*& OutKeyPtr, void*& OutValPtr);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
