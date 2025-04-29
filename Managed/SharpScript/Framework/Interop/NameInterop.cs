using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for FName.
/// </summary>
[NativeCallbacks]
static unsafe class NameInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<Name, ref NativeString, void> NameToString;
	internal static delegate* unmanaged<ref Name, IntPtr, void> StringToName;
#pragma warning restore CS0649
}