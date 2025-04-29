using System.Runtime.InteropServices;
using UnrealEngine.Intrinsic;

namespace UnrealEngine.CoreUObject;

[StructLayout(LayoutKind.Sequential)]
public partial struct TopLevelAssetPath : IEquatable<TopLevelAssetPath>, IComparable<TopLevelAssetPath>
{
	/// <summary>
	/// Check if this could possibly refer to a real object
	/// </summary>
	public bool IsValid()
	{
		return PackageName != Name.None;
	}

	/// <summary>
	///  Checks to see if this is initialized to null
	/// </summary>
	public bool IsNull()
	{
		return PackageName == Name.None;
	}

	public bool Equals(TopLevelAssetPath other)
	{
		return PackageName == other.PackageName && AssetName == other.AssetName;
	}

	public override bool Equals(object? obj)
	{
		return obj is TopLevelAssetPath other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(PackageName, AssetName);
	}

	public static bool operator ==(TopLevelAssetPath lhs, TopLevelAssetPath rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(TopLevelAssetPath lhs, TopLevelAssetPath rhs)
	{
		return !(lhs == rhs);
	}

	public int CompareTo(TopLevelAssetPath other)
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
		if (PackageName == Name.None)
		{
			return string.Empty;
		}

		if (AssetName == Name.None)
		{
			return PackageName;
		}

		return $"{PackageName}.{AssetName}";
	}
}
