using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for UPackage.
/// </summary>
[NativeCallbacks]
static unsafe class PackageInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<IntPtr, UInt32> GetPackageFlags;
	internal static delegate* unmanaged<IntPtr, UInt64> GetPackageId;
	internal static delegate* unmanaged<IntPtr, Name> GetPackageName;
#pragma warning restore CS0649
}
