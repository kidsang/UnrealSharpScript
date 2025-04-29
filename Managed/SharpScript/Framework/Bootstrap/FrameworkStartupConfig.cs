using System.Runtime.InteropServices;

namespace SharpScript.Bootstrap;

// Startup configuration version number
public enum FrameworkStartupConfigVersion
{
	// Initial version
	Initial = 100,
}

// Initialization configuration passed by the C# framework
// This structure should be consistent with FSharpScriptStartupConfig
// See: SharpScript/Source/SharpScript/Private/SharpScriptStartupConfig.h
[StructLayout(LayoutKind.Sequential)]
public struct FrameworkStartupConfig
{
	// Startup configuration version number, used to check compatibility between C++ and C# frameworks
	public Int32 Version;

	// Size of the startup configuration structure, used to check compatibility between C++ and C# frameworks
	public Int32 ConfigSize;

	// Built-in function pointers passed from C++ to C#
	public IntrinsicPointers IntrinsicPointers;

	// Current structure version number
	public static readonly Int32 CurrentVersion = (Int32)FrameworkStartupConfigVersion.Initial;
}

