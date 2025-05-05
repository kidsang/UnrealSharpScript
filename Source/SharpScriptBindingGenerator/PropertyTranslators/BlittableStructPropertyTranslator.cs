using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class BlittableStructPropertyTranslator : StructPropertyTranslator
{
	public override bool IsBlittable => true;

	public override bool ParamNeedInitialize => false;

	public override bool CanExport(UhtProperty property)
	{
		UhtStructProperty? structProperty = property as UhtStructProperty;
		return structProperty != null
				&& BindingGenerator.ShouldExportTypes.Contains(structProperty.ScriptStruct)
				&& structProperty.ScriptStruct.IsBlittable();
	}

	public override string GetMarshaller(UhtProperty property)
	{
		string structManagedType = GetParamManagedType(property);
		return $"BlittableMarshaller<{structManagedType}>";
	}

	public override void ExportProperty(CodeBuilder codeBuilder, UhtProperty property, bool forClass, GetSetPair? getSetPair)
	{
		if (getSetPair != null)
		{
			base.ExportProperty(codeBuilder, property, forClass, getSetPair);
			return;
		}

		string protection = property.GetProtection();
		string managedType = GetParamManagedType(property);
		string propertyName = property.GetPropertyName();

		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string getter = $"ref *({managedType}*)({nativePtr} + {property.EngineName}_Offset);";

		codeBuilder.AppendTooltip(property);
		ExportDeprecation(codeBuilder, property);
		codeBuilder.AppendLine($"{protection}unsafe ref {managedType} {propertyName}");
		using (new CodeBlock(codeBuilder)) // property body
		{
			codeBuilder.AppendLine("get");
			if (forClass)
			{
				using (new CodeBlock(codeBuilder)) // getter body
				{
					codeBuilder.AppendLine("ThrowIfNotValid();");
					codeBuilder.AppendLine($"return {getter}");
				}
			}
			else
			{
				codeBuilder.Append($" => {getter}");
			}
		}
	}

	public override void ExportBlittableStructProperty(CodeBuilder codeBuilder, UhtProperty property, string structName)
	{
		string protection = property.GetProtection();
		string managedType = GetParamManagedType(property);
		string propertyName = property.GetPropertyName();

		codeBuilder.AppendTooltip(property);
		ExportDeprecation(codeBuilder, property);
		codeBuilder.AppendLine($"{protection}unsafe ref {managedType} {propertyName}");
		using (new CodeBlock(codeBuilder)) // property body
		{
			codeBuilder.AppendLine($"get => ref (*({structName}*)nativePtr).{propertyName};");
		}
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

	public override void ExportStructPropertyFromManaged(CodeBuilder codeBuilder, UhtProperty property)
	{
		string propertyName = property.GetPropertyName();
		codeBuilder.AppendLine($"{propertyName} = value.{propertyName};");
	}
}
