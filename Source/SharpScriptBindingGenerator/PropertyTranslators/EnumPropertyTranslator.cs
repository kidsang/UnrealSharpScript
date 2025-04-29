using System;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Exporters;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class EnumPropertyTranslator : BlittableTypePropertyTranslator
{
	public EnumPropertyTranslator() : base(typeof(UhtEnumProperty), "")
	{
	}

	public override bool CanExport(UhtProperty property)
	{
		if (property is not (UhtEnumProperty or UhtByteProperty))
		{
			return false;
		}

		UhtEnum? enumObj = GetEnum(property);
		return enumObj != null && BindingGenerator.ShouldExportTypes.Contains(enumObj);
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		UhtEnum enumObj = GetEnum(property)!;
		return enumObj.GetFullManagedName();
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		UhtEnum enumObj = GetEnum(property)!;
		return enumObj.GetFullManagedName();
	}

	public override string ConvertCppDefaultValue(string defaultValue, UhtProperty parameter)
	{
		UhtEnum enumObj = GetEnum(parameter)!;
		int index = enumObj.GetIndexByName(defaultValue);
		string valueName = EnumExporter.GetCleanEnumValueName(enumObj, enumObj.EnumValues[index]);
		return $"{GetParamManagedType(parameter)}.{valueName}";
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		int indexOfDot = defaultValue.LastIndexOf("::", StringComparison.Ordinal);
		if (indexOfDot != -1)
		{
			defaultValue = defaultValue.Substring(indexOfDot + 2);
		}

		string fullEnumName = GetParamManagedType(property);
		codeBuilder.AppendLine($"{fullEnumName} {paramName} = {fullEnumName}.{defaultValue};");
	}

	public override string GetMarshaller(UhtProperty property)
	{
		return $"EnumMarshaller<{GetParamManagedType(property)}>";
	}

	private static UhtEnum? GetEnum(UhtProperty property)
	{
		return property switch
		{
			UhtEnumProperty enumProperty => enumProperty.Enum,
			UhtByteProperty byteProperty => byteProperty.Enum,
			_ => throw new InvalidOperationException("Property is not an enum or byte property")
		};
	}
}
