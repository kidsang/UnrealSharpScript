#include "SsWeakObjectPtrInterop.h"
#include "SsHouseKeeper.h"

void USsWeakObjectPtrInterop::SetObject(TWeakObjectPtr<UObject>& WeakObject, UObject* Object)
{
	WeakObject = Object;
}

const void* USsWeakObjectPtrInterop::GetObject(TWeakObjectPtr<UObject> WeakObjectPtr)
{
	if (!WeakObjectPtr.IsValid())
	{
		return nullptr;
	}

	UObject* Object = WeakObjectPtr.Get();
	return USsHouseKeeper::GetManagedObject(Object);
}

int USsWeakObjectPtrInterop::IsValid(TWeakObjectPtr<UObject> WeakObjectPtr)
{
	return WeakObjectPtr.IsValid() ? 1 : 0;
}

int USsWeakObjectPtrInterop::IsStale(TWeakObjectPtr<UObject> WeakObjectPtr)
{
	return WeakObjectPtr.IsStale() ? 1 : 0;
}

void USsWeakObjectPtrInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&SetObject, TEXT("WeakObjectPtrInterop.SetObject"));
	BindNativeCallbackFunc(&GetObject, TEXT("WeakObjectPtrInterop.GetObject"));
	BindNativeCallbackFunc(&IsValid, TEXT("WeakObjectPtrInterop.IsValid"));
	BindNativeCallbackFunc(&IsStale, TEXT("WeakObjectPtrInterop.IsStale"));
}
