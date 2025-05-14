using System.Runtime.InteropServices;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace UnrealEngine.CoreUObject;

public partial class UObject
{
	/// <summary>
	/// Returns the logical name of this object
	/// </summary>
	public FName GetName()
	{
		if (IsValid())
		{
			unsafe
			{
				return ObjectInterop.GetName(NativeObject);
			}
		}
		return FName.None;
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
	public UClass GetClass()
	{
		ThrowIfNotValid();
		unsafe
		{
			IntPtr handlePtr = ObjectInterop.GetClass(NativeObject);
			return (UClass)GCHandle.FromIntPtr(handlePtr).Target!;
		}
	}

	/// <summary>
	/// Returns the UObject this object resides in
	/// </summary>
	public UObject GetOuter()
	{
		ThrowIfNotValid();
		unsafe
		{
			IntPtr handlePtr = ObjectInterop.GetOuter(NativeObject);
			return (UObject)GCHandle.FromIntPtr(handlePtr).Target!;
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
	public UPackage GetPackage()
	{
		ThrowIfNotValid();
		unsafe
		{
			IntPtr handlePtr = ObjectInterop.GetPackage(NativeObject);
			return (UPackage)GCHandle.FromIntPtr(handlePtr).Target!;
		}
	}

	public override string ToString()
	{
		if (IsValid())
		{
			unsafe
			{
				FName name = ObjectInterop.GetName(NativeObject);
				return name.ToString();
			}
		}
		return base.ToString()!;
	}
}
