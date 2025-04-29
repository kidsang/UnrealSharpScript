#include "SsPackageInterop.h"
#include "UObject/Package.h"

uint32 USsPackageInterop::GetPackageFlags(const UPackage* InPackage)
{
	return InPackage->GetPackageFlags();
}

uint64 USsPackageInterop::GetPackageId(const UPackage* InPackage)
{
	return InPackage->GetPackageId().Value();
}

FName USsPackageInterop::GetPackageName(const UPackage* InPackage)
{
	return InPackage->GetLoadedPath().GetPackageFName();
}

void USsPackageInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&GetPackageFlags, TEXT("PackageInterop.GetPackageFlags"));
	BindNativeCallbackFunc(&GetPackageId, TEXT("PackageInterop.GetPackageId"));
	BindNativeCallbackFunc(&GetPackageName, TEXT("PackageInterop.GetPackageName"));
}
