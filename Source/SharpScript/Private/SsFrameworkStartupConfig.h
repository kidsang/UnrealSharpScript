#pragma once
#include "CoreMinimal.h"
#include "Intrinsic/SsIntrinsicPointers.h"

// Startup configuration version number
enum class EFrameworkStartupConfigVersion
{
	// Initial version
	Initial = 100,
};

/**
 * Initialization configuration passed from C# framework
 * This structure should be consistent with SharpScript.Bootstrap.FrameworkStartupConfig
 * See: SharpScript/Managed/SharpScript/Bootstrap/FrameworkStartupConfig.cs
 */
struct FSsFrameworkStartupConfig
{
	// Startup configuration version number, used to check compatibility between C++ and C# frameworks
	int32 Version = (int32)EFrameworkStartupConfigVersion::Initial;

	// Size of the startup configuration structure, used to check compatibility between C++ and C# frameworks
	int32 ConfigSize = sizeof(FSsFrameworkStartupConfig);

	// Built-in function pointers passed from C++ to C#
	FSsIntrinsicPointers IntrinsicPointers;
};
