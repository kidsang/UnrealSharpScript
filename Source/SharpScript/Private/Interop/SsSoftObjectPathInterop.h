#pragma once
#include "SsNativeFuncExporter.h"
#include "SsSoftObjectPathInterop.generated.h"

/**
 * Provides interop methods for FSoftObjectPath.
 */
UCLASS()
class USsSoftObjectPathInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static const void* TryLoad(const FTopLevelAssetPath& InAssetPath, const TCHAR* InSubPathString);

	static const void* ResolveObject(const FTopLevelAssetPath& InAssetPath, const TCHAR* InSubPathString);

	static void GetOrCreateIdForObject(const UObject* Object, FSoftObjectPath& OutSoftObjectPath);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
