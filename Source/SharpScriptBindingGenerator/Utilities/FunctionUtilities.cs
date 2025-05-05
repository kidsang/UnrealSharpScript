using System;
using System.Collections.Generic;
using System.Linq;
using EpicGames.Core;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.PropertyTranslators;

namespace SharpScriptBindingGenerator.Utilities;

public static class FunctionUtilities
{
	public static bool HasAnyFlags(this UhtFunction function, EFunctionFlags flags)
	{
		return (function.FunctionFlags & flags) != 0;
	}

	public static bool HasAllFlags(this UhtFunction function, EFunctionFlags flags)
	{
		return (function.FunctionFlags & flags) == flags;
	}

	public static bool IsInterfaceFunction(this UhtFunction function)
	{
		if (function.Outer is not UhtClass classOwner)
		{
			return false;
		}

		if (classOwner.HasAnyFlags(EClassFlags.Interface))
		{
			return true;
		}

		string engineName = function.EngineName.EndsWith("_Implementation")
			? function.EngineName.Substring(0, function.EngineName.Length - 15)
			: function.EngineName;

		UhtClass? currentClass = classOwner;
		while (currentClass != null)
		{
			foreach (UhtClass interfaceClass in currentClass.GetInterfacesIncludeSuper())
			{
				if (interfaceClass.FindFunctionByName(engineName) != null)
				{
					return true;
				}
			}

			currentClass = currentClass.Super as UhtClass;
		}

		return false;
	}

	public static bool HasOutParams(this UhtFunction function)
	{
		// Multicast delegates can have out params, but the UFunction flag isn't set.
		foreach (UhtProperty param in function.Properties)
		{
			if (param.HasAnyFlags(EPropertyFlags.OutParm))
			{
				return true;
			}
		}

		return false;
	}

	public static bool HasParametersOrReturnValue(this UhtFunction function)
	{
		return function.Children.Count > 0;
	}

	public static bool HasSingleBlittableParam(this UhtFunction function)
	{
		if (function.Children.Count != 1)
		{
			return false;
		}

		UhtProperty param = (UhtProperty)function.Children[0];
		PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(param);
		return translator.IsBlittable;
	}

	public static bool HasSameSignature(this UhtFunction function, UhtFunction otherFunction)
	{
		if (function.Children.Count != otherFunction.Children.Count)
		{
			return false;
		}

		for (int i = 0; i < function.Children.Count; i++)
		{
			UhtProperty param = (UhtProperty)function.Children[i];
			UhtProperty otherParam = (UhtProperty)otherFunction.Children[i];
			if (!param.IsSameType(otherParam))
			{
				return false;
			}
		}

		return true;
	}

	// private static bool IsBlueprintAccessor(this UhtFunction function, string accessorType, Func<UhtProperty, UhtFunction?> getBlueprintAccessor)
	// {
	// 	if (function.Properties.Count() != 1)
	// 	{
	// 		return false;
	// 	}
	//
	// 	if (function.HasMetadata(accessorType))
	// 	{
	// 		return true;
	// 	}
	//
	// 	if (function.Outer is not UhtClass classObj)
	// 	{
	// 		return false;
	// 	}
	//
	// 	foreach (UhtProperty property in classObj.Properties)
	// 	{
	// 		if (function != getBlueprintAccessor(property)! || !function.VerifyBlueprintAccessor(property))
	// 		{
	// 			continue;
	// 		}
	//
	// 		return true;
	// 	}
	//
	// 	return false;
	// }
	//
	// public static bool VerifyBlueprintAccessor(this UhtFunction function, UhtProperty property)
	// {
	// 	if (!function.Properties.Any() || function.Properties.Count() != 1)
	// 	{
	// 		return false;
	// 	}
	//
	// 	UhtProperty firstProperty = function.Properties.First();
	// 	return firstProperty.IsSameType(property);
	// }
	//
	// public static bool IsNativeAccessor(this UhtFunction function, GetterSetterMode accessorType)
	// {
	// 	UhtClass classObj = (function.Outer as UhtClass)!;
	// 	foreach (UhtProperty property in classObj.Properties)
	// 	{
	// 		if (accessorType + property.EngineName == function.SourceName)
	// 		{
	// 			switch (accessorType)
	// 			{
	// 				case GetterSetterMode.Get:
	// 					return property.HasNativeGetter();
	// 				case GetterSetterMode.Set:
	// 					return property.HasNativeSetter();
	// 			}
	// 		}
	// 	}
	//
	// 	return false;
	// }
	//
	// public static bool IsAnyGetter(this UhtFunction function)
	// {
	// 	if (function.Properties.Count() != 1)
	// 	{
	// 		return false;
	// 	}
	//
	// 	return function.IsBlueprintAccessor("BlueprintGetter", property => property.GetBlueprintGetter())
	// 			|| function.IsNativeAccessor(GetterSetterMode.Get);
	// }
	//
	// public static bool IsAnySetter(this UhtFunction function)
	// {
	// 	if (function.Properties.Count() != 1)
	// 	{
	// 		return false;
	// 	}
	//
	// 	return function.IsBlueprintAccessor("BlueprintSetter", property => property.GetBlueprintSetter())
	// 			|| function.IsNativeAccessor(GetterSetterMode.Set);
	// }

	public static bool HasCustomStructParamSupport(this UhtFunction function)
	{
		if (!function.HasMetadata("CustomStructureParam")) return false;

		var customStructParams = function.GetCustomStructParams();
		return customStructParams.All(customParamName =>
			function.Properties.Count(param => param.EngineName == customParamName) == 1);
	}

	public static List<string> GetCustomStructParams(this UhtFunction function)
	{
		if (!function.HasMetadata("CustomStructureParam")) return new List<string>();

		return function.GetMetadata("CustomStructureParam").Split(",").ToList();
	}

	public static int GetCustomStructParamCount(this UhtFunction function) => function.GetCustomStructParams().Count;

	public static List<string> GetCustomStructParamTypes(this UhtFunction function)
	{
		if (!function.HasMetadata("CustomStructureParam")) return new List<string>();
		int paramCount = function.GetCustomStructParamCount();
		if (paramCount == 1) return new List<string> { "CSP" };
		return Enumerable.Range(0, paramCount).ToList().ConvertAll(i => $"CSP{i}");
	}
}
