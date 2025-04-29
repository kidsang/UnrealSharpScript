using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for MulticastScriptDelegate.
/// </summary>
[NativeCallbacks]
static unsafe class MulticastDelegateInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<IntPtr, int> IsBound;
	internal static delegate* unmanaged<IntPtr, IntPtr, in Name, int> ContainsUFunction;
	internal static delegate* unmanaged<IntPtr, IntPtr, in Name, void> AddUFunction;
	internal static delegate* unmanaged<IntPtr, IntPtr, in Name, void> AddUniqueUFunction;
	internal static delegate* unmanaged<IntPtr, IntPtr, in Name, void> RemoveUFunction;
	internal static delegate* unmanaged<IntPtr, IntPtr, void> RemoveAll;
	internal static delegate* unmanaged<IntPtr, void> Clear;
	internal static delegate* unmanaged<IntPtr, IntPtr, void> ProcessMulticastDelegate;
	internal static delegate* unmanaged<IntPtr, ref NativeString, void> MulticastDelegateToString;
#pragma warning restore CS0649
}
