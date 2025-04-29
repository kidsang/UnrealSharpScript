#include "SharpScript.h"
#include "SsAssemblyManager.h"
#include "SsCommon.h"
#include "SsFrameworkStartupConfig.h"
#include "SsFrameworkUtils.h"
#include "SsHouseKeeper.h"
#include "SsTypeRegistry.h"
#include "Interfaces/IPluginManager.h"
#include "Misc/Paths.h"

DEFINE_LOG_CATEGORY(LogSharpScript);
DEFINE_LOG_CATEGORY(LogCSharp);

#define LOCTEXT_NAMESPACE "FSharpScriptModule"

/** Callback function when the framework starts */
static int (*ManagedStartupFramework)(FSsFrameworkStartupConfig StartupConfig);

/** Callback function when the framework ends */
static void (*ManagedShutdownFramework)();

void FSharpScriptModule::StartupModule()
{
	if (!SsInternal::BindManagedFrameworkFunction(TEXT("StartupFramework"), (void**)&ManagedStartupFramework))
	{
		return;
	}

	if (!SsInternal::BindManagedFrameworkFunction(TEXT("ShutdownFramework"), (void**)&ManagedShutdownFramework))
	{
		return;
	}

	FSsFrameworkStartupConfig StartupConfig;
	if (ManagedStartupFramework(StartupConfig) != 0)
	{
		UE_LOG(LogSharpScript, Error, TEXT("Call 'StartupFramework' function failed!"));
		return;
	}

	if (!FSsAssemblyManager::Initialize())
	{
		UE_LOG(LogSharpScript, Error, TEXT("AssemblyManager initialize failed!"));
		return;
	}

	if (!USsHouseKeeper::Initialize())
	{
		UE_LOG(LogSharpScript, Error, TEXT("HouseKeeper initialize failed!"));
		return;
	}

	if (!USsTypeRegistry::Initialize())
	{
		UE_LOG(LogSharpScript, Error, TEXT("TypeRegistry initialize failed!"));
		return;
	}
}

void FSharpScriptModule::ShutdownModule()
{
	ManagedShutdownFramework();
	USsHouseKeeper::Finalize();
	FSsAssemblyManager::Finalize();
}

FString FSharpScriptModule::GetPluginDir()
{
	const FString PluginDir = FPaths::ConvertRelativePathToFull(
		IPluginManager::Get().FindPlugin(UE_PLUGIN_NAME)->GetBaseDir());
	return PluginDir;
}

FString FSharpScriptModule::GetDotnetExePath()
{
	FString PluginDir = GetPluginDir();
	return PluginDir / "dotnet" / "sdk" / "dotnet.exe";
}

#undef LOCTEXT_NAMESPACE

IMPLEMENT_MODULE(FSharpScriptModule, SharpScript)
