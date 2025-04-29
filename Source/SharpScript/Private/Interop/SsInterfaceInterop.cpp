#include "SsInterfaceInterop.h"
#include "SsHouseKeeper.h"

void USsInterfaceInterop::SetObjectAndInterface(FScriptInterface* ScriptInterface, UObject* InObject, UClass* InInterfaceClass)
{
	ScriptInterface->SetObject(InObject);
	ScriptInterface->SetInterface(InObject->GetInterfaceAddress(InInterfaceClass));
}

const void* USsInterfaceInterop::GetObject(const FScriptInterface* ScriptInterface)
{
	UObject* Object = ScriptInterface->GetObject();
	if (Object)
	{
		return USsHouseKeeper::GetManagedObject(Object);
	}
	return nullptr;
}

void USsInterfaceInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&SetObjectAndInterface, TEXT("InterfaceInterop.SetObjectAndInterface"));
	BindNativeCallbackFunc(&GetObject, TEXT("InterfaceInterop.GetObject"));
}
