using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace UnrealEngine.CoreUObject;

public partial class Package
{
	/// <summary>
	/// Gets the package flags.
	/// </summary>
	public UInt32 GetPackageFlags()
	{
		unsafe
		{
			return PackageInterop.GetPackageFlags(NativeObject);
		}
	}

	/// <summary>
	/// returns the unique package id
	/// </summary>
	public UInt64 GetPackageId()
	{
		unsafe
		{
			return PackageInterop.GetPackageId(NativeObject);
		}
	}

	/// <summary>
	/// Return as an FName the PackageName indicated by this PackagePath if known or available, or NAME_None if not
	/// </summary>
	/// <remarks>
	/// Returning a valid name is possible only for MountedPath and PackageOnly internal types.
	/// Will attempt to mount before returning NAME_None
	/// </remarks>
	public Name GetPackageName()
	{
		unsafe
		{
			return PackageInterop.GetPackageName(NativeObject);
		}
	}
}
