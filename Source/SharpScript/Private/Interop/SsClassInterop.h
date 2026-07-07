#pragma once
#include "SsNativeFuncExporter.h"
#include "SsClassInterop.generated.h"

/**
 * Provides interop methods for UClass.
 */
UCLASS()
class USsClassInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static uint32 GetClassFlags(const UClass* InClass);

	static const void* GetSuperClass(const UClass* InClass);

	static int IsChildOf(const UClass* InClass, const UClass* InOther);

	static int ImplementsInterface(const UClass* InClass, const UClass* InOther);

	static const void* GetDefaultObject(const UClass* InClass, int bCreateIfNeeded);

	static int HasMetaData(const UClass* InClass, const TCHAR* Key);

	static void GetMetaData(const UClass* InClass, const TCHAR* Key, FString& OutValue);

	static FName GetClassConfigName(const UClass* InClass);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
