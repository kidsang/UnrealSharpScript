using SharpScript;
using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using Object = UnrealEngine.CoreUObject.Object;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// Template to allow UClass types to be passed around with type safety
/// </summary>
/// <typeparam name="T">The base class that the subclass must inherit from.</typeparam>
public readonly struct SubclassOf<T> : IEquatable<Class?>, IComparable<SubclassOf<T>>
	where T : Object
{
	public SubclassOf() : this(typeof(T))
	{
	}

	public SubclassOf(Class? cls)
	{
		Class = cls;
	}

	public SubclassOf(Type type)
	{
		IntPtr nativeClass = TypeInterop.FindClass(type.Name);
		Class = HouseKeeper.GetManagedObject<Class>(nativeClass);
	}

	public SubclassOf(IntPtr nativeClass)
	{
		Class = HouseKeeper.GetManagedObject<Class>(nativeClass);
	}

	public static implicit operator SubclassOf<T>(Type type)
	{
		return new SubclassOf<T>(type);
	}

	public static implicit operator SubclassOf<T>(Class? cls)
	{
		return new SubclassOf<T>(cls);
	}

	public static implicit operator Class?(SubclassOf<T> subclass)
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
	public Class? Class { get; }

	/// <summary>
	/// Return the native class pointer.
	/// </summary>
	public IntPtr NativeClass => Class?.NativeObject ?? IntPtr.Zero;

	public bool Equals(Class? other)
	{
		return Class == other;
	}

	public int CompareTo(SubclassOf<T> other)
	{
		return (int)NativeClass - (int)other.NativeClass;
	}

	public override bool Equals(object? obj)
	{
		if (obj is SubclassOf<T> other)
		{
			return Equals(other.Class);
		}

		if (obj is Class otherPtr)
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

	public static bool operator ==(SubclassOf<T> lhs, Class? rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(SubclassOf<T> lhs, Class? rhs)
	{
		return !(lhs == rhs);
	}
}
