#include "SsNativeFuncExporter.h"
#include "UObject/UObjectIterator.h"

void USsNativeFuncExporter::ExportFunctions(FSsBindNativeCallback BindNativeCallback, const void* UnmanagedDelegates)
{
	for (TObjectIterator<UClass> It; It; ++It)
	{
		const UClass* Class = *It;
		if (!Class->HasAnyClassFlags(CLASS_Abstract) && Class->IsChildOf(StaticClass()))
		{
			USsNativeFuncExporter* Exporter = Class->GetDefaultObject<USsNativeFuncExporter>();
			Exporter->DoExportFunctions(FSsBindNativeCallbackFunc(BindNativeCallback, UnmanagedDelegates));
		}
	}
}
