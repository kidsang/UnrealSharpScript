using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class ClassPropertyTranslator : SimpleTypePropertyTranslator
{
	public ClassPropertyTranslator() : base(typeof(UhtClassProperty))
	{
	}

	public override bool CanExport(UhtProperty property)
	{
		UhtClassProperty? classProperty = property as UhtClassProperty;
		if (classProperty == null)
		{
			return false;
		}

		return BindingGenerator.ShouldExportTypes.Contains(classProperty.MetaClass!);
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		UhtClassProperty classProperty = (UhtClassProperty)property;
		string managedType = classProperty.MetaClass!.GetFullManagedName();
		return $"TSubclassOf<{managedType}>";
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		return GetPropManagedType(property);
	}

	public override string GetGenericParamManagedType(UhtProperty property, string typeArgument)
	{
		return $"TSubclassOf<{typeArgument}>";
	}

	public override string GetMarshaller(UhtProperty property)
	{
		UhtClassProperty classProperty = (UhtClassProperty)property;
		string managedType = classProperty.MetaClass!.GetFullManagedName();
		return $"SubclassOfMarshaller<{managedType}>";
	}

	public override void ExportGenericParamToNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string typeArgument, string nativeBufferName)
	{
		string funcEngineName = function.StrippedFunctionName;
		string paramSourceName = property.SourceName;
		string marshaller = $"SubclassOfMarshaller<{typeArgument}>";
		codeBuilder.AppendLine($"{marshaller}.ToNative({nativeBufferName} + {funcEngineName}_{paramSourceName}_Offset, {paramName});");
	}

	public override void ExportGenericParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string typeArgument, string nativeBufferName, bool needDeclaration)
	{
		string funcEngineName = function.StrippedFunctionName;
		string paramSourceName = property.SourceName;
		string marshaller = $"SubclassOfMarshaller<{typeArgument}>";
		string declaration = needDeclaration ? $"TSubclassOf<{typeArgument}> " : "";
		codeBuilder.AppendLine($"{declaration}{paramName} = {marshaller}.FromNative({nativeBufferName} + {funcEngineName}_{paramSourceName}_Offset);");
	}
}
