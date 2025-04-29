using System.Runtime.InteropServices;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace UnrealEngine.CoreUObject;

public struct NativeSoftObjectPath
{
	public TopLevelAssetPath AssetPath;
	public NativeString SubPathString;
}

public partial struct SoftObjectPath : IEquatable<SoftObjectPath>, IComparable<SoftObjectPath>
{
	// ReSharper disable once ConvertToPrimaryConstructor
	public SoftObjectPath()
	{
		AssetPath = default;
		SubPathString = string.Empty;
	}

	public static implicit operator SoftObjectPath(in NativeSoftObjectPath value)
	{
		return new SoftObjectPath
		{
			AssetPath = value.AssetPath,
			SubPathString = value.SubPathString.ToString(),
		};
	}

	/// <summary>
	/// Returns /package/path, leaving off the asset name and sub object
	/// </summary>
	public string GetLongPackageName()
	{
		Name packageName = AssetPath.PackageName;
		return packageName == Name.None ? string.Empty : packageName.ToString();
	}

	/// <summary>
	/// Returns assetname string, leaving off the /package/path part and sub object
	/// </summary>
	public string GetAssetName()
	{
		Name assetName = AssetPath.AssetName;
		return assetName == Name.None ? string.Empty : assetName.ToString();
	}

	/// <summary>
	/// Attempts to load the asset, this will call LoadObject which can be very slow
	/// </summary>
	/// <returns>Loaded UObject, or nullptr if the reference is null or the asset fails to load</returns>
	public unsafe Object? TryLoad()
	{
		fixed (char* subPathString = SubPathString)
		{
			IntPtr managedHandlePtr = SoftObjectPathInterop.TryLoad(AssetPath, subPathString);
			if (managedHandlePtr == IntPtr.Zero)
			{
				return null;
			}

			GCHandle managedHandle = GCHandle.FromIntPtr(managedHandlePtr);
			return managedHandle.Target as Object;
		}
	}

	/// <summary>
	/// Attempts to find a currently loaded object that matches this path
	/// </summary>
	/// <returns>Found UObject, or nullptr if not currently in memory</returns>
	public unsafe Object? ResolveObject()
	{
		fixed (char* subPathString = SubPathString ?? string.Empty)
		{
			IntPtr managedHandlePtr = SoftObjectPathInterop.ResolveObject(AssetPath, subPathString);
			if (managedHandlePtr == IntPtr.Zero)
			{
				return null;
			}

			GCHandle managedHandle = GCHandle.FromIntPtr(managedHandlePtr);
			return managedHandle.Target as Object;
		}
	}

	/// <summary>
	/// Resets reference to point to null
	/// </summary>
	public void Reset()
	{
		AssetPath = default;
		SubPathString = string.Empty;
	}

	/// <summary>
	/// Check if this could possibly refer to a real object, or was initialized to null
	/// </summary>
	public bool IsValid()
	{
		return AssetPath.IsValid();
	}

	/// <summary>
	/// Checks to see if this is initialized to null
	/// </summary>
	public bool IsNull()
	{
		return AssetPath.IsNull();
	}

	/// <summary>
	/// Check if this represents an asset, meaning it is not null but does not have a sub path
	/// </summary>
	public bool IsAsset()
	{
		return !AssetPath.IsNull() && String.IsNullOrEmpty(SubPathString);
	}

	/// <summary>
	/// Check if this represents a sub object, meaning it has a sub path
	/// </summary>
	public bool IsSubobject()
	{
		return !AssetPath.IsNull() && !String.IsNullOrEmpty(SubPathString);
	}

	public bool Equals(SoftObjectPath other)
	{
		return AssetPath == other.AssetPath && SubPathString == other.SubPathString;
	}

	public int CompareTo(SoftObjectPath other)
	{
		int result = AssetPath.CompareTo(other.AssetPath);
		if (result == 0)
		{
			result = String.Compare(SubPathString, other.SubPathString, StringComparison.Ordinal);
		}

		return result;
	}

	public override bool Equals(object? obj)
	{
		return obj is SoftObjectPath other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(AssetPath, SubPathString ?? string.Empty);
	}

	public static bool operator ==(SoftObjectPath lhs, SoftObjectPath rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(SoftObjectPath lhs, SoftObjectPath rhs)
	{
		return !(lhs == rhs);
	}

	public override string ToString()
	{
		if (String.IsNullOrEmpty(SubPathString))
		{
			return AssetPath.ToString();
		}

		return $"{AssetPath}:{SubPathString}";
	}
}