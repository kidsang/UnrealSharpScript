using System;
using System.Collections.Generic;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class StructPropertyTranslator : NativeReferencePropertyTranslator
{
	public StructPropertyTranslator() : base(EPropertyUsageFlags.Any)
	{
	}

	public override bool ExportDefaultParameter => false;

	public override bool CanExport(UhtProperty property)
	{
		UhtStructProperty? structProperty = property as UhtStructProperty;
		return structProperty != null && BindingGenerator.ShouldExportTypes.Contains(structProperty.ScriptStruct);
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		string structManagedType = GetParamManagedType(property);
		return $"{structManagedType}NativeRef";
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		UhtStructProperty structProperty = (UhtStructProperty)property;
		return structProperty.ScriptStruct.GetFullManagedName();
	}

	public override string GetMarshaller(UhtProperty property)
	{
		string structManagedType = GetParamManagedType(property);
		return $"StructMarshaller<{structManagedType}>";
	}

	public override string GetNullValue(UhtProperty property)
	{
		return "default";
	}

	public override string ConvertCppDefaultValue(string defaultValue, UhtProperty parameter)
	{
		// Structs cannot be default values for function parameters.
		throw new NotImplementedException();
	}

	public override void ExportPropertyGetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string propEngineName = property.EngineName;
		codeBuilder.Append($"new({nativePtr} + {propEngineName}_Offset);");
	}

	public override void ExportParamToNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName)
	{
		string propManagedType = GetPropManagedType(property);
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		codeBuilder.AppendLine($"new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset).FromManaged({paramName});");
	}

	public override void ExportParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName, bool needDeclaration)
	{
		string propManagedType = GetPropManagedType(property);
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		string declaration = needDeclaration ? $"{GetParamManagedType(property)} " : "";
		codeBuilder.AppendLine($"{declaration}{paramName} = new {propManagedType}({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset);");
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		string managedType = GetParamManagedType(property);
		if (defaultValue is "" or "()")
		{
			codeBuilder.AppendLine($"{managedType} {paramName} = new();");
			return;
		}

		if (defaultValue.StartsWith("(") && defaultValue.EndsWith(")"))
		{
			defaultValue = defaultValue.Substring(1, defaultValue.Length - 2);
		}

		List<string> fieldInitializers = new List<string>();
		string[] parts = defaultValue.Split(',');

		foreach (string part in parts)
		{
			fieldInitializers.Add(part.Trim());
		}

		codeBuilder.AppendLine($"{managedType} {paramName} = new {managedType}");
		using (new CodeBlock(codeBuilder))
		{
			UhtStructProperty structProperty = (UhtStructProperty)property;
			string structName = structProperty.ScriptStruct.EngineName;

			if (structName == "Color")
			{
				(fieldInitializers[0], fieldInitializers[2]) = (fieldInitializers[2], fieldInitializers[0]);
			}

			int fieldCount = fieldInitializers.Count;
			for (int i = 0; i < fieldCount; i++)
			{
				UhtProperty childProperty = (UhtProperty)structProperty.ScriptStruct.Children[i];
				bool isFloat = childProperty is UhtFloatProperty;
				string fieldName = childProperty.EngineName;
				string fieldInitializer = fieldInitializers[i];

				int pos = fieldInitializer.IndexOf("=", StringComparison.Ordinal);
				if (pos < 0)
				{
					codeBuilder.AppendLine(isFloat ? $"{fieldName}={fieldInitializer}f," : $"{fieldName}={fieldInitializer},");
				}
				else
				{
					codeBuilder.AppendLine(isFloat ? $"{fieldInitializer}f," : $"{fieldInitializer},");
				}
			}
		}

		codeBuilder.Append(";");
	}

	public override void ExportStructPropertyFromManaged(CodeBuilder codeBuilder, UhtProperty property)
	{
		string propertyName = property.GetPropertyName();
		codeBuilder.AppendLine($"{propertyName}.FromManaged(value.{propertyName});");
	}
}
