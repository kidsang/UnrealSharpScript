using System.Runtime.InteropServices;
using SharpScript.Interop;

namespace UnrealEngine.CoreUObject;

/// <summary>
/// Wrapper structure for a GUID that uniquely identifies registered UObjects.
/// The actual GUID is stored in an object annotation that is updated when a new reference is made.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct UniqueObjectGuid(Guid guid) : IEquatable<UniqueObjectGuid>, IComparable<UniqueObjectGuid>
{
	/// <summary>
	/// Guid representing the object, should be unique
	/// </summary>
	internal Guid Guid = guid;

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
	public Guid GetGuid()
	{
		return Guid;
	}

	/// <summary>
	/// Attempts to find a currently loaded object that matches this object ID
	/// </summary>
	/// <returns>Found UObject, or nullptr if not currently loaded</returns>
	public unsafe Object? ResolveObject()
	{
		IntPtr managedHandlePtr = UniqueObjectGuidInterop.ResolveObject(this);
		if (managedHandlePtr == IntPtr.Zero)
		{
			return null;
		}

		GCHandle managedHandle = GCHandle.FromIntPtr(managedHandlePtr);
		return managedHandle.Target as Object;
	}

	public bool Equals(UniqueObjectGuid other)
	{
		return Guid == other.Guid;
	}

	public override bool Equals(object? obj)
	{
		return obj is UniqueObjectGuid other && Equals(other);
	}

	public override int GetHashCode()
	{
		return Guid.GetHashCode();
	}

	public int CompareTo(UniqueObjectGuid other)
	{
		return Guid.CompareTo(other.Guid);
	}

	public static bool operator ==(UniqueObjectGuid lhs, UniqueObjectGuid rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(UniqueObjectGuid lhs, UniqueObjectGuid rhs)
	{
		return !(lhs == rhs);
	}

	public override string ToString()
	{
		return Guid.ToString();
	}
}
