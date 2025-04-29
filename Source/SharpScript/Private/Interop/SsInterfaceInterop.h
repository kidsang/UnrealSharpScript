#pragma once
#include "SsNativeFuncExporter.h"
#include "SsInterfaceInterop.generated.h"

/**
 * Provides interop methods for UInterface.
 */
UCLASS()
class USsInterfaceInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static void SetObjectAndInterface(FScriptInterface* ScriptInterface, UObject* InObject, UClass* InInterfaceClass);

	static const void* GetObject(const FScriptInterface* ScriptInterface);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
