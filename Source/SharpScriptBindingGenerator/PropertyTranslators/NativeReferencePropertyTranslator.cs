using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

/// <summary>
/// This type of property is exported as a reference to a C++ type. They have handwritten C# bindings and no setter.
/// </summary>
public abstract class NativeReferencePropertyTranslator : PropertyTranslator
{
	protected NativeReferencePropertyTranslator(EPropertyUsageFlags supportedPropertyUsage) : base(supportedPropertyUsage)
	{
	}

	public override bool SupportsSetter => false;

	public override void ExportProperty(CodeBuilder codeBuilder, UhtProperty property, bool forClass, GetSetPair? getSetPair)
	{
		if (getSetPair != null)
		{
			base.ExportProperty(codeBuilder, property, forClass, getSetPair);
			return;
		}

		string protection = property.GetProtection();
		string managedType = GetPropManagedType(property);
		string propertyName = property.GetPropertyName();
		string backingPropertyName = NameMapper.PascalToCamelCase(propertyName);
		if (backingPropertyName.StartsWith("@"))
		{
			backingPropertyName = "_" + backingPropertyName.Substring(1);
		}
		else
		{
			backingPropertyName = "_" + backingPropertyName;
		}

		codeBuilder.AppendLine($"private {managedType}? {backingPropertyName};");
		codeBuilder.AppendLine();

		codeBuilder.AppendTooltip(property);
		ExportDeprecation(codeBuilder, property);
		codeBuilder.AppendLine($"{protection}{managedType} {propertyName}");
		using (new CodeBlock(codeBuilder)) // property body
		{
			codeBuilder.AppendLine("get");
			if (forClass)
			{
				using (new CodeBlock(codeBuilder)) // getter body
				{
					codeBuilder.AppendLine("ThrowIfNotValid();");
					codeBuilder.AppendLine($"return {backingPropertyName} ??= ");
					ExportPropertyGetter(codeBuilder, property, propertyName, forClass);
				}
			}
			else
			{
				codeBuilder.Append($" => {backingPropertyName} ??= ");
				ExportPropertyGetter(codeBuilder, property, propertyName, forClass);
			}
		}
	}

	public override void ExportPropertySetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		// Native reference type properties do not have a setter.
		throw new System.NotImplementedException();
	}

	public override void ExportStructProperty(CodeBuilder codeBuilder, UhtProperty property)
	{
		string protection = property.GetProtection();
		string managedType = GetParamManagedType(property);
		string propertyName = property.GetPropertyName();

		codeBuilder.AppendTooltip(property);
		ExportDeprecation(codeBuilder, property);
		codeBuilder.AppendLine($"{protection}{managedType} {propertyName};");
	}
}
