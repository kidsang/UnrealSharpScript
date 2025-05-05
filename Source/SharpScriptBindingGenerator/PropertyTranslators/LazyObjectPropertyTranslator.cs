using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class LazyObjectPropertyTranslator : ObjectPropertyTranslator
{
	public override bool IsNullable => false;

	public override bool ParamNeedInitialize => true;

	public override string GetPropManagedType(UhtProperty property)
	{
		return $"LazyObjectPtr<{GetManagedType(property)}>";
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		return $"LazyObjectPtr<{GetManagedType(property)}>";
	}

	public override string GetMarshaller(UhtProperty property)
	{
		return $"LazyObjectPtrMarshaller<{GetManagedType(property)}>";
	}

	public override string GetNullValue(UhtProperty property)
	{
		return "default";
	}
}
