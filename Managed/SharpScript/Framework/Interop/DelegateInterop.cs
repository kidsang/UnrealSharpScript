using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for FScriptDelegate.
/// </summary>
[NativeCallbacks]
static unsafe class DelegateInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<ref ScriptDelegate, IntPtr, in Name, void> BindUFunction;
	internal static delegate* unmanaged<ref ScriptDelegate, int> IsBound;
	internal static delegate* unmanaged<ref ScriptDelegate, IntPtr, int> IsBoundToObject;
	internal static delegate* unmanaged<ref ScriptDelegate, void> Unbind;
	internal static delegate* unmanaged<ref ScriptDelegate, ref NativeArray, void> DelegateToString;
	internal static delegate* unmanaged<ref ScriptDelegate, IntPtr> GetUObject;
	internal static delegate* unmanaged<ref ScriptDelegate, Name> GetFunctionName;
	internal static delegate* unmanaged<ref ScriptDelegate, IntPtr, void> ProcessDelegate;
	internal static delegate* unmanaged<ref ScriptDelegate, ref ScriptDelegate, int> DelegateEquals;
	internal static delegate* unmanaged<ref ScriptDelegate, int> DoGetTypeHash;
#pragma warning restore CS0649
}
