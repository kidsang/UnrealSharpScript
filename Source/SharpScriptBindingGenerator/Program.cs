using System;
using System.Diagnostics;
using System.IO;
using EpicGames.Core;
using EpicGames.UHT.Tables;
using EpicGames.UHT.Utils;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator;

[UnrealHeaderTool]
public static class Program
{
	/// <summary>
	/// The directory where this plugin is located.
	/// </summary>
	public static string PluginDirectory = "";

	/// <summary>
	/// The directory of generated codes for the engine and plugins.
	/// </summary>
	public static string EngineGeneratedPath = "";

	/// <summary>
	/// The directory of assembly dlls.
	/// </summary>
	public static string ManagedBinariesPath = "";

	/// <summary>
	/// Whether is building unreal editor.
	/// </summary>
	public static bool BuildingEditor;

	public static IUhtExportFactory Factory { get; private set; } = null!;

	public static UHTManifest.Module PluginModule => Factory.PluginModule!;

	[UhtExporter(Name = "SharpScriptBindingGenerator",
		Description = "Exports C++ to C# code",
		Options = UhtExporterOptions.Default,
		ModuleName = "SharpScript",
		CppFilters = new[] { "*.generated.cs" },
		HeaderFilters = new[] { "*.generated.cs" },
		OtherFilters = new[] { "*.generated.cs" })]
	public static void Main(IUhtExportFactory factory)
	{
		Console.WriteLine("Initializing SharpScriptBindingGenerator...");
		Factory = factory;

		InitializeStatics();

		try
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			Console.WriteLine("Exporting C++ to C#...");
			BindingGenerator.StartExport();

			FileExporter.CleanOldGeneratedFiles();

			Console.WriteLine($"Export process completed successfully in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
			stopwatch.Stop();

			if (BindingGenerator.HasGeneratedChanged && BuildingEditor)
			{
				Console.WriteLine("Detected modified generated code. Starting the build process...");
				string projectPath = Path.Combine(PluginDirectory, "Managed", "SharpScript");
				string outputPath = Path.Combine(PluginDirectory, "Managed", "Assemblies");
				DotNetUtilities.BuildGeneratedProject(projectPath, outputPath);
			}

			TryCreateCSharpProjects();
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("An error occurred during the export process:");
			Console.WriteLine($"Error Message: {ex.Message}");
			Console.WriteLine("Stack Trace:");
			Console.WriteLine(ex.StackTrace);
			Console.ResetColor();
		}
	}

	private static void InitializeStatics()
	{
		PluginDirectory = Factory.PluginModule!.BaseDirectory;
		PluginDirectory = Directory.GetParent(PluginDirectory)!.Parent!.ToString();

		EngineGeneratedPath = Path.Combine(PluginDirectory, "Managed", "SharpScript", "Generated");

		ManagedBinariesPath = Path.Combine(PluginDirectory, "Binaries", "Managed");

		BuildingEditor = GetPluginDefine("BUILDING_EDITOR") == "1";
	}

	private static string GetPluginDefine(string key)
	{
		PluginModule.TryGetDefine(key, out string? value);
		return value!;
	}

	private static void TryCreateCSharpProjects()
	{
		DotNetUtilities.InvokeSharpScriptBuildTool("GenerateProject");
	}
}
