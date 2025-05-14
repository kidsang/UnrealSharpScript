using SharpScript;
using UnrealEngine.CoreUObject;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// Template to allow UClass types to be passed around with type safety
/// </summary>
/// <typeparam name="T">The base class that the subclass must inherit from.</typeparam>
public readonly struct TSubclassOf<T>(UClass? Cls) : IEquatable<UClass?>, IComparable<TSubclassOf<T>>
	where T : UObject
{
	public TSubclassOf()
		: this(null)
	{
	}

	public TSubclassOf(IntPtr nativeClass)
		: this(HouseKeeper.GetManagedObject<UClass>(nativeClass))
	{
	}

	public static implicit operator TSubclassOf<T>(UClass? cls)
	{
		return new TSubclassOf<T>(cls);
	}

	public static implicit operator UClass?(TSubclassOf<T> subclass)
	{
		return subclass.Class;
	}

	public bool IsValid()
	{
		return Class != null && Class.IsValid();
	}

	/// <summary>
	/// Get the CDO if we are referencing a valid class
	/// </summary>
	/// <returns>the CDO, or null if class is null</returns>
	public T? GetDefaultObject()
	{
		if (IsValid())
		{
			return Class!.GetDefaultObject() as T;
		}

		return null;
	}

	/// <summary>
	/// UClass object.
	/// </summary>
	public UClass? Class { get; } = Cls;

	/// <summary>
	/// Return the native class pointer.
	/// </summary>
	public IntPtr NativeClass => Class?.NativeObject ?? IntPtr.Zero;

	public bool Equals(UClass? other)
	{
		return Class == other;
	}

	public int CompareTo(TSubclassOf<T> other)
	{
		return (int)NativeClass - (int)other.NativeClass;
	}

	public override bool Equals(object? obj)
	{
		if (obj is TSubclassOf<T> other)
		{
			return Equals(other.Class);
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
		return (Class != null ? Class.GetHashCode() : 0);
	}

	public static bool operator ==(TSubclassOf<T> lhs, UClass? rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(TSubclassOf<T> lhs, UClass? rhs)
	{
		return !(lhs == rhs);
	}
}
