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
			string propSourceName = property.SourceName;
			codeBuilder.AppendLine($"internal static readonly byte {propSourceName}_FieldMask;");
		}
	}

	public override void ExportStaticConstructor(CodeBuilder codeBuilder, UhtProperty property)
	{
		base.ExportStaticConstructor(codeBuilder, property);
		if (property.IsBitfield)
		{
			string propSourceName = property.SourceName;
			codeBuilder.AppendLine($"{propSourceName}_FieldMask = TypeInterop.GetBoolPropertyFieldMask({propSourceName}_NativeProp);");
		}
	}

	public override void ExportPropertyGetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		string marshaller = GetMarshaller(property);
		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string propSourceName = property.SourceName;
		codeBuilder.Append(property.IsBitfield
			? $"{marshaller}.FromNative({nativePtr} + {propSourceName}_Offset, {propSourceName}_FieldMask);"
			: $"{marshaller}.FromNative({nativePtr} + {propSourceName}_Offset);");
	}

	public override void ExportPropertySetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		string marshaller = GetMarshaller(property);
		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string propSourceName = property.SourceName;
		codeBuilder.Append(property.IsBitfield
			? $"{marshaller}.ToNative({nativePtr} + {propSourceName}_Offset, {propSourceName}_FieldMask, value);"
			: $"{marshaller}.ToNative({nativePtr} + {propSourceName}_Offset, value);");
	}
}
