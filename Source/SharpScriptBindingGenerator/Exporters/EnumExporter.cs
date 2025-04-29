using System;
using EpicGames.Core;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.Exporters;

public static class EnumExporter
{
	public static void ExportEnum(UhtEnum enumObj)
	{
		using var codeBuilder = new CodeBuilder();
		codeBuilder.AppendNamespace(enumObj);

		codeBuilder.AppendTooltip(enumObj);

		if (enumObj.EnumFlags.HasAnyFlags(EEnumFlags.Flags))
		{
			codeBuilder.AppendLine("[Flags]");
		}

		string underlyingType = UnderlyingTypeToString(enumObj.UnderlyingType);
		codeBuilder.AppendLine($"public enum {enumObj.EngineName} : {underlyingType}");

		using (new CodeBlock(codeBuilder)) // enum body
		{
			int enumValuesCount = enumObj.EnumValues.Count;
			for (int i = 0; i < enumValuesCount; i++)
			{
				if (i > 0)
				{
					codeBuilder.AppendLine();
				}

				UhtEnumValue enumValue = enumObj.EnumValues[i];

				string toolTip = enumObj.GetMetadata("Tooltip", i);
				codeBuilder.AppendTooltip(toolTip);

				string cleanValueName = GetCleanEnumValueName(enumObj, enumValue);
				string value = enumValue.Value == -1 ? "," : $" = {enumValue.Value},";
				codeBuilder.AppendLine($"{cleanValueName}{value}");
			}
		}

		codeBuilder.AppendLine();
		FileExporter.SaveGeneratedToDisk(enumObj, codeBuilder);
	}

	private static string UnderlyingTypeToString(UhtEnumUnderlyingType underlyingType)
	{
		return underlyingType switch
		{
			UhtEnumUnderlyingType.Unspecified => "int",
			UhtEnumUnderlyingType.Uint8 => "byte",
			UhtEnumUnderlyingType.Uint16 => "ushort",
			UhtEnumUnderlyingType.Uint32 => "uint",
			UhtEnumUnderlyingType.Uint64 => "ulong",
			UhtEnumUnderlyingType.Int8 => "sbyte",
			UhtEnumUnderlyingType.Int16 => "short",
			UhtEnumUnderlyingType.Int32 => "int",
			UhtEnumUnderlyingType.Int64 => "long",
			UhtEnumUnderlyingType.Int => "int",
			_ => throw new ArgumentOutOfRangeException(nameof(underlyingType), underlyingType, null)
		};
	}

	public static string GetCleanEnumValueName(UhtEnum enumObj, UhtEnumValue enumValue)
	{
		if (enumObj.CppForm == UhtEnumCppForm.Regular)
		{
			return enumValue.Name;
		}

		int delimiterIndex = enumValue.Name.IndexOf("::", StringComparison.Ordinal);
		return delimiterIndex < 0 ? enumValue.Name : enumValue.Name.Substring(delimiterIndex + 2);
	}
}
