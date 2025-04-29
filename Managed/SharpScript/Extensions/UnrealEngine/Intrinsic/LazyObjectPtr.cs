using System.Runtime.InteropServices;
using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using Object = UnrealEngine.CoreUObject.Object;

namespace UnrealEngine.Intrinsic;

[StructLayout(LayoutKind.Sequential)]
public struct LazyObjectPtr<T> : IEquatable<T?>, IComparable<LazyObjectPtr<T>>
	where T : Object
{
	internal WeakObjectPtr<T> WeakPtr;
	internal UniqueObjectGuid ObjectId;

	public LazyObjectPtr(T? obj)
	{
		if (obj is not null)
		{
			WeakPtr = obj;
			unsafe
			{
				UniqueObjectGuidInterop.GetOrCreateIdForObject(obj.NativeObject, ref ObjectId);
			}
		}
		else
		{
			Reset();
		}
	}

	public static implicit operator LazyObjectPtr<T>(T? value) => new(value);

	public static implicit operator T?(LazyObjectPtr<T> value) => value.Get();

	/// <summary>
	/// Reset the lazy pointer back to the null state
	/// </summary>
	public void Reset()
	{
		WeakPtr.Reset();
		ObjectId.Reset();
	}

	/// <summary>
	/// Dereference the lazy pointer.
	/// </summary>
	/// <returns>nullptr if this object is gone or the lazy pointer was null, otherwise a valid UObject pointer</returns>
	internal T? Get()
	{
		T? obj = WeakPtr.Get();
		if (obj != null)
		{
			return obj;
		}

		if (ObjectId.IsValid())
		{
			obj = ObjectId.ResolveObject() as T;
			if (obj != null)
			{
				WeakPtr = obj;
			}
		}

		return obj;
	}

	/// <summary>
	/// Test if this points to a live UObject
	/// </summary>
	/// <returns>true if Get() would return a valid non-null pointer</returns>
	public bool IsValid()
	{
		return Get() != null;
	}

	/// <summary>
	/// Slightly different than !IsValid(), returns true if this used to point to a UObject, but doesn't any more and has not been assigned or reset in the mean time.
	/// </summary>
	/// <returns>true if this used to point at a real object but no longer does.</returns>
	public bool IsStale()
	{
		return WeakPtr.IsStale();
	}

	/// <summary>
	/// Test if this does not point to a live UObject, but may in the future
	/// </summary>
	/// <returns>true if this does not point to a real object, but could possibly</returns>
	public bool IsPending()
	{
		return Get() == null && ObjectId.IsValid();
	}

	/// <summary>
	/// Test if this can never point to a live UObject
	/// </summary>
	/// <returns>true if this is explicitly pointing to no object</returns>
	public bool IsNull()
	{
		return !ObjectId.IsValid();
	}

	/// <summary>
	/// Gets the unique object identifier associated with this lazy pointer. Valid even if pointer is not currently valid
	/// </summary>
	/// <returns>Unique ID for this object, or an invalid FUniqueObjectID if this pointer isn't set to anything</returns>
	public UniqueObjectGuid GetUniqueId()
	{
		return ObjectId;
	}

	public override string ToString()
	{
		return ObjectId.ToString();
	}

	public bool Equals(T? other)
	{
		return Get() == other;
	}

	public override bool Equals(object? obj)
	{
		if (obj is LazyObjectPtr<T> other)
		{
			return other.ObjectId == ObjectId;
		}

		if (obj is T otherPtr)
		{
			return Equals(otherPtr);
		}

		if (obj is null)
		{
			return Equals(null);
		}

		return false;
	}

	public override int GetHashCode()
	{
		return ObjectId.GetHashCode();
	}

	public int CompareTo(LazyObjectPtr<T> other)
	{
		return ObjectId.CompareTo(other.ObjectId);
	}

	public static bool operator ==(LazyObjectPtr<T> lhs, T? rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(LazyObjectPtr<T> lhs, T? rhs)
	{
		return !(lhs == rhs);
	}
}
