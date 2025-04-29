using CommandLine;
using SharpScriptBuildTool.Actions;

namespace SharpScriptBuildTool;

public static class Program
{
	public static BuildToolOptions BuildToolOptions = null!;

	public static int Main(string[] args)
	{
		try
		{
			Console.WriteLine(">>> SharpScriptBuildTool");
			Parser parser = new Parser(with => with.HelpWriter = null);
			ParserResult<BuildToolOptions> result = parser.ParseArguments<BuildToolOptions>(args);

			if (result.Tag == ParserResultType.NotParsed)
			{
				BuildToolOptions.PrintHelp(result);

				string errors = string.Empty;
				foreach (Error error in result.Errors)
				{
					if (error is TokenError tokenError)
					{
						errors += $"{tokenError.Tag}: {tokenError.Token} \n";
					}
				}

				throw new Exception($"Invalid arguments. Errors: {errors}");
			}

			BuildToolOptions = result.Value;

			if (!BuildToolAction.InitializeAction())
			{
				throw new Exception("Failed to initialize action.");
			}

			Console.WriteLine($"SharpScriptBuildTool executed {BuildToolOptions.Action.ToString()} action successfully.");
		}
		catch (Exception exception)
		{
			Console.WriteLine("An error occurred: " + exception.Message + Environment.NewLine + exception.StackTrace);
			return 1;
		}

		return 0;
	}

	public static string GetVersion()
	{
		Version currentVersion = Environment.Version;
		string currentVersionStr = $"{currentVersion.Major}.{currentVersion.Minor}";
		return "net" + currentVersionStr;
	}
}