using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class SetPropertyTranslator : ContainerPropertyTranslator
{
	public override bool CanExport(UhtProperty property)
	{
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		return valueTranslator.GetType() != typeof(UnsupportedPropertyTranslator) && valueTranslator.IsSupportedAsInnerKey;
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		UhtProperty valueProperty = GetValueProperty(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		return $"Set<{valueTranslator.GetParamManagedType(valueProperty)}>";
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		UhtProperty valueProperty = GetValueProperty(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		return $"HashSet<{valueTranslator.GetParamManagedType(valueProperty)}>";
	}

	public override void ExportPropertyGetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string propEngineName = property.EngineName;
		UhtProperty valueProperty = GetValueProperty(property);
		string valueMarshaller = valueTranslator.GetMarshaller(valueProperty);
		codeBuilder.Append($"new({nativePtr} + {propEngineName}_Offset, {propEngineName}_NativeProp, {valueMarshaller}.Instance);");
	}

	public override void ExportParamToNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName)
	{
		string propManagedType = GetPropManagedType(property);
		UhtProperty valueProperty = GetValueProperty(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		string valueMarshaller = valueTranslator.GetMarshaller(valueProperty);
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		codeBuilder.AppendLine($"new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp, {valueMarshaller}.Instance).CopyFrom({paramName});");
	}

	public override void ExportParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName, bool needDeclaration)
	{
		string propManagedType = GetPropManagedType(property);
		UhtProperty valueProperty = GetValueProperty(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		string valueMarshaller = valueTranslator.GetMarshaller(valueProperty);
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		string declaration = needDeclaration ? $"{GetParamManagedType(property)} " : "";
		codeBuilder.AppendLine($"{declaration}{paramName} = new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp, {valueMarshaller}.Instance);");
	}
}
