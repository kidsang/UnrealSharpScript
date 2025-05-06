using EpicGames.Core;
using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.Utilities;

public static class PropertyUtilities
{
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
		return !isReadOnly && property.PropertyFlags.HasAnyFlags(EPropertyFlags.Edit | EPropertyFlags.BlueprintAssignable);
	}

	public static bool IsEditDefaultsOnly(this UhtProperty property)
	{
		return property.HasAllFlags(EPropertyFlags.BlueprintReadOnly | EPropertyFlags.Edit);
	}

	public static string GetProtection(this UhtProperty property)
	{
		UhtClass? classObj = property.Outer as UhtClass;
		bool isClassOwner = classObj != null;

		if (property.HasAllFlags(EPropertyFlags.NativeAccessSpecifierPublic) ||
			(property.HasAllFlags(EPropertyFlags.NativeAccessSpecifierPrivate) && property.HasMetaData("AllowPrivateAccess")) ||
			(!isClassOwner && property.HasAllFlags(EPropertyFlags.Protected)))
		{
			return "public ";
		}

		if (isClassOwner && property.HasAllFlags(EPropertyFlags.Protected))
		{
			return "protected ";
		}

		return "internal ";
	}

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
