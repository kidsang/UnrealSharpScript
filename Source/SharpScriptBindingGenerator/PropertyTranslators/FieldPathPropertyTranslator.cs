using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class FieldPathPropertyTranslator : SimpleTypePropertyTranslator
{
	public FieldPathPropertyTranslator() : base(typeof(UhtFieldPathProperty), "FieldPath")
	{
	}

	public override bool ExportDefaultParameter => false;

	public override string GetMarshaller(UhtProperty property)
	{
		return "FieldPathMarshaller";
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		codeBuilder.AppendLine(defaultValue == "None" ? $"FieldPath {paramName} = default;" : $"FieldPath {paramName} = new(\"{defaultValue}\");");
	}
}
