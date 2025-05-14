using System.Collections.Generic;
using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.Utilities;

public static class NameMapper
{
	private static readonly List<string> ReservedKeywords = new()
	{
		"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue",
		"decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally",
		"fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
		"long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
		"readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
		"this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual",
		"void", "volatile", "while", "System"
	};

	public static string GetParameterName(this UhtProperty property)
	{
		string scriptName = property.EngineName;
		if (IsAKeyword(scriptName))
		{
			scriptName = "@" + scriptName;
		}

		return scriptName;
	}

	public static string GetPropertyName(this UhtProperty property)
	{
		string propertyName = property.Deprecated ? property.SourceName : property.EngineName;
		if (IsAKeyword(propertyName))
		{
			propertyName = $"@{propertyName}";
		}

		return propertyName;
	}

	public static string GetFunctionName(this UhtFunction function)
	{
		string functionName = function.StrippedFunctionName;
		if (function.MetaData.TryGetValue("ScriptName", out string? scriptName))
		{
			scriptName = scriptName.Trim();
			if (scriptName.Length > 0)
			{
				functionName = scriptName;
			}
		}

		if (functionName == "GetType")
		{
			// never override System.Object.GetType()
			return "K2_GetType";
		}

		return functionName;
	}

	public static string GetManagedName(this UhtType type)
	{
		return type.SourceName;
	}

	public static string GetFullManagedName(this UhtType type)
	{
		return $"{type.GetNamespace()}.{type.SourceName}";
	}

	public static string GetInterfaceFullManagedName(this UhtType type)
	{
		return $"{type.GetNamespace()}.I{type.EngineName}";
	}

	public static string GetNamespace(this UhtType typeObj)
	{
		string packageShortName = typeObj.Package.GetShortName();
		return $"UnrealEngine.{packageShortName}";
	}

	public static string GetDelegateNamespace(this UhtFunction function)
	{
		string packageShortName = function.Package.GetShortName();
		if (function.Outer is UhtClass outerClass)
		{
			return $"UnrealEngine.{packageShortName}.{outerClass.EngineName}";
		}

		return $"UnrealEngine.{packageShortName}";
	}

	public static string GetDelegateInvokerName(this UhtFunction function)
	{
		if (function.Outer is UhtClass outerClass)
		{
			return $"{outerClass.EngineName}_F{function.StrippedFunctionName}_Invoker";
		}

		return $"F{function.StrippedFunctionName}_Invoker";
	}

	private static bool IsAKeyword(string name)
	{
		return ReservedKeywords.Contains(name);
	}

	public static string PascalToCamelCase(string name)
	{
		return char.ToLowerInvariant(name[0]) + name.Substring(1);
	}
}
