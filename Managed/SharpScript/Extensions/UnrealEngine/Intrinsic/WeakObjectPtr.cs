using System.Runtime.InteropServices;
using SharpScript.Interop;
using Object = UnrealEngine.CoreUObject.Object;

namespace UnrealEngine.Intrinsic;

[StructLayout(LayoutKind.Sequential)]
public struct WeakObjectData
{
	internal int ObjectIndex;
	internal int ObjectSerialNumber;
}

/// <summary>
/// Provides a wrapper for TWeakObjectPtr.
/// </summary>
/// <remarks>
/// FWeakObjectPtr is a weak pointer to a UObject.
/// It can return nullptr later if the object is garbage collected.
/// It has no impact on if the object is garbage collected or not.
/// It can't be directly used across a network.
///
/// Most often it is used when you explicitly do NOT want to prevent something from being garbage collected.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct WeakObjectPtr<T> : IEquatable<WeakObjectPtr<T>> where T : Object
{
	internal WeakObjectData Data;

	/// <summary>
	/// Dereference the weak pointer.
	/// </summary>
	public T? Object => Get();

	/// <summary>
	/// Copy from an object pointer
	/// </summary>
	/// <param name="obj">object to create a weak pointer to</param>
	public WeakObjectPtr(T? obj)
	{
		WeakObjectPtrInterop.SetObject(ref Data, obj?.NativeObject ?? IntPtr.Zero);
	}

	public WeakObjectPtr(int objectIndex, int objectSerialNumber)
	{
		Data.ObjectIndex = objectIndex;
		Data.ObjectSerialNumber = objectSerialNumber;
	}

	public static implicit operator WeakObjectPtr<T>(T? value) => new(value);

	public static implicit operator T?(WeakObjectPtr<T> value) => value.Get();

	/// <summary>
	/// Reset the weak pointer back to the null state
	/// </summary>
	public void Reset()
	{
		Data = default;
	}

	/// <summary>
	/// Dereference the weak pointer.
	/// </summary>
	/// <returns>nullptr if this object is gone or the weak pointer is explicitly null, otherwise a valid uobject pointer</returns>
	public T? Get()
	{
		IntPtr managedHandlePtr = WeakObjectPtrInterop.GetObject(Data);
		if (managedHandlePtr == IntPtr.Zero)
		{
			return null;
		}

		GCHandle managedHandle = GCHandle.FromIntPtr(managedHandlePtr);
		return managedHandle.Target as T;
	}

	/// <summary>
	/// Test if this points to a live UObject
	/// This should be done only when needed as excess resolution of the underlying pointer can cause performance issues.
	/// </summary>
	/// <returns>true if Get() would return a valid non-null pointer</returns>
	public bool IsValid()
	{
		return WeakObjectPtrInterop.IsValid(Data) != 0;
	}

	/// <summary>
	///  Slightly different than !IsValid(), returns true if this used to point to a UObject, but doesn't any more and has not been assigned or reset in the mean time.
	/// </summary>
	/// <returns>true if this used to point at a real object but no longer does.</returns>
	public bool IsStale()
	{
		return WeakObjectPtrInterop.IsStale(Data) != 0;
	}

	/// <summary>
	/// Returns true if this pointer was explicitly assigned to null, was reset, or was never initialized.
	/// If this returns true, IsValid() and IsStale() will both return false.
	/// </summary>
	/// <returns></returns>
	public bool IsExplicitlyNull()
	{
		return Data is { ObjectIndex: 0, ObjectSerialNumber: 0 };
	}

	public bool Equals(WeakObjectPtr<T> other)
	{
		return Data.ObjectIndex == other.Data.ObjectIndex && Data.ObjectSerialNumber == other.Data.ObjectSerialNumber;
	}

	public override bool Equals(object? obj)
	{
		return obj is WeakObjectPtr<T> other && Equals(other);
	}

	public override int GetHashCode()
	{
		return Data.ObjectIndex ^ Data.ObjectSerialNumber;
	}
}