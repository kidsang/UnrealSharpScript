using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class StringPropertyTranslator : SimpleTypePropertyTranslator
{
	public StringPropertyTranslator() : base(typeof(UhtStrProperty), "string")
	{
	}

	public override string GetMarshaller(UhtProperty property)
	{
		return "StringMarshaller";
	}

	public override string GetNullValue(UhtProperty property)
	{
		return "\"\"";
	}

	public override string ConvertCppDefaultValue(string defaultValue, UhtProperty parameter)
	{
		return $"\"{defaultValue}\"";
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		codeBuilder.AppendLine($"{ManagedType} {paramName} = \"{defaultValue}\";");
	}
}
