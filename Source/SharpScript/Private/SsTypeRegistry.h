#pragma once
#include "CoreMinimal.h"
#include "SsNativeFuncExporter.h"
#include "SsTypeRegistry.generated.h"

/**
 * Manages the mapping between UE types and C# types.<br/>
 * @remarks
 * Thread safety note: All interfaces are only allowed to be called on the game thread.
 */
UCLASS()
class USsTypeRegistry : public USsNativeFuncExporter
{
	GENERATED_BODY()

public:
	/** Initialize the manager */
	static bool Initialize();

	/** Finalize the manager */
	static void Finalize();

	/**
	 * Returns the corresponding C# type based on the input UClass.
	 * @param InClass UClass pointer
	 * @return Corresponding C# type
	 * @remarks If the corresponding C# type is not found, returns a null pointer.
	 */
	static const void* GetManagedClassType(const UClass* InClass);

private:
	/**
	 * Called by C# to bind UE type with C# type.
	 * @param InClass UClass pointer
	 * @param InManagedType C# type
	 */
	static void RegisterClassType(const UClass* InClass, const void* InManagedType);

	/**
	 * Called by C# to bind UE type with C# type.
	 * @param InClassName UClass class name
	 * @param InManagedType C# type
	 */
	static void RegisterClassTypeByName(const TCHAR* InClassName, const void* InManagedType);

	/**
	 * Unbind UE type from C# type.
	 * @param InClass UClass pointer
	 */
	static void UnregisterClassType(const UClass* InClass);

	/**
	 * Unbind UE type from C# type.
	 * @param InClassName UClass class name
	 */
	static void UnregisterClassTypeByName(const TCHAR* InClassName);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;

private:
	/** Mapping from UClass to C# type. */
	static TMap<const UClass*, const void*> NativeToManagedClassMap;

	/** C# types not yet bound to UClass. */
	static TMap<FName, const void*> UnresolvedManagedClasses;
};
