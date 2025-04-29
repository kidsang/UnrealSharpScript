#include "SsClassInterop.h"
#include "SsHouseKeeper.h"

uint32 USsClassInterop::GetClassFlags(const UClass* InClass)
{
	return InClass->GetClassFlags();
}

const void* USsClassInterop::GetSuperClass(const UClass* InClass)
{
	const UClass* SuperClass = InClass->GetSuperClass();
	if (SuperClass)
	{
		return USsHouseKeeper::GetManagedObject(SuperClass);
	}
	return nullptr;
}

int USsClassInterop::IsChildOf(const UClass* InClass, const UClass* InOther)
{
	bool bResult = InClass->IsChildOf(InOther);
	return bResult ? 1 : 0;
}

int USsClassInterop::ImplementsInterface(const UClass* InClass, const UClass* InOther)
{
	bool bResult = InClass->ImplementsInterface(InOther);
	return bResult ? 1 : 0;
}

const void* USsClassInterop::GetDefaultObject(const UClass* InClass, int bCreateIfNeeded)
{
	const UObject* CDO = InClass->GetDefaultObject(bool(bCreateIfNeeded));
	if (CDO)
	{
		return USsHouseKeeper::GetManagedObject(CDO);
	}
	return nullptr;
}

void USsClassInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&GetClassFlags, TEXT("ClassInterop.GetClassFlags"));
	BindNativeCallbackFunc(&GetSuperClass, TEXT("ClassInterop.GetSuperClass"));
	BindNativeCallbackFunc(&IsChildOf, TEXT("ClassInterop.IsChildOf"));
	BindNativeCallbackFunc(&ImplementsInterface, TEXT("ClassInterop.ImplementsInterface"));
	BindNativeCallbackFunc(&GetDefaultObject, TEXT("ClassInterop.GetDefaultObject"));
}
