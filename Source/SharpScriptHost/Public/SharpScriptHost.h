#pragma once
#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"

DECLARE_LOG_CATEGORY_EXTERN(LogSharpScriptHost, Log, All);

/**
 * Load dotnet runtime and expose host APIs.
 * Visual Studio's mixed debugging mode [Mixed (.Net Core)] can only set breakpoints/step after CoreCLR is loaded,
 * so this module will load CoreCLR as early as possible.
 */
class FSharpScriptHostModule : public IModuleInterface
{
public:
    virtual void StartupModule() override;
    virtual void ShutdownModule() override;

	/**
	* Whether using Mono runtime.
	*/
	SHARPSCRIPTHOST_API static bool IsUsingMonoRuntime();

private:
	/**
	 * Callback function when dotnet runtime reports an error during initiating.
	 * See: hostfxr_error_writer_fn
	 */
	static void OnHostError(const TCHAR* Message);

private:
	/** Holds reference to hostfxr dll */
	void* HostfxrDllHandle = nullptr;
};

