using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class FloatPropertyTranslator : BlittableTypePropertyTranslator
{
	public FloatPropertyTranslator() : base(typeof(UhtFloatProperty), "float")
	{
	}

	public override string ConvertCppDefaultValue(string defaultValue, UhtProperty parameter)
	{
		return $"{defaultValue}f";
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		codeBuilder.AppendLine($"{GetParamManagedType(property)} {paramName} = {defaultValue}f;");
	}
}
