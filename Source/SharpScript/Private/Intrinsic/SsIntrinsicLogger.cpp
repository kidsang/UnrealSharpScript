#include "SsIntrinsicLogger.h"
#include "SsCommon.h"

void FSsIntrinsicLogger::Log(const TCHAR* Message)
{
	UE_LOG(LogCSharp, Log, TEXT("%s"), Message);
}

void FSsIntrinsicLogger::Display(const TCHAR* Message)
{
	UE_LOG(LogCSharp, Display, TEXT("%s"), Message);
}

void FSsIntrinsicLogger::Warning(const TCHAR* Message)
{
	UE_LOG(LogCSharp, Warning, TEXT("%s"), Message);
}

void FSsIntrinsicLogger::Error(const TCHAR* Message)
{
	UE_LOG(LogCSharp, Error, TEXT("%s"), Message);
}
