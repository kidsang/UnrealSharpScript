#include "SsTypeRegistry.h"
#include "SsCommon.h"

bool USsTypeRegistry::Initialize()
{
	return true;
}

void USsTypeRegistry::Finalize()
{
}

const void* USsTypeRegistry::GetManagedClassType(const UClass* InClass)
{
	check(IsInGameThread());
	const void** Result = NativeToManagedClassMap.Find(InClass);
	if (Result)
	{
		return *Result;
	}

	Result = UnresolvedManagedClasses.Find(InClass->GetFName());
	if (Result)
	{
		NativeToManagedClassMap.Add(InClass, *Result);
		UnresolvedManagedClasses.Remove(InClass->GetFName());
		return *Result;
	}

	return nullptr;
}

void USsTypeRegistry::RegisterClassType(const UClass* InClass, const void* InManagedType)
{
	check(IsInGameThread());
	check(NativeToManagedClassMap.Find(InClass) == nullptr);
	NativeToManagedClassMap.Add(InClass, InManagedType);
}

void USsTypeRegistry::RegisterClassTypeByName(const TCHAR* InClassName, const void* InManagedType)
{
	check(IsInGameThread());
	const UClass* Class = FindFirstObject<UClass>(InClassName);
	if (!Class)
	{
		FName ClassName = InClassName;
		check(UnresolvedManagedClasses.Find(ClassName) == nullptr);
		UnresolvedManagedClasses.Add(ClassName, InManagedType);
		return;
	}

	check(NativeToManagedClassMap.Find(Class) == nullptr);
	NativeToManagedClassMap.Add(Class, InManagedType);
}

void USsTypeRegistry::UnregisterClassType(const UClass* InClass)
{
	check(IsInGameThread());
	NativeToManagedClassMap.Remove(InClass);
}

void USsTypeRegistry::UnregisterClassTypeByName(const TCHAR* InClassName)
{
	check(IsInGameThread());
	const UClass* Class = FindFirstObject<UClass>(InClassName);
	if (!Class)
	{
		UnresolvedManagedClasses.Remove(InClassName);
		return;
	}

	NativeToManagedClassMap.Remove(Class);
}

void USsTypeRegistry::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&RegisterClassType, TEXT("TypeRegistry.NativeRegisterClassType"));
	BindNativeCallbackFunc(&RegisterClassTypeByName, TEXT("TypeRegistry.NativeRegisterClassTypeByName"));
	BindNativeCallbackFunc(&UnregisterClassType, TEXT("TypeRegistry.NativeUnregisterClassType"));
	BindNativeCallbackFunc(&UnregisterClassTypeByName, TEXT("TypeRegistry.NativeUnegisterClassTypeByName"));
	BindNativeCallbackFunc(&GetManagedClassType, TEXT("TypeRegistry.NativeGetManagedClassType"));
}

TMap<const UClass*, const void*> USsTypeRegistry::NativeToManagedClassMap;
TMap<FName, const void*> USsTypeRegistry::UnresolvedManagedClasses;
