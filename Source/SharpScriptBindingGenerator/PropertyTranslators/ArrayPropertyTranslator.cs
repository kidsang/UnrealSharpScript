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
}
