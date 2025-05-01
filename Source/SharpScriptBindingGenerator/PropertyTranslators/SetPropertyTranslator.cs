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

	public override string GetGenericParamManagedType(UhtProperty property, string typeArgument)
	{
		UhtProperty valueProperty = GetValueProperty(property);
		if (valueProperty is not UhtObjectPropertyBase)
		{
			return GetParamManagedType(property);
		}

		return $"HashSet<{typeArgument}?>";
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

	public override void ExportGenericParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string typeArgument, string nativeBufferName, bool needDeclaration)
	{
		UhtProperty valueProperty = GetValueProperty(property);
		if (valueProperty is not UhtObjectPropertyBase)
		{
			ExportParamFromNative(codeBuilder, function, property, paramName, nativeBufferName, needDeclaration);
			return;
		}

		string propManagedType = $"Set<{typeArgument}?>";
		string valueMarshaller = $"ObjectMarshaller<{typeArgument}>";
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		string declaration = needDeclaration ? $"{GetGenericParamManagedType(property, typeArgument)} " : "";
		codeBuilder.AppendLine($"{declaration}{paramName} = new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp, {valueMarshaller}.Instance);");
	}
}
