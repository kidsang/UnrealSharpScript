using System;
using System.IO;
using UnrealBuildTool;

public class SharpScriptHost : ModuleRules
{
	public SharpScriptHost(ReadOnlyTargetRules Target) : base(Target)
	{
		// todo: twx convenient for debugging during development
		bUseUnity = false;
		PCHUsage = PCHUsageMode.NoPCHs;
		OptimizeCode = CodeOptimization.Never;

		PublicDependencyModuleNames.AddRange(
			new string[]
			{
				"Core",
			}
		);

		PrivateDependencyModuleNames.AddRange(
			new string[]
			{
				"CoreUObject",
				"DeveloperSettings",
				"Projects",
			}
		);

		PublicIncludePaths.Add(Path.Combine(PluginDirectory, "dotnet", "include"));

		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			SetupWindowsLibraries();
		}
		else
		{
			throw new Exception($"Platform not support yet: {Target.Platform}");
		}
	}

	private void SetupWindowsLibraries()
	{
		AddRuntimeDependencies(Path.Combine(PluginDirectory, "dotnet", "runtime", "coreclr-win-x64"), "dll");
		AddRuntimeDependencies(Path.Combine(PluginDirectory, "dotnet", "runtime", "mono-win-x64"), "dll");
	}

	private void AddRuntimeDependencies(string rootDir, string ext)
	{
		string[] files = Directory.GetFiles(rootDir, $"*.{ext}", SearchOption.AllDirectories);
		foreach (string file in files)
		{
			Console.WriteLine(file);
			RuntimeDependencies.Add(file);
		}
	}
}
