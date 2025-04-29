using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using SharpScript.Bootstrap;
using UnrealEngine.Intrinsic;

namespace SharpScript;

/// <summary>
/// Framework initialization error codes
/// </summary>
enum StartupError
{
	/// <summary>
	/// Framework version mismatch
	/// </summary>
	VersionNotMatched = 0x1001,

	/// <summary>
	/// Framework startup configuration size mismatch
	/// </summary>
	StarupConfigSizeNotMatched = 0x1002,
}

public static class Main
{
	/// <summary>
	/// Framework startup entry point
	/// </summary>
	/// <param name="startupConfig"> Startup configuration parameters </param>
	/// <returns> Startup error code </returns>
	[UnmanagedCallersOnly]
	public static unsafe int StartupFramework(FrameworkStartupConfig startupConfig)
	{
		if (startupConfig.Version != FrameworkStartupConfig.CurrentVersion)
		{
			return (int)StartupError.VersionNotMatched;
		}

		int startupConfigSize = Marshal.SizeOf(typeof(FrameworkStartupConfig));
		if (startupConfigSize != startupConfig.ConfigSize)
		{
			return (int)StartupError.StarupConfigSizeNotMatched;
		}

		// Assign various function pointers passed from C++ to C# to respective managers
		startupConfig.IntrinsicPointers.AssignIntrinsicPointers();

		// Initialize global exception handling function
		UnhandledExceptionHandler.Initialize();

		// Bind C++ functions
		NativeCallbackBinder.Initialize(startupConfig.IntrinsicPointers.ExportNativeFuncs);

		// Register C# types
		TypeRegistry.RegisterClassTypesInAssembly(Assembly.GetExecutingAssembly());

		Logger.Log("Hello, sharp script!");
		return 0;
	}

	/// <summary>
	/// Framework shutdown callback
	/// </summary>
	[UnmanagedCallersOnly]
	public static void ShutdownFramework()
	{
		UnhandledExceptionHandler.Uninitialize();
	}

	/// <summary>
	/// Register the path corresponding to the assembly name
	/// </summary>
	/// <param name="nativeAssemblyName"> Assembly name, must be unique </param>
	/// <param name="nativeAssemblyPath"> Relative path of the assembly in UE, similar to /Game/Managed/Assemblies/Game.dll </param>
	/// <returns> Returns 0 if successful, otherwise returns an error code </returns>
	/// <remarks>
	/// Before loading (<see cref="LoadAssembly"/>) or reloading (<see cref="ReloadAssembly"/>) an assembly,
	/// this function must be called to register the assembly path, otherwise the loading will fail.
	/// </remarks>
	[UnmanagedCallersOnly]
	public static int RegisterAssembly(IntPtr nativeAssemblyName, IntPtr nativeAssemblyPath)
	{
		string assemblyName = Marshal.PtrToStringUni(nativeAssemblyName)!;
		string assemblyPath = Marshal.PtrToStringUni(nativeAssemblyPath)!;
		return AssemblyManager.RegisterAssembly(assemblyName, assemblyPath) ? 0 : 1;
	}

	/// <summary>
	/// Unregister an assembly
	/// </summary>
	/// <param name="nativeAssemblyName"> Assembly name </param>
	[UnmanagedCallersOnly]
	public static void UnregisterAssembly(IntPtr nativeAssemblyName)
	{
		string assemblyName = Marshal.PtrToStringUni(nativeAssemblyName)!;
		AssemblyManager.UnregisterAssembly(assemblyName);
	}

	/// <summary>
	/// Query if an assembly name has been registered
	/// </summary>
	/// <param name="nativeAssemblyName"> Assembly name to query </param>
	/// <returns> Whether the assembly has been registered </returns>
	[UnmanagedCallersOnly]
	public static int IsAssemblyRegistered(IntPtr nativeAssemblyName)
	{
		string assemblyName = Marshal.PtrToStringUni(nativeAssemblyName)!;
		return AssemblyManager.IsAssemblyRegistered(assemblyName) ? 0 : 1;
	}

	/// <summary>
	/// Load an assembly
	/// </summary>
	/// <param name="nativeAssemblyName"> Assembly name </param>
	/// <returns> Load result, see <see cref="LoadAssemblyResult"/> </returns>
	/// <remarks> If the assembly with the specified name is already loaded, it will return directly. </remarks>
	[UnmanagedCallersOnly]
	public static int LoadAssembly(IntPtr nativeAssemblyName)
	{
		string assemblyName = Marshal.PtrToStringUni(nativeAssemblyName)!;
		return (int)AssemblyManager.LoadAssembly(assemblyName);
	}

