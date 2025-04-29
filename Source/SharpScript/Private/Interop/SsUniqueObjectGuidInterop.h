#pragma once
#include "SsNativeFuncExporter.h"
#include "SsUniqueObjectGuidInterop.generated.h"

/**
 * Provides interop methods for FUniqueObjectGuid.
 */
UCLASS()
class USsUniqueObjectGuidInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static const void* ResolveObject(const FUniqueObjectGuid& InUniqueObjectGuid);

	static void GetOrCreateIdForObject(const UObject* Object, FUniqueObjectGuid& OutUniqueObjectGuid);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
