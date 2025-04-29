using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using SharpScript.Bootstrap;
using UnrealEngine.Intrinsic;

namespace SharpScript;

/// <summary>
/// Assembly loading error codes
/// </summary>
public enum LoadAssemblyResult
{
	/// <summary>
	/// Load successful
	/// </summary>
	Success = 0,

	/// <summary>
	/// Assembly path to be loaded not found
	/// </summary>
	AssemblyPathNotFound = 1,

	/// <summary>
	/// Assembly unload failed, usually due to existing references to the assembly.
	/// See official documentation https://learn.microsoft.com/en-us/dotnet/standard/assembly/unloadability
	/// </summary>
	AssemblyUnloadFailed = 2,

	/// <summary>
	/// Assembly load failed
	/// </summary>
	AssemblyLoadFailed = 3,
}

/// <summary>
/// Record dynamically loaded assembly information, used to support unloading and reloading
/// </summary>
class LoadedAssemblyInfo(string assemblyName, string assemblyPath)
{
	/// <summary>
	/// Assembly name
	/// </summary>
	/// <remarks> The assembly name must be unique, currently duplicate assembly names are not allowed. </remarks>
	public readonly string AssemblyName = assemblyName;

	/// <summary>
	/// Assembly path
	/// </summary>
	/// <remarks>
	/// This is a relative path, for example /Game/Plugins/SharpScript/Managed/Assemblies/SharpScript.dll
	/// Relative path is used to support reading assemblies from pak.
	/// </remarks>
	public readonly string AssemblyPath = assemblyPath;

	/// <summary>
	/// Assembly load context
	/// </summary>
	public readonly SingleAssemblyLoadContext? LoadContext = new(assemblyName, assemblyPath);

	/// <summary>
	/// Loaded assembly
	/// </summary>
	public Assembly? Assembly;

	// /// <summary>
	// /// Records the names of assemblies that reference this assembly
	// /// </summary>
	// /// // todo: twx This field is not used yet
	// public readonly List<string> ReferencedByAssemblies = new();
}

/// <summary>
/// Record unloaded assembly information
/// </summary>
struct UnloadedAssemblyInfo(string assemblyName, WeakReference alcRef)
{
	/// <summary>
	/// Name of the unloaded assembly
	/// </summary>
	public readonly string AssemblyName = assemblyName;

	/// <summary>
	/// Weak reference to the unloaded assembly context, used to determine if the assembly has been successfully GC'd
	/// </summary>
	public readonly WeakReference AlcRef = alcRef;
}

/// <summary>
/// ALC that supports dynamic loading and unloading, needs to be used with AssemblyManager
/// </summary>
class SingleAssemblyLoadContext : AssemblyLoadContext
{
	/// <summary>
	/// Initialize using the primary assembly name and path.
	/// </summary>
	/// <param name="primaryAssemblyName"> Primary assembly name </param>
	/// <param name="primaryAssemblyPath"> Primary assembly load path </param>
	internal SingleAssemblyLoadContext(string primaryAssemblyName, string primaryAssemblyPath)
		: base(isCollectible: true, name: primaryAssemblyName)
	{
		_primaryAssemblyName = primaryAssemblyName;
		_primaryAssemblyPath = primaryAssemblyPath;
		Resolving += (context, assemblyName)
			=> (context as SingleAssemblyLoadContext)!.LoadDependingAssembly(assemblyName);
	}

	/// <summary>
	/// Load the primary assembly.
	/// </summary>
	internal unsafe Assembly? LoadPrimaryAssembly()
	{
		Debug.Assert(!Assemblies.Any());

		GCHandle loadContextHandle = GCHandle.Alloc(this);
		int result;
		fixed (char* assemblyPathPointer = _primaryAssemblyPath)
		{
			result = AssemblyManager.NativeLoadAssembly((IntPtr)loadContextHandle, assemblyPathPointer);
		}

		loadContextHandle.Free();

		if (result != 0)
		{
			return null;
		}

		Debug.Assert(Assemblies.Any());
		Debug.Assert(Assemblies.First().GetName().Name == _primaryAssemblyName);
		Assembly assembly = Assemblies.First();
		OnAssemblyLoaded(assembly);
		return assembly;
	}

