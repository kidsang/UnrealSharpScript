using UnrealEngine.CoreUObject;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for FUniqueObjectGuid.
/// </summary>
[NativeCallbacks]
public unsafe class UniqueObjectGuidInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<in UniqueObjectGuid, IntPtr> ResolveObject;
	internal static delegate* unmanaged<IntPtr, ref UniqueObjectGuid, void> GetOrCreateIdForObject;
#pragma warning restore CS0649
}
