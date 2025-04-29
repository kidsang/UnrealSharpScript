#include "SsObjectInterop.h"

#include "SsCommon.h"
#include "SsHouseKeeper.h"

int USsObjectInterop::IsValid(const UObject* Object)
{
	if (::IsValid(Object))
	{
		return 1;
	}
	return 0;
}

FName USsObjectInterop::GetName(const UObject* Object)
{
	return Object->GetFName();
}

uint32 USsObjectInterop::GetUniqueId(const UObject* Object)
{
	return Object->GetUniqueID();
}

const void* USsObjectInterop::GetClass(const UObject* Object)
{
	const UClass* Class = Object->GetClass();
	return USsHouseKeeper::GetManagedObject(Class);
}

const void* USsObjectInterop::GetOuter(const UObject* Object)
{
	const UObject* Outer = Object->GetOuter();
	return USsHouseKeeper::GetManagedObject(Outer);
}

uint32 USsObjectInterop::GetFlags(const UObject* Object)
{
	return Object->GetFlags();
}

const void* USsObjectInterop::GetPackage(const UObject* Object)
{
	const UPackage* Package = Object->GetPackage();
	return USsHouseKeeper::GetManagedObject((const UObject*)Package);
}

int USsObjectInterop::InvokeFunctionCall(UObject* InObj, const UFunction* InFunc, void* InBaseParamsAddr)
{
	bool bThrewException = false;
	FScopedScriptExceptionHandler ExceptionHandler([InObj, &bThrewException](ELogVerbosity::Type Verbosity, const TCHAR* ExceptionMessage, const TCHAR* StackMessage)
	{
		if (Verbosity == ELogVerbosity::Error)
		{
			UE_LOG(LogSharpScript, Error, TEXT("%s: %s"), *InObj->GetName(), ExceptionMessage);
			bThrewException = true;
		}
		else if (Verbosity == ELogVerbosity::Warning)
		{
			UE_LOG(LogSharpScript, Warning, TEXT("%s: %s"), *InObj->GetName(), ExceptionMessage);
		}
		else
		{
#if !NO_LOGGING
			FMsg::Logf_Internal(__FILE__, __LINE__, LogSharpScript.GetCategoryName(), Verbosity, TEXT("%s"), ExceptionMessage);
#endif
		}
	});

#if WITH_EDITOR
	FEditorScriptExecutionGuard ScriptGuard;
#endif // WITH_EDITOR
	InObj->ProcessEvent((UFunction*)InFunc, InBaseParamsAddr);

	return bThrewException ? 0 : 1;
}

int USsObjectInterop::InvokeStaticFunctionCall(const UClass* InClass, const UFunction* InFunc, void* InBaseParamsAddr)
{
	return InvokeFunctionCall(InClass->ClassDefaultObject, InFunc, InBaseParamsAddr);
}

void USsObjectInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&IsValid, TEXT("ObjectInterop.IsValid"));
	BindNativeCallbackFunc(&GetName, TEXT("ObjectInterop.GetName"));
	BindNativeCallbackFunc(&GetUniqueId, TEXT("ObjectInterop.GetUniqueId"));
	BindNativeCallbackFunc(&GetClass, TEXT("ObjectInterop.GetClass"));
	BindNativeCallbackFunc(&GetOuter, TEXT("ObjectInterop.GetOuter"));
	BindNativeCallbackFunc(&GetFlags, TEXT("ObjectInterop.GetFlags"));
	BindNativeCallbackFunc(&GetPackage, TEXT("ObjectInterop.GetPackage"));
	BindNativeCallbackFunc(&InvokeFunctionCall, TEXT("ObjectInterop.InvokeFunctionCall"));
	BindNativeCallbackFunc(&InvokeStaticFunctionCall, TEXT("ObjectInterop.InvokeStaticFunctionCall"));
}
