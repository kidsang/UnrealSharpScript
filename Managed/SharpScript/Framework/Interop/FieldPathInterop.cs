using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for FFieldPath.
/// </summary>
[NativeCallbacks]
static unsafe class FieldPathInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<int> GetNativeDataSize;
	internal static delegate* unmanaged<IntPtr, ref NativeString, void> FieldPathToString;
	internal static delegate* unmanaged<IntPtr, char*, void> FieldPathFromString;
#pragma warning restore CS0649
}
