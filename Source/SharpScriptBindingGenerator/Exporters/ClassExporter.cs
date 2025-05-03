using System;
using System.Collections.Generic;
using EpicGames.Core;
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

		codeBuilder.AppendLine($"namespace {classObj.GetNamespace()}");
		using (new CodeBlock(codeBuilder)) // namespace
		{
			string className = classObj.GetScriptName();
			string superClassName;
			if (classObj.SuperClass != null)
			{
				superClassName = classObj.Package == classObj.SuperClass.Package ? classObj.SuperClass.GetScriptName() : classObj.SuperClass.GetFullManagedName();
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
		}

		ExportExtensionMethods(codeBuilder, classObj.GetFullManagedName(), exportedFunctions, unsupportedFunctions);

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

	private static void ExportClassFunctions(CodeBuilder codeBuilder, List<UhtFunction> exportedFunctions, HashSet<UhtFunction> unsupportedFunctions)
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

	private static string GetExtensionMethodName(UhtFunction function)
	{
		string methodName = function.GetMetadata("ScriptMethod").Trim();
		if (methodName.Length > 0)
		{
			int semiColonIndex = methodName.IndexOf(";", StringComparison.Ordinal);
			if (semiColonIndex >= 0)
			{
				methodName = methodName.Substring(0, semiColonIndex);
			}
		}

		return methodName.Length > 0 ? methodName : function.GetScriptName();
	}

	private static void ExportExtensionMethods(CodeBuilder codeBuilder, string hostClassName, List<UhtFunction> exportedFunctions, HashSet<UhtFunction> unsupportedFunctions)
	{
		Dictionary<UhtStruct, List<ExtensionMethod>> extensionMethodsByTarget = new();
		foreach (UhtFunction function in exportedFunctions)
		{
			if (!function.HasAnyFlags(EFunctionFlags.Static))
			{
				continue;
			}

			if (unsupportedFunctions.Contains(function))
			{
				continue;
			}

			if (!function.MetaData.ContainsKey("ScriptMethod"))
			{
				continue;
			}

			if (function.Children.Count == 0)
			{
				continue;
			}

			UhtProperty firstParam = (UhtProperty)function.Children[0];
			if (firstParam.HasAnyFlags(EPropertyFlags.ReturnParm))
			{
				continue;
			}

			UhtStruct? typeObj;
			if (firstParam is UhtObjectProperty objectProperty)
			{
				typeObj = objectProperty.Class;
			}
			else if (firstParam is UhtStructProperty structProperty)
			{
				typeObj = structProperty.ScriptStruct;
			}
			else
			{
				continue;
			}

			if (!extensionMethodsByTarget.TryGetValue(typeObj, out var functions))
			{
				functions = new List<ExtensionMethod>();
				extensionMethodsByTarget.Add(typeObj, functions);
			}

			EExtensionMethodType methodtype = EExtensionMethodType.Normal;
			if (function.MetaData.ContainsKey("BlueprintAutocast"))
			{
				if (function.Children.Count == 2
					&& ((UhtProperty)function.Children[1]).HasAnyFlags(EPropertyFlags.ReturnParm)
					&& function.Children[1] is UhtStructProperty)
				{
					methodtype = EExtensionMethodType.Autocast;
				}
			}

			string methodName = methodtype switch
			{
				EExtensionMethodType.Normal => GetExtensionMethodName(function),
				_ => ""
			};

			ExtensionMethod method = new ExtensionMethod()
			{
				MethodType = methodtype,
				MethodName = methodName,
				Function = function,
			};

			functions.Add(method);
		}

		foreach (var pair in extensionMethodsByTarget)
		{
			UhtStruct typeObj = pair.Key;
			string typeDecl = typeObj is UhtClass ? "class" : "struct";

			codeBuilder.AppendLine();
			codeBuilder.AppendLine($"namespace {typeObj.GetNamespace()}");
			using (new CodeBlock(codeBuilder)) // namespace
			{
				codeBuilder.AppendLine($"public partial {typeDecl} {typeObj.GetScriptName()}");
				using (new CodeBlock(codeBuilder)) // type body
				{
					var extensionMethods = pair.Value;
					for (int i = 0; i < extensionMethods.Count; i++)
					{
						if (i > 0)
						{
							codeBuilder.AppendLine();
						}

						ExtensionMethod extensionMethod = extensionMethods[i];
						using var withEditorBlock = new WithEditorBlock(codeBuilder, extensionMethod.Function);
						FunctionExporter.ExportExtensionMethod(codeBuilder, hostClassName, extensionMethod);
					}
				}
			}
		}
	}
}
