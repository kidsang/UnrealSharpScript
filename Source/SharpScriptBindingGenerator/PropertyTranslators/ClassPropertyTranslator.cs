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
		UhtClassProperty? objectProperty = property as UhtClassProperty;
		if (objectProperty == null)
		{
			return false;
		}

		return BindingGenerator.ShouldExportTypes.Contains(objectProperty.MetaClass!);
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

	public override string GetMarshaller(UhtProperty property)
	{
		UhtClassProperty classProperty = (UhtClassProperty)property;
		string managedType = classProperty.MetaClass!.GetFullManagedName();
		return $"SubclassOfMarshaller<{managedType}>";
	}
}
