using System.Runtime.InteropServices;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace UnrealEngine.CoreUObject;

public partial class Object
{
	/// <summary>
	/// Returns the logical name of this object
	/// </summary>
	public Name GetName()
	{
		if (IsValid())
		{
			unsafe
			{
				return ObjectInterop.GetName(NativeObject);
			}
		}
		return Name.None;
	}

	/// <summary>
	/// Returns the unique ID of the object...these are reused so it is only unique while the object is alive.
	/// Useful as a tag.
	/// </summary>
	public UInt32 GetUniqueId()
	{
		ThrowIfNotValid();
		unsafe
		{
			return ObjectInterop.GetUniqueId(NativeObject);
		}
	}

	/// <summary>
	/// Returns the UClass that defines the fields of this object
	/// </summary>
	public Class GetClass()
	{
		ThrowIfNotValid();
		unsafe
		{
			IntPtr handlePtr = ObjectInterop.GetClass(NativeObject);
			return (Class)GCHandle.FromIntPtr(handlePtr).Target!;
		}
	}

	/// <summary>
	/// Returns the UObject this object resides in
	/// </summary>
	public Object GetOuter()
	{
		ThrowIfNotValid();
		unsafe
		{
			IntPtr handlePtr = ObjectInterop.GetOuter(NativeObject);
			return (Object)GCHandle.FromIntPtr(handlePtr).Target!;
		}
	}

	/// <summary>
	/// Retrieve the object flags directly
	/// </summary>
	public UInt32 GetFlags()
	{
		ThrowIfNotValid();
		unsafe
		{
			return ObjectInterop.GetFlags(NativeObject);
		}
	}

	/// <summary>
	///  Walks up the list of outers until it finds a package directly associated with the object.
	/// </summary>
	public Package GetPackage()
	{
		ThrowIfNotValid();
		unsafe
		{
			IntPtr handlePtr = ObjectInterop.GetPackage(NativeObject);
			return (Package)GCHandle.FromIntPtr(handlePtr).Target!;
		}
	}

	public override string ToString()
	{
		if (IsValid())
		{
			unsafe
			{
				Name name = ObjectInterop.GetName(NativeObject);
				return name.ToString();
			}
		}
		return base.ToString()!;
	}
}
