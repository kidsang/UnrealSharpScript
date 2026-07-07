#include "SsClassInterop.h"
#include "SsHouseKeeper.h"

uint32 USsClassInterop::GetClassFlags(const UClass* InClass)
{
	return InClass->GetClassFlags();
}

const void* USsClassInterop::GetSuperClass(const UClass* InClass)
{
	if (const UClass* SuperClass = InClass->GetSuperClass())
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
	if (const UObject* CDO = InClass->GetDefaultObject(static_cast<bool>(bCreateIfNeeded)))
	{
		return USsHouseKeeper::GetManagedObject(CDO);
	}
	return nullptr;
}

int USsClassInterop::HasMetaData(const UClass* InClass, const TCHAR* Key)
{
#if WITH_EDITORONLY_DATA
	return InClass->HasMetaData(Key) ? 1 : 0;
#else
	return 0;
#endif
}

void USsClassInterop::GetMetaData(const UClass* InClass, const TCHAR* Key, FString& OutValue)
{
#if WITH_EDITORONLY_DATA
	if (InClass->HasMetaData(Key))
	{
		OutValue = InClass->GetMetaData(Key);
		return;
	}
#endif
	OutValue.Reset();
}

FName USsClassInterop::GetClassConfigName(const UClass* InClass)
{
	return InClass->ClassConfigName;
}

void USsClassInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&GetClassFlags, TEXT("ClassInterop.GetClassFlags"));
	BindNativeCallbackFunc(&GetSuperClass, TEXT("ClassInterop.GetSuperClass"));
	BindNativeCallbackFunc(&IsChildOf, TEXT("ClassInterop.IsChildOf"));
	BindNativeCallbackFunc(&ImplementsInterface, TEXT("ClassInterop.ImplementsInterface"));
	BindNativeCallbackFunc(&GetDefaultObject, TEXT("ClassInterop.GetDefaultObject"));
	BindNativeCallbackFunc(&HasMetaData, TEXT("ClassInterop.HasMetaData"));
	BindNativeCallbackFunc(&GetMetaData, TEXT("ClassInterop.GetMetaData"));
	BindNativeCallbackFunc(&GetClassConfigName, TEXT("ClassInterop.GetClassConfigName"));
}
