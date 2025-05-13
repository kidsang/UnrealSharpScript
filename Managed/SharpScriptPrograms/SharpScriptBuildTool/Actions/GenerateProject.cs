using System.Xml;

namespace SharpScriptBuildTool.Actions;

public class GenerateProject : BuildToolAction
{
	public override bool RunAction()
	{
		string pluginDirectory = Program.BuildToolOptions.PluginDirectory;
		string pluginProjectPath = Path.Combine(pluginDirectory, "Managed", "SharpScript", "SharpScript.csproj");

		// Collect all C# projects to be added to the sln.
		List<string> csharpProjectPaths =
		[
			pluginProjectPath,
		];

		// If the unittest plugin project can be found, add it to the sln as well.
		string unittestProjectPath = Path.Combine(pluginDirectory, "Managed", "SharpScriptUnitTest", "SharpScriptUnitTest.csproj");
		if (File.Exists(unittestProjectPath))
		{
			csharpProjectPaths.Add(unittestProjectPath);
		}

		// Generate C# only sln.
		if (!GenerateManagedSolution(csharpProjectPaths))
		{
			return false;
		}

		// Generate C++/C# mixed sln.
		if (!GenerateMixedSolution(csharpProjectPaths))
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// Generate C# project (.csproj).
	/// </summary>
	private bool GenerateCSharpProject(string projectName, string projectDirectory, bool forced)
	{
		if (!Directory.Exists(projectDirectory))
		{
			Directory.CreateDirectory(projectDirectory);
		}

		BuildToolProcess process = new BuildToolProcess();
		process.StartInfo.ArgumentList.Add("new");
		process.StartInfo.ArgumentList.Add("classlib");
		process.StartInfo.ArgumentList.Add("-f");
		process.StartInfo.ArgumentList.Add(Program.GetVersion());
		process.StartInfo.ArgumentList.Add("-n");
		process.StartInfo.ArgumentList.Add(projectName);
		process.StartInfo.ArgumentList.Add("-o");
		process.StartInfo.ArgumentList.Add(projectDirectory);
		process.StartInfo.ArgumentList.Add("--no-restore");
		if (forced)
		{
			process.StartInfo.ArgumentList.Add("--force");
		}

		if (!process.StartBuildToolProcess())
		{
			return false;
		}

		// dotnet new class lib generates a file named Class1, remove it.
		string myClassFile = Path.Combine(projectDirectory, "Class1.cs");
		if (File.Exists(myClassFile))
		{
			File.Delete(myClassFile);
		}

		return true;
	}

	/// <summary>
	/// Add C# project dependency.
	/// </summary>
	private bool AddCSharpProjectReference(string projectPath, string referenceProjectPath)
	{
		BuildToolProcess process = new BuildToolProcess();
		process.StartInfo.ArgumentList.Add("add");
		process.StartInfo.ArgumentList.Add(projectPath);
		process.StartInfo.ArgumentList.Add("reference");
		process.StartInfo.ArgumentList.Add(referenceProjectPath);
		return process.StartBuildToolProcess();
	}

	/// <summary>
	/// Edit .csproj file, add necessary modifications.
	/// </summary>
	private void ModifyCSharpProject(string projectPath)
	{
		void AddProperty(string name, string value, XmlDocument doc, XmlNode propertyGroup)
		{
			XmlNode? newProperty = propertyGroup.SelectSingleNode(name);
			if (newProperty == null)
			{
				newProperty = doc.CreateElement(name);
				propertyGroup.AppendChild(newProperty);
			}

			newProperty.InnerText = value;
		}

		try
		{
			XmlDocument csprojDocument = new XmlDocument();
			csprojDocument.Load(projectPath);

			XmlNode propertyGroup = csprojDocument.SelectSingleNode("//PropertyGroup") ?? csprojDocument.CreateElement("PropertyGroup");
			AddProperty("AllowUnsafeBlocks", "true", csprojDocument, propertyGroup);

			csprojDocument.Save(projectPath);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"An error occurred while updating the .csproj file: {ex.Message}", ex);
		}
	}

