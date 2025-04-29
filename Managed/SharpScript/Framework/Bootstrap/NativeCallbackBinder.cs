using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnrealEngine.Intrinsic;

namespace SharpScript.Bootstrap;

/// <summary>
/// Binds unmanaged delegate members marked with [NativeCallback] in classes to C++ functions with the same name.
/// </summary>
public static unsafe class NativeCallbackBinder
{
	/// <summary>
	/// Caches the exported C++ function pointer.
	/// See SsNativeFuncExporter.h `USsNativeFuncExporter::ExportFunctions`.
	/// </summary>
	private static delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<IntPtr, char*, IntPtr, void>, IntPtr, void> _exportNativeFuncs;

	internal static void Initialize(
		delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<IntPtr, char*, IntPtr, void>, IntPtr, void> exportNativeFuncs)
	{
		_exportNativeFuncs = exportNativeFuncs;
		BindNativeCallbacks([Assembly.GetExecutingAssembly()]);
	}

	/// <summary>
	/// Iterates through classes marked with [NativeCallback] in the assemblies and binds unmanaged delegates in these classes to C++ methods.
	/// </summary>
	/// <param name="assemblies">List of assemblies.</param>
	public static void BindNativeCallbacks(List<Assembly> assemblies)
	{
		// Collected unmanaged delegates, key is [ClassName].[FunctionName].
		Dictionary<string, FieldInfo> unmanagedDelegates = new();
		GCHandle unmanagedDelegatesHandle = GCHandle.Alloc(unmanagedDelegates, GCHandleType.Normal);

		foreach (Assembly assembly in assemblies)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (!Attribute.IsDefined(type, typeof(NativeCallbacksAttribute)))
				{
					continue;
				}

				foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
				{
					if (!field.IsStatic || !field.FieldType.IsUnmanagedFunctionPointer)
					{
						continue;
					}
					unmanagedDelegates.TryAdd($"{type.Name}.{field.Name}", field);
				}
			}
		}

		try
		{
			IntPtr unmanagedDelegatesPtr = GCHandle.ToIntPtr(unmanagedDelegatesHandle);
			_exportNativeFuncs(&BindNativeCallback, unmanagedDelegatesPtr);

			foreach (var unmanagedDelegate in unmanagedDelegates)
			{
				if (unmanagedDelegate.Value.GetValue(null) == null)
				{
					Logger.Error($"Failed to bind unmanaged delegate {unmanagedDelegate.Key}");
				}
			}
		}
		catch (Exception e)
		{
			Logger.Error($"Failed to bind native callbacks: {e}");
		}

		unmanagedDelegatesHandle.Free();
	}

	[UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
	private static void BindNativeCallback(IntPtr nativeFuncPtr, char* nativeFuncName, IntPtr unmanagedDelegatesPtr)
	{
		GCHandle handle = GCHandle.FromIntPtr(unmanagedDelegatesPtr);
		Dictionary<string, FieldInfo> unmanagedDelegates = (Dictionary<string, FieldInfo>)handle.Target!;

		string funcName = new string(nativeFuncName);
		if (unmanagedDelegates.TryGetValue(funcName, out FieldInfo? unmanagedDelegate))
		{
			unmanagedDelegate.SetValue(null, nativeFuncPtr);
		}
	}
}
