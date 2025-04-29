#include "SsSoftObjectPathInterop.h"
#include "UObject/SoftObjectPath.h"
#include "SsHouseKeeper.h"

const void* USsSoftObjectPathInterop::TryLoad(const FTopLevelAssetPath& InAssetPath, const TCHAR* InSubPathString)
{
	FSoftObjectPath SoftObjectPath(InAssetPath, InSubPathString);
	UObject* Object = SoftObjectPath.TryLoad();
	if (!Object)
	{
		return nullptr;
	}

	return USsHouseKeeper::GetManagedObject(Object);
}

const void* USsSoftObjectPathInterop::ResolveObject(const FTopLevelAssetPath& InAssetPath, const TCHAR* InSubPathString)
{
	FSoftObjectPath SoftObjectPath(InAssetPath, InSubPathString);
	UObject* Object = SoftObjectPath.ResolveObject();
	if (!Object)
	{
		return nullptr;
	}

	return USsHouseKeeper::GetManagedObject(Object);
}

void USsSoftObjectPathInterop::GetOrCreateIdForObject(const UObject* Object, FSoftObjectPath& OutSoftObjectPath)
{
	OutSoftObjectPath = FSoftObjectPath::GetOrCreateIDForObject(Object);
}

void USsSoftObjectPathInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&TryLoad, TEXT("SoftObjectPathInterop.TryLoad"));
	BindNativeCallbackFunc(&ResolveObject, TEXT("SoftObjectPathInterop.ResolveObject"));
	BindNativeCallbackFunc(&GetOrCreateIdForObject, TEXT("SoftObjectPathInterop.GetOrCreateIdForObject"));
}
