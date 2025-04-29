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

	public override string GetMarshaller(UhtProperty property)
	{
		return $"ObjectMarshaller<{GetManagedType(property)}>";
	}

	public override string GetNullValue(UhtProperty property)
	{
		return "null";
	}
}
