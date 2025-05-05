using System.Collections.Generic;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.PropertyTranslators;

namespace SharpScriptBindingGenerator.Utilities;

public static class StaticConstructorUtilities
{
	public static void ExportStaticConstructor(CodeBuilder codeBuilder,
		UhtStruct typeObj,
		List<UhtProperty> exportedProperties,
		List<UhtFunction>? exportedFunctions = null,
		List<UhtFunction>? exportedOverrides = null,
		HashSet<UhtFunction>? unsupportedFunctions = null)
	{
		string typeName = typeObj.GetManagedName();
		UhtClass? classObj = typeObj as UhtClass;
		UhtScriptStruct? structObj = typeObj as UhtScriptStruct;

		if (classObj != null)
		{
			codeBuilder.AppendLine(typeName == "Object"
				? $"public static SubclassOf<{typeName}> StaticClass {{ get; }}"
				: $"public new static SubclassOf<{typeName}> StaticClass {{ get; }}");

			if (classObj.GetInterfaces().Count > 0)
			{
				// When facing multiple interfaces inheritance, implement a dummy InterfaceClass getter to avoid error CS8705.
				// CS8705: Interface member does not have a most specific implementation.
				codeBuilder.AppendLine("public static UnrealEngine.CoreUObject.Class InterfaceClass => null!;");
			}

			codeBuilder.AppendLine();
		}

		if (structObj != null)
		{
			codeBuilder.AppendLine("internal static readonly IntPtr NativeType;");
			codeBuilder.AppendLine("internal static readonly int NativeDataSize;");
		}
		else if (classObj != null)
		{
			codeBuilder.AppendLine(typeName == "Object"
				? "internal static readonly IntPtr NativeType;"
				: "internal new static readonly IntPtr NativeType;");
		}

		ExportPropertiesStaticField(codeBuilder, exportedProperties);

		if (exportedFunctions != null)
		{
			ExportFunctionsStaticField(codeBuilder, exportedFunctions, unsupportedFunctions!);
		}

		if (exportedOverrides != null)
		{
			ExportFunctionsStaticField(codeBuilder, exportedOverrides, unsupportedFunctions!);
		}

		codeBuilder.AppendLine();
		codeBuilder.AppendLine(structObj != null ? $"static {typeName}NativeRef()" : $"static {typeName}()");

		using (new CodeBlock(codeBuilder)) // static constructor body
		{
			if (classObj != null)
			{
				codeBuilder.AppendLine($"NativeType = TypeInterop.FindClass(\"{typeName}\");");
				codeBuilder.AppendLine($"StaticClass = new SubclassOf<{typeName}>(NativeType);");
			}
			else if (structObj != null)
			{
				codeBuilder.AppendLine($"NativeType = TypeInterop.FindStruct(\"{typeName}\");");
				codeBuilder.AppendLine("NativeDataSize = TypeInterop.GetStructureSize(NativeType);");
			}

			if (exportedProperties.Count > 0)
			{
				codeBuilder.AppendLine();
				codeBuilder.AppendLine("PropertyIterator propIter = new PropertyIterator(NativeType);");
				ExportPropertiesStaticConstructor(codeBuilder, exportedProperties);
			}

			if (exportedFunctions != null)
			{
				ExportFunctionsStaticConstructor(codeBuilder, exportedFunctions, unsupportedFunctions!);
			}

			if (exportedOverrides != null)
			{
				ExportFunctionsStaticConstructor(codeBuilder, exportedOverrides, unsupportedFunctions!);
			}
		}
	}

	private static void ExportPropertiesStaticField(CodeBuilder codeBuilder, List<UhtProperty> exportedProperties)
	{
		foreach (UhtProperty property in exportedProperties)
		{
			using var withEditorBlock = new WithEditorBlock(codeBuilder, property);
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
			translator.ExportStaticField(codeBuilder, property);
		}
	}

	private static void ExportPropertiesStaticConstructor(CodeBuilder codeBuilder, List<UhtProperty> exportedProperties)
	{
		foreach (UhtProperty property in exportedProperties)
		{
			using var withEditorBlock = new WithEditorBlock(codeBuilder, property);
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
			translator.ExportStaticConstructor(codeBuilder, property);
		}
	}

	private static void ExportFunctionsStaticField(CodeBuilder codeBuilder, List<UhtFunction> exportedFunctions, HashSet<UhtFunction> unsupportedFunctions, bool forDelegate = false)
	{
		foreach (UhtFunction function in exportedFunctions)
		{
			if (unsupportedFunctions.Contains(function))
			{
				continue;
			}

			using var withEditorBlock = new WithEditorBlock(codeBuilder, function);

			string funcName = function.StrippedFunctionName;
			string nativeFuncName = $"{funcName}_NativeFunc";
			string modifier = forDelegate ? "private static" : "internal static readonly";

			codeBuilder.AppendLine($"{modifier} IntPtr {nativeFuncName};");

			if (!function.HasParametersOrReturnValue() || function.HasSingleBlittableParam())
			{
				continue;
			}

			codeBuilder.AppendLine($"{modifier} int {funcName}_ParamsSize;");

			foreach (UhtType parameter in function.Children)
			{
				if (parameter is UhtProperty property)
				{
					PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
					translator.ExportParamsStaticField(codeBuilder, property, function, modifier);
				}
			}
		}
	}

	private static void ExportFunctionsStaticConstructor(CodeBuilder codeBuilder, List<UhtFunction> exportedFunctions, HashSet<UhtFunction> unsupportedFunctions, bool forDelegate = false)
	{
		foreach (UhtFunction function in exportedFunctions)
		{
			if (unsupportedFunctions.Contains(function))
			{
				continue;
			}

			using var withEditorBlock = new WithEditorBlock(codeBuilder, function);

			string funcName = function.StrippedFunctionName;
			string nativeFuncName = $"{funcName}_NativeFunc";
			codeBuilder.AppendLine(forDelegate
				? $"{nativeFuncName} = TypeInterop.GetDelegateSignatureFunction(nativeDelegateProp);"
				: $"{nativeFuncName} = TypeInterop.FindFunction(NativeType, \"{funcName}\");");

			if (!function.HasParametersOrReturnValue() || function.HasSingleBlittableParam())
			{
				continue;
			}

			codeBuilder.AppendLine($"{funcName}_ParamsSize = TypeInterop.GetFunctionParamsSize({nativeFuncName});");
			codeBuilder.AppendLine($"PropertyIterator {funcName}_PropIter = new({nativeFuncName});");

			foreach (UhtType parameter in function.Children)
			{
				if (parameter is UhtProperty property)
				{
					PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
					translator.ExportParamsStaticConstructor(codeBuilder, property, function);
				}
			}
		}
	}

	public static void ExportDelegateInvokerStaticConstructor(CodeBuilder codeBuilder, UhtFunction function)
	{
		List<UhtFunction> exportedFunctions = new List<UhtFunction> { function };
		HashSet<UhtFunction> unsupportedFunctions = new();
		ExportFunctionsStaticField(codeBuilder, exportedFunctions, unsupportedFunctions, forDelegate: true);

		codeBuilder.AppendLine();
		codeBuilder.AppendLine("public static void Initialize(IntPtr nativeDelegateProp)");
		using (new CodeBlock(codeBuilder)) // static constructor body
		{
			ExportFunctionsStaticConstructor(codeBuilder, exportedFunctions, unsupportedFunctions, forDelegate: true);
		}
	}
}
