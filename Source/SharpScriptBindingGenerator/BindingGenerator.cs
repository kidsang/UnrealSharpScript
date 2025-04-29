using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using EpicGames.Core;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Exporters;
using SharpScriptBindingGenerator.PropertyTranslators;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator;

/// <summary>
/// C# code generator for generating C# bindings for engine and project code.
/// </summary>
public static class BindingGenerator
{
	/// <summary>
	/// Records the last modification time of module directories.
	/// </summary>
	private class ModuleFolders
	{
		/// <summary>
		/// Key: Module directory path Value: Modification time of the corresponding module source code when generating C# binding code
		/// </summary>
		public readonly Dictionary<string, DateTime> DirectoriesModifyTime = new();
	}

	/// <summary>
	/// Cache file name for module last modification time.
	/// </summary>
	private const string ModuleModifyTimeFileName = "SharpScriptModuleModifyTime.json";

	/// <summary>
	/// Module directory modification time.
	/// Key: Module name
	/// </summary>
	private static Dictionary<string, ModuleFolders> _modulesModifyInfo = new();

	/// <summary>
	/// Cache the modification time of each directory.
	/// Key: Directory path Val: Last modification time
	/// </summary>
	private static readonly Dictionary<string, DateTime> CachedDirectoriesModifyTime = new();

	/// <summary>
	/// List of types that will be exported.
	/// </summary>
	public static readonly HashSet<UhtType> ShouldExportTypes = new();

	/// <summary>
	/// Parallel export tasks.
	/// </summary>
	private static readonly List<Task> Tasks = new();

	/// <summary>
	/// Whether the generated code has been modified.
	/// </summary>
	public static bool HasGeneratedChanged = false;

	public static void StartExport()
	{
		LoadModulesModifyTime();

		foreach (UhtPackage package in Program.Factory.Session.Packages)
		{
			CollectShouldExportTypes(package);
		}

		foreach (UhtPackage package in Program.Factory.Session.Packages)
		{
			ExportPackage(package);
		}

		WaitForTasks();

		SaveModulesModifyTime();
	}

