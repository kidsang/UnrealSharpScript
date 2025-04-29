using System.Runtime.InteropServices;
using SharpScript;
using UnrealEngine.Intrinsic;
using Object = UnrealEngine.CoreUObject.Object;

namespace UnrealEngine;

/// <summary>
/// Global static UE methods.
/// </summary>
[NativeCallbacks]
// ReSharper disable once PartialTypeWithSinglePart
public static unsafe partial class Globals
{
	/// <summary>
	/// Deletes all unreferenced objects, keeping only referenced objects (this command will be queued and happen at the end of the frame)
	/// Note: This can be a slow operation, and should only be performed where a hitch would be acceptable
	/// </summary>
	public static void CollectGarbage()
	{
		NativeCollectGarbage();
	}

	/// <summary>
	/// Convenience template for constructing a gameplay object
	/// </summary>
	/// <param name="cls">the class of object to construct</param>
	/// <param name="name">the name for the new object.  If not specified, the object will be given a transient name via MakeUniqueObjectName</param>
	/// <typeparam name="T">subclass of Object</typeparam>
	/// <returns>a pointer of type T to a new object of the specified class</returns>
	public static T NewObject<T>(SubclassOf<T> cls = default, Name name = default) where T : Object
	{
		if (!cls.IsValid())
		{
			cls = new SubclassOf<T>();
		}

		IntPtr handle = NativeNewObjectSimple(cls.NativeClass, name);
		return (T)(GCHandle.FromIntPtr(handle).Target!);
	}

	/// <summary>
	/// Convenience template for constructing a gameplay object
	/// </summary>
	/// <param name="outer">the outer for the new object.  If not specified, object will be created in the transient package.</param>
	/// <param name="cls">the class of object to construct</param>
	/// <param name="name">the name for the new object.  If not specified, the object will be given a transient name via MakeUniqueObjectName</param>
	/// <param name="flags">the object flags to apply to the new object</param>
	/// <typeparam name="T">subclass of Object</typeparam>
	/// <returns>a pointer of type T to a new object of the specified class</returns>
	public static T NewObject<T>(Object outer, SubclassOf<T> cls = default, Name name = default, UInt32 flags = 0) where T : Object
	{
		if (!cls.IsValid())
		{
			cls = new SubclassOf<T>();
		}

		IntPtr handle = NativeNewObject(outer.NativeObject, cls.NativeClass, name, flags, IntPtr.Zero);
		return (T)(GCHandle.FromIntPtr(handle).Target!);
	}

#pragma warning disable CS0649
	internal static delegate* unmanaged[Cdecl]<void> NativeCollectGarbage;
	internal static delegate* unmanaged[Cdecl]<IntPtr/*Class*/, Name/*Name*/, IntPtr> NativeNewObjectSimple;
	internal static delegate* unmanaged[Cdecl]<IntPtr/*Outer*/, IntPtr/*Class*/, Name/*Name*/, UInt32/*Flags*/, IntPtr/*Template*/, IntPtr> NativeNewObject;
#pragma warning restore CS0649
}
