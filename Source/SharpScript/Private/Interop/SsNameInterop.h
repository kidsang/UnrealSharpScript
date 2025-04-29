#pragma once
#include "SsNativeFuncExporter.h"
#include "SsNameInterop.generated.h"

/**
 * Provides interop methods for FName.
 */
UCLASS()
class USsNameInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static void NameToString(FName Name, FString& OutString);

	static void StringToName(FName& Name, const TCHAR* String);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
