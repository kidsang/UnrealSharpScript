using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for TArray.
/// </summary>
[NativeCallbacks]
static unsafe class ArrayInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<ref NativeArray, void> Destroy;
	internal static delegate* unmanaged<IntPtr, NativeArray*, void> EmptyValues;
	internal static delegate* unmanaged<IntPtr, NativeArray*, int, void> EmptyAndAddValues;
	internal static delegate* unmanaged<IntPtr, NativeArray*, int> AddValue;
	internal static delegate* unmanaged<IntPtr, NativeArray*, int, int> InsertValue;
	internal static delegate* unmanaged<IntPtr, NativeArray*, int, int> RemoveValue;
#pragma warning restore CS0649
}
