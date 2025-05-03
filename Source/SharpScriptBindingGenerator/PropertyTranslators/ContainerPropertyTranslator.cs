using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public abstract class ContainerPropertyTranslator : NativeReferencePropertyTranslator
{
	private const EPropertyUsageFlags SupportedUsages = EPropertyUsageFlags.Property
														| EPropertyUsageFlags.Parameter;

	protected ContainerPropertyTranslator() : base(SupportedUsages)
	{
	}

	protected UhtProperty GetValueProperty(UhtProperty property)
	{
		UhtContainerBaseProperty containerProperty = (UhtContainerBaseProperty)property;
		return containerProperty.ValueProperty;
	}

	protected PropertyTranslator GetValueTranslator(UhtProperty property)
	{
		UhtContainerBaseProperty containerProperty = (UhtContainerBaseProperty)property;
		return PropertyTranslatorManager.GetTranslator(containerProperty.ValueProperty);
	}

	public override bool CanExport(UhtProperty property)
	{
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		return valueTranslator.GetType() != typeof(UnsupportedPropertyTranslator) && valueTranslator.IsSupportedAsInnerValue;
	}

	public override string GetMarshaller(UhtProperty property)
	{
		// Container type properties do not have a Marshaller.
		throw new System.NotImplementedException();
	}

	public override string GetNullValue(UhtProperty property)
	{
		return "null";
	}

	public override string ConvertCppDefaultValue(string defaultValue, UhtProperty parameter)
	{
		// Container type properties do not have default values other than null.
		throw new System.NotImplementedException();
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		// Container type function parameters do not have default values.
		throw new System.NotImplementedException();
	}

	public override void ExportStructPropertyFromManaged(CodeBuilder codeBuilder, UhtProperty property)
	{
		string propertyName = property.GetPropertyName();
		codeBuilder.AppendLine($"{propertyName}.CopyFrom(value.{propertyName});");
	}

	public override void ExportParamsStaticField(CodeBuilder codeBuilder, UhtProperty property, UhtFunction function, string modifier)
	{
		string funcName = function.StrippedFunctionName;
		string propName = property.EngineName;
		codeBuilder.AppendLine($"{modifier} IntPtr {funcName}_{propName}_NativeProp;");
		codeBuilder.AppendLine($"{modifier} int {funcName}_{propName}_Offset;");
	}

	public override void ExportParamsStaticConstructor(CodeBuilder codeBuilder, UhtProperty property, UhtFunction function)
	{
		string funcName = function.StrippedFunctionName;
		string propName = property.EngineName;
		codeBuilder.AppendLine($"{funcName}_{propName}_NativeProp = {funcName}_PropIter.FindNext(\"{propName}\");");
		codeBuilder.AppendLine($"{funcName}_{propName}_Offset = TypeInterop.GetPropertyOffset({funcName}_{propName}_NativeProp);");
	}
}
