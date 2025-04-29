using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using Object = UnrealEngine.CoreUObject.Object;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// FSoftObjectPtr is a type of weak pointer to a UObject, that also keeps track of the path to the object on disk.
/// It will change back and forth between being Valid and Pending as the referenced object loads or unloads.
/// It has no impact on if the object is garbage collected or not.
///
/// This is useful to specify assets that you may want to asynchronously load on demand.
/// </summary>
public struct SoftObjectPtr<T>(T? Obj) : IEquatable<T?>, IComparable<SoftObjectPtr<T>>
	where T : Object
{
	internal SoftObjectPtrData Data = new(Obj);

	public static implicit operator SoftObjectPtr<T>(T? value) => new(value);

	public static implicit operator T?(SoftObjectPtr<T> value) => value.Get();

	/// <summary>
	/// Reset the soft pointer back to the null state
	/// </summary>
	public void Reset()
	{
		Data.Reset();
	}

	/// <summary>
	/// Resets the weak ptr only, call this when ObjectId may change
	/// </summary>
	public void ResetWeakPtr()
	{
		Data.ResetWeakPtr();
	}

	/// <summary>
	/// Dereference the soft pointer.
	/// </summary>
	/// <returns>nullptr if this object is gone or the lazy pointer was null, otherwise a valid UObject pointer</returns>
	public T? Get()
	{
		return Data.Get() as T;
	}

	/// <summary>
	/// Synchronously load (if necessary) and return the asset object represented by this asset ptr
	/// </summary>
	public T? LoadSynchronous()
	{
		return Data.LoadSynchronous() as T;
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
		return Data.WeakPtr.IsStale();
	}

	/// <summary>
	/// Test if this does not point to a live UObject, but may in the future
	/// </summary>
	/// <returns>true if this does not point to a real object, but could possibly</returns>
	public bool IsPending()
	{
		return Get() == null && Data.ObjectId.IsValid();
	}

	/// <summary>
	/// Test if this can never point to a live UObject
	/// </summary>
	/// <returns>true if this is explicitly pointing to no object</returns>
	public bool IsNull()
	{
		return !Data.ObjectId.IsValid();
	}

	/// <summary>
	/// Returns the StringObjectPath that is wrapped by this SoftObjectPtr
	/// </summary>
	public SoftObjectPath GetUniqueId()
	{
		return Data.ObjectId;
	}

	/// <summary>
	/// Returns the StringObjectPath that is wrapped by this SoftObjectPtr
	/// </summary>
	public SoftObjectPath ToSoftObjectPath()
	{
		return Data.ObjectId;
	}

	/// <summary>
	/// Returns /package/path string, leaving off the asset name
	/// </summary>
	public string GetLongPackageName()
	{
		return Data.ObjectId.GetLongPackageName();
	}

	/// <summary>
	/// Returns assetname string, leaving off the /package/path. part
	/// </summary>
	public string GetAssetName()
	{
		return Data.ObjectId.GetAssetName();
	}

	public override string ToString()
	{
		return Data.ObjectId.ToString();
	}

	public bool Equals(T? other)
	{
		return Get() == other;
	}

	public override bool Equals(object? obj)
	{
		if (obj is SoftObjectPtr<T> other)
		{
			return other.Data.ObjectId == Data.ObjectId;
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
		return Data.ObjectId.GetHashCode();
	}

	public int CompareTo(SoftObjectPtr<T> other)
	{
		return Data.ObjectId.CompareTo(other.Data.ObjectId);
	}

	public static bool operator ==(SoftObjectPtr<T> lhs, T? rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(SoftObjectPtr<T> lhs, T? rhs)
	{
		return !(lhs == rhs);
	}
}

internal struct SoftObjectPtrData
{
	internal WeakObjectPtr<Object> WeakPtr;
	internal SoftObjectPath ObjectId;

	internal SoftObjectPtrData(Object? obj)
	{
		if (obj is not null)
		{
			WeakPtr = obj;
			unsafe
			{
				NativeSoftObjectPath buffer = new NativeSoftObjectPath();
				try
				{
					SoftObjectPathInterop.GetOrCreateIdForObject(obj.NativeObject, ref buffer);
					ObjectId = buffer;
				}
				finally
				{
					ArrayInterop.Destroy(ref buffer.SubPathString.Array);
				}
			}
		}
		else
		{
			Reset();
		}
	}

	internal void Reset()
	{
		WeakPtr.Reset();
		ObjectId.Reset();
	}

	internal void ResetWeakPtr()
	{
		WeakPtr.Reset();
	}

	internal Object? Get()
	{
		Object? obj = WeakPtr.Get();
		if (obj != null)
		{
			return obj;
		}

		if (ObjectId.IsValid())
		{
			obj = ObjectId.ResolveObject();
			if (obj != null)
			{
				WeakPtr = obj;
			}
		}

		return obj;
	}

	internal Object? LoadSynchronous()
	{
		Object? asset = Get();
		if (asset == null && ObjectId.IsValid())
		{
			ObjectId.TryLoad();
			asset = Get();
		}

		return asset;
	}
}
