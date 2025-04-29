#pragma once
#include "SsNativeFuncExporter.h"
#include "SsArrayInterop.generated.h"

/**
 * Provides interop methods for TArray.
 */
UCLASS()
class USsArrayInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static void Destroy(FScriptArray* Instance);

	static void EmptyValues(const FArrayProperty* ArrayProp, const FScriptArray* Instance);

	static void EmptyAndAddValues(const FArrayProperty* ArrayProp, const FScriptArray* Instance, int Count);

	static int32 AddValue(const FArrayProperty* ArrayProp, const FScriptArray* Instance);

	static void InsertValue(const FArrayProperty* ArrayProp, const FScriptArray* Instance, int Index);

	static void RemoveValue(const FArrayProperty* ArrayProp, const FScriptArray* Instance, int Index);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
