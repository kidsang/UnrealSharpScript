#include "SsHouseKeeper.h"
#include "SsFrameworkUtils.h"
#include "SsTypeRegistry.h"
#include "Misc/CoreDelegates.h"

bool USsHouseKeeper::Initialize()
{
	USsHouseKeeper* HouseKeeperInstance = StaticClass()->GetDefaultObject<USsHouseKeeper>();

	// Pre-allocate the C# object marker array.
	ManagedObjectArray.Initialize(GUObjectArray.GetObjectArrayCapacity());

	// Listen for UObject destruction events to release corresponding C# objects.
	GUObjectArray.AddUObjectDeleteListener(&HouseKeeperInstance->ObjectDeleteListener);

	// Unregister the UObject destruction event listener when the engine shuts down.
	// The C# vm is destroyed earlier than most engine modules. If we don't unregister the UObject destruction event listener in time, it will continue to try to destroy C# objects after vm is destroyed, which causing a crash.
	FCoreDelegates::OnPreExit.AddUObject(HouseKeeperInstance, &USsHouseKeeper::OnEnginePreExit);

	if (!BindManagedCallbacks())
	{
		return false;
	}

	return true;
}

void USsHouseKeeper::Finalize()
{
}

const void* USsHouseKeeper::GetManagedObject(const UObject* InObject)
{
	check(IsInGameThread());
	check(InObject);

	// If a C# object has been created before, return the previously created one.
	FManagedObjectTracker* Tracker = NativeToManagedMap.Find(InObject);
	if (Tracker)
	{
		check(Tracker->ManagedObject);
		return Tracker->ManagedObject;
	}

	// Look for the C# wrapper class. If not found, continue to search for parent classes, up to UObject.
	ensure(::IsValid(InObject));
	const void* ManagedType = nullptr;
	const UClass* Class = InObject->GetClass();
	while (Class)
	{
		ManagedType = USsTypeRegistry::GetManagedClassType(Class);
		if (ManagedType)
		{
			break;
		}
		Class = Class->GetSuperClass();
	}
	check(ManagedType);

	const void* ManagedObject = ManagedCreateManagedObject(InObject, ManagedType);
	check(ManagedObject);

	FManagedObjectTracker& NewTracker = NativeToManagedMap.Emplace(InObject, ManagedObject);
#if !UE_BUILD_SHIPPING
	NewTracker.ObjectName = InObject->GetFName();
	NewTracker.ClassName = InObject->GetClass()->GetFName();
#endif // !UE_BUILD_SHIPPING

	int32 ObjectIndex = GUObjectArray.ObjectToIndex(InObject);
	check(!ManagedObjectArray.HasManagedObject(ObjectIndex)); // A UObject that hasn't created a C# object before cannot exist in this array.
	ManagedObjectArray.AddManagedObject(ObjectIndex);

	return ManagedObject;
}

bool USsHouseKeeper::BindManagedCallbacks()
{
	static TPair<const TCHAR*, void**> FuncInfos[] = {
		{TEXT("CreateManagedObject"), (void**)&ManagedCreateManagedObject},
		{TEXT("FreeManagedObject"), (void**)&ManagedFreeManagedObject},
	};

	static constexpr const TCHAR* FrameworkTypeName = TEXT("SharpScript.HouseKeeper, SharpScript");
	for (auto& Pair : FuncInfos)
	{
		if (!SsInternal::BindManagedFrameworkFunction(FrameworkTypeName, Pair.Key, Pair.Value))
		{
			return false;
		}
	}

	return true;
}

void USsHouseKeeper::OnEnginePreExit()
{
	GUObjectArray.RemoveUObjectDeleteListener(&ObjectDeleteListener);
}

void USsHouseKeeper::FreeManagedObjectsByClassName(const TCHAR* InClassName)
{
	check(IsInGameThread());
	FName ClassName(InClassName);
	TArray<const UObject*> ObjectsToFree;
	const UObject* ClassToFree = nullptr;
	for (auto& Pair : NativeToManagedMap)
	{
		if (Pair.Key->GetClass()->GetName() == ClassName)
		{
			ObjectsToFree.Add(Pair.Key);
		}
		else if (Pair.Key->GetName() == ClassName)
		{
			check(Pair.Key->GetClass() == UClass::StaticClass());
			ClassToFree = Pair.Key;
		}
	}

	if (ClassToFree)
	{
		ObjectsToFree.Add(ClassToFree);
	}

	for (const UObject* Object : ObjectsToFree)
	{
		int32 ObjectIndex = GUObjectArray.ObjectToIndex(Object);
		ManagedObjectArray.FreeManagedObject(ObjectIndex);
		FreeManagedObject(Object);
	}
}

void USsHouseKeeper::FreeManagedObject(const UObject* InObject)
{
	// check(IsInGameThread()); // Already checked in outer calls, no need to check again here.

	// Remove the C# object corresponding to the UObject
	FManagedObjectTracker Tracker = NativeToManagedMap.FindAndRemoveChecked(InObject);

	// Notify dotnet runtime to recycle the C# object
	const void* ManagedObject = Tracker.ManagedObject;
	ManagedFreeManagedObject(ManagedObject);
}

void USsHouseKeeper::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&GetManagedObject, TEXT("HouseKeeper.NativeGetManagedObject"));
	BindNativeCallbackFunc(&FreeManagedObjectsByClassName, TEXT("HouseKeeper.NativeFreeManagedObjectsByClassName"));
}

void USsHouseKeeper::FObjectListener::NotifyUObjectDeleted(const UObjectBase* InObject, int32 InObjectIndex)
{
	if (!ManagedObjectArray.HasManagedObject(InObjectIndex))
	{
		return;
	}

	ManagedObjectArray.FreeManagedObject(InObjectIndex);

	// The following codes are not thread-safe and must be called on the Game thread.
	check(IsInGameThread());
	const UObject* Object = (const UObject*)InObject;
	FreeManagedObject(Object);
}

void USsHouseKeeper::FObjectListener::OnUObjectArrayShutdown()
{
	GUObjectArray.RemoveUObjectDeleteListener(this);
}

const void* (*USsHouseKeeper::ManagedCreateManagedObject)(const UObject* InObject, const void* InManagedType) = nullptr;
void (*USsHouseKeeper::ManagedFreeManagedObject)(const void* InManagedObject) = nullptr;

TMap<const UObject*, USsHouseKeeper::FManagedObjectTracker> USsHouseKeeper::NativeToManagedMap;

FSsManagedObjectArray USsHouseKeeper::ManagedObjectArray;
