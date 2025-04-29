using System.Runtime.InteropServices;
using UnrealEngine.Intrinsic;

namespace SharpScript.Bootstrap;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct IntrinsicPointers
{
	// Logger
	public delegate* unmanaged[Cdecl]<void*, void> Logger_Log;
	public delegate* unmanaged[Cdecl]<void*, void> Logger_Display;
	public delegate* unmanaged[Cdecl]<void*, void> Logger_Warning;
	public delegate* unmanaged[Cdecl]<void*, void> Logger_Error;

	// NativeFuncExporter
	public delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<IntPtr, char*, IntPtr, void>, IntPtr, void> ExportNativeFuncs;

	public void AssignIntrinsicPointers()
	{
		// Logger
		Logger.NativeLog = Logger_Log;
		Logger.NativeDisplay = Logger_Display;
		Logger.NativeWarning = Logger_Warning;
		Logger.NativeError = Logger_Error;
	}
}
