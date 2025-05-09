﻿using System;
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

		Dictionary<UhtProperty, GetSetPair>? getSetPairs = GeneratorUtilities.GetPropertyGetSetPairs(classObj, exportedProperties);

		using var codeBuilder = new CodeBuilder();

		codeBuilder.AddUsing("#nullable enable");
		codeBuilder.AddUsing("#pragma warning disable CS0108"); // CS0108: 'member1' hides inherited member 'member2'. Use the new keyword if hiding was intended.
		codeBuilder.AddUsing("using UnrealEngine.Intrinsic;");
		codeBuilder.AddUsing("using SharpScript.Interop;");

		codeBuilder.AppendLine($"namespace {classObj.GetNamespace()}");
		using (new CodeBlock(codeBuilder)) // namespace
		{
			string className = classObj.GetManagedName();
			string superClassName;
			if (classObj.SuperClass != null)
			{
				superClassName = classObj.Package == classObj.SuperClass.Package ? classObj.SuperClass.GetManagedName() : classObj.SuperClass.GetFullManagedName();
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

				ExportProterties(codeBuilder, exportedProperties, getSetPairs);
				ExportClassFunctions(codeBuilder, exportedFunctions, unsupportedFunctions);
			}
		}

		ExportExtensionMethods(codeBuilder, classObj.GetFullManagedName(), exportedFunctions, unsupportedFunctions);

		codeBuilder.AppendLine();
		FileExporter.SaveGeneratedToDisk(classObj, codeBuilder);
	}

	private static void ExportProterties(CodeBuilder codeBuilder, List<UhtProperty> exportedProperties, Dictionary<UhtProperty, GetSetPair>? getSetPairs)
	{
		foreach (UhtProperty property in exportedProperties)
		{
			codeBuilder.AppendLine();
			using var withEditorBlock = new WithEditorBlock(codeBuilder, property);
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
			if (getSetPairs != null)
			{
				getSetPairs.TryGetValue(property, out var getSetPair);
				translator.ExportProperty(codeBuilder, property, forClass: true, getSetPair: getSetPair);
			}
			else
			{
				translator.ExportProperty(codeBuilder, property, forClass: true, getSetPair: null);
			}
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

		return methodName.Length > 0 ? methodName : function.GetManagedName();
	}

	private static EExtensionMethodType GetExtensionMethodTypeAndName(UhtFunction function, out string methodName)
	{
		string? operatorName = null;

		EExtensionMethodType methodType = EExtensionMethodType.Normal;
		if (function.MetaData.ContainsKey("BlueprintAutocast"))
		{
			if (function.Children.Count == 2
				&& ((UhtProperty)function.Children[1]).HasAnyFlags(EPropertyFlags.ReturnParm)
				&& function.Children[1] is UhtStructProperty)
			{
				methodType = EExtensionMethodType.Autocast;
			}
		}
		else if (function.MetaData.ContainsKey("ScriptOperator"))
		{
			operatorName = function.GetMetadata("ScriptOperator").Trim();
			if (operatorName == "==")
			{
				// see UKismetMathLibrary::NotEqual_IntPointIntPoint
				if (function.StrippedFunctionName.Contains("NotEqual", StringComparison.OrdinalIgnoreCase))
				{
					operatorName = "!=";
				}
			}
			else if (operatorName.Contains('+'))
			{
				operatorName = "+";
			}
			else if (operatorName.Contains('-'))
			{
				operatorName = "-";
			}
			else if (operatorName.Contains('*'))
			{
				operatorName = "*";
			}
			else if (operatorName.Contains('/'))
			{
				operatorName = "/";
			}

			switch (operatorName)
			{
				case "==":
				case "!=":
				{
					if (function.Children.Count == 3
						&& function.Children[2] is UhtBoolProperty ret
						&& ret.HasAnyFlags(EPropertyFlags.ReturnParm)
						&& function.Children[0] is UhtStructProperty lhs
						&& function.Children[1] is UhtStructProperty rhs
						&& lhs.ScriptStruct == rhs.ScriptStruct)
					{
						methodType = EExtensionMethodType.Operator;
					}

					break;
				}

				case "+":
				case "-":
				case "*":
				case "/":
				{
					if (function.Children.Count == 3
						&& function.Children[2] is UhtStructProperty ret
						&& ret.HasAnyFlags(EPropertyFlags.ReturnParm)
						&& function.Children[0] is UhtStructProperty lhs
						&& lhs.ScriptStruct == ret.ScriptStruct)
					{
						methodType = EExtensionMethodType.Operator;
					}

					break;
				}

				case "neg":
				{
					if (function.Children.Count == 2
						&& function.Children[1] is UhtStructProperty ret
						&& ret.HasAnyFlags(EPropertyFlags.ReturnParm)
						&& function.Children[0] is UhtStructProperty lhs
						&& lhs.ScriptStruct == ret.ScriptStruct)
					{
						operatorName = "-";
						methodType = EExtensionMethodType.Operator;
					}

					break;
				}
			}
		}

		methodName = methodType switch
		{
			EExtensionMethodType.Normal => GetExtensionMethodName(function),
			EExtensionMethodType.Autocast => "->",
			EExtensionMethodType.Operator => operatorName!,
			// ReSharper disable once UnreachableSwitchArmDueToIntegerAnalysis
			_ => ""
		};

		return methodType;
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

			EExtensionMethodType methodtype = GetExtensionMethodTypeAndName(function, out var methodName);
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
			ExportOneExtensionMethod(codeBuilder, hostClassName, pair.Key, pair.Value);
		}
	}

	private static void ExportOneExtensionMethod(CodeBuilder codeBuilder, string hostClassName, UhtType typeObj, List<ExtensionMethod> extensionMethods)
	{
		HashSet<string> methodNames = new HashSet<string>();
		foreach (ExtensionMethod extensionMethod in extensionMethods)
		{
			methodNames.Add(extensionMethod.MethodName);
		}

		bool hasEqualsOperator = false;

		// Some operators must be overloaded in pairs (eg. "==" and "!=").
		// Go through the operators to ensure this rule.
		for (int i = extensionMethods.Count - 1; i >= 0; i--)
		{
			var extensionMethod = extensionMethods[i];
			if (extensionMethod.MethodType != EExtensionMethodType.Operator)
			{
				continue;
			}

			string methodName = extensionMethod.MethodName;
			if (methodName == "==")
			{
				hasEqualsOperator = true;

				if (methodNames.Add("!="))
				{
					// Add operator overload "!=" using negative version of "Equals".
					extensionMethods.Insert(i + 1, new ExtensionMethod
					{
						MethodType = EExtensionMethodType.Operator,
						MethodName = "!(==)",
						Function = extensionMethod.Function,
					});
				}

				if (methodNames.Add("Equals"))
				{
					// Add method "Equals".
					extensionMethods.Insert(i, new ExtensionMethod
					{
						MethodType = EExtensionMethodType.Normal,
						MethodName = "Equals",
						Function = extensionMethod.Function,
					});
				}
			}
			else if (methodName == "!=")
			{
				if (!methodNames.Contains("=="))
				{
					extensionMethods.RemoveAt(i);
				}
			}
		}

		codeBuilder.AppendLine();
		codeBuilder.AppendLine($"namespace {typeObj.GetNamespace()}");
		using var namespaceBlock = new CodeBlock(codeBuilder);

		string typeDecl = typeObj is UhtClass ? "class" : "struct";
		string typeName = typeObj.GetManagedName();
		codeBuilder.AppendLine($"public partial {typeDecl} {typeName}");

		if (hasEqualsOperator)
		{
			codeBuilder.Append($" : IEquatable<{typeName}>");
		}

		using var typeBodyBlock = new CodeBlock(codeBuilder);
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

		if (hasEqualsOperator)
		{
			// override object.Equals to fix CS0661.
			// CS0661: https://learn.microsoft.com/dotnet/csharp/misc/cs0661
			codeBuilder.AppendLine();
			codeBuilder.AppendLine("public override bool Equals(object? obj)");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine($"return obj is {typeName} other && Equals(other);");
			}

			// override ValueType.GetHashCode to fix CS0660.
			// CS0660: https://learn.microsoft.com/dotnet/csharp/misc/cs0660
			codeBuilder.AppendLine();
			codeBuilder.AppendLine("public override int GetHashCode()");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine("return base.GetHashCode();");
			}
		}
	}
}
