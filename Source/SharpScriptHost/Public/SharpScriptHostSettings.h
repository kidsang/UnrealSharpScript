#pragma once
#include "CoreMinimal.h"
#include "Engine/DeveloperSettings.h"
#include "SharpScriptHostSettings.generated.h"

/**
 * .Net VM configuration.
 */
UCLASS(config=SharpScript, defaultconfig, meta=(DisplayName="SharpScript"))
class SHARPSCRIPTHOST_API USharpScriptHostSettings : public UDeveloperSettings
{
	GENERATED_BODY()

public:
	/**
	 * .Net runtime version number.
	 */
	UPROPERTY(config, EditAnywhere, Category="Host", meta=(ConfigRestartRequired=true, GetOptions=GetListOfRuntimeVersions))
	FString RuntimeVersion;

	/**
	 * CoreCLR runtime path.
	 * @remarks Can use relative paths starting with special macros.
	 * @remarks $(PluginDir) - Will be replaced with the plugin directory.
	 */
	UPROPERTY(config, EditAnywhere, Category="Host|Windows", meta=(ConfigRestartRequired=true))
	FString WindowsCoreClrRuntimePath;

	/**
	 * Mono runtime path.
	 * @remarks Can use relative paths starting with special macros.
	 * @remarks $(PluginDir) - Will be replaced with the plugin directory.
	 */
	UPROPERTY(config, EditAnywhere, Category="Host|Windows", meta=(ConfigRestartRequired=true))
	FString WindowsMonoRuntimePath;

	/**
	 * Whether to force the use of Mono runtime.
	 * @remarks By default, we will use CoreCLR runtime on windows platform. If this option is set to true, we will always use Mono runtime.
	 * @remarks Adding "-forcemono" to the startup arguments will have the same effect.
	 */
	UPROPERTY(config, EditAnywhere, Category="Host|Windows", meta=(ConfigRestartRequired=true))
	bool bWindowsForceUseMono;

public:
	virtual FName GetCategoryName() const override;

private:
	/**
	 * @return Returns a list of all available .Net runtime versions.
	 */
	UFUNCTION()
	static TArray<FString> GetListOfRuntimeVersions()
	{
		return {
			FString(TEXT("9.0.4")),
		};
	}
};

