using System;
using System.Collections.Generic;
using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public static class PropertyTranslatorManager
{
	private static readonly UnsupportedPropertyTranslator UnsupportedPropertyTranslator;
	private static readonly Dictionary<Type, List<PropertyTranslator>?> RegisteredTranslators = new();

	/// <summary>
	/// Manually marked blittable structs.
	/// </summary>
	public static readonly HashSet<string> BlittableStructs = new();

	/// <summary>
	/// Types with manually written bindings, no need for generation.
	/// </summary>
	public static readonly HashSet<string> ManuallyExportedTypes = new();

	static PropertyTranslatorManager()
	{
		ManuallyExportedTypes.Add("EStreamingSourcePriority");
		ManuallyExportedTypes.Add("ETriggerEvent");

		UnsupportedPropertyTranslator = new UnsupportedPropertyTranslator();

		EnumPropertyTranslator enumPropertyTranslator = new();
		AddPropertyTranslator(typeof(UhtEnumProperty), enumPropertyTranslator);
		AddPropertyTranslator(typeof(UhtByteProperty), enumPropertyTranslator);

		AddBlittablePropertyTranslator(typeof(UhtInt8Property), "sbyte");
		AddBlittablePropertyTranslator(typeof(UhtIntProperty), "int");
		AddBlittablePropertyTranslator(typeof(UhtInt16Property), "short");
		AddBlittablePropertyTranslator(typeof(UhtInt64Property), "long");
		AddBlittablePropertyTranslator(typeof(UhtByteProperty), "byte");
		AddBlittablePropertyTranslator(typeof(UhtUInt16Property), "ushort");
		AddBlittablePropertyTranslator(typeof(UhtUInt32Property), "uint");
		AddBlittablePropertyTranslator(typeof(UhtUInt64Property), "ulong");
		AddPropertyTranslator(typeof(UhtFloatProperty), new FloatPropertyTranslator());
		AddBlittablePropertyTranslator(typeof(UhtDoubleProperty), "double");
		AddBlittablePropertyTranslator(typeof(UhtLargeWorldCoordinatesRealProperty), "double");

		MulticastDelegatePropertyTranslator multicastDelegatePropertyTranslator = new();
		AddPropertyTranslator(typeof(UhtMulticastDelegateProperty), multicastDelegatePropertyTranslator);
		AddPropertyTranslator(typeof(UhtMulticastSparseDelegateProperty), multicastDelegatePropertyTranslator);
		AddPropertyTranslator(typeof(UhtMulticastInlineDelegateProperty), multicastDelegatePropertyTranslator);
		AddPropertyTranslator(typeof(UhtDelegateProperty), new DelegatePropertyTranslator());

		AddPropertyTranslator(typeof(UhtBoolProperty), new BoolPropertyTranslator());
		AddPropertyTranslator(typeof(UhtStrProperty), new StringPropertyTranslator());
		AddPropertyTranslator(typeof(UhtNameProperty), new NamePropertyTranslator());
		AddPropertyTranslator(typeof(UhtTextProperty), new TextPropertyTranslator());
		AddPropertyTranslator(typeof(UhtFieldPathProperty), new FieldPathPropertyTranslator());

		ObjectPropertyTranslator objectPropertyTranslator = new();
		AddPropertyTranslator(typeof(UhtObjectPropertyBase), objectPropertyTranslator);
		AddPropertyTranslator(typeof(UhtObjectProperty), objectPropertyTranslator);
		AddPropertyTranslator(typeof(UhtObjectPtrProperty), objectPropertyTranslator);
		AddPropertyTranslator(typeof(UhtSoftObjectProperty), new SoftObjectPropertyTranslator());
		AddPropertyTranslator(typeof(UhtWeakObjectPtrProperty), objectPropertyTranslator);
		AddPropertyTranslator(typeof(UhtLazyObjectPtrProperty), new LazyObjectPropertyTranslator());

		AddPropertyTranslator(typeof(UhtClassProperty), new ClassPropertyTranslator());
		AddPropertyTranslator(typeof(UhtClassPtrProperty), objectPropertyTranslator);
		AddPropertyTranslator(typeof(UhtSoftClassProperty), new SoftClassPropertyTranslator());

		AddPropertyTranslator(typeof(UhtInterfaceProperty), new InterfacePropertyTranslator());

		AddPropertyTranslator(typeof(UhtArrayProperty), new ArrayPropertyTranslator());
		AddPropertyTranslator(typeof(UhtMapProperty), new MapPropertyTranslator());
		AddPropertyTranslator(typeof(UhtSetProperty), new SetPropertyTranslator());

		AddPropertyTranslator(typeof(UhtStructProperty), new BlittableStructPropertyTranslator());
		AddPropertyTranslator(typeof(UhtStructProperty), new StructPropertyTranslator());

		BlittableStructs.Add("SsBindingTestBlittableStruct");
		BlittableStructs.Add("SsTestNumericStruct");
	}

	public static PropertyTranslator GetTranslator(UhtProperty property)
	{
		if (!RegisteredTranslators.TryGetValue(property.GetType(), out var translators))
		{
			return UnsupportedPropertyTranslator;
		}

		foreach (PropertyTranslator propertyTranslator in translators!)
		{
			if (propertyTranslator.CanExport(property))
			{
				return propertyTranslator;
			}
		}

		return UnsupportedPropertyTranslator;
	}

	public static void AddBlittablePropertyTranslator(Type propertyType, string managedType)
	{
		if (RegisteredTranslators.TryGetValue(propertyType, out var translators))
		{
			translators!.Add(new BlittableTypePropertyTranslator(propertyType, managedType));
			return;
		}

		RegisteredTranslators.Add(propertyType, new List<PropertyTranslator> { new BlittableTypePropertyTranslator(propertyType, managedType) });
	}

	private static void AddPropertyTranslator(Type propertyClass, PropertyTranslator translator)
	{
		if (RegisteredTranslators.TryGetValue(propertyClass, out var translators))
		{
			translators!.Add(translator);
			return;
		}

		RegisteredTranslators.Add(propertyClass, new List<PropertyTranslator> { translator });
	}
}
