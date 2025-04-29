#include "SsStringInterop.h"

void USsStringInterop::CopyToNative(FString* String, const TCHAR* ManagedString)
{
	*String = ManagedString;
}

void USsStringInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&CopyToNative, TEXT("StringInterop.CopyToNative"));
}
