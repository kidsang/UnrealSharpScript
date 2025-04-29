using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SharpScriptBindingGenerator.Utilities;

public static class DotNetUtilities
{
	public static void InvokeDotNet(List<string> arguments, string? workingDirectory = null)
	{
		string dotnetPath = GetDotNetExecutable();

		var startInfo = new ProcessStartInfo
		{
			FileName = dotnetPath,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			StandardOutputEncoding = Encoding.UTF8
		};

		foreach (string argument in arguments)
		{
			startInfo.ArgumentList.Add(argument);
		}

		if (workingDirectory != null)
		{
			startInfo.WorkingDirectory = workingDirectory;
		}

		// Set the MSBuild environment variables to the latest .NET SDK that U# supports.
		// Otherwise, we'll use the .NET SDK that comes with the Unreal Engine.
		{
			string latestDotNetSdkPath = GetLatestDotNetSdkPath();
			startInfo.Environment["MSBuildExtensionsPath"] = latestDotNetSdkPath;
			startInfo.Environment["MSBUILD_EXE_PATH"] = $@"{latestDotNetSdkPath}\MSBuild.dll";
			startInfo.Environment["MSBuildSDKsPath"] = $@"{latestDotNetSdkPath}\Sdks";
		}

		string argumentsString = string.Join(" ", startInfo.ArgumentList);
		Console.WriteLine($"Started process: {dotnetPath} {argumentsString}");

		using Process process = new Process();
		process.StartInfo = startInfo;

		try
		{
			process.Start();
		}
		catch (Exception ex)
		{
			throw new Exception($"Failed to start process '{dotnetPath}' with arguments: {argumentsString}", ex);
		}

		string standardOutput = "";
		string standardError = "";
		while (!process.WaitForExit(200))
		{
			var output = process.StandardOutput.ReadToEnd();
			if (output.Length > 0)
			{
				standardOutput += output;
				Console.Write(output);
			}

			var error = process.StandardError.ReadToEnd();
			if (error.Length > 0)
			{
				standardError += error;
				Console.Write(error);
			}
		}

		if (process.ExitCode == 0)
		{
			return;
		}

		string errorDetails = $@"
    Failed to invoke dotnet command:
    Executable: {dotnetPath}
    Arguments: {argumentsString}
    Exit Code: {process.ExitCode}
    Standard Output: {standardOutput}
    Standard Error: {standardError}";

		throw new Exception(errorDetails);
	}

	public static void BuildGeneratedProject(string projectPath, string outputPath)
	{
		List<string> arguments = new List<string>()
		{
			"build",
			"-c",
			"Release",
			projectPath,
			"--output",
			outputPath,
		};

		InvokeDotNet(arguments);
	}

	public static void InvokeSharpScriptBuildTool(string action, Dictionary<string, string>? additionalArguments = null)
	{
		string dotNetExe = GetDotNetExecutable();
		string projectName = Path.GetFileNameWithoutExtension(Program.Factory.Session.ProjectFile)!;
		string buildToolPath = Path.Combine(Program.ManagedBinariesPath, "SharpScriptBuildTool.dll");

		if (!File.Exists(buildToolPath))
		{
			throw new Exception($"Failed to find SharpScriptBuildTool.dll at: {buildToolPath}");
		}

		List<string> arguments = new List<string>
		{
			buildToolPath,

			"--Action",
			action,

			"--EngineDirectory",
			$"{Program.Factory.Session.EngineDirectory}",

			"--ProjectDirectory",
			$"{Program.Factory.Session.ProjectDirectory}",

			"--ProjectName",
			projectName,

			"--PluginDirectory",
			$"{Program.PluginDirectory}",

			"--DotNetPath",
			$"{dotNetExe}"
		};

		if (additionalArguments != null)
		{
			arguments.Add("--AdditionalArgs");

			foreach (var argument in additionalArguments)
			{
				arguments.Add($"{argument.Key}={argument.Value}");
			}
		}

		InvokeDotNet(arguments);
	}

	private static string GetDotNetExecutable()
	{
		return Path.Combine(Program.PluginDirectory, "dotnet", "sdk", "dotnet.exe");
	}

	private static string GetLatestDotNetSdkPath()
	{
		string dotNetExecutable = GetDotNetExecutable();
		string dotNetExecutableDirectory = Path.GetDirectoryName(dotNetExecutable)!;
		string dotNetSdkDirectory = Path.Combine(dotNetExecutableDirectory, "sdk");

		string[] folderPaths = Directory.GetDirectories(dotNetSdkDirectory);

		string highestVersion = "0.0.0";

		foreach (string folderPath in folderPaths)
		{
			string folderName = Path.GetFileName(folderPath);

			if (string.IsNullOrEmpty(folderName) || !char.IsDigit(folderName[0]))
			{
				continue;
			}

			if (string.Compare(folderName, highestVersion, StringComparison.Ordinal) > 0)
			{
				highestVersion = folderName;
			}
		}

		if (highestVersion == "0.0.0")
		{
			throw new Exception("Failed to find the latest .NET SDK version.");
		}

		return Path.Combine(dotNetSdkDirectory, highestVersion);
	}
}
