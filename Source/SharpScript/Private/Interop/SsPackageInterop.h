#pragma once
#include "SsNativeFuncExporter.h"
#include "SsPackageInterop.generated.h"

/**
 * Provides interop methods for UPackage.
 */
UCLASS()
class USsPackageInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static uint32 GetPackageFlags(const UPackage* InPackage);

	static uint64 GetPackageId(const UPackage* InPackage);

	static FName GetPackageName(const UPackage* InPackage);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
