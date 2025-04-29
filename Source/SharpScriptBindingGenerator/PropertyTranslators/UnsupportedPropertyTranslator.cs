using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

/// <summary>
/// For properties of unsupported types, export comments.
/// </summary>
public class UnsupportedPropertyTranslator : PropertyTranslator
{
	public UnsupportedPropertyTranslator() : base(EPropertyUsageFlags.Any)
	{
	}

	public override bool CanExport(UhtProperty property)
	{
		return true;
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		if (property is UhtClassProperty classProperty)
		{
			return $"SubclassOf<{classProperty.Class.EngineName}>";
		}

		if (property is UhtObjectPropertyBase objectProperty)
		{
			return $"{property.EngineName}({objectProperty.Class.EngineName})";
		}

		if (property is UhtStructProperty structProperty)
		{
			return structProperty.ScriptStruct.EngineName;
		}

		if (property is UhtEnumProperty enumProperty)
		{
			return enumProperty.Enum.EngineName;
		}

		return property.ToString();
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		return GetPropManagedType(property);
	}

	public override string GetMarshaller(UhtProperty property)
	{
		return "";
	}

	public override string GetNullValue(UhtProperty property)
	{
		return "";
	}

	public override string ConvertCppDefaultValue(string defaultValue, UhtProperty parameter)
	{
		return defaultValue;
	}

	public override void ExportStaticField(CodeBuilder codeBuilder, UhtProperty property)
	{
		// Generate nothing.
	}

	public override void ExportStaticConstructor(CodeBuilder codeBuilder, UhtProperty property)
	{
		// Generate nothing.
	}

	public override void ExportProperty(CodeBuilder codeBuilder, UhtProperty property, bool forClass)
	{
		using var unsupportedBlock = new UnsupportedBlock(codeBuilder);
		string protection = property.GetProtection();
		string managedType = GetPropManagedType(property);
		string propertyName = property.GetPropertyName();
		codeBuilder.AppendLine($"{protection}{managedType} {propertyName}");
	}

	public override void ExportPropertyGetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		// Generate nothing.
	}

	public override void ExportPropertySetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		// Generate nothing.
	}

	public override void ExportParamToNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName)
	{
		// Generate nothing.
	}

	public override void ExportParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName, bool needDeclaration)
	{
		// Generate nothing.
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		// Generate nothing.
	}

	public override void ExportStructProperty(CodeBuilder codeBuilder, UhtProperty property)
	{
		using var unsupportedBlock = new UnsupportedBlock(codeBuilder);
		base.ExportStructProperty(codeBuilder, property);
	}

	public override void ExportStructPropertyToManaged(CodeBuilder codeBuilder, UhtProperty property)
	{
		// Generate nothing.
	}

	public override void ExportStructPropertyFromManaged(CodeBuilder codeBuilder, UhtProperty property)
	{
		// Generate nothing.
	}
}
