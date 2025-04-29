#pragma once
#include "SsNativeFuncExporter.h"
#include "SsFieldPathInterop.generated.h"

/**
 * Provides interop methods for FFieldPath.
 */
UCLASS()
class USsFieldPathInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static int GetNativeDataSize();

	static void FieldPathToString(const FFieldPath* InFieldPath, FString& OutString);

	static void FieldPathFromString(FFieldPath* FieldPath, const TCHAR* InPath);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
