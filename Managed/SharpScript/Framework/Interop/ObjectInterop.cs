using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for UObject.
/// </summary>
[NativeCallbacks]
static unsafe class ObjectInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<IntPtr, int> IsValid;
	internal static delegate* unmanaged<IntPtr, Name> GetName;
	internal static delegate* unmanaged<IntPtr, UInt32> GetUniqueId;
	internal static delegate* unmanaged<IntPtr, IntPtr> GetClass;
	internal static delegate* unmanaged<IntPtr, IntPtr> GetOuter;
	internal static delegate* unmanaged<IntPtr, UInt32> GetFlags;
	internal static delegate* unmanaged<IntPtr, IntPtr> GetPackage;
	internal static delegate* unmanaged<IntPtr, IntPtr, IntPtr, int> InvokeFunctionCall;
	internal static delegate* unmanaged<IntPtr, IntPtr, IntPtr, int> InvokeStaticFunctionCall;
#pragma warning restore CS0649
}
