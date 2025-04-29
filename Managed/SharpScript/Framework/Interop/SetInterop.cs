namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for TSet.
/// </summary>
[NativeCallbacks]
static unsafe class SetInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<IntPtr, IntPtr, int> GetNum;
	internal static delegate* unmanaged<IntPtr, IntPtr, int> GetMaxIndex;
	internal static delegate* unmanaged<IntPtr, IntPtr, IntPtr, int> FindElementIndexFromHash;
	internal static delegate* unmanaged<IntPtr, IntPtr, int, int> IsValidIndex;
	internal static delegate* unmanaged<IntPtr, IntPtr, int, IntPtr> GetElementPtr;
	internal static delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> AddElement;
	internal static delegate* unmanaged<IntPtr, IntPtr, int, void> RemoveAt;
	internal static delegate* unmanaged<IntPtr, IntPtr, int, void> EmptyElements;
	internal static delegate* unmanaged<IntPtr, IntPtr, void> Rehash;
	internal static delegate* unmanaged<IntPtr, IntPtr, IntPtr> AddDefaultValueAndGetPtr;
#pragma warning restore CS0649
}
