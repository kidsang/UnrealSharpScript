using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for TWeakObjectPtr.
/// </summary>
[NativeCallbacks]
static unsafe class WeakObjectPtrInterop
{
#pragma warning disable CS0649
	public static delegate* unmanaged<WeakObjectData, IntPtr> GetObject;
	public static delegate* unmanaged<ref WeakObjectData, IntPtr, void> SetObject;
	public static delegate* unmanaged<WeakObjectData, int> IsValid;
	public static delegate* unmanaged<WeakObjectData, int> IsStale;
#pragma warning restore CS0649
}
