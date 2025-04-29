#pragma once
#include "SsNativeFuncExporter.h"
#include "SsStringInterop.generated.h"

/**
 * Provides interop methods for FString.
 */
UCLASS()
class USsStringInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static void CopyToNative(FString* String, const TCHAR* ManagedString);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