	/// <summary>
	/// Unload an assembly
	/// </summary>
	/// <param name="nativeAssemblyName"> Assembly name </param>
	/// <returns> Unload result, see <see cref="LoadAssemblyResult"/> </returns>
	/// <remarks>
	/// If the assembly to be unloaded is being depended on by other assemblies, all dependent assemblies will be unloaded together.
	/// </remarks>
	[UnmanagedCallersOnly]
	public static int UnloadAssembly(IntPtr nativeAssemblyName)
	{
		string assemblyName = Marshal.PtrToStringUni(nativeAssemblyName)!;
		return (int)AssemblyManager.UnloadAssembly(assemblyName);
	}

	/// <summary>
	/// Reload an assembly
	/// </summary>
	/// <param name="nativeAssemblyName"> Assembly name </param>
	/// <returns> Load result, see <see cref="LoadAssemblyResult"/> </returns>
	/// <remarks>
	/// If the assembly to be reloaded has not been loaded, it will be loaded directly.
	/// If the assembly to be reloaded is being depended on by other assemblies, all dependent assemblies will be reloaded together.
	/// </remarks>
	[UnmanagedCallersOnly]
	public static int ReloadAssembly(IntPtr nativeAssemblyName)
	{
		string assemblyName = Marshal.PtrToStringUni(nativeAssemblyName)!;
		return (int)AssemblyManager.ReloadAssembly(assemblyName);
	}

	/// <summary>
	/// Query if an assembly with the corresponding name has been loaded
	/// </summary>
	/// <param name="nativeAssemblyName"> Assembly name to query </param>
	/// <returns> Returns whether the assembly has been loaded </returns>
	[UnmanagedCallersOnly]
	public static int IsAssemblyLoaded(IntPtr nativeAssemblyName)
	{
		string assemblyName = Marshal.PtrToStringUni(nativeAssemblyName)!;
		return AssemblyManager.IsAssemblyLoaded(assemblyName) ? 0 : 1;
	}

	/// <summary>
	/// Load an assembly from a byte array. This interface is for internal use only.
	/// </summary>
	/// <param name="nativeLoadContext"> ALC that loads this assembly </param>
	/// <param name="nativeAssemblyBytes"> Assembly (dll) byte array </param>
	/// <param name="assemblyBytesLength"> Length of the assembly byte array </param>
	/// <param name="nativeSymbolBytes"> Assembly symbol table (pdb) byte array, null means no debug symbol data </param>
	/// <param name="symbolBytesLength"> Length of the assembly symbol table byte array, 0 means no debug symbol data </param>
	/// <returns> Whether the loading was successful </returns>
	[UnmanagedCallersOnly]
	public static unsafe int InternalLoadAssemblyBytes(IntPtr nativeLoadContext, byte* nativeAssemblyBytes, int assemblyBytesLength, byte* nativeSymbolBytes, int symbolBytesLength)
	{
		GCHandle loadContextHandle = (GCHandle)nativeLoadContext;
		AssemblyLoadContext loadContext = (loadContextHandle.Target as AssemblyLoadContext)!;

		UnmanagedMemoryStream assemblyStream = new UnmanagedMemoryStream(nativeAssemblyBytes, assemblyBytesLength);
		UnmanagedMemoryStream? symbolStream = null;
		if (nativeSymbolBytes != null && symbolBytesLength > 0)
		{
			symbolStream = new UnmanagedMemoryStream(nativeSymbolBytes, symbolBytesLength);
		}

		loadContext.LoadFromStream(assemblyStream, symbolStream);
		return 0;
	}

	/// <summary>
	/// Get the static function pointer of a specified class from a specified assembly
	/// </summary>
	/// <param name="nativeAssemblyName"> Assembly name </param>
	/// <param name="nativeTypeName"> Class name </param>
	/// <param name="nativeFuncName"> Function name </param>
	/// <param name="functionHandle"> Returned function pointer </param>
	/// <returns> Whether the retrieval was successful </returns>
	/// <remarks>
	/// The C# function must be a static function and have the [UnmanagedCallersOnly] attribute.
	/// If the assembly is unloaded or reloaded, the function pointer will become invalid.
	/// </remarks>
	[UnmanagedCallersOnly]
	public static int GetAssemblyFunctionPointer(IntPtr nativeAssemblyName, IntPtr nativeTypeName, IntPtr nativeFuncName, IntPtr functionHandle)
	{
		string assemblyName = Marshal.PtrToStringUni(nativeAssemblyName)!;
		string typeName = Marshal.PtrToStringUni(nativeTypeName)!;
		string funcName = Marshal.PtrToStringUni(nativeFuncName)!;
		return AssemblyManager.GetAssemblyFunctionPointer(assemblyName, typeName, funcName, functionHandle) ? 0 : 1;
	}
}

