using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class InterfacePropertyTranslator : PropertyTranslator
{
	private const EPropertyUsageFlags SupportedUsages = EPropertyUsageFlags.Property
														| EPropertyUsageFlags.Parameter
														| EPropertyUsageFlags.ReturnValue
														| EPropertyUsageFlags.InnerValue;

	public InterfacePropertyTranslator() : base(SupportedUsages)
	{
	}

	public override bool IsNullable => true;

	public override bool CanExport(UhtProperty property)
	{
		UhtInterfaceProperty? interfaceProperty = property as UhtInterfaceProperty;
		if (interfaceProperty == null)
		{
			return false;
		}

		return BindingGenerator.ShouldExportTypes.Contains(interfaceProperty.InterfaceClass);
	}

	private string GetManagedType(UhtProperty property)
	{
		UhtInterfaceProperty interfaceProperty = (UhtInterfaceProperty)property;
		return interfaceProperty.InterfaceClass.GetInterfaceFullManagedName();
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		return $"{GetManagedType(property)}?";
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		return $"{GetManagedType(property)}?";
	}

	public override string GetMarshaller(UhtProperty property)
	{
		return $"InterfaceMarshaller<{GetManagedType(property)}>";
	}

	public override string GetNullValue(UhtProperty property)
	{
		return "null";
	}

	public override string ConvertCppDefaultValue(string defaultValue, UhtProperty parameter)
	{
		return defaultValue;
	}

	public override void ExportPropertyGetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		string marshaller = GetMarshaller(property);
		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string propEngineName = property.EngineName;
		codeBuilder.Append($"{marshaller}.FromNative({nativePtr} + {propEngineName}_Offset);");
	}

	public override void ExportPropertySetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		string marshaller = GetMarshaller(property);
		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string propEngineName = property.EngineName;
		codeBuilder.Append($"{marshaller}.ToNative({nativePtr} + {propEngineName}_Offset, value);");
	}

	public override void ExportParamToNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName)
	{
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		string marshaller = GetMarshaller(property);
		codeBuilder.AppendLine($"{marshaller}.ToNative({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {paramName});");
	}

	public override void ExportParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName, bool needDeclaration)
	{
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		string marshaller = GetMarshaller(property);
		string declaration = needDeclaration ? $"{GetParamManagedType(property)} " : "";
		codeBuilder.AppendLine($"{declaration}{paramName} = {marshaller}.FromNative({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset);");
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		codeBuilder.AppendLine($"{GetParamManagedType(property)} {paramName} = {defaultValue};");
	}
}