	/// <summary>
	/// Generate C# solution.
	/// </summary>
	private bool GenerateManagedSolution(List<string> csharpProjectPaths)
	{
		string projectDirectory = Program.BuildToolOptions.ProjectDirectory;
		string projectName = Program.BuildToolOptions.ProjectName;
		string managedSlnName = $"{projectName}_Managed";
		string managedSlnPath = Path.Combine(projectDirectory, managedSlnName + ".sln");
		if (File.Exists(managedSlnPath))
		{
			return true;
		}

		// Create new solution.
		BuildToolProcess process = new BuildToolProcess();
		process.StartInfo.ArgumentList.Add("new");
		process.StartInfo.ArgumentList.Add("sln");
		process.StartInfo.ArgumentList.Add("-n");
		process.StartInfo.ArgumentList.Add(managedSlnName);
		process.StartInfo.ArgumentList.Add("-o");
		process.StartInfo.ArgumentList.Add(projectDirectory);
		process.StartInfo.ArgumentList.Add("--force");
		if (!process.StartBuildToolProcess())
		{
			return false;
		}

		process = new BuildToolProcess();
		process.StartInfo.ArgumentList.Add("sln");
		process.StartInfo.ArgumentList.Add(managedSlnPath);
		process.StartInfo.ArgumentList.Add("add");
		foreach (var projectPath in csharpProjectPaths)
		{
			process.StartInfo.ArgumentList.Add(projectPath);
		}

		process.StartInfo.ArgumentList.Add("--in-root");

		if (!process.StartBuildToolProcess())
		{
			return false;
		}

		// Remove unused build platforms.
		string[] removePlatforms =
		[
			"|x64",
			"|x86",
		];
		RemoveUnusedPlatforms(managedSlnPath, removePlatforms);

		// Modify C# project build configuration.
		List<string> newSolutionConfigurations =
		[
			"Debug_Editor|Any CPU",
			"Release_Editor|Any CPU",
		];
		ModifyBuildConfigurations(managedSlnPath, csharpProjectPaths, newSolutionConfigurations);

		return true;
	}

	/// <summary>
	/// Generate C++/C# mixed solution.
	/// </summary>
	private bool GenerateMixedSolution(List<string> csharpProjectPaths)
	{
		string projectDirectory = Program.BuildToolOptions.ProjectDirectory;
		string projectName = Program.BuildToolOptions.ProjectName;
		string projectSlnPath = Path.Combine(projectDirectory, projectName + ".sln");
		if (!File.Exists(projectSlnPath))
		{
			// If the C++ solution file does not exist, it means the user has not yet executed the GenerateProject operation.
			return true;
		}

		string mixedSlnPath = Path.Combine(projectDirectory, $"{projectName}_Mixed.sln");
		if (File.Exists(mixedSlnPath))
		{
			return true;
		}

		// Copy mixed sln from origin sln.
		File.Copy(projectSlnPath, mixedSlnPath, overwrite: true);

		// Add C# projects to mixed sln.
		BuildToolProcess process = new BuildToolProcess();
		process.StartInfo.ArgumentList.Add("sln");
		process.StartInfo.ArgumentList.Add(mixedSlnPath);
		process.StartInfo.ArgumentList.Add("add");
		foreach (var projectPath in csharpProjectPaths)
		{
			process.StartInfo.ArgumentList.Add(projectPath);
		}

		process.StartInfo.ArgumentList.Add("--in-root");

		if (!process.StartBuildToolProcess())
		{
			return false;
		}

		// Remove unused build platforms.
		string[] removePlatforms =
		[
			"|Any CPU",
			"|x64",
			"|x86",
		];
		RemoveUnusedPlatforms(mixedSlnPath, removePlatforms, projectSlnPath);

		// Modify C# project build configuration.
		ModifyBuildConfigurations(mixedSlnPath, csharpProjectPaths);

		return true;
	}

	/// <summary>
	/// By default dotnet tool will add 3 platform to sln for C# project, witch will dramatically increase sln size.
	/// Remove these platforms will decrease sln file size, and prevent other issues as well.
	/// </summary>
	private void RemoveUnusedPlatforms(string slnPath, string[] removePlatforms, string? originSlnPath = null)
	{
		List<string> slnLines = File.ReadAllLines(slnPath).ToList();

		HashSet<string> originSlnLines = new HashSet<string>();
		if (originSlnPath != null)
		{
			originSlnLines = new HashSet<string>(File.ReadAllLines(originSlnPath));
		}

		for (int i = slnLines.Count - 1; i >= 0; i--)
		{
			string line = slnLines[i];
			foreach (string platform in removePlatforms)
			{
				if (originSlnLines.Contains(line))
				{
					continue;
				}

				if (line.Contains(platform))
				{
					slnLines.RemoveAt(i);
					break;
				}
			}
		}

		File.WriteAllLines(slnPath, slnLines);
	}

