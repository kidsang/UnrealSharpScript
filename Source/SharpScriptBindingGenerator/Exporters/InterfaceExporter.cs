using System.Collections.Generic;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.Exporters;

public static class InterfaceExporter
{
	public static void ExportInterface(UhtClass classObj)
	{
		using var codeBuilder = new CodeBuilder();
		codeBuilder.AddUsing("#nullable enable");
		codeBuilder.AddUsing("using UnrealEngine.Intrinsic;");
		codeBuilder.AddUsing("using UnrealEngine.CoreUObject;");
		codeBuilder.AddUsing("using SharpScript.Interop;");
		codeBuilder.AppendNamespace(classObj);

		GenerateInterfaceClass(codeBuilder, classObj);
		GenerateInterface(codeBuilder, classObj);

		codeBuilder.AppendLine();
		FileExporter.SaveGeneratedToDisk(classObj, codeBuilder);
	}

	private static void GenerateInterfaceClass(CodeBuilder codeBuilder, UhtClass classObj)
	{
		string className = classObj.EngineName;
		string superClassName = $"{classObj.SuperClass!.GetFullManagedName()}, IStaticClass<{className}>";

		codeBuilder.AppendTooltip(classObj);
		codeBuilder.AppendLine($"public class {className} : {superClassName}");
		using (new CodeBlock(codeBuilder)) // class body
		{
			StaticConstructorUtilities.ExportStaticConstructor(codeBuilder, classObj, new());
		}
	}

	private static void GenerateInterface(CodeBuilder codeBuilder, UhtClass classObj)
	{
		List<UhtFunction> exportedFunctions = new();
		List<UhtFunction> exportedOverrides = new();
		HashSet<UhtFunction> unsupportedFunctions = new();
		GeneratorUtilities.GetExportedFunctions(classObj, exportedFunctions, exportedOverrides, unsupportedFunctions);

		codeBuilder.AppendLine();
		codeBuilder.AppendTooltip(classObj);

		string superInterfaceName = classObj.SuperClass!.EngineName == "Interface"
			? "IInterface"
			: classObj.SuperClass!.GetInterfaceFullManagedName();
		codeBuilder.AppendLine($"public interface I{classObj.EngineName} : {superInterfaceName}");

		using (new CodeBlock(codeBuilder)) // interface body
		{
			codeBuilder.AppendLine($"static Class IGetInterfaceClass.InterfaceClass => {classObj.EngineName}.StaticClass.Class!;");
			ExportInterfaceFunctions(codeBuilder, exportedFunctions, unsupportedFunctions);
		}
	}

	static void ExportInterfaceFunctions(CodeBuilder codeBuilder, List<UhtFunction> exportedFunctions, HashSet<UhtFunction> unsupportedFunctions)
	{
		foreach (UhtFunction function in exportedFunctions)
		{
			codeBuilder.AppendLine();
			bool unsupported = unsupportedFunctions.Contains(function);
			using var unsupportedBlock = new UnsupportedBlock(codeBuilder, unsupported);
			using var withEditorBlock = new WithEditorBlock(codeBuilder, function);
			codeBuilder.AppendTooltip(function);
			FunctionExporter.ExportInterfaceFunction(codeBuilder, function);
		}
	}
}
