#pragma once
#include "SsNativeFuncExporter.h"
#include "SsGlobalFunctions.generated.h"

/**
 * Expose various global methods to C#.
 */
UCLASS()
class USsGlobalFunctions : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static void CollectGarbage();

	static const void* NewObjectSimple(const UClass* Class, FName Name);

	static const void* NewObject(UObject* Outer, const UClass* Class, FName Name, EObjectFlags Flags, UObject* Template);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
