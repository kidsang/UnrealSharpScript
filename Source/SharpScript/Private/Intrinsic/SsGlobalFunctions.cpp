#include "SsGlobalFunctions.h"
#include "SsHouseKeeper.h"
#include "Engine/Engine.h"

void USsGlobalFunctions::CollectGarbage()
{
	::CollectGarbage(GARBAGE_COLLECTION_KEEPFLAGS, true);
}

const void* USsGlobalFunctions::NewObjectSimple(const UClass* Class, FName Name)
{
	UObject* Object = ::NewObject<UObject>((UObject*)GetTransientPackage(), Class, Name);
	return USsHouseKeeper::GetManagedObject(Object);
}

const void* USsGlobalFunctions::NewObject(UObject* Outer, const UClass* Class, FName Name, EObjectFlags Flags, UObject* Template)
{
	if (Outer == nullptr)
	{
		Outer = (UObject*)GetTransientPackage();
	}

	UObject* Object = ::NewObject<UObject>(Outer, Class, Name, Flags, Template);
	return USsHouseKeeper::GetManagedObject(Object);
}

void USsGlobalFunctions::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&CollectGarbage, TEXT("Globals.NativeCollectGarbage"));
	BindNativeCallbackFunc(&NewObjectSimple, TEXT("Globals.NativeNewObjectSimple"));
	BindNativeCallbackFunc(&NewObject, TEXT("Globals.NativeNewObject"));
}
