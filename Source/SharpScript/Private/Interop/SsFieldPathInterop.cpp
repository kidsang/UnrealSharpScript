#include "SsFieldPathInterop.h"

int USsFieldPathInterop::GetNativeDataSize()
{
	return (int)sizeof(FFieldPath);
}

void USsFieldPathInterop::FieldPathToString(const FFieldPath* InFieldPath, FString& OutString)
{
	OutString = InFieldPath->ToString();
}

void USsFieldPathInterop::FieldPathFromString(FFieldPath* FieldPath, const TCHAR* InPath)
{
	if (InPath)
	{
		FieldPath->Generate(InPath);
	}
	else
	{
		FieldPath->Reset();
	}
}

void USsFieldPathInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&GetNativeDataSize, TEXT("FieldPathInterop.GetNativeDataSize"));
	BindNativeCallbackFunc(&FieldPathToString, TEXT("FieldPathInterop.FieldPathToString"));
	BindNativeCallbackFunc(&FieldPathFromString, TEXT("FieldPathInterop.FieldPathFromString"));
}
