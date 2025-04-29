using System;
using System.Diagnostics;
using System.IO;
using UnrealBuildTool;

public class SharpScript : ModuleRules
{
	public SharpScript(ReadOnlyTargetRules Target) : base(Target)
	{
		// todo: twx convenient for debugging during development
		bUseUnity = false;
		PCHUsage = PCHUsageMode.NoPCHs;
		OptimizeCode = CodeOptimization.Never;

		PublicDefinitions.Add("BUILDING_EDITOR=" + (Target.bBuildEditor ? "1" : "0"));

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
				"Engine",
				"Slate",
				"SlateCore",
				"Projects",
				"SharpScriptHost",
			}
		);

		if (Target.bBuildEditor)
		{
			string managedPath = Path.Combine(PluginDirectory, "Managed");
			PublishSolution(Path.Combine(managedPath, "SharpScriptPrograms"));
		}
	}

	string GetDotNetExecutable()
	{
		return Path.Combine(PluginDirectory, "dotnet", "sdk", "dotnet.exe");
	}

	void PublishSolution(string projectRootDirectory)
	{
		if (!Directory.Exists(projectRootDirectory))
		{
			throw new Exception($"Couldn't find project root directory: {projectRootDirectory}");
		}

		string dotnetPath = GetDotNetExecutable();

		Process process = new Process();
		process.StartInfo.FileName = dotnetPath;

		process.StartInfo.ArgumentList.Add("publish");
		process.StartInfo.ArgumentList.Add($"\"{projectRootDirectory}\"");

		string managedBinariesPath = Path.Combine(PluginDirectory, "Binaries", "Managed");
		process.StartInfo.ArgumentList.Add($"-p:PublishDir=\"{managedBinariesPath}\"");

		process.Start();
		process.WaitForExit();

		if (process.ExitCode != 0)
		{
			Console.WriteLine($"Failed to publish solution: {projectRootDirectory}");
		}
	}
}
