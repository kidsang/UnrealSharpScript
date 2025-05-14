using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class TextPropertyTranslator : SimpleTypePropertyTranslator
{
	public TextPropertyTranslator() : base(typeof(UhtTextProperty), "FText")
	{
	}

	public override bool ExportDefaultParameter => false;

	public override bool ParamNeedInitialize => true;

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

		codeBuilder.AppendLine(defaultValue == "\"\"" ? $"FText {paramName} = FText.None;" : $"FText {paramName} = new({defaultValue});");
	}
}