	/// <summary>
	/// Load the last modification time of module directories, used to determine whether the C# binding for the corresponding module needs incremental generation.
	/// </summary>
	private static void LoadModulesModifyTime()
	{
		if (HasGeneratorSourceChangedRecently())
		{
			return;
		}

		if (!Directory.Exists(Program.EngineGeneratedPath))
		{
			return;
		}

		string cachePath = Path.Combine(Program.PluginModule.OutputDirectory, ModuleModifyTimeFileName);
		if (!File.Exists(cachePath))
		{
			return;
		}

		using FileStream fileStream = new FileStream(cachePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		Dictionary<string, ModuleFolders>? jsonValue = JsonSerializer.Deserialize<Dictionary<string, ModuleFolders>>(fileStream);
		if (jsonValue != null)
		{
			_modulesModifyInfo = new Dictionary<string, ModuleFolders>(jsonValue);
		}
	}

	/// <summary>
	/// Save the last modification time of module directories.
	/// </summary>
	private static void SaveModulesModifyTime()
	{
		string cachePath = Path.Combine(Program.PluginModule.OutputDirectory, ModuleModifyTimeFileName);
		using FileStream fs = new FileStream(cachePath, FileMode.Create, FileAccess.Write);
		JsonSerializer.Serialize(fs, _modulesModifyInfo);
	}

	/// <summary>
	/// Returns whether the code generator itself has been modified. If modified, a full generation of static binding code is required.
	/// </summary>
	private static bool HasGeneratorSourceChangedRecently()
	{
		string executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
		DateTime executingAssemblyLastWriteTime = File.GetLastWriteTimeUtc(executingAssemblyPath);

		string generatedCodeDirectory = Program.PluginModule.OutputDirectory;
		string timestampFilePath = Path.Combine(generatedCodeDirectory, "Timestamp");

		if (!File.Exists(timestampFilePath))
		{
			return true;
		}

		DateTime savedTimestampUtc = File.GetLastWriteTimeUtc(timestampFilePath);
		return executingAssemblyLastWriteTime > savedTimestampUtc;
	}

	public static bool HasBeenExported(string directory)
	{
		if (_modulesModifyInfo.TryGetValue(directory, out ModuleFolders? moduleFolders))
		{
			return moduleFolders.DirectoriesModifyTime.Count > 0;
		}

		return false;
	}

	/// <summary>
	/// Determine whether the C# binding code for the module needs to be regenerated.
	/// </summary>
	private static bool ShouldExportDirectory(string directoryPath, ModuleFolders moduleFolders)
	{
		if (!CachedDirectoriesModifyTime.TryGetValue(directoryPath, out DateTime cachedTime))
		{
			DateTime currentWriteTime = Directory.GetLastWriteTimeUtc(directoryPath);
			CachedDirectoriesModifyTime[directoryPath] = currentWriteTime;
			cachedTime = currentWriteTime;
		}

		return !moduleFolders.DirectoriesModifyTime.TryGetValue(directoryPath, out DateTime lastEditTimeValue) || lastEditTimeValue != cachedTime;
	}

	/// <summary>
	/// Update the incremental generation information for modules.
	/// </summary>
	private static void UpdateDirectoriesLastExportTime(HashSet<string> directories, ModuleFolders moduleFolders)
	{
		foreach (string directory in directories)
		{
			if (!CachedDirectoriesModifyTime.TryGetValue(directory, out DateTime cachedTime))
			{
				continue;
			}

			moduleFolders.DirectoriesModifyTime[directory] = cachedTime;
		}
	}

	private static void WaitForTasks()
	{
		Task[] waitTasks = Tasks.ToArray();
		if (waitTasks.Length > 0)
		{
			Task.WaitAll(waitTasks);
		}

		Tasks.Clear();
	}

	private static void CollectShouldExportTypes(UhtPackage package)
	{
		if (!Program.BuildingEditor && package.PackageFlags.HasAnyFlags(EPackageFlags.EditorOnly | EPackageFlags.UncookedOnly))
		{
			return;
		}

		void CollectSholdExportType(UhtType type)
		{
			ShouldExportTypes.Add(type);
		}

		foreach (UhtType child in package.Children)
		{
			ForEachChild(child, CollectSholdExportType);
		}
	}

	private static void ExportPackage(UhtPackage package)
	{
		if (!Program.BuildingEditor && package.PackageFlags.HasAnyFlags(EPackageFlags.EditorOnly | EPackageFlags.UncookedOnly))
		{
			return;
		}

		string packageName = package.GetShortName();
		if (!_modulesModifyInfo.TryGetValue(packageName, out ModuleFolders? moduleFolders))
		{
			moduleFolders = new ModuleFolders();
			_modulesModifyInfo.Add(packageName, moduleFolders);
		}

		HashSet<string> processedDirectories = new();
		foreach (UhtType child in package.Children)
		{
			string directoryName = Path.GetDirectoryName(child.HeaderFile.FilePath)!;
			if (ShouldExportDirectory(directoryName, moduleFolders))
			{
				processedDirectories.Add(directoryName);
				ForEachChild(child, ExportType);
			}
			else
			{
				ForEachChild(child, FileExporter.AddUnchangedType);
			}
		}

		if (processedDirectories.Count > 0)
		{
			UpdateDirectoriesLastExportTime(processedDirectories, moduleFolders);
		}
	}

	private static void ForEachChild(UhtType child, Action<UhtType> action)
	{
		foreach (UhtType type in child.Children)
		{
			action(type);
			foreach (UhtType innerType in type.Children)
			{
				action(innerType);
			}
		}
	}

	private static void ExportType(UhtType type)
	{
		if (!ShouldExportTypes.Contains(type))
		{
			return;
		}

		if (PropertyTranslatorManager.ManuallyExportedTypes.Contains(type.EngineName))
		{
			return;
		}

		if (type is UhtClass classObj)
		{
			if (classObj.HasAllFlags(EClassFlags.Interface))
			{
				if (classObj.ClassType is UhtClassType.Interface)
				{
					Tasks.Add(Program.Factory.CreateTask(_ => { InterfaceExporter.ExportInterface(classObj); })!);
				}
				else if (classObj == Program.Factory.Session.UInterface)
				{
					Tasks.Add(Program.Factory.CreateTask(_ => { ClassExporter.ExportClass(classObj); })!);
				}
			}
			else
			{
				Tasks.Add(Program.Factory.CreateTask(_ => { ClassExporter.ExportClass(classObj); })!);
			}
		}
		else if (type is UhtEnum enumObj)
		{
			Tasks.Add(Program.Factory.CreateTask(_ => { EnumExporter.ExportEnum(enumObj); })!);
		}
		else if (type is UhtScriptStruct structObj)
		{
			Tasks.Add(Program.Factory.CreateTask(_ => { StructExporter.ExportStruct(structObj); })!);
		}
		else if (type.EngineType == UhtEngineType.Delegate)
		{
			UhtFunction delegateFunction = (UhtFunction)type;
			Tasks.Add(Program.Factory.CreateTask(_ => { DelegateExporter.ExportDelegate(delegateFunction); })!);
		}
	}
}
