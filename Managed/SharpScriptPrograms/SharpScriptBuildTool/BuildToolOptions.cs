using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace SharpScriptBuildTool;

public enum BuildAction
{
	GenerateProject,
}

// ReSharper disable once ClassNeverInstantiated.Global
public class BuildToolOptions
{
	[Option("Action", Required = true, HelpText = "The action the build tool should process. Possible values: GenerateProject.")]
	public BuildAction Action { get; set; }

	[Option("DotNetPath", Required = false, HelpText = "The path to the dotnet.exe")]
	public string? DotNetPath { get; set; }

	[Option("ProjectDirectory", Required = true, HelpText = "The directory where the .uproject file resides.")]
	public required string ProjectDirectory { get; set; }

	[Option("PluginDirectory", Required = false, HelpText = "The SharpScript plugin directory.")]
	public required string PluginDirectory { get; set; }

	[Option("EngineDirectory", Required = false, HelpText = "The Unreal Engine directory.")]
	public required string EngineDirectory { get; set; }

	[Option("ProjectName", Required = true, HelpText = "The name of the Unreal Engine project.")]
	public required string ProjectName { get; set; }

	[Option("AdditionalArgs", Required = false, HelpText = "Additional key-value arguments for the build tool.")]
	public required IEnumerable<string> AdditionalArgs { get; set; }

	public static void PrintHelp(ParserResult<BuildToolOptions> result)
	{
		string name = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()!.Location);
		Console.Error.WriteLine($"Usage: {name} [options]");
		Console.Error.WriteLine("Options:");

		var helpText = HelpText.AutoBuild(result, h => h, e => e);
		Console.WriteLine(helpText);
	}
}