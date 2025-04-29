using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EpicGames.Core;
using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.Utilities;

public static class FileExporter
{
	private static readonly ReaderWriterLockSlim ReadWriteLock = new();
	private static readonly List<string> ChangedFiles = new();
	private static readonly List<string> UnchangedFiles = new();

	public static string GetGeneratedDirectory(UhtPackage package)
	{
		if (package == null)
		{
			throw new Exception("Package is null");
		}

		return Path.Combine(Program.EngineGeneratedPath, package.GetShortName());
	}

	public static string GetGeneratedFilePath(string typeName, string directory)
	{
		return Path.Combine(directory, $"{typeName}.generated.cs");
	}

	public static void AddUnchangedType(UhtType type)
	{
		string directory = GetGeneratedDirectory(type.Package);
		string filePath = GetGeneratedFilePath(type.EngineName, directory);
		UnchangedFiles.Add(filePath);
	}

	public static void SaveGeneratedToDisk(UhtType type, CodeBuilder codeBuilder)
	{
		string directory = GetGeneratedDirectory(type.Package);
		SaveGeneratedToDisk(type.Package, directory, type.EngineName, codeBuilder.ToString());
	}

	public static void SaveGeneratedToDisk(UhtFunction function, CodeBuilder codeBuilder)
	{
		string directory = GetGeneratedDirectory(function.Package);
		string typeName = function.EngineName;
		if (function.HasAnyFlags(EFunctionFlags.Delegate) && function.Outer is UhtClass outerClass)
		{
			typeName = $"{outerClass.EngineName}_{typeName}";
		}

		SaveGeneratedToDisk(function.Package, directory, typeName, codeBuilder.ToString());
	}

	public static void SaveGeneratedToDisk(UhtPackage package, string directory, string typeName, string text)
	{
		string absoluteFilePath = GetGeneratedFilePath(typeName, directory);
		bool directoryExists = Directory.Exists(directory);
		bool generatedExists = File.Exists(absoluteFilePath);
		bool generatedUnchanged = generatedExists && File.ReadAllText(absoluteFilePath) == text;

		ReadWriteLock.EnterWriteLock();
		try
		{
			// If the directory exists and the file exists with the same text, we can return early
			if (directoryExists && generatedUnchanged)
			{
				UnchangedFiles.Add(absoluteFilePath);
				return;
			}

			if (!directoryExists)
			{
				Directory.CreateDirectory(directory);
			}

			File.WriteAllText(absoluteFilePath, text);
			ChangedFiles.Add(absoluteFilePath);
			BindingGenerator.HasGeneratedChanged = true;
		}
		finally
		{
			ReadWriteLock.ExitWriteLock();
		}
	}

	public static void CleanOldGeneratedFiles()
	{
		Console.WriteLine("Cleaning up old generated C# files...");
		CleanFilesInDirectories(Program.EngineGeneratedPath, true);
	}

	private static void CleanFilesInDirectories(string path, bool recursive = false)
	{
		if (!Directory.Exists(path))
		{
			return;
		}

		string[] directories = Directory.GetDirectories(path);

		foreach (var directory in directories)
		{
			string moduleName = Path.GetFileName(directory);
			if (!BindingGenerator.HasBeenExported(moduleName))
			{
				continue;
			}

			int removedFiles = 0;
			string[] files = Directory.GetFiles(directory);

			foreach (var file in files)
			{
				if (ChangedFiles.Contains(file) || UnchangedFiles.Contains(file))
				{
					continue;
				}

				File.Delete(file);
				removedFiles++;
			}

			if (removedFiles == files.Length)
			{
				Directory.Delete(directory, recursive);
			}
		}
	}
}
