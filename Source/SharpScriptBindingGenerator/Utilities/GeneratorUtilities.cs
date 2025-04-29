using System;
using System.Collections.Generic;
using EpicGames.Core;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.PropertyTranslators;

namespace SharpScriptBindingGenerator.Utilities;

public static class GeneratorUtilities
{
	public static void GetExportedProperties(UhtStruct typeObj, List<UhtProperty> properties)
	{
		foreach (UhtProperty property in typeObj.Properties)
		{
			properties.Add(property);
		}
	}

	public static bool CanExportFunction(UhtFunction function)
	{
		if (function.HasAnyFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate))
		{
			return false;
		}

		return true;
	}

	public static bool CanExportParameters(UhtFunction function)
	{
		bool CanExportParameter(UhtProperty property, Func<PropertyTranslator, bool> isSupported)
		{
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
			return translator.GetType() != typeof(UnsupportedPropertyTranslator) &&
					isSupported(translator) && translator.CanExport(property);
		}

		if (function.ReturnProperty != null && !CanExportParameter(function.ReturnProperty, translator => translator.IsSupportedAsReturnValue))
		{
			return false;
		}

		foreach (UhtProperty parameter in function.Properties)
		{
			if (!CanExportParameter(parameter, translator => translator.IsSupportedAsParameter))
			{
				return false;
			}
		}

		return true;
	}

	public static void GetExportedFunctions(UhtClass classObj,
		List<UhtFunction> functions,
		List<UhtFunction> overridableFunctions,
		HashSet<UhtFunction> unsupportedFunctions,
		HashSet<UhtClass>? unsupportedInterfaces = null)
	{
		List<UhtFunction> exportedFunctions = new();

		bool HasFunction(List<UhtFunction> functionsToCheck, UhtFunction functionToTest)
		{
			foreach (UhtFunction function in functionsToCheck)
			{
				if (function.SourceName == functionToTest.SourceName || function.CppImplName == functionToTest.CppImplName)
				{
					return true;
				}
			}

			return false;
		}

		foreach (UhtFunction function in classObj.Functions)
		{
			if (!CanExportFunction(function))
			{
				continue;
			}

			// if (function.IsAnyGetter() || function.IsAnySetter())
			// {
			// 	continue;
			// }

			if (!CanExportParameters(function))
			{
				unsupportedFunctions.Add(function);
			}

			if (function.FunctionFlags.HasAnyFlags(EFunctionFlags.BlueprintEvent))
			{
				overridableFunctions.Add(function);
			}
			// else if (function.IsAutocast())
			// {
			// 	functions.Add(function);
			//
			// 	if (function.Properties.First() is not UhtStructProperty structToConvertProperty)
			// 	{
			// 		continue;
			// 	}
			//
			// 	AutocastExporter.AddAutocastFunction(structToConvertProperty.ScriptStruct, function);
			// }
			// else if (TryMakeGetterSetterPair(function, classObj, getterSetterPairs))
			// {
			// 	// pass
			// }
			else
			{
				functions.Add(function);
			}

			exportedFunctions.Add(function);
		}

		foreach (UhtClass interfaceClass in classObj.GetInterfacesIncludeSuper())
		{
			foreach (UhtFunction function in interfaceClass.Functions)
			{
				if (HasFunction(exportedFunctions, function) || !CanExportFunction(function))
				{
					continue;
				}

				if (!CanExportParameters(function))
				{
					unsupportedFunctions.Add(function);
					unsupportedInterfaces?.Add(interfaceClass);
				}

				if (function.FunctionFlags.HasAnyFlags(EFunctionFlags.BlueprintEvent))
				{
					overridableFunctions.Add(function);
				}
				else
				{
					functions.Add(function);
				}
			}
		}
	}
}
