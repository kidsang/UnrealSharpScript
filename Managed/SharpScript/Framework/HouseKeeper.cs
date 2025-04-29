using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnrealEngine.Intrinsic;

namespace SharpScript;

/// <summary>
/// Records the correspondence between UObject and C# wrapper objects, and provides conversion interfaces between them.
/// See C++ class `FSsHouseKeeper` for details.
/// </summary>
[NativeCallbacks]
public static unsafe class HouseKeeper
{
	/// <summary>
	/// Returns the C# wrapper object corresponding to the UObject.
	/// </summary>
	/// <remarks>
	/// When a UObject calls this interface for the first time, a new C# object will be created.<br/>
	/// The UObject and its corresponding C# object are always one-to-one throughout their lifetime, and the same UObject always returns the same C# object.<br/>
	/// Thread safety note: All interfaces are only allowed to be called on the game thread.
	/// </remarks>
	/// <param name="nativeObject">C++ pointer of the UObject.</param>
	/// <typeparam name="T">`UnrealEngine.CoreUObject.Object` or its subclass.</typeparam>
	/// <returns>C# wrapper object.</returns>
	public static T GetManagedObject<T>(IntPtr nativeObject) where T : ObjectBase
	{
		return (T)GetManagedObject(nativeObject);
	}

	/// <summary>
	/// Returns the C# wrapper object corresponding to the UObject.
	/// </summary>
	/// <param name="nativeObject">C++ pointer of the UObject.</param>
	/// <returns>C# wrapper object.</returns>
	private static ObjectBase GetManagedObject(IntPtr nativeObject)
	{
		IntPtr managedHandlePtr = NativeGetManagedObject(nativeObject);
		GCHandle managedHandle = GCHandle.FromIntPtr(managedHandlePtr);
		return (managedHandle.Target as ObjectBase)!;
	}

	/// <summary>
	/// Called by C++ to create a corresponding C# wrapper object for the UObject.
	/// </summary>
	/// <param name="nativeObject">UObject pointer.</param>
	/// <param name="typeHandlePtr">C# type handle</param>
	/// <returns></returns>
	[UnmanagedCallersOnly]
	private static IntPtr CreateManagedObject(IntPtr nativeObject, IntPtr typeHandlePtr)
	{
		RuntimeTypeHandle typeHandle = RuntimeTypeHandle.FromIntPtr(typeHandlePtr);
		Type type = Type.GetTypeFromHandle(typeHandle)!;

		// Call the constructor of the C# type through reflection to properly construct the C# object.
		ObjectBase managedObject = (ObjectBase)RuntimeHelpers.GetUninitializedObject(type);
		const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		var foundConstructor = (delegate*<object, void>) type.GetConstructor(bindingFlags, Type.EmptyTypes)!.MethodHandle.GetFunctionPointer();
		managedObject.NativeObject = nativeObject;
		foundConstructor(managedObject);

		GCHandle managedHadle = GCHandle.Alloc(managedObject);
		return GCHandle.ToIntPtr(managedHadle);
	}

	/// <summary>
	/// Called by C++ to cut off the C# object's reference to the UObject after the UObject is destroyed, and make it available for GC.
	/// </summary>
	/// <param name="managedHandlePtr">Handle of the C# object.</param>
	[UnmanagedCallersOnly]
	private static void FreeManagedObject(IntPtr managedHandlePtr)
	{
		GCHandle managedHandle = GCHandle.FromIntPtr(managedHandlePtr);
		if (managedHandle.Target is ObjectBase objectBase)
		{
			objectBase.NativeObject = IntPtr.Zero;
		}
		managedHandle.Free();
	}

	/// <summary>
	/// Release all C# objects in an assembly at once.
	/// This method is typically called before unloading the assembly to clean up object references.
	/// </summary>
	/// <param name="assembly">Assembly</param>
	internal static void FreeAllManagedObjectsInAssembly(Assembly assembly)
	{
		Type objectBaseType = typeof(ObjectBase);
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (type.IsSubclassOf(objectBaseType))
			{
				fixed (char* typeName = type.Name)
				{
					NativeFreeManagedObjectsByClassName(typeName);
				}
			}
		}
	}

#pragma warning disable CS0649
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> NativeGetManagedObject;
	internal static delegate* unmanaged[Cdecl]<char*, void> NativeFreeManagedObjectsByClassName;
#pragma warning restore CS0649
}

