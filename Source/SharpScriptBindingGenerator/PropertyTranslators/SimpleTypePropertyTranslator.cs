using System;
using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public abstract class SimpleTypePropertyTranslator : PropertyTranslator
{
	private readonly Type _propertyType;
	protected readonly string ManagedType;

	protected SimpleTypePropertyTranslator(Type propertyType, string managedType = "") : base(EPropertyUsageFlags.Any)
	{
		_propertyType = propertyType;
		ManagedType = managedType;
	}

	public override bool CanExport(UhtProperty property)
	{
		return property.GetType() == _propertyType || property.GetType().IsSubclassOf(_propertyType);
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		return ManagedType;
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		return ManagedType;
	}

	public override string GetNullValue(UhtProperty property)
	{
		return "default";
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
