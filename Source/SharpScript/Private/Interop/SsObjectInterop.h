#pragma once
#include "SsNativeFuncExporter.h"
#include "SsObjectInterop.generated.h"

/**
 * Provides interop methods for UObject.
 */
UCLASS()
class USsObjectInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static int IsValid(const UObject* Object);

	static FName GetName(const UObject* Object);

	static uint32 GetUniqueId(const UObject* Object);

	static const void* GetClass(const UObject* Object);

	static const void* GetOuter(const UObject* Object);

	static uint32 GetFlags(const UObject* Object);

	static const void* GetPackage(const UObject* Object);

	static int InvokeFunctionCall(UObject* InObj, const UFunction* InFunc, void* InBaseParamsAddr);

	static int InvokeStaticFunctionCall(const UClass* InClass, const UFunction* InFunc, void* InBaseParamsAddr);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
