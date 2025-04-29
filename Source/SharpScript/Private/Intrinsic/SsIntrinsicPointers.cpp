#include "SsIntrinsicPointers.h"
#include "SsIntrinsicLogger.h"
#include "SsNativeFuncExporter.h"

FSsIntrinsicPointers::FSsIntrinsicPointers()
{
	// UnrealEngine.Intrinsic.Logger
	Logger_Log = FSsIntrinsicLogger::Log;
	Logger_Display = FSsIntrinsicLogger::Display;
	Logger_Warning = FSsIntrinsicLogger::Warning;
	Logger_Error = FSsIntrinsicLogger::Error;

	// NativeFuncExporter
	ExportNativeFuncs = USsNativeFuncExporter::ExportFunctions;
}
