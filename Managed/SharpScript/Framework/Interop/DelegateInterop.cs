using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for FScriptDelegate.
/// </summary>
[NativeCallbacks]
static unsafe class DelegateInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<ref FScriptDelegate, IntPtr, in FName, void> BindUFunction;
	internal static delegate* unmanaged<ref FScriptDelegate, int> IsBound;
	internal static delegate* unmanaged<ref FScriptDelegate, IntPtr, int> IsBoundToObject;
	internal static delegate* unmanaged<ref FScriptDelegate, void> Unbind;
	internal static delegate* unmanaged<ref FScriptDelegate, ref NativeArray, void> DelegateToString;
	internal static delegate* unmanaged<ref FScriptDelegate, IntPtr> GetUObject;
	internal static delegate* unmanaged<ref FScriptDelegate, FName> GetFunctionName;
	internal static delegate* unmanaged<ref FScriptDelegate, IntPtr, void> ProcessDelegate;
	internal static delegate* unmanaged<ref FScriptDelegate, ref FScriptDelegate, int> DelegateEquals;
	internal static delegate* unmanaged<ref FScriptDelegate, int> DoGetTypeHash;
#pragma warning restore CS0649
}
