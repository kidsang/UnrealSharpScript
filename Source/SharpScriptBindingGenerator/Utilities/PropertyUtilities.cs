﻿using EpicGames.Core;
using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.Utilities;

public static class PropertyUtilities
{
	public static bool IsOuter<T>(this UhtProperty property)
	{
		return property.Outer is T;
	}

	public static bool HasAnyFlags(this UhtProperty property, EPropertyFlags flags)
	{
		return (property.PropertyFlags & flags) != 0;
	}

	public static bool HasAllFlags(this UhtProperty property, EPropertyFlags flags)
	{
		return (property.PropertyFlags & flags) == flags;
	}

	public static string GetMetaData(this UhtProperty property, string key)
	{
		return property.MetaData.TryGetValue(key, out var value) ? value : string.Empty;
	}

	public static bool HasMetaData(this UhtProperty property, string key)
	{
		return property.MetaData.ContainsKey(key);
	}

	public static bool HasNativeGetter(this UhtProperty property)
	{
		if (property.Outer is UhtScriptStruct)
		{
			return false;
		}

		return !string.IsNullOrEmpty(property.Getter);
	}

	public static bool HasNativeSetter(this UhtProperty property)
	{
		if (property.Outer is UhtScriptStruct)
		{
			return false;
		}

		return !string.IsNullOrEmpty(property.Setter);
	}

	public static bool HasAnyNativeGetterSetter(this UhtProperty property)
	{
		return property.HasNativeGetter() || property.HasNativeSetter();
	}

	public static bool HasBlueprintGetter(this UhtProperty property)
	{
		return false; // todo: twx not implement yet
		// return property.GetBlueprintGetter() != null;
	}

	public static bool HasBlueprintSetter(this UhtProperty property)
	{
		return false; // todo: twx not implement yet
		// return property.GetBlueprintSetter() != null;
	}

	public static bool HasBlueprintGetterOrSetter(this UhtProperty property)
	{
		return property.HasBlueprintGetter() || property.HasBlueprintSetter();
	}

	public static bool HasBlueprintGetterSetterPair(this UhtProperty property)
	{
		return property.HasBlueprintGetter() && property.HasBlueprintSetter();
	}

	public static bool HasAnyGetterOrSetter(this UhtProperty property)
	{
		return property.HasAnyNativeGetterSetter() || property.HasBlueprintGetterOrSetter();
	}

	public static bool HasAnyGetter(this UhtProperty property)
	{
		return property.HasNativeGetter() || property.HasBlueprintGetter();
	}

	public static bool HasAnySetter(this UhtProperty property)
	{
		return property.HasNativeSetter() || property.HasBlueprintSetter();
	}

	public static bool HasGetterSetterPair(this UhtProperty property)
	{
		return property.HasAnyGetter() && property.HasAnySetter();
	}

	// public static UhtFunction? GetBlueprintGetter(this UhtProperty property)
	// {
	//     return property.TryGetBlueprintAccessor(GetterSetterMode.Get);
	// }
	//
	// public static UhtFunction? GetBlueprintSetter(this UhtProperty property)
	// {
	//     return property.TryGetBlueprintAccessor(GetterSetterMode.Set);
	// }

	// public static bool IsWorldContextParameter(this UhtProperty property)
	// {
	//     if (property.Outer is not UhtFunction function)
	//     {
	//         return false;
	//     }
	//
	//     if (property is not UhtObjectProperty objectProperty || objectProperty.Class != Program.Factory.Session.UObject)
	//     {
	//         return false;
	//     }
	//
	//     string sourceName = property.SourceName;
	//     return function.GetMetadata("WorldContext") == sourceName || sourceName is "WorldContextObject" or "WorldContext" or "ContextObject";
	// }

	public static bool IsReadWrite(this UhtProperty property)
	{
		bool isReadOnly = property.HasAllFlags(EPropertyFlags.BlueprintReadOnly);
		return !isReadOnly && (property.PropertyFlags.HasAnyFlags(EPropertyFlags.Edit | EPropertyFlags.BlueprintAssignable) || property.HasAnySetter());
	}

	public static bool IsEditDefaultsOnly(this UhtProperty property)
	{
		return property.HasAllFlags(EPropertyFlags.BlueprintReadOnly | EPropertyFlags.Edit);
	}

