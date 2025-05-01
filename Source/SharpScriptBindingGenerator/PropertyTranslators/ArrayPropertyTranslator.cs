using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class ArrayPropertyTranslator : ContainerPropertyTranslator
{
	public override string GetPropManagedType(UhtProperty property)
	{
		UhtProperty valueProperty = GetValueProperty(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		return valueTranslator switch
		{
			StructPropertyTranslator => $"Array<{valueTranslator.GetParamManagedType(valueProperty)}, {valueTranslator.GetPropManagedType(valueProperty)}>",
			DelegatePropertyTranslator => $"DelegateArray<{valueTranslator.GetParamManagedType(valueProperty)}, {valueTranslator.GetPropManagedType(valueProperty)}>",
			_ => $"Array<{valueTranslator.GetPropManagedType(valueProperty)}>"
		};
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		UhtProperty valueProperty = GetValueProperty(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		return $"List<{valueTranslator.GetParamManagedType(valueProperty)}>";
	}

	public override string GetGenericParamManagedType(UhtProperty property, string typeArgument)
	{
		UhtProperty valueProperty = GetValueProperty(property);
		if (valueProperty is not UhtObjectPropertyBase)
		{
			return GetParamManagedType(property);
		}

		return $"List<{typeArgument}?>";
	}

	public override void ExportPropertyGetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string propEngineName = property.EngineName;
		if (valueTranslator is StructPropertyTranslator)
		{
			codeBuilder.Append($"new({nativePtr} + {propEngineName}_Offset, {propEngineName}_NativeProp);");
		}
		else
		{
			UhtProperty valueProperty = GetValueProperty(property);
			string valueMarshaller = valueTranslator.GetMarshaller(valueProperty);
			codeBuilder.Append($"new({nativePtr} + {propEngineName}_Offset, {propEngineName}_NativeProp, {valueMarshaller}.Instance);");
		}
	}

	public override void ExportParamToNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName)
	{
		string propManagedType = GetPropManagedType(property);
		UhtProperty valueProperty = GetValueProperty(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		string valueMarshaller = valueTranslator.GetMarshaller(valueProperty);
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		codeBuilder.AppendLine(valueTranslator is StructPropertyTranslator
			? $"new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp).CopyFrom({paramName});"
			: $"new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp, {valueMarshaller}.Instance).CopyFrom({paramName});");
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
		codeBuilder.AppendLine(valueTranslator is StructPropertyTranslator
			? $"{declaration}{paramName} = new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp);"
			: $"{declaration}{paramName} = new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp, {valueMarshaller}.Instance);");
	}

	public override void ExportGenericParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string typeArgument, string nativeBufferName, bool needDeclaration)
	{
		UhtProperty valueProperty = GetValueProperty(property);
		if (valueProperty is not UhtObjectPropertyBase)
		{
			ExportParamFromNative(codeBuilder, function, property, paramName, nativeBufferName, needDeclaration);
			return;
		}

		string propManagedType = $"Array<{typeArgument}?>";
		string valueMarshaller = $"ObjectMarshaller<{typeArgument}>";
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		string declaration = needDeclaration ? $"{GetGenericParamManagedType(property, typeArgument)} " : "";
		codeBuilder.AppendLine($"{declaration}{paramName} = new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp, {valueMarshaller}.Instance);");
	}
}
