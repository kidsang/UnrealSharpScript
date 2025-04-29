using System.Collections.Generic;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.PropertyTranslators;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.Exporters;

public static class ClassExporter
{
	public static void ExportClass(UhtClass classObj)
	{
		List<UhtFunction> exportedFunctions = new();
		List<UhtFunction> exportedOverrides = new();
		HashSet<UhtFunction> unsupportedFunctions = new();
		HashSet<UhtClass> unsupportedInterfaces = new();
		GeneratorUtilities.GetExportedFunctions(classObj, exportedFunctions, exportedOverrides, unsupportedFunctions, unsupportedInterfaces);

		List<UhtProperty> exportedProperties = new List<UhtProperty>();
		GeneratorUtilities.GetExportedProperties(classObj, exportedProperties);

		using var codeBuilder = new CodeBuilder();

		codeBuilder.AddUsing("#nullable enable");
		codeBuilder.AddUsing("#pragma warning disable CS0108"); // CS0108: 'member1' hides inherited member 'member2'. Use the new keyword if hiding was intended.
		codeBuilder.AddUsing("using UnrealEngine.Intrinsic;");
		codeBuilder.AddUsing("using SharpScript.Interop;");
		codeBuilder.AppendNamespace(classObj);

		string className = classObj.EngineName;
		string superClassName;
		if (classObj.SuperClass != null)
		{
			superClassName = classObj.Package == classObj.SuperClass.Package ? classObj.SuperClass.EngineName : classObj.SuperClass.GetFullManagedName();
		}
		else
		{
			superClassName = "ObjectBase";
		}

		superClassName = $"{superClassName}, IStaticClass<{className}>";

		List<UhtClass> interfaces = classObj.GetInterfaces();
		foreach (UhtClass interfaceClass in unsupportedInterfaces)
		{
			interfaces.Remove(interfaceClass);
		}

		codeBuilder.AppendTooltip(classObj);
		codeBuilder.AppendTypeDeclare("class", className, baseTypeName: superClassName, nativeInterfaces: interfaces);

		using (new CodeBlock(codeBuilder)) // class body
		{
			StaticConstructorUtilities.ExportStaticConstructor(codeBuilder, classObj,
				exportedProperties,
				exportedFunctions,
				exportedOverrides,
				unsupportedFunctions);

			ExportProterties(codeBuilder, exportedProperties);
			ExportClassFunctions(codeBuilder, exportedFunctions, unsupportedFunctions);
		}

		codeBuilder.AppendLine();
		FileExporter.SaveGeneratedToDisk(classObj, codeBuilder);
	}

	private static void ExportProterties(CodeBuilder codeBuilder, List<UhtProperty> exportedProperties)
	{
		foreach (UhtProperty property in exportedProperties)
		{
			codeBuilder.AppendLine();
			using var withEditorBlock = new WithEditorBlock(codeBuilder, property);
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
			translator.ExportProperty(codeBuilder, property, forClass: true);
		}
	}

	static void ExportClassFunctions(CodeBuilder codeBuilder, List<UhtFunction> exportedFunctions, HashSet<UhtFunction> unsupportedFunctions)
	{
		foreach (UhtFunction function in exportedFunctions)
		{
			codeBuilder.AppendLine();
			bool unsupported = unsupportedFunctions.Contains(function);
			using var unsupportedBlock = new UnsupportedBlock(codeBuilder, unsupported);
			using var withEditorBlock = new WithEditorBlock(codeBuilder, function);
			FunctionExporter.ExportFunction(codeBuilder, function);
		}
	}
}
