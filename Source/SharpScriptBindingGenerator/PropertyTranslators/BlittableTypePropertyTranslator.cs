using System;
using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class BlittableTypePropertyTranslator : SimpleTypePropertyTranslator
{
	public BlittableTypePropertyTranslator(Type propertyType, string managedType) : base(propertyType, managedType)
	{
	}

	public override string GetMarshaller(UhtProperty property)
	{
		return $"BlittableMarshaller<{GetPropManagedType(property)}>";
	}
}
