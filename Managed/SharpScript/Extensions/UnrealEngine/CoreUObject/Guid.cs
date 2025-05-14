using System.Runtime.InteropServices;

namespace UnrealEngine.CoreUObject;

public partial struct FGuid : IEquatable<FGuid>, IComparable<FGuid>
{
	/// <summary>
	/// Invalidates the GUID
	/// </summary>
	/// <seealso cref="IsValid"/>
	public void Invalidate()
	{
		this = default;
	}

	/// <summary>
	/// Checks whether this GUID is valid or not.
	///
	/// A GUID that has all its components set to zero is considered invalid.
	/// </summary>
	/// <returns>true if valid, false otherwise.</returns>
	/// <seealso cref="Invalidate"/>
	public bool IsValid()
	{
		return ((A | B | C | D) != 0);
	}

	public bool Equals(FGuid other)
	{
		return A == other.A && B == other.B && C == other.C && D == other.D;
	}

	public override bool Equals(object? obj)
	{
		return obj is FGuid other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(A, B, C, D);
	}

	public int CompareTo(FGuid other)
	{
		var aComparison = A.CompareTo(other.A);
		if (aComparison != 0) return aComparison;
		var bComparison = B.CompareTo(other.B);
		if (bComparison != 0) return bComparison;
		var cComparison = C.CompareTo(other.C);
		if (cComparison != 0) return cComparison;
		return D.CompareTo(other.D);
	}

	public static bool operator ==(FGuid lhs, FGuid rhst)
	{
		return lhs.Equals(rhst);
	}

	public static bool operator !=(FGuid lhs, FGuid rhst)
	{
		return !(lhs == rhst);
	}

	public static implicit operator Guid(in FGuid guid)
	{
		ReadOnlySpan<byte> span = MemoryMarshal.Cast<FGuid, byte>(new ReadOnlySpan<FGuid>(in guid));
		return new Guid(span);
	}

	public static implicit operator FGuid(in Guid guid)
	{
		ReadOnlySpan<int> span = MemoryMarshal.Cast<Guid, int>(new ReadOnlySpan<Guid>(in guid));
		return new FGuid
		{
			A = span[0],
			B = span[1],
			C = span[2],
			D = span[3],
		};
	}
}
