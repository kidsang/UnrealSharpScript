using UnrealEngine.CoreUObject;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for FSoftObjectPath.
/// </summary>
[NativeCallbacks]
public unsafe class SoftObjectPathInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<in TopLevelAssetPath, char*, IntPtr> TryLoad;
	internal static delegate* unmanaged<in TopLevelAssetPath, char*, IntPtr> ResolveObject;
	internal static delegate* unmanaged<IntPtr, ref NativeSoftObjectPath, void> GetOrCreateIdForObject;
#pragma warning restore CS0649
}