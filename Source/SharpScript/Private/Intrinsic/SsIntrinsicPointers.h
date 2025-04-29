#pragma once
#include "CoreMinimal.h"

struct FSsIntrinsicPointers
{
	// Logger
	void (*Logger_Log)(const TCHAR*);
	void (*Logger_Display)(const TCHAR*);
	void (*Logger_Warning)(const TCHAR*);
	void (*Logger_Error)(const TCHAR*);

	// NativeFuncExporter
	void* ExportNativeFuncs;

	FSsIntrinsicPointers();
};
