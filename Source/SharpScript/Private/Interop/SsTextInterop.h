#pragma once
#include "SsNativeFuncExporter.h"
#include "SsTextInterop.generated.h"

/**
 * Provides interop methods for FText.
 */
UCLASS()
class USsTextInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

private:
	static int SizeOfText();

	static void TextToString(const FText* Text, FString& OutString);

	static void StringToText(FText* Text, const TCHAR* String);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
