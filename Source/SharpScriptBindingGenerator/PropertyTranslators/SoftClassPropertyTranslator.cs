using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class SoftClassPropertyTranslator : SimpleTypePropertyTranslator
{
	public SoftClassPropertyTranslator() : base(typeof(UhtSoftClassProperty))
	{
	}

	public override bool ParamNeedInitialize => true;

	public override bool CanExport(UhtProperty property)
	{
		UhtSoftClassProperty? softClassProperty = property as UhtSoftClassProperty;
		if (softClassProperty == null)
		{
			return false;
		}

		return BindingGenerator.ShouldExportTypes.Contains(softClassProperty.MetaClass!);
	}

	private string GetManagedType(UhtProperty property)
	{
		UhtSoftClassProperty softClassProperty = (UhtSoftClassProperty)property;
		return softClassProperty.MetaClass!.GetFullManagedName();
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		return $"SoftClassPtr<{GetManagedType(property)}>";
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		return $"SoftClassPtr<{GetManagedType(property)}>";
	}

	public override string GetMarshaller(UhtProperty property)
	{
		return $"SoftClassPtrMarshaller<{GetManagedType(property)}>";
	}
}
