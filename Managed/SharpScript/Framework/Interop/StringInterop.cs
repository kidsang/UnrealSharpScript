namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for FString.
/// </summary>
[NativeCallbacks]
static unsafe class StringInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged[Cdecl]<IntPtr, char*, void> CopyToNative;
#pragma warning restore CS0649
}
