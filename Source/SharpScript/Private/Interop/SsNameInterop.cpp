#include "SsNameInterop.h"

void USsNameInterop::NameToString(FName Name, FString& OutString)
{
	Name.ToString(OutString);
}

void USsNameInterop::StringToName(FName& Name, const TCHAR* String)
{
	Name = FName(String);
}

void USsNameInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&NameToString, TEXT("NameInterop.NameToString"));
	BindNativeCallbackFunc(&StringToName, TEXT("NameInterop.StringToName"));
}
