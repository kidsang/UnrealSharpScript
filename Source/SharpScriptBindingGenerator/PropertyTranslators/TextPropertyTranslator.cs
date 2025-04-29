using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class TextPropertyTranslator : SimpleTypePropertyTranslator
{
	public TextPropertyTranslator() : base(typeof(UhtTextProperty), "Text")
	{
	}

	public override bool ExportDefaultParameter => false;

	public override string GetMarshaller(UhtProperty property)
	{
		return "TextMarshaller";
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		if (defaultValue.StartsWith("INVTEXT("))
		{
			int length = defaultValue.Length - 9;
			defaultValue = defaultValue.Substring(8, length);
		}

		codeBuilder.AppendLine(defaultValue == "\"\"" ? $"Text {paramName} = Text.None;" : $"Text {paramName} = new({defaultValue});");
	}
}
