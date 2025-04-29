#include "SharpScriptHost.h"
#include "coreclr_delegates.h"
#include "hostfxr.h"
#include "SharpScriptHostAPI.h"
#include "HAL/PlatformProcess.h"
#include "Interfaces/IPluginManager.h"
#include "Misc/CommandLine.h"
#include "Misc/ConfigCacheIni.h"
#include "Misc/Paths.h"

DEFINE_LOG_CATEGORY(LogSharpScriptHost);

#define LOCTEXT_NAMESPACE "FSharpScriptHostModule"

load_assembly_and_get_function_pointer_fn HostfxrLoadAssemblyAndGetFunctionPointer = nullptr;
get_function_pointer_fn HostfxrGetFunctionPointer = nullptr;
load_assembly_fn HostfxrLoadAssembly = nullptr;
load_assembly_bytes_fn HostfxrLoadAssemblyBytes = nullptr;

/**
* Whether the Mono runtime is currently being used.
*/
static bool GIsUsingMono = false;


void FSharpScriptHostModule::StartupModule()
{
	const FString PluginDir = FPaths::ConvertRelativePathToFull(IPluginManager::Get().FindPlugin(UE_PLUGIN_NAME)->GetBaseDir());
	const FString ProjectConfigDir = FPaths::ConvertRelativePathToFull(FPaths::ProjectConfigDir());

	// SharpScriptHost is loaded too early, so we can't directly use GetDefault<USharpScriptHostSettings>().
	// We can only manually parse the contents of DefaultSharpScript.ini and BaseSharpScript.ini.
	FString BaseConfigPath = FPaths::Combine(PluginDir, "Config", "Base" UE_PLUGIN_NAME ".ini");
	FString DefaultConfigPath = FPaths::Combine(ProjectConfigDir, "Default" UE_PLUGIN_NAME ".ini");

	FString RuntimeVersion;
	if (!GConfig->GetString(TEXT("/Script/SharpScriptHost.SharpScriptHostSettings"), TEXT("RuntimeVersion"), RuntimeVersion, *DefaultConfigPath))
	{
		GConfig->GetString(TEXT("/Script/SharpScriptHost.SharpScriptHostSettings"), TEXT("RuntimeVersion"), RuntimeVersion, *BaseConfigPath);
	}

#if PLATFORM_WINDOWS
	bool bForceMono = FParse::Param(FCommandLine::Get(), TEXT("forcemono"));
	bool bForceMonoFromSetting = false;
	if (!GConfig->GetBool(TEXT("/Script/SharpScriptHost.SharpScriptHostSettings"), TEXT("bWindowsForceUseMono"), bForceMonoFromSetting, *DefaultConfigPath))
	{
		GConfig->GetBool(TEXT("/Script/SharpScriptHost.SharpScriptHostSettings"), TEXT("bWindowsForceUseMono"), bForceMonoFromSetting, *BaseConfigPath);
	}

	FString WindowsCoreClrRuntimePath;
	if (!GConfig->GetString(TEXT("/Script/SharpScriptHost.SharpScriptHostSettings"), TEXT("WindowsCoreClrRuntimePath"), WindowsCoreClrRuntimePath, *DefaultConfigPath))
	{
		GConfig->GetString(TEXT("/Script/SharpScriptHost.SharpScriptHostSettings"), TEXT("WindowsCoreClrRuntimePath"), WindowsCoreClrRuntimePath, *BaseConfigPath);
	}

	FString WindowsMonoRuntimePath;
	if (!GConfig->GetString(TEXT("/Script/SharpScriptHost.SharpScriptHostSettings"), TEXT("WindowsMonoRuntimePath"), WindowsMonoRuntimePath, *DefaultConfigPath))
	{
		GConfig->GetString(TEXT("/Script/SharpScriptHost.SharpScriptHostSettings"), TEXT("WindowsMonoRuntimePath"), WindowsMonoRuntimePath, *BaseConfigPath);
	}

	FString DotnetRootDir = WindowsCoreClrRuntimePath;
	if (bForceMono || bForceMonoFromSetting)
	{
		DotnetRootDir = WindowsMonoRuntimePath;
		GIsUsingMono = true;
	}
	//DotnetRootDir = "F:/dotnet/dotnet-runtime-8.0.6-win-x64-debug"; // for local debug
	//DotnetRootDir = "F:/dotnet/dotnet-runtime-8.0.6-win-x64-mono-debug"; // for local debug
#else
	FString DotnetRootDir;
#error "Unsuported platform"
#endif

	static constexpr const TCHAR* PluginDirMacro = TEXT("$(PluginDir)");
	if (DotnetRootDir.StartsWith(PluginDirMacro))
	{
		DotnetRootDir = PluginDir / DotnetRootDir.RightChop(FCString::Strlen(PluginDirMacro));
	}

	const FString HostfxrPath = DotnetRootDir /  "host/fxr" / RuntimeVersion / "hostfxr.dll";
	const FString AssembliesPath = PluginDir / "Managed/Assemblies";
	const FString RuntimeConfigPath = AssembliesPath / "SharpScript.runtimeconfig.json";
	const FString FrameworkAssemblyPath = AssembliesPath / "SharpScript.dll";

	UE_LOG(LogSharpScriptHost, Display, TEXT("Loading hostfxr library from \"%s\""), *HostfxrPath);

	HostfxrDllHandle = FPlatformProcess::GetDllHandle(*HostfxrPath);
	if (!HostfxrDllHandle)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("hostfxr loading failed!"));
		return;
	}

	UE_LOG(LogSharpScriptHost, Display, TEXT("hostfxr loaded successfuly!"));

	hostfxr_set_error_writer_fn HostfxrSetErrorWriter = (hostfxr_set_error_writer_fn)FPlatformProcess::GetDllExport(
		HostfxrDllHandle, TEXT("hostfxr_set_error_writer"));
	if (!HostfxrSetErrorWriter)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("Unable to locate hostfxr_set_error_writer entry point!"));
		return;
	}

	hostfxr_initialize_for_runtime_config_fn HostfxrInitializeForRuntimeConfig = (
		hostfxr_initialize_for_runtime_config_fn)FPlatformProcess::GetDllExport(
		HostfxrDllHandle, TEXT("hostfxr_initialize_for_runtime_config"));
	if (!HostfxrInitializeForRuntimeConfig)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("Unable to locate hostfxr_initialize_for_runtime_config entry point!"));
		return;
	}

	hostfxr_get_runtime_delegate_fn HostfxrGetRuntimeDelegate = (hostfxr_get_runtime_delegate_fn)
		FPlatformProcess::GetDllExport(HostfxrDllHandle, TEXT("hostfxr_get_runtime_delegate"));
	if (!HostfxrGetRuntimeDelegate)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("Unable to locate hostfxr_get_runtime_delegate entry point!"));
		return;
	}

	hostfxr_close_fn HostfxrClose = (hostfxr_close_fn)FPlatformProcess::GetDllExport(
		HostfxrDllHandle, TEXT("hostfxr_close"));
	if (!HostfxrClose)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("Unable to locate hostfxr_close entry point!"));
		return;
	}

	// Set the error callback function before initializing the virtual machine to capture errors that occur during the initialization process
	HostfxrSetErrorWriter(&OnHostError);

	hostfxr_handle HostfxrContext = nullptr;
	if (HostfxrInitializeForRuntimeConfig(*RuntimeConfigPath, nullptr, &HostfxrContext) != 0 || !HostfxrContext)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("Unable to initialize the host! Please, try to restart the engine."));
		HostfxrClose(HostfxrContext);
		return;
	}

	if (HostfxrGetRuntimeDelegate(HostfxrContext, hdt_load_assembly_and_get_function_pointer, (void**)&HostfxrLoadAssemblyAndGetFunctionPointer) != 0
		|| !HostfxrLoadAssemblyAndGetFunctionPointer)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("Unable to get hdt_load_assembly_and_get_function_pointer runtime delegate!"));
		HostfxrClose(HostfxrContext);
		return;
	}

	if (HostfxrGetRuntimeDelegate(HostfxrContext, hdt_get_function_pointer, (void**)&HostfxrGetFunctionPointer) != 0
		|| !HostfxrGetFunctionPointer)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("Unable to get hdt_get_function_pointer runtime delegate!"));
		HostfxrClose(HostfxrContext);
		return;
	}

	if (HostfxrGetRuntimeDelegate(HostfxrContext, hdt_load_assembly, (void**)&HostfxrLoadAssembly) != 0
		|| !HostfxrLoadAssembly)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("Unable to get hdt_load_assembly runtime delegate!"));
		HostfxrClose(HostfxrContext);
		return;
	}

	if (HostfxrGetRuntimeDelegate(HostfxrContext, hdt_load_assembly_bytes, (void**)&HostfxrLoadAssemblyBytes) != 0
		|| !HostfxrLoadAssemblyBytes)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("Unable to get hdt_load_assembly_bytes runtime delegate!"));
		HostfxrClose(HostfxrContext);
		return;
	}

	UE_LOG(LogSharpScriptHost, Display, TEXT("Host functions loaded successfuly!"));
	HostfxrClose(HostfxrContext);

	int result = HostfxrLoadAssembly(
		*FrameworkAssemblyPath,
		nullptr, nullptr);
	if (result != 0)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("Load framework assembly failed!"));
		return;
	}

	int (*PreloadForDebugger)() = nullptr;
	result = HostfxrGetFunctionPointer(
		TEXT("SharpScript.Preload, SharpScript"),
		TEXT("PreloadForDebugger"),
		UNMANAGEDCALLERSONLY_METHOD,
		nullptr, nullptr,
		(void**)&PreloadForDebugger);
	if (result != 0 || !PreloadForDebugger || PreloadForDebugger() != 0)
	{
		UE_LOG(LogSharpScriptHost, Error, TEXT("Host initialize failed!"));
		return;
	}

	UE_LOG(LogSharpScriptHost, Display, TEXT("Host initialize successfuly!"));
}

void FSharpScriptHostModule::ShutdownModule()
{
	if (HostfxrDllHandle)
	{
		FPlatformProcess::FreeDllHandle(HostfxrDllHandle);
		HostfxrDllHandle = nullptr;
	}
}

bool FSharpScriptHostModule::IsUsingMonoRuntime()
{
	return GIsUsingMono;
}

void FSharpScriptHostModule::OnHostError(const TCHAR* Message)
{
	UE_LOG(LogSharpScriptHost, Error, TEXT("%s"), Message);
}

#undef LOCTEXT_NAMESPACE

IMPLEMENT_MODULE(FSharpScriptHostModule, SharpScriptHost)

