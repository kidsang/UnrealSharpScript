#pragma once
#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"

class SHARPSCRIPT_API FSharpScriptModule : public IModuleInterface
{
public:
	virtual void StartupModule() override;
	virtual void ShutdownModule() override;

	/**
	 * Returns the plugin directory.
	 */
	static FString GetPluginDir();

	/**
	 * Returns the path to dotnet.exe.
	 */
	static FString GetDotnetExePath();
};
