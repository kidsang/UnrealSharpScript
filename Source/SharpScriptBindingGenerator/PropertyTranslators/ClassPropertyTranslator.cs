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
		return $"SubclassOf<{managedType}>";
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		return GetPropManagedType(property);
	}

	public override string GetGenericParamManagedType(UhtProperty property, string typeArgument)
	{
		return $"SubclassOf<{typeArgument}>";
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
		string paramEngineName = property.EngineName;
		string marshaller = $"SubclassOfMarshaller<{typeArgument}>";
		codeBuilder.AppendLine($"{marshaller}.ToNative({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {paramName});");
	}

	public override void ExportGenericParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string typeArgument, string nativeBufferName, bool needDeclaration)
	{
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		string marshaller = $"SubclassOfMarshaller<{typeArgument}>";
		string declaration = needDeclaration ? $"SubclassOf<{typeArgument}> " : "";
		codeBuilder.AppendLine($"{declaration}{paramName} = {marshaller}.FromNative({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset);");
	}
}
