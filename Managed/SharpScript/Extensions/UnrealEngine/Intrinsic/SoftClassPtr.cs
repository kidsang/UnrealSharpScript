using UnrealEngine.CoreUObject;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// TSoftClassPtr is a templatized wrapper around FSoftObjectPtr that works like a TSubclassOf, it can be used in UProperties for blueprint subclasses
/// </summary>
public struct TSoftClassPtr<T> : IEquatable<UClass?>, IComparable<TSoftClassPtr<T>>
	where T : UObject, IStaticClass<T>
{
	internal SoftObjectPtrData Data;

	public TSoftClassPtr(UClass? cls)
	{
		Data = new(cls);
	}

	public TSoftClassPtr(TSubclassOf<T> cls)
	{
		Data = new(cls.Class);
	}

	public static implicit operator TSoftClassPtr<T>(UClass? cls)
	{
		return new TSoftClassPtr<T>(cls);
	}

	public static implicit operator UClass?(TSoftClassPtr<T> value)
	{
		return value.Get();
	}

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
	public TSubclassOf<T> Get()
	{
		if (Data.Get() is UClass cls && cls.IsChildOf(T.StaticClass!))
		{
			return cls;
		}

		return new(null);
	}

	/// <summary>
	/// Synchronously load (if necessary) and return the asset object represented by this asset ptr
	/// </summary>
	public TSubclassOf<T> LoadSynchronous()
	{
		if (Data.LoadSynchronous() is UClass cls && cls.IsChildOf(T.StaticClass!))
		{
			return cls;
		}

		return new(null);
	}

	/// <summary>
	/// Test if this points to a live UObject
	/// </summary>
	/// <returns>true if Get() would return a valid non-null pointer</returns>
	public bool IsValid()
	{
		return Get().Class != null;
	}

	/// <summary>
	/// Test if this does not point to a live UObject, but may in the future
	/// </summary>
	/// <returns>true if this does not point to a real object, but could possibly</returns>
	public bool IsPending()
	{
		return Get().Class == null && Data.ObjectId.IsValid();
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
	/// Returns the StringObjectPath that is wrapped by this TSoftClassPtr
	/// </summary>
	public FSoftObjectPath GetUniqueId()
	{
		return Data.ObjectId;
	}

	/// <summary>
	/// Returns the StringObjectPath that is wrapped by this TSoftClassPtr
	/// </summary>
	public FSoftObjectPath ToSoftClassPath()
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

	public bool Equals(UClass? other)
	{
		return Get().Class == other;
	}

	public int CompareTo(TSoftClassPtr<T> other)
	{
		return Data.ObjectId.CompareTo(other.Data.ObjectId);
	}

	public override bool Equals(object? obj)
	{
		if (obj is TSoftClassPtr<T> other)
		{
			return other.Data.ObjectId == Data.ObjectId;
		}

		if (obj is UClass otherPtr)
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

	public static bool operator ==(TSoftClassPtr<T> lhs, UClass? rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(TSoftClassPtr<T> lhs, UClass? rhs)
	{
		return !(lhs == rhs);
	}
}
