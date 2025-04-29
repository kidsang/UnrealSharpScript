using System.Runtime.InteropServices;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace UnrealEngine.CoreUObject;

public partial class Class
{
	/// <summary>
	/// Gets the class flags.
	/// </summary>
	public UInt32 GetClassFlags()
	{
		ThrowIfNotValid();
		unsafe
		{
			return ClassInterop.GetClassFlags(NativeObject);
		}
	}

	/// <summary>
	/// Returns parent class, the parent of a Class is always another class
	/// </summary>
	public Class? GetSuperClass()
	{
		ThrowIfNotValid();
		unsafe
		{
			IntPtr handlePtr = ClassInterop.GetSuperClass(NativeObject);
			if (handlePtr == IntPtr.Zero)
			{
				return null;
			}

			return GCHandle.FromIntPtr(handlePtr).Target as Class;
		}
	}

	/// <summary>
	/// Returns true if this struct either is SomeBase, or is a child of SomeBase.
	/// </summary>
	public bool IsChildOf(Class other)
	{
		if (!other.IsValid())
		{
			return false;
		}

		ThrowIfNotValid();
		unsafe
		{
			int result = ClassInterop.IsChildOf(NativeObject, other.NativeObject);
			return result != 0;
		}
	}

	/// <summary>
	/// This will return whether or not this class implements the passed in class / interface
	/// </summary>
	public bool ImplementsInterface<T>(SubclassOf<T> someInterface) where T : Interface
	{
		if (!someInterface.IsValid())
		{
			return false;
		}

		ThrowIfNotValid();
		unsafe
		{
			int result = ClassInterop.ImplementsInterface(NativeObject, someInterface.NativeClass);
			return result != 0;
		}
	}

	/// <summary>
	/// Get the default object from the class
	/// </summary>
	/// <param name="bCreateIfNeeded">if true (default) then the CDO is created if it is null</param>
	/// <returns>the CDO for this class</returns>
	public Object? GetDefaultObject(bool bCreateIfNeeded = true)
	{
		ThrowIfNotValid();
		unsafe
		{
			int createIfNeeded = bCreateIfNeeded ? 1 : 0;
			IntPtr handlePtr = ClassInterop.GetDefaultObject(NativeObject, createIfNeeded);
			if (handlePtr == IntPtr.Zero)
			{
				return null;
			}

			return GCHandle.FromIntPtr(handlePtr).Target as Object;
		}
	}
}