	/// <summary>
	/// Load other assemblies that the primary assembly depends on.
	/// </summary>
	/// <param name="dependingAssemblyName">Referenced assembly name</param>
	/// <remarks>The assembly needs to be in the same directory as the primary assembly.</remarks>
	private unsafe Assembly? LoadDependingAssembly(AssemblyName dependingAssemblyName)
	{
		string assemblyPath = Path.GetDirectoryName(_primaryAssemblyPath)!;
		assemblyPath = Path.Combine(assemblyPath, dependingAssemblyName.Name + ".dll");

		GCHandle loadContextHandle = GCHandle.Alloc(this);
		int result;
		fixed (char* assemblyPathPointer = assemblyPath)
		{
			result = AssemblyManager.NativeLoadAssembly((IntPtr)loadContextHandle, assemblyPathPointer);
		}

		loadContextHandle.Free();

		if (result != 0)
		{
			return null;
		}

		Debug.Assert(Assemblies.Last().GetName().Name == dependingAssemblyName.Name);
		Assembly assembly = Assemblies.Last();
		OnAssemblyLoaded(assembly);
		return assembly;
	}

	/// <summary>
	/// Callback for successful assembly loading.
	/// </summary>
	/// <param name="assembly"></param>
	private void OnAssemblyLoaded(Assembly assembly)
	{
		NativeCallbackBinder.BindNativeCallbacks([assembly]);
		TypeRegistry.RegisterClassTypesInAssembly(assembly);
	}

	/// <summary>
	/// Primary assembly name
	/// </summary>
	private readonly string _primaryAssemblyName;

	/// <summary>
	/// Primary assembly load path
	/// </summary>
	private readonly string _primaryAssemblyPath;
}

/// <summary>
/// Manage dynamic loading and unloading of assemblies
/// </summary>
[NativeCallbacks]
public static unsafe class AssemblyManager
{
	/// <summary>
	/// Register the path corresponding to the assembly name
	/// </summary>
	/// <param name="assemblyName"> Assembly name, must be unique </param>
	/// <param name="assemblyPath"> Relative path of the assembly in UE, for example: /Game/Managed/Assemblies/Game.dll </param>
	/// <returns> Returns whether the registration was successful. If an assembly with the corresponding name is already registered, it returns failure. </returns>
	/// <remarks>
	/// Before loading (<see cref="LoadAssembly"/>) or reloading (<see cref="ReloadAssembly"/>) an assembly,
	/// this function must be called to register the assembly path, otherwise the loading will fail.
	/// </remarks>
	public static bool RegisterAssembly(string assemblyName, string assemblyPath)
	{
		return AssemblyPaths.TryAdd(assemblyName, assemblyPath);
	}

	/// <summary>
	/// Unregister assembly
	/// </summary>
	/// <param name="assemblyName"> Assembly name </param>
	public static void UnregisterAssembly(string assemblyName)
	{
		AssemblyPaths.Remove(assemblyName);
	}

	/// <summary>
	/// Query whether the assembly name has been registered
	/// </summary>
	/// <param name="assemblyName"> Assembly name to query </param>
	/// <returns> Whether the assembly has been registered </returns>
	public static bool IsAssemblyRegistered(string assemblyName)
	{
		return AssemblyPaths.ContainsKey(assemblyName);
	}

	/// <summary>
	/// Load assembly
	/// </summary>
	/// <param name="assemblyName"> Assembly name, must be unique </param>
	/// <returns> Load result, see <see cref="LoadAssemblyResult"/> </returns>
	/// <remarks> If the assembly with the specified name is already loaded, it will directly return success. </remarks>
	public static LoadAssemblyResult LoadAssembly(string assemblyName)
	{
		if (LoadedAssemblyInfos.ContainsKey(assemblyName))
		{
			return LoadAssemblyResult.Success;
		}

		if (!AssemblyPaths.TryGetValue(assemblyName, out var assemblyPath))
		{
			return LoadAssemblyResult.AssemblyPathNotFound;
		}

		var assemblyInfo = new LoadedAssemblyInfo(assemblyName, assemblyPath);
		Assembly? assembly = assemblyInfo.LoadContext!.LoadPrimaryAssembly();
		if (assembly == null)
		{
			return LoadAssemblyResult.AssemblyLoadFailed;
		}

		Debug.Assert(assemblyInfo.Assembly == null);
		assemblyInfo.Assembly = assembly;

		Debug.Assert(!LoadedAssemblyInfos.ContainsKey(assemblyName));
		LoadedAssemblyInfos.Add(assemblyName, assemblyInfo);

		return LoadAssemblyResult.Success;
	}

