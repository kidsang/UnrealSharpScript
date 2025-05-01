using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class ObjectPropertyTranslator : SimpleTypePropertyTranslator
{
	public ObjectPropertyTranslator() : base(typeof(UhtObjectPropertyBase))
	{
	}

	public override bool IsNullable => true;

	public override bool CanExport(UhtProperty property)
	{
		UhtObjectPropertyBase? objectProperty = property as UhtObjectPropertyBase;
		if (objectProperty == null)
		{
			return false;
		}

		return BindingGenerator.ShouldExportTypes.Contains(objectProperty.Class);
	}

	protected string GetManagedType(UhtProperty property)
	{
		UhtObjectPropertyBase objectProperty = (UhtObjectPropertyBase)property;
		return objectProperty.Class.GetFullManagedName();
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		return $"{GetManagedType(property)}?";
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		return $"{GetManagedType(property)}?";
	}

	public override string GetGenericParamManagedType(UhtProperty property, string typeArgument)
	{
		return $"{typeArgument}?";
	}

	public override string GetMarshaller(UhtProperty property)
	{
		return $"ObjectMarshaller<{GetManagedType(property)}>";
	}

	public override string GetNullValue(UhtProperty property)
	{
		return "null";
	}

	public override void ExportGenericParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string typeArgument, string nativeBufferName, bool needDeclaration)
	{
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		string marshaller = $"ObjectMarshaller<{typeArgument}>";
		string declaration = needDeclaration ? $"{typeArgument}? " : "";
		codeBuilder.AppendLine($"{declaration}{paramName} = {marshaller}.FromNative({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset);");
	}
}
