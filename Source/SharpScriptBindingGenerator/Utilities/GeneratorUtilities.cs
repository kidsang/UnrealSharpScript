using System.Collections.Generic;
using EpicGames.Core;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.PropertyTranslators;

namespace SharpScriptBindingGenerator.Utilities;

public class GetSetPair
{
	public UhtProperty Property = null!;
	public UhtFunction GetterFunc = null!;
	public UhtFunction? SetterFunc;
}

public static class GeneratorUtilities
{
	public static void GetExportedProperties(UhtStruct typeObj, List<UhtProperty> properties)
	{
		foreach (UhtProperty property in typeObj.Properties)
		{
			properties.Add(property);
		}
	}

	public static Dictionary<UhtProperty, GetSetPair>? GetPropertyGetSetPairs(UhtClass classObj, List<UhtProperty> properties)
	{
		Dictionary<UhtProperty, GetSetPair>? getSetPairs = null;
		foreach (UhtProperty property in classObj.Properties)
		{
			if (!property.MetaData.TryGetValue("BlueprintGetter", out var getterFuncName))
			{
				continue;
			}

			UhtFunction? getterFunc = classObj.FindFunctionByName(getterFuncName);
			if (getterFunc == null
				|| getterFunc.Children.Count != 1
				|| getterFunc.Children[0] is not UhtProperty returnParam
				|| !returnParam.HasAnyFlags(EPropertyFlags.ReturnParm)
				|| !returnParam.IsSameType(property))
			{
				continue;
			}

			UhtFunction? setterFunc = null;
			if (property.MetaData.TryGetValue("BlueprintSetter", out var setterFuncName))
			{
				setterFunc = classObj.FindFunctionByName(setterFuncName);
				if (setterFunc == null
					|| setterFunc.Children.Count != 1
					|| setterFunc.Children[0] is not UhtProperty inputParam
					|| inputParam.HasAnyFlags(EPropertyFlags.ReturnParm)
					|| inputParam.HasAnyFlags(EPropertyFlags.OutParm) && !inputParam.HasAnyFlags(EPropertyFlags.ReferenceParm)
					|| !inputParam.IsSameType(property))
				{
					setterFunc = null;
				}
			}

			getSetPairs ??= new();
			getSetPairs[property] = new GetSetPair
			{
				Property = property,
				GetterFunc = getterFunc,
				SetterFunc = setterFunc
			};
		}

		return getSetPairs;
	}

	public static bool CanExportFunction(UhtFunction function)
	{
		if (function.HasAnyFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate))
		{
			return false;
		}

		if (!function.HasAnyFlags(EFunctionFlags.BlueprintCallable | EFunctionFlags.BlueprintEvent)
			|| function.HasMetadata("ScriptNoExport"))
		{
			return false;
		}

		return true;
	}

	public static bool CanExportParameters(UhtFunction function)
	{
		foreach (UhtProperty parameter in function.Properties)
		{
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(parameter);
			if (translator.GetType() == typeof(UnsupportedPropertyTranslator)
				|| !translator.IsSupportedAsParameter
				|| !translator.CanExport(parameter))
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
