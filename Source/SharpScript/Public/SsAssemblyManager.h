#pragma once
#include "CoreMinimal.h"
#include "SsNativeFuncExporter.h"
#include "SsAssemblyManager.generated.h"

/**
 * Manage dynamic loading and unloading of C# assemblies
 */
class SHARPSCRIPT_API FSsAssemblyManager
{
public:
	/** Return the manager instance */
	static FSsAssemblyManager* Get();

	/** Initialize the manager */
	static bool Initialize();

	/** Finalize the manager */
	static void Finalize();

	/**
	 * Register the path corresponding to the assembly name
	 * @param AssemblyName Assembly name, must be unique
	 * @param AssemblyPath Relative path of the assembly in UE, e.g.: /Game/Managed/Assemblies/Game.dll
	 * @return Returns whether the registration was successful. If an assembly with the corresponding name is already registered, it returns failure.
	 * @remarks This function must be called to register the assembly path before loading or reloading the assembly, otherwise the loading will fail.
	 * @see LoadAssembly ReloadAssembly
	 */
	static bool RegisterAssembly(const TCHAR* AssemblyName, const TCHAR* AssemblyPath);

	/**
	 * Unregister the assembly
	 * @param AssemblyName Assembly name
	 */
	static void UnregisterAssembly(const TCHAR* AssemblyName);

	/**
	 * Query whether the assembly name has been registered
	 * @param AssemblyName The assembly name to query
	 * @return Whether the assembly has been registered
	 */
	static bool IsAssemblyRegistered(const TCHAR* AssemblyName);

	/**
	 * Load the assembly
	 * @param AssemblyName Assembly name
	 * @return Whether the loading was successful
	 * @remarks If the assembly with the specified name is already loaded, it will directly return success.
	 */
	static bool LoadAssembly(const TCHAR* AssemblyName);

	/**
	 * Unload the assembly
	 * @param AssemblyName Assembly name
	 * @return Whether the unloading was successful
	 * @remarks If the assembly to be unloaded is being depended on by other assemblies, all dependent assemblies will be unloaded together.
	 */
	static bool UnloadAssembly(const TCHAR* AssemblyName);

	/**
	 * Reload the assembly
	 * @param AssemblyName Assembly name
	 * @return Whether the reloading was successful
	 * @remarks If the assembly to be reloaded has not been loaded, it will directly load the assembly.
	 *			If the assembly to be reloaded is being depended on by other assemblies, all dependent assemblies will be reloaded together.
	 */
	static bool ReloadAssembly(const TCHAR* AssemblyName);

	/**
	 * Query whether the assembly with the corresponding name has been loaded
	 * @param AssemblyName The assembly name to query
	 * @return Returns whether the assembly has been loaded
	 */
	static bool IsAssemblyLoaded(const TCHAR* AssemblyName);

	/**
	 * Get the static function pointer of the specified class from the specified assembly
	 * @param AssemblyName Assembly name
	 * @param TypeName Class name
	 * @param FuncName Function name
	 * @param OutFuncPointer Returned function pointer
	 * @return Whether the operation was successful
	 * @remarks The C# function must be a static function and have the [UnmanagedCallersOnly] attribute.
	 *			If the assembly is unloaded or reloaded, the function pointer will become invalid.
	 */
	static bool GetAssemblyFunctionPointer(const TCHAR* AssemblyName, const TCHAR* TypeName, const TCHAR* FuncName, void** OutFuncPointer);

private:
	/**
	 * Load the assembly into the corresponding ALC
	 * @param AssemblyLoadContext Assembly load context
	 * @param AssemblyPath Assembly path, this is a relative path.
	 *		For example: /Game/Plugins/SharpScript/Managed/Assemblies/SharpScript.dll
	 * @return Returns 0 if successful, otherwise returns an error code.
	 * @remarks This interface is for C# use, do not call it in C++.
	 */
	static int InternalLoadAssembly(void* AssemblyLoadContext, const TCHAR* AssemblyPath);

private:
	static FSsAssemblyManager* Instance;

	/** C# function pointers */
	static int (*ManagedRegisterAssembly)(const TCHAR* AssemblyName, const TCHAR* AssemblyPath);
	static void (*ManagedUnregisterAssembly)(const TCHAR* AssemblyName);
	static int (*ManagedIsAssemblyRegistered)(const TCHAR* AssemblyName);
	static int (*ManagedLoadAssembly)(const TCHAR* AssemblyName);
	static int (*ManagedUnloadAssembly)(const TCHAR* AssemblyName);
	static int (*ManagedReloadAssembly)(const TCHAR* AssemblyName);
	static int (*ManagedIsAssemblyLoaded)(const TCHAR* AssemblyName);
	static int (*ManagedInternalLoadAssemblyBytes)(void* AssemblyLoadContext, const uint8* AssemblyBytes, uint32 AssemblyBytesLength, const uint8* SymbolBytes, uint32 SymbolBytesLength);
	static int (*ManagedGetAssemblyFunctionPointer)(const TCHAR* AssemblyName, const TCHAR* TypeName, const TCHAR* FuncName, void** OutFuncPointer);

private:
	friend class USsAssemblyManagerExporter;
};

UCLASS()
class USsAssemblyManagerExporter : public USsNativeFuncExporter
{
	GENERATED_BODY()

protected:
	//~ Begin USsNativeFuncExporter Interface
	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
	//~ End USsNativeFuncExporter Interface
};
