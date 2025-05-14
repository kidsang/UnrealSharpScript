using System.Runtime.InteropServices;
using UnrealEngine.Intrinsic;

namespace UnrealEngine.CoreUObject;

[StructLayout(LayoutKind.Sequential)]
public partial struct FTopLevelAssetPath : IEquatable<FTopLevelAssetPath>, IComparable<FTopLevelAssetPath>
{
	/// <summary>
	/// Check if this could possibly refer to a real object
	/// </summary>
	public bool IsValid()
	{
		return PackageName != FName.None;
	}

	/// <summary>
	///  Checks to see if this is initialized to null
	/// </summary>
	public bool IsNull()
	{
		return PackageName == FName.None;
	}

	public bool Equals(FTopLevelAssetPath other)
	{
		return PackageName == other.PackageName && AssetName == other.AssetName;
	}

	public override bool Equals(object? obj)
	{
		return obj is FTopLevelAssetPath other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(PackageName, AssetName);
	}

	public static bool operator ==(FTopLevelAssetPath lhs, FTopLevelAssetPath rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(FTopLevelAssetPath lhs, FTopLevelAssetPath rhs)
	{
		return !(lhs == rhs);
	}

	public int CompareTo(FTopLevelAssetPath other)
	{
		int diff = PackageName.CompareTo(other.PackageName);
		if (diff != 0)
		{
			return diff;
		}

		return AssetName.CompareTo(other.AssetName);
	}

	public override string ToString()
	{
		if (PackageName == FName.None)
		{
			return string.Empty;
		}

		if (AssetName == FName.None)
		{
			return PackageName;
		}

		return $"{PackageName}.{AssetName}";
	}
}
