#include "SsMulticastDelegateInterop.h"

int USsMulticastDelegateInterop::IsBound(const FMulticastScriptDelegate* MulticastDelegate)
{
	return MulticastDelegate->IsBound() ? 1 : 0;
}

int USsMulticastDelegateInterop::ContainsUFunction(const FMulticastScriptDelegate* MulticastDelegate, UObject* InObject, const FName& InFunctionName)
{
	FScriptDelegate ScriptDelegate;
	ScriptDelegate.BindUFunction(InObject, InFunctionName);
	return MulticastDelegate->Contains(ScriptDelegate) ? 1 : 0;
}

void USsMulticastDelegateInterop::AddUFunction(FMulticastScriptDelegate* MulticastDelegate, UObject* InObject, const FName& InFunctionName)
{
	FScriptDelegate ScriptDelegate;
	ScriptDelegate.BindUFunction(InObject, InFunctionName);
	MulticastDelegate->Add(ScriptDelegate);
}

void USsMulticastDelegateInterop::AddUniqueUFunction(FMulticastScriptDelegate* MulticastDelegate, UObject* InObject, const FName& InFunctionName)
{
	FScriptDelegate ScriptDelegate;
	ScriptDelegate.BindUFunction(InObject, InFunctionName);
	MulticastDelegate->AddUnique(ScriptDelegate);
}

void USsMulticastDelegateInterop::RemoveUFunction(FMulticastScriptDelegate* MulticastDelegate, const UObject* InObject, const FName& InFunctionName)
{
	MulticastDelegate->Remove(InObject, InFunctionName);
}

void USsMulticastDelegateInterop::RemoveAll(FMulticastScriptDelegate* MulticastDelegate, const UObject* InObject)
{
	MulticastDelegate->RemoveAll(InObject);
}

void USsMulticastDelegateInterop::Clear(FMulticastScriptDelegate* MulticastDelegate)
{
	MulticastDelegate->Clear();
}

void USsMulticastDelegateInterop::ProcessMulticastDelegate(const FMulticastScriptDelegate* MulticastDelegate, void* Parameters)
{
	MulticastDelegate->ProcessMulticastDelegate<UObject>(Parameters);
}

void USsMulticastDelegateInterop::MulticastDelegateToString(const FMulticastScriptDelegate* MulticastDelegate, FString& OutString)
{
	OutString = MulticastDelegate->ToString<UObject>();
}

void USsMulticastDelegateInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&IsBound, TEXT("MulticastDelegateInterop.IsBound"));
	BindNativeCallbackFunc(&ContainsUFunction, TEXT("MulticastDelegateInterop.ContainsUFunction"));
	BindNativeCallbackFunc(&AddUFunction, TEXT("MulticastDelegateInterop.AddUFunction"));
	BindNativeCallbackFunc(&RemoveUFunction, TEXT("MulticastDelegateInterop.RemoveUFunction"));
	BindNativeCallbackFunc(&RemoveAll, TEXT("MulticastDelegateInterop.RemoveAll"));
	BindNativeCallbackFunc(&Clear, TEXT("MulticastDelegateInterop.Clear"));
	BindNativeCallbackFunc(&ProcessMulticastDelegate, TEXT("MulticastDelegateInterop.ProcessMulticastDelegate"));
	BindNativeCallbackFunc(&MulticastDelegateToString, TEXT("MulticastDelegateInterop.MulticastDelegateToString"));
}
