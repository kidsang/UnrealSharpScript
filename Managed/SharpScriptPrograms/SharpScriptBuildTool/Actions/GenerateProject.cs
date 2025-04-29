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
		string unitestProjectPath = Path.Combine(pluginDirectory, "Managed", "SharpScriptUnitTest", "SharpScriptUnitTest.csproj");
		if (File.Exists(unitestProjectPath))
		{
			csharpProjectPaths.Add(unitestProjectPath);
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
	private bool GenerateManagedSolution(List<String> csharpProjectPaths)
	{
		string projectDirectory = Program.BuildToolOptions.ProjectDirectory;
		string projectName = Program.BuildToolOptions.ProjectName;
		string managedSlnName = $"{projectName}_Managed";
		string managedSlnPath = Path.Combine(projectDirectory, managedSlnName + ".sln");
		if (File.Exists(managedSlnPath))
		{
			return true;
		}

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

		return process.StartBuildToolProcess();
	}

	/// <summary>
	/// Generate C++/C# mixed solution.
	/// </summary>
	private bool GenerateMixedSolution(List<String> csharpProjectPaths)
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

		File.Copy(projectSlnPath, mixedSlnPath, overwrite: true);

		BuildToolProcess process = new BuildToolProcess();
		process.StartInfo.ArgumentList.Add("sln");
		process.StartInfo.ArgumentList.Add(mixedSlnPath);
		process.StartInfo.ArgumentList.Add("add");
		foreach (var projectPath in csharpProjectPaths)
		{
			process.StartInfo.ArgumentList.Add(projectPath);
		}

		process.StartInfo.ArgumentList.Add("--in-root");

		return process.StartBuildToolProcess();
	}
}