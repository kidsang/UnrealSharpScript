using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class MapPropertyTranslator : ContainerPropertyTranslator
{
	private UhtProperty GetKeyProperty(UhtProperty property)
	{
		UhtMapProperty containerProperty = (UhtMapProperty)property;
		return containerProperty.KeyProperty;
	}

	private PropertyTranslator GetKeyTranslator(UhtProperty property)
	{
		UhtMapProperty containerProperty = (UhtMapProperty)property;
		return PropertyTranslatorManager.GetTranslator(containerProperty.KeyProperty);
	}

	public override bool CanExport(UhtProperty property)
	{
		PropertyTranslator keyTranslator = GetKeyTranslator(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		if (valueTranslator.GetType() == typeof(UnsupportedPropertyTranslator) || keyTranslator.GetType() == typeof(UnsupportedPropertyTranslator))
		{
			return false;
		}

		return keyTranslator.IsSupportedAsInnerKey && valueTranslator.IsSupportedAsInnerValue;
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		UhtProperty keyProperty = GetKeyProperty(property);
		PropertyTranslator keyTranslator = GetKeyTranslator(property);
		string keyManagedType = keyTranslator.GetParamManagedType(keyProperty);
		if (keyManagedType.EndsWith("?"))
		{
			keyManagedType = keyManagedType.Substring(0, keyManagedType.Length - 1);
		}

		UhtProperty valueProperty = GetValueProperty(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		string valueManagedType = valueTranslator.GetParamManagedType(valueProperty);
		return valueTranslator switch
		{
			StructPropertyTranslator => $"Map<{keyManagedType}, {valueManagedType}, {valueTranslator.GetPropManagedType(valueProperty)}>",
			DelegatePropertyTranslator => $"DelegateMap<{keyManagedType}, {valueManagedType}, {valueTranslator.GetPropManagedType(valueProperty)}>",
			_ => $"Map<{keyManagedType}, {valueManagedType}>"
		};
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		UhtProperty keyProperty = GetKeyProperty(property);
		PropertyTranslator keyTranslator = GetKeyTranslator(property);
		string keyManagedType = keyTranslator.GetParamManagedType(keyProperty);
		if (keyManagedType.EndsWith("?"))
		{
			keyManagedType = keyManagedType.Substring(0, keyManagedType.Length - 1);
		}

		UhtProperty valueProperty = GetValueProperty(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		string valueManagedType = valueTranslator.GetParamManagedType(valueProperty);

		return $"Dictionary<{keyManagedType}, {valueManagedType}>";
	}

	public override void ExportPropertyGetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		UhtProperty keyProperty = GetKeyProperty(property);
		PropertyTranslator keyTranslator = GetKeyTranslator(property);
		string keyMarshaller = $"{keyTranslator.GetMarshaller(keyProperty)}.Instance";
		if (keyTranslator.IsNullable)
		{
			keyMarshaller += "!";
		}

		PropertyTranslator valueTranslator = GetValueTranslator(property);
		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string propEngineName = property.EngineName;
		if (valueTranslator is StructPropertyTranslator)
		{
			codeBuilder.Append($"new({nativePtr} + {propEngineName}_Offset, {propEngineName}_NativeProp, {keyMarshaller});");
		}
		else
		{
			UhtProperty valueProperty = GetValueProperty(property);
			string valueMarshaller = valueTranslator.GetMarshaller(valueProperty);
			codeBuilder.Append($"new({nativePtr} + {propEngineName}_Offset, {propEngineName}_NativeProp, {keyMarshaller}, {valueMarshaller}.Instance);");
		}
	}

	public override void ExportParamToNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName)
	{
		UhtProperty keyProperty = GetKeyProperty(property);
		PropertyTranslator keyTranslator = GetKeyTranslator(property);
		string keyMarshaller = $"{keyTranslator.GetMarshaller(keyProperty)}.Instance";
		if (keyTranslator.IsNullable)
		{
			keyMarshaller += "!";
		}

		UhtProperty valueProperty = GetValueProperty(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		string valueMarshaller = valueTranslator.GetMarshaller(valueProperty);

		string propManagedType = GetPropManagedType(property);
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		switch (valueTranslator)
		{
			case StructPropertyTranslator:
				codeBuilder.AppendLine($"new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp, {keyMarshaller}).CopyFrom({paramName});");
				break;
			default:
				codeBuilder.AppendLine($"new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp, {keyMarshaller}, {valueMarshaller}.Instance).CopyFrom({paramName});");
				break;
		}
	}

	public override void ExportParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName, bool needDeclaration)
	{
		UhtProperty keyProperty = GetKeyProperty(property);
		PropertyTranslator keyTranslator = GetKeyTranslator(property);
		string keyMarshaller = $"{keyTranslator.GetMarshaller(keyProperty)}.Instance";
		if (keyTranslator.IsNullable)
		{
			keyMarshaller += "!";
		}

		UhtProperty valueProperty = GetValueProperty(property);
		PropertyTranslator valueTranslator = GetValueTranslator(property);
		string valueMarshaller = valueTranslator.GetMarshaller(valueProperty);

		string propManagedType = GetPropManagedType(property);
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		string declaration = needDeclaration ? $"{GetParamManagedType(property)} " : "";
		switch (valueTranslator)
		{
			case StructPropertyTranslator:
				codeBuilder.AppendLine($"{declaration}{paramName} = new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp, {keyMarshaller});");
				break;
			default:
				codeBuilder.AppendLine($"{declaration}{paramName} = new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {funcEngineName}_{paramEngineName}_NativeProp, {keyMarshaller}, {valueMarshaller}.Instance);");
				break;
		}
	}
}
