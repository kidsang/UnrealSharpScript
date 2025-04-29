#pragma once
#include "SsNativeFuncExporter.h"
#include "SsDelegateInterop.generated.h"

/**
 * Provides interop methods for FScriptDelegate.
 */
UCLASS()
class USsDelegateInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static void BindUFunction(FScriptDelegate& ScriptDelegate, UObject* InObject, const FName& InFunctionName);

	static int IsBound(const FScriptDelegate& ScriptDelegate);

	static int IsBoundToObject(const FScriptDelegate& ScriptDelegate, const UObject* InObject);

	static void Unbind(FScriptDelegate& ScriptDelegate);

	static void DelegateToString(const FScriptDelegate& ScriptDelegate, FString& OutString);

	static const void* GetUObject(const FScriptDelegate& ScriptDelegate);

	static FName GetFunctionName(const FScriptDelegate& ScriptDelegate);

	static void ProcessDelegate(const FScriptDelegate& ScriptDelegate, void* Parameters);

	static int DelegateEquals(const FScriptDelegate& ScriptDelegate, const FScriptDelegate& Other);

	static uint32 DoGetTypeHash(const FScriptDelegate& ScriptDelegate);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