	/// <summary>
	/// Unload assembly
	/// </summary>
	/// <param name="assemblyName"> Assembly name </param>
	/// <returns> Unload result, see <see cref="LoadAssemblyResult"/> </returns>
	/// <remarks>
	/// If the assembly to be unloaded is being depended on by other assemblies, all dependent assemblies will be unloaded together.
	/// </remarks>
	public static LoadAssemblyResult UnloadAssembly(string assemblyName)
	{
		var unloadedAssemblyInfos = new List<UnloadedAssemblyInfo>();
		InternalUnloadAssembly(assemblyName, ref unloadedAssemblyInfos);
		bool unloadSuccess = InternalWaitUnloadAssemblies(unloadedAssemblyInfos);
		return unloadSuccess ? LoadAssemblyResult.Success : LoadAssemblyResult.AssemblyUnloadFailed;
	}

	/// <summary>
	/// Reload assembly
	/// </summary>
	/// <param name="assemblyName"> Assembly name </param>
	/// <returns> Load result, see <see cref="LoadAssemblyResult"/> </returns>
	/// <remarks>
	/// If the assembly to be reloaded has not been loaded, it will directly load the assembly.
	/// If the assembly to be reloaded is being depended on by other assemblies, all dependent assemblies will be reloaded together.
	/// </remarks>
	public static LoadAssemblyResult ReloadAssembly(string assemblyName)
	{
		var unloadedAssemblyInfos = new List<UnloadedAssemblyInfo>();
		InternalUnloadAssembly(assemblyName, ref unloadedAssemblyInfos);
		if (!InternalWaitUnloadAssemblies(unloadedAssemblyInfos))
		{
			return LoadAssemblyResult.AssemblyUnloadFailed;
		}

		LoadAssemblyResult result = LoadAssembly(assemblyName);
		if (result != LoadAssemblyResult.Success)
		{
			return result;
		}

		if (unloadedAssemblyInfos.Count > 0)
		{
			Debug.Assert(unloadedAssemblyInfos[0].AssemblyName == assemblyName);
			// Reload other assemblies that depend on this assembly
			// The list is ordered. The first assembly must be the one currently being reloaded, so it doesn't need to be loaded again.
			for (int i = 1; i < unloadedAssemblyInfos.Count; ++i)
			{
				result = LoadAssembly(unloadedAssemblyInfos[i].AssemblyName);
				if (result != LoadAssemblyResult.Success)
				{
					return result;
				}
			}
		}

		return LoadAssemblyResult.Success;
	}

	/// <summary>
	/// Query whether the assembly with the corresponding name has been loaded
	/// </summary>
	/// <param name="assemblyName"> Name of the assembly to query </param>
	/// <returns> Returns whether the assembly has been loaded </returns>
	public static bool IsAssemblyLoaded(string assemblyName)
	{
		if (LoadedAssemblyInfos.TryGetValue(assemblyName, out var loadedAssemblyInfo))
		{
			return loadedAssemblyInfo.Assembly != null;
		}

		return false;
	}

	/// <summary>
	/// Get the loaded assembly by name
	/// </summary>
	/// <param name="assemblyName"> Assembly name </param>
	/// <returns> Returns the loaded assembly. If the corresponding assembly has not been loaded, it returns null. </returns>
	public static Assembly? GetLoadedAssembly(string assemblyName)
	{
		if (LoadedAssemblyInfos.TryGetValue(assemblyName, out var loadedAssemblyInfo))
		{
			return loadedAssemblyInfo.Assembly;
		}

		return null;
	}

