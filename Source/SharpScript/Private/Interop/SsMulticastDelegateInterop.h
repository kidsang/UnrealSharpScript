#pragma once
#include "SsNativeFuncExporter.h"
#include "SsMulticastDelegateInterop.generated.h"

/**
 * Provides interop methods for MulticastScriptDelegate.
 */
UCLASS()
class USsMulticastDelegateInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:

	static int IsBound(const FMulticastScriptDelegate* MulticastDelegate);

	static int ContainsUFunction(const FMulticastScriptDelegate* MulticastDelegate, UObject* InObject, const FName& InFunctionName);

	static void AddUFunction(FMulticastScriptDelegate* MulticastDelegate, UObject* InObject, const FName& InFunctionName);

	static void AddUniqueUFunction(FMulticastScriptDelegate* MulticastDelegate, UObject* InObject, const FName& InFunctionName);

	static void RemoveUFunction(FMulticastScriptDelegate* MulticastDelegate, const UObject* InObject, const FName& InFunctionName);

	static void RemoveAll(FMulticastScriptDelegate* MulticastDelegate, const UObject* InObject);

	static void Clear(FMulticastScriptDelegate* MulticastDelegate);

	static void ProcessMulticastDelegate(const FMulticastScriptDelegate* MulticastDelegate, void* Parameters);

	static void MulticastDelegateToString(const FMulticastScriptDelegate* MulticastDelegate, FString& OutString);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
