#include "SsDelegateInterop.h"

#include "SsHouseKeeper.h"

void USsDelegateInterop::BindUFunction(FScriptDelegate& ScriptDelegate, UObject* InObject, const FName& InFunctionName)
{
	ScriptDelegate.BindUFunction(InObject, InFunctionName);
}

int USsDelegateInterop::IsBound(const FScriptDelegate& ScriptDelegate)
{
	return ScriptDelegate.IsBound() ? 1 : 0;
}

int USsDelegateInterop::IsBoundToObject(const FScriptDelegate& ScriptDelegate, const UObject* InObject)
{
	return ScriptDelegate.IsBoundToObject(InObject) ? 1 : 0;
}

void USsDelegateInterop::Unbind(FScriptDelegate& ScriptDelegate)
{
	ScriptDelegate.Unbind();
}

void USsDelegateInterop::DelegateToString(const FScriptDelegate& ScriptDelegate, FString& OutString)
{
	OutString = ScriptDelegate.ToString<UObject>();
}

const void* USsDelegateInterop::GetUObject(const FScriptDelegate& ScriptDelegate)
{
	const UObject* Object = ScriptDelegate.GetUObject();
	if (Object)
	{
		return USsHouseKeeper::GetManagedObject(Object);
	}
	return nullptr;
}

FName USsDelegateInterop::GetFunctionName(const FScriptDelegate& ScriptDelegate)
{
	return ScriptDelegate.GetFunctionName();
}

void USsDelegateInterop::ProcessDelegate(const FScriptDelegate& ScriptDelegate, void* Parameters)
{
	ScriptDelegate.ProcessDelegate<UObject>(Parameters);
}

int USsDelegateInterop::DelegateEquals(const FScriptDelegate& ScriptDelegate, const FScriptDelegate& Other)
{
	return ScriptDelegate == Other ? 1 : 0;
}

uint32 USsDelegateInterop::DoGetTypeHash(const FScriptDelegate& ScriptDelegate)
{
	return GetTypeHash(ScriptDelegate);
}

void USsDelegateInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&BindUFunction, TEXT("DelegateInterop.BindUFunction"));
	BindNativeCallbackFunc(&IsBound, TEXT("DelegateInterop.IsBound"));
	BindNativeCallbackFunc(&IsBoundToObject, TEXT("DelegateInterop.IsBoundToObject"));
	BindNativeCallbackFunc(&Unbind, TEXT("DelegateInterop.Unbind"));
	BindNativeCallbackFunc(&DelegateToString, TEXT("DelegateInterop.DelegateToString"));
	BindNativeCallbackFunc(&GetUObject, TEXT("DelegateInterop.GetUObject"));
	BindNativeCallbackFunc(&GetFunctionName, TEXT("DelegateInterop.GetFunctionName"));
	BindNativeCallbackFunc(&ProcessDelegate, TEXT("DelegateInterop.ProcessDelegate"));
	BindNativeCallbackFunc(&DelegateEquals, TEXT("DelegateInterop.DelegateEquals"));
	BindNativeCallbackFunc(&DoGetTypeHash, TEXT("DelegateInterop.DoGetTypeHash"));
}