	/// <summary>
	/// Associate solution build configuration to project.
	/// </summary>
	private void ModifyBuildConfigurations(string slnPath, List<string> csharpProjectPaths, List<string>? newSolutionConfigurations = null)
	{
		List<string> slnLines = File.ReadAllLines(slnPath).ToList();

		List<string> searchProjects = new();
		foreach (string csharpProjectPath in csharpProjectPaths)
		{
			string projectName = Path.GetFileName(csharpProjectPath);
			projectName = Path.GetFileNameWithoutExtension(projectName);
			searchProjects.Add($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{projectName}\"");
		}

		List<string> projectGuids = new();
		foreach (string line in slnLines)
		{
			foreach (string searchProject in searchProjects)
			{
				if (line.StartsWith(searchProject))
				{
					int startIndex = line.LastIndexOf('{');
					int endIndex = line.LastIndexOf('}');
					string projectGuid = line.Substring(startIndex + 1, endIndex - startIndex - 1);
					projectGuids.Add(projectGuid);
					break;
				}
			}

			if (projectGuids.Count == searchProjects.Count)
			{
				break;
			}
		}

		int solutionConfigurationPlatformsBegin = -1;
		int solutionConfigurationPlatformsEnd = -1;
		List<string> solutionConfigurationPlatforms = new();
		for (int i = 0; i < slnLines.Count; i++)
		{
			string line = slnLines[i].Trim();
			if (solutionConfigurationPlatformsBegin == -1)
			{
				if (line.StartsWith("GlobalSection(SolutionConfigurationPlatforms) = preSolution"))
				{
					solutionConfigurationPlatformsBegin = i;
				}
			}
			else if (solutionConfigurationPlatformsEnd == -1)
			{
				if (line.StartsWith("EndGlobalSection"))
				{
					solutionConfigurationPlatformsEnd = i;
				}
				else
				{
					line = line.Split(" = ")[0].Trim();
					solutionConfigurationPlatforms.Add(line);
				}
			}
			else
			{
				break;
			}
		}

		if (newSolutionConfigurations != null)
		{
			foreach (string newConfiguration in newSolutionConfigurations)
			{
				solutionConfigurationPlatforms.Add(newConfiguration);
				slnLines.Insert(solutionConfigurationPlatformsEnd++, $"\t\t{newConfiguration} = {newConfiguration}");
			}
		}

		int projectConfigurationPlatformsBegin = -1;
		int projectConfigurationPlatformsEnd = -1;
		for (int i = solutionConfigurationPlatformsEnd; i < slnLines.Count; i++)
		{
			string line = slnLines[i].Trim();
			if (projectConfigurationPlatformsBegin == -1)
			{
				if (line.StartsWith("GlobalSection(ProjectConfigurationPlatforms) = postSolution"))
				{
					projectConfigurationPlatformsBegin = i;
				}
			}
			else if (projectConfigurationPlatformsEnd == -1)
			{
				if (line.StartsWith("EndGlobalSection"))
				{
					projectConfigurationPlatformsEnd = i;
				}
			}
			else
			{
				break;
			}
		}

		foreach (string projectGuid in projectGuids)
		{
			foreach (string solutionPlatform in solutionConfigurationPlatforms)
			{
				string projectPlatform = solutionPlatform.StartsWith("Debug") ? "Debug" : "Release";
				if (solutionPlatform.Contains("Editor"))
				{
					projectPlatform += "_Editor";
				}

				slnLines.Insert(projectConfigurationPlatformsEnd++,
					$"\t\t{{{projectGuid}}}.{solutionPlatform}.ActiveCfg = {projectPlatform}|Any CPU");
				slnLines.Insert(projectConfigurationPlatformsEnd++,
					$"\t\t{{{projectGuid}}}.{solutionPlatform}.Build.0 = {projectPlatform}|Any CPU");
			}
		}

		File.WriteAllLines(slnPath, slnLines);
	}
}