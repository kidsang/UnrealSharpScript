#pragma once
#include "SsNativeFuncExporter.h"
#include "SsWeakObjectPtrInterop.generated.h"

/**
 * Provides interop methods for TWeakObjectPtr.
 */
UCLASS()
class USsWeakObjectPtrInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static void SetObject(TWeakObjectPtr<UObject>& WeakObject, UObject* Object);

	static const void* GetObject(TWeakObjectPtr<UObject> WeakObjectPtr);

	static int IsValid(TWeakObjectPtr<UObject> WeakObjectPtr);

	static int IsStale(TWeakObjectPtr<UObject> WeakObjectPtr);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
