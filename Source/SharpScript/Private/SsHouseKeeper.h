#pragma once
#include "CoreMinimal.h"
#include "SsManagedObjectArray.h"
#include "SsNativeFuncExporter.h"
#include "SsHouseKeeper.generated.h"

/**
 * C# wrapper object manager.
 * Records the correspondence between UObject and C# wrapped objects, and manages their lifecycles.
 * @remarks
 * When a UObject is first accessed in C#, a corresponding C# object is created.<br/>
 * Once a C# object is created, it will exists until the corresponding UObject is destroyed.<br/>
 * UObjects and their corresponding C# objects will always have a one-to-one relationship throughout their lifecycle.<br/>
 * Thread safety note: All interfaces are only allowed to be called on game thread.
 */
UCLASS()
class USsHouseKeeper : public USsNativeFuncExporter
{
	GENERATED_BODY()

public:
	/** Initialize the manager */
	static bool Initialize();

	/** Finalize the manager */
	static void Finalize();

	/**
	 * Returns the C# object corresponding to the UObject. If the C# object doesn't exist, it will be created immediately.
	 * @param InObject The UObject for which to get the C# object.
	 * @return Returns the C# object.
	 */
	static const void* GetManagedObject(const UObject* InObject);

private:
	static bool BindManagedCallbacks();

	void OnEnginePreExit();

	/** Called by C#, destroys all C# objects of a type at once (including the type itself) */
	static void FreeManagedObjectsByClassName(const TCHAR* InClassName);

	/** Internal call, releases a C# object */
	static void FreeManagedObject(const UObject* InObject);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;

private:
	/** C# function pointers */
	static const void* (*ManagedCreateManagedObject)(const UObject* InObject, const void* InManagedType);
	static void (*ManagedFreeManagedObject)(const void* InManagedObject);

private:
	struct FManagedObjectTracker
	{
		// C# object pointer
		const void* ManagedObject;
#if !UE_BUILD_SHIPPING
		// For debugging, UObject name
		FName ObjectName;
		// For debugging, class name of UObject
		FName ClassName;
#endif // !UE_BUILD_SHIPPING

		explicit FManagedObjectTracker(const void* InManagedObject)
			: ManagedObject(InManagedObject)
		{
		}
	};

	/** Records the mapping relationship between UObjects and C# objects. */
	static TMap<const UObject*, FManagedObjectTracker> NativeToManagedMap;

	/** Records which UObjects have C# objects. */
	static FSsManagedObjectArray ManagedObjectArray;

private:
	struct FObjectListener : public FUObjectArray::FUObjectDeleteListener
	{
		virtual void NotifyUObjectDeleted(const UObjectBase* InObject, int32 InObjectIndex) override;
		virtual void OnUObjectArrayShutdown() override;
	};

	// Allows Listener to directly operate on HouseKeeper members
	friend struct FObjectListener;

	// Listens for UObject destruction events, cutting off the connection with Python Wrapper
	FObjectListener ObjectDeleteListener;
};
