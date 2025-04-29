#include "SsUniqueObjectGuidInterop.h"
#include "SsHouseKeeper.h"

const void* USsUniqueObjectGuidInterop::ResolveObject(const FUniqueObjectGuid& InUniqueObjectGuid)
{
	UObject* Object = InUniqueObjectGuid.ResolveObject();
	if (!Object)
	{
		return nullptr;
	}

	return USsHouseKeeper::GetManagedObject(Object);
}

void USsUniqueObjectGuidInterop::GetOrCreateIdForObject(const UObject* Object, FUniqueObjectGuid& OutUniqueObjectGuid)
{
	OutUniqueObjectGuid = FUniqueObjectGuid::GetOrCreateIDForObject(Object);
}

void USsUniqueObjectGuidInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&ResolveObject, TEXT("UniqueObjectGuidInterop.ResolveObject"));
	BindNativeCallbackFunc(&GetOrCreateIdForObject, TEXT("UniqueObjectGuidInterop.GetOrCreateIdForObject"));
}