	// public static UhtFunction? TryGetBlueprintAccessor(this UhtProperty property, GetterSetterMode accessorType)
	// {
	//     if (property.Outer is UhtScriptStruct || property.Outer is not UhtClass classObj)
	//     {
	//         return null;
	//     }
	//
	//     UhtFunction? TryFindFunction(string name)
	//     {
	//         UhtFunction? function = classObj.FindFunctionByName(name, (uhtFunction, typeName) =>
	//         {
	//             if (uhtFunction.SourceName == typeName
	//                 || (uhtFunction.SourceName.Length == typeName.Length
	//                     && uhtFunction.SourceName.Contains(typeName, StringComparison.InvariantCultureIgnoreCase)))
	//             {
	//                 return true;
	//             }
	//
	//             if (uhtFunction.GetScriptName() == typeName
	//                 || (uhtFunction.GetScriptName().Length == typeName.Length
	//                     && uhtFunction.GetScriptName().Contains(typeName, StringComparison.InvariantCultureIgnoreCase)))
	//             {
	//                 return true;
	//             }
	//
	//             return false;
	//         });
	//
	//         if (function != null && function.VerifyBlueprintAccessor(property))
	//         {
	//             return function;
	//         }
	//
	//         return null;
	//     }
	//
	//     string accessorName = property.GetMetaData(accessorType == GetterSetterMode.Get ? "BlueprintGetter" : "BlueprintSetter");
	//     UhtFunction? function = TryFindFunction(accessorName);
	//     if (function != null)
	//     {
	//         return function;
	//     }
	//
	//     function = TryFindFunction(accessorType + property.SourceName);
	//     if (function != null)
	//     {
	//         return function;
	//     }
	//
	//     function = TryFindFunction(accessorType + property.GetPropertyName());
	//     if (function != null)
	//     {
	//         return function;
	//     }
	//
	//     function = TryFindFunction(accessorType + NameMapper.ScriptifyName(property.SourceName, ENameType.Property));
	//     if (function != null)
	//     {
	//         return function;
	//     }
	//
	//     return null;
	// }

	public static string GetNativePropertyName(this UhtProperty property)
	{
		return $"{property.SourceName}_NativeProperty";
	}

	public static string GetProtection(this UhtProperty property)
	{
		UhtClass? classObj = property.Outer as UhtClass;
		bool isClassOwner = classObj != null;

		// if (isClassOwner && property.HasAnyGetterOrSetter())
		// {
		//     UhtFunction? getter = property.GetBlueprintGetter();
		//     UhtFunction? setter = property.GetBlueprintSetter();
		//
		//     if ((getter != null && getter.FunctionFlags.HasAnyFlags(EFunctionFlags.Public)) || (setter != null && setter.FunctionFlags.HasAnyFlags(EFunctionFlags.Public)))
		//     {
		//         return "public ";
		//     }
		//
		//     if ((getter != null && getter.FunctionFlags.HasAnyFlags(EFunctionFlags.Protected)) || (setter != null && setter.FunctionFlags.HasAnyFlags(EFunctionFlags.Protected)))
		//     {
		//         return "protected ";
		//     }
		// }

		if (property.HasAllFlags(EPropertyFlags.NativeAccessSpecifierPublic) ||
			(property.HasAllFlags(EPropertyFlags.NativeAccessSpecifierPrivate) && property.HasMetaData("AllowPrivateAccess")) ||
			(!isClassOwner && property.HasAllFlags(EPropertyFlags.Protected)))
		{
			return "public ";
		}

		// if (isClassOwner && property.HasAllFlags(EPropertyFlags.Protected))
		// {
		// 	return "protected ";
		// }

		return "internal ";
	}
	//
	// public static bool IsCustomStructureType(this UhtProperty property)
	// {
	//     if (property.Outer is not UhtFunction function) return false;
	//     if (!function.HasCustomStructParamSupport()) return false;
	//
	//     if (function.GetCustomStructParams().Contains(property.EngineName))
	//     {
	//         PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
	//         return translator.CanSupportCustomStruct(property);
	//     }
	//
	//     return false;
	// }
	//
	// public static List<UhtProperty>? GetPrecedingParams(this UhtProperty property)
	// {
	//     if (property.Outer is not UhtFunction function) return null;
	//     return function.Children.Cast<UhtProperty>().TakeWhile(param => param != property).ToList();
	// }
	//
	// public static int GetPrecedingCustomStructParams(this UhtProperty property)
	// {
	//     if (property.Outer is not UhtFunction function) return 0;
	//     if (!function.HasCustomStructParamSupport()) return 0;
	//
	//     return property.GetPrecedingParams()!
	//         .Count(param => param.IsCustomStructureType());
	// }
}
