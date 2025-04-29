#pragma once
#include "CoreMinimal.h"

struct FSsIntrinsicLogger
{
	static void Log(const TCHAR* Message);

	static void Display(const TCHAR* Message);

	static void Warning(const TCHAR* Message);

	static void Error(const TCHAR* Message);
};
