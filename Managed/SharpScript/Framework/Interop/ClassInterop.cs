using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for UClass.
/// </summary>
[NativeCallbacks]
static unsafe class ClassInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<IntPtr, UInt32> GetClassFlags;
	internal static delegate* unmanaged<IntPtr, IntPtr> GetSuperClass;
	internal static delegate* unmanaged<IntPtr, IntPtr, int> IsChildOf;
	internal static delegate* unmanaged<IntPtr, IntPtr, int> ImplementsInterface;
	internal static delegate* unmanaged<IntPtr, int, IntPtr> GetDefaultObject;
	internal static delegate* unmanaged<IntPtr, FName> GetClassConfigName;
#pragma warning restore CS0649
}
