using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class BoolPropertyTranslator : SimpleTypePropertyTranslator
{
	public BoolPropertyTranslator() : base(typeof(UhtBoolProperty), "bool")
	{
	}

	public override string GetMarshaller(UhtProperty property)
	{
		return property.IsBitfield ? "BitfieldBoolMarshaller" : "BoolMarshaller";
	}

	public override void ExportStaticField(CodeBuilder codeBuilder, UhtProperty property)
	{
		base.ExportStaticField(codeBuilder, property);
		if (property.IsBitfield)
		{
			string propEngineName = property.EngineName;
			codeBuilder.AppendLine($"internal static readonly byte {propEngineName}_FieldMask;");
		}
	}

	public override void ExportStaticConstructor(CodeBuilder codeBuilder, UhtProperty property)
	{
		base.ExportStaticConstructor(codeBuilder, property);
		if (property.IsBitfield)
		{
			string propEngineName = property.EngineName;
			codeBuilder.AppendLine($"{propEngineName}_FieldMask = TypeInterop.GetBoolPropertyFieldMask({propEngineName}_NativeProp);");
		}
	}

	public override void ExportPropertyGetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		string marshaller = GetMarshaller(property);
		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string propEngineName = property.EngineName;
		codeBuilder.Append(property.IsBitfield
			? $"{marshaller}.FromNative({nativePtr} + {propEngineName}_Offset, {propEngineName}_FieldMask);"
			: $"{marshaller}.FromNative({nativePtr} + {propEngineName}_Offset);");
	}

	public override void ExportPropertySetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		string marshaller = GetMarshaller(property);
		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string propEngineName = property.EngineName;
		codeBuilder.Append(property.IsBitfield
			? $"{marshaller}.ToNative({nativePtr} + {propEngineName}_Offset, {propEngineName}_FieldMask, value);"
			: $"{marshaller}.ToNative({nativePtr} + {propEngineName}_Offset, value);");
	}
}
