#pragma once
#include "SsNativeFuncExporter.h"
#include "SsSetInterop.generated.h"

/**
 * Provides interop methods for TSet.
 */
UCLASS()
class USsSetInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static int GetNum(const FSetProperty* SetProp, const void* SetPtr);

	static int GetMaxIndex(const FSetProperty* SetProp, const void* SetPtr);

	static int FindElementIndexFromHash(const FSetProperty* SetProp, const void* SetPtr, const void* elemPtr);

	static int IsValidIndex(const FSetProperty* SetProp, const void* SetPtr, int Index);

	static void* GetElementPtr(const FSetProperty* SetProp, const void* SetPtr, int Index);

	static void AddElement(const FSetProperty* SetProp, const void* SetPtr, const void* elemPtr);

	static void RemoveAt(const FSetProperty* SetProp, const void* SetPtr, int Index);

	static void EmptyElements(const FSetProperty* SetProp, const void* SetPtr, int Slack);

	static void Rehash(const FSetProperty* SetProp, const void* SetPtr);

	static void* AddDefaultValueAndGetPtr(const FSetProperty* SetProp, const void* SetPtr);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
