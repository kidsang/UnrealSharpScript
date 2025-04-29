using System.Reflection;
using UnrealEngine.Intrinsic;

namespace SharpScript;

/// <summary>
/// Manages the mapping between UE types and C# types.<br/>
/// Thread safety note: All interfaces are only allowed to be called on the game thread.
/// </summary>
[NativeCallbacks]
public static unsafe class TypeRegistry
{
	/// <summary>
	/// Binds all C# types in the assembly to UE types.
	/// </summary>
	public static void RegisterClassTypesInAssembly(Assembly assembly)
	{
		Type objectBaseType = typeof(ObjectBase);
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (type.IsSubclassOf(objectBaseType))
			{
				RuntimeTypeHandle typeHandle = type.TypeHandle;
				fixed (char* typeName = type.Name)
				{
					NativeRegisterClassTypeByName(typeName, RuntimeTypeHandle.ToIntPtr(typeHandle));
				}
			}
		}
	}

	/// <summary>
	/// Unbinds all C# types in the assembly.
	/// </summary>
	public static void UnregisterClassTypesInAssembly(Assembly assembly)
	{
		Type objectBaseType = typeof(ObjectBase);
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (type.IsSubclassOf(objectBaseType))
			{
				fixed (char* typeName = type.Name)
				{
					NativeUnegisterClassTypeByName(typeName);
				}
			}
		}
	}

	/// <summary>
	/// Binds UE type to C# type.
	/// </summary>
	/// <param name="nativeClass">UClass pointer</param>
	/// <param name="type">C# type</param>
	public static void RegisterClassType(IntPtr nativeClass, Type type)
	{
		RuntimeTypeHandle typeHandle = type.TypeHandle;
		NativeRegisterClassType(nativeClass, RuntimeTypeHandle.ToIntPtr(typeHandle));
	}

	/// <summary>
	/// Unbinds UE type from C# type.
	/// </summary>
	/// <param name="nativeClass">UClass pointer</param>
	public static void UnregisterClassType(IntPtr nativeClass)
	{
		NativeUnregisterClassType(nativeClass);
	}

	/// <summary>
	/// Returns the C# type corresponding to the UE type.
	/// </summary>
	/// <param name="nativeClass">UClass pointer</param>
	/// <returns>Returns the C# type corresponding to the UClass, which may be null.</returns>
	public static Type? GetManagedClassType(IntPtr nativeClass)
	{
		IntPtr typeHandlePtr = NativeGetManagedClassType(nativeClass);
		if (typeHandlePtr == IntPtr.Zero)
		{
			return null;
		}

		RuntimeTypeHandle typeHandle = RuntimeTypeHandle.FromIntPtr(typeHandlePtr);
		return Type.GetTypeFromHandle(typeHandle);
	}

#pragma warning disable CS0649
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> NativeRegisterClassType;
	internal static delegate* unmanaged[Cdecl]<char*, IntPtr, void> NativeRegisterClassTypeByName;
	internal static delegate* unmanaged[Cdecl]<IntPtr, void> NativeUnregisterClassType;
	internal static delegate* unmanaged[Cdecl]<char*, void> NativeUnegisterClassTypeByName;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> NativeGetManagedClassType;
#pragma warning restore CS0649
}
