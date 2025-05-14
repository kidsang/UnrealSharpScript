using System.Runtime.InteropServices;
using SharpScript.Interop;

namespace UnrealEngine.Intrinsic;

[StructLayout(LayoutKind.Sequential)]
public struct FName : IEquatable<FName>, IComparable<FName>
{
    private uint ComparisonIndex;
    private uint Number;
#if WITH_EDITOR
	// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
	private uint DisplayIndex;
#endif

	// Special zero value, meaning no name.
	public static readonly FName None = default;

	public FName(string? name)
	{
		unsafe
		{
			fixed (char* stringPtr = name ?? String.Empty)
			{
				NameInterop.StringToName(ref this, (IntPtr)stringPtr);
			}
		}
	}

	public override string ToString()
	{
		unsafe
		{
			NativeString buffer = new NativeString();
			try
			{
				NameInterop.NameToString(this, ref buffer);
				return buffer.ToString();
			}
			finally
			{
				ArrayInterop.Destroy(ref buffer.Array);
			}
		}
	}

	/// <summary>
	/// Check if the name is None.
	/// </summary>
	/// <returns>True if the name is None, false otherwise.</returns>
	public bool IsNone => this == None;

	public static bool operator ==(FName lhs, FName rhs)
	{
		return lhs.ComparisonIndex == rhs.ComparisonIndex && lhs.Number == rhs.Number;
	}

	public static bool operator !=(FName lhs, FName rhs)
	{
		return !(lhs == rhs);
	}

	public static implicit operator FName(string name)
	{
		return name.Length != 0 ? new FName(name) : None;
	}

	public static implicit operator string(FName name)
	{
		return name.IsNone ? string.Empty : name.ToString();
	}

	public bool Equals(FName other)
	{
		return this == other;
	}

	public override bool Equals(object? obj)
	{
		if (obj is FName name)
		{
			return this == name;
		}

		return false;
	}

	public override int GetHashCode()
	{
		return (int)ComparisonIndex;
	}

	public int CompareTo(FName other)
	{
		uint diff = ComparisonIndex - other.ComparisonIndex;
		if (diff != 0)
		{
			return (int)diff;
		}

		return (int)(Number - other.Number);
	}
}