	/// <summary>
	/// Recursively release the assembly and all its dependencies
	/// </summary>
	/// <param name="assemblyName"> Name of the assembly to release </param>
	/// <param name="unloadedAssemblyInfos"> Returns the list of released assemblies </param>
	[MethodImpl(MethodImplOptions.NoInlining)] // Prevent JIT from inlining code causing alc reference residue that cannot be unloaded
	private static void InternalUnloadAssembly(string assemblyName, ref List<UnloadedAssemblyInfo> unloadedAssemblyInfos)
	{
		if (!LoadedAssemblyInfos.Remove(assemblyName, out var assemblyInfo))
		{
			// The assembly has already been released
			return;
		}

		unloadedAssemblyInfos.Add(new(assemblyInfo.AssemblyName, new WeakReference(assemblyInfo.LoadContext)));

		// // If the assembly to be unloaded is being referenced by other assemblies, other assemblies referencing this assembly need to be unloaded at the same time
		// if (assemblyInfo.ReferencedByAssemblies.Count > 0)
		// {
		// 	foreach (var referencedByAssemblyName in assemblyInfo.ReferencedByAssemblies)
		// 	{
		// 		InternalUnloadAssembly(referencedByAssemblyName, ref unloadedAssemblyInfos);
		// 	}
		// }

		// Clean up instances and types in the assembly
		Debug.Assert(assemblyInfo.Assembly != null);
		Debug.Assert(assemblyInfo.LoadContext != null);
		foreach (Assembly assembly in assemblyInfo.LoadContext.Assemblies)
		{
			HouseKeeper.FreeAllManagedObjectsInAssembly(assembly);
			TypeRegistry.UnregisterClassTypesInAssembly(assembly);
		}

		// Trigger assembly unload
		assemblyInfo.LoadContext.Unload();
	}

	/// <summary>
	/// Wait for assembly unloading to complete
	/// </summary>
	/// <param name="unloadedAssemblyInfos"> List of assemblies being unloaded </param>
	/// <returns> Whether all assemblies have been successfully unloaded </returns>
	private static bool InternalWaitUnloadAssemblies(List<UnloadedAssemblyInfo> unloadedAssemblyInfos)
	{
		bool unloadSuccess = true;
		foreach (var unloadedAssemblyInfo in unloadedAssemblyInfos)
		{
			var assemblyName = unloadedAssemblyInfo.AssemblyName;
			var alcRef = unloadedAssemblyInfo.AlcRef;

			for (int i = 0; alcRef.IsAlive && (i < 10); ++i)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}

			if (alcRef.IsAlive)
			{
				Logger.Warning($"failed to unload assembly \"{assemblyName}\"");
				unloadSuccess = false;
			}
		}

		return unloadSuccess;
	}

	/// <summary>
	/// Get the static function pointer of the specified class from the specified assembly
	/// </summary>
	/// <param name="assemblyName"> Assembly name </param>
	/// <param name="typeName"> Class name </param>
	/// <param name="funcName"> Function name </param>
	/// <param name="functionHandle"> Returned function pointer </param>
	/// <returns> Whether the acquisition was successful </returns>
	/// <remarks>
	/// The C# function must be a static function and have the [UnmanagedCallersOnly] attribute.
	/// If the assembly is unloaded or reloaded, the function pointer will become invalid.
	/// </remarks>
	public static bool GetAssemblyFunctionPointer(string assemblyName, string typeName, string funcName, IntPtr functionHandle)
	{
		if (!LoadedAssemblyInfos.TryGetValue(assemblyName, out var loadedAssemblyInfo))
		{
			Logger.Error($"failed to get loaded assembly info \"{assemblyName}\"");
			return false;
		}

		Assembly? assembly = loadedAssemblyInfo.Assembly;
		if (assembly == null)
		{
			Logger.Error($"failed to get assembly from loaded assembly info \"{assemblyName}\"");
			return false;
		}

		Type type = assembly.GetType(typeName, throwOnError: true)!;

		BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		MethodInfo? methodInfo = type.GetMethod(funcName, bindingFlags);
		if (methodInfo == null)
		{
			throw new MissingMethodException(typeName, funcName);
		}

		if (methodInfo.GetCustomAttribute<UnmanagedCallersOnlyAttribute>() == null)
		{
			throw new InvalidOperationException("Function not marked with UnmanagedCallersOnlyAttribute.");
		}

		*(IntPtr*)functionHandle = methodInfo.MethodHandle.GetFunctionPointer();
		return true;
	}

	/// <summary>
	/// Mapping of assembly names to paths
	/// Key: Assembly name
	/// Val: Assembly path
	/// </summary>
	private static readonly Dictionary<string, string> AssemblyPaths = new();

	/// <summary>
	/// Currently loaded assemblies
	/// Key: Assembly name
	/// </summary>
	private static readonly Dictionary<string, LoadedAssemblyInfo> LoadedAssemblyInfos = new();

#pragma warning disable CS0649
	internal static delegate* unmanaged[Cdecl]<IntPtr, void*, int> NativeLoadAssembly;
#pragma warning restore CS0649
}

