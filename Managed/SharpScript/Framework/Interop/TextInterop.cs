using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for FText.
/// </summary>
[NativeCallbacks]
static unsafe class TextInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<int> SizeOfText;
	internal static delegate* unmanaged<IntPtr, ref NativeString, void> TextToString;
	internal static delegate* unmanaged<IntPtr, char*, void> StringToText;
#pragma warning restore CS0649
}
