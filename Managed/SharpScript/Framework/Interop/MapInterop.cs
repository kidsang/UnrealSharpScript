namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for TMap.
/// </summary>
[NativeCallbacks]
static unsafe class MapInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<IntPtr, IntPtr, int> GetNum;
	internal static delegate* unmanaged<IntPtr, IntPtr, int> GetMaxIndex;
	internal static delegate* unmanaged<IntPtr, IntPtr, int, int> IsValidIndex;
	internal static delegate* unmanaged<IntPtr, IntPtr, int, out IntPtr, out IntPtr, int> GetPairPtr;
	internal static delegate* unmanaged<IntPtr, IntPtr, IntPtr, int> FindMapIndexWithKey;
	internal static delegate* unmanaged<IntPtr, IntPtr, int, void> EmptyValues;
	internal static delegate* unmanaged<IntPtr, IntPtr, void> Rehash;
	internal static delegate* unmanaged<IntPtr, IntPtr, IntPtr, IntPtr, void> AddPair;
	internal static delegate* unmanaged<IntPtr, IntPtr, int, void> RemoveAt;
	internal static delegate* unmanaged<IntPtr, IntPtr, out IntPtr, out IntPtr, void> AddDefaultValueAndGetPair;
#pragma warning restore CS0649
}
