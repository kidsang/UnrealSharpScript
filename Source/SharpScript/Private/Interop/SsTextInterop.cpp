#include "SsTextInterop.h"

int USsTextInterop::SizeOfText()
{
	return (int)sizeof(FText);
}

void USsTextInterop::TextToString(const FText* Text, FString& OutString)
{
	OutString = Text->ToString();
}

void USsTextInterop::StringToText(FText* Text, const TCHAR* String)
{
	*Text = FText::FromString(String);
}

void USsTextInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&SizeOfText, TEXT("TextInterop.SizeOfText"));
	BindNativeCallbackFunc(&TextToString, TEXT("TextInterop.TextToString"));
	BindNativeCallbackFunc(&StringToText, TEXT("TextInterop.StringToText"));
}
