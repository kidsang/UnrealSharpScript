using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class NamePropertyTranslator : BlittableTypePropertyTranslator
{
	public NamePropertyTranslator() : base(typeof(UhtNameProperty), "Name")
	{
	}

	public override bool ExportDefaultParameter => false;

	public override string GetMarshaller(UhtProperty property)
	{
		return "NameMarshaller";
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		codeBuilder.AppendLine(defaultValue == "None" ? $"Name {paramName} = Name.None;" : $"Name {paramName} = new(\"{defaultValue}\");");
	}
}
