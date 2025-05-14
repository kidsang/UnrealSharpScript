using System.Runtime.InteropServices;
using SharpScript.Interop;

namespace UnrealEngine.CoreUObject;

/// <summary>
/// Wrapper structure for a GUID that uniquely identifies registered UObjects.
/// The actual GUID is stored in an object annotation that is updated when a new reference is made.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FUniqueObjectGuid(FGuid guid) : IEquatable<FUniqueObjectGuid>, IComparable<FUniqueObjectGuid>
{
	/// <summary>
	/// Guid representing the object, should be unique
	/// </summary>
	internal FGuid Guid = guid;

	/// <summary>
	/// Reset the guid pointer back to the invalid state
	/// </summary>
	public void Reset()
	{
		Guid.Invalidate();
	}

	/// <summary>
	///  Test if this can ever point to a live UObject
	/// </summary>
	public bool IsValid()
	{
		return Guid.IsValid();
	}

	/// <summary>
	/// Returns wrapped Guid
	/// </summary>
	public FGuid GetGuid()
	{
		return Guid;
	}

	/// <summary>
	/// Attempts to find a currently loaded object that matches this object ID
	/// </summary>
	/// <returns>Found UObject, or nullptr if not currently loaded</returns>
	public unsafe UObject? ResolveObject()
	{
		IntPtr managedHandlePtr = UniqueObjectGuidInterop.ResolveObject(this);
		if (managedHandlePtr == IntPtr.Zero)
		{
			return null;
		}

		GCHandle managedHandle = GCHandle.FromIntPtr(managedHandlePtr);
		return managedHandle.Target as UObject;
	}

	public bool Equals(FUniqueObjectGuid other)
	{
		return Guid == other.Guid;
	}

	public override bool Equals(object? obj)
	{
		return obj is FUniqueObjectGuid other && Equals(other);
	}

	public override int GetHashCode()
	{
		return Guid.GetHashCode();
	}

	public int CompareTo(FUniqueObjectGuid other)
	{
		return Guid.CompareTo(other.Guid);
	}

	public static bool operator ==(FUniqueObjectGuid lhs, FUniqueObjectGuid rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(FUniqueObjectGuid lhs, FUniqueObjectGuid rhs)
	{
		return !(lhs == rhs);
	}

	public override string ToString()
	{
		return Guid.ToString();
	}
}
