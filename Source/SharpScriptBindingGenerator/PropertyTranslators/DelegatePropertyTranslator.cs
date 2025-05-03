using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class DelegatePropertyTranslator : NativeReferencePropertyTranslator
{
	private const EPropertyUsageFlags SupportedUsages = EPropertyUsageFlags.Property
														| EPropertyUsageFlags.Parameter
														| EPropertyUsageFlags.InnerValue;

	public DelegatePropertyTranslator() : base(SupportedUsages)
	{
	}

	public override bool CanExport(UhtProperty property)
	{
		UhtDelegateProperty delegateProperty = (UhtDelegateProperty)property;
		return BindingGenerator.ShouldExportTypes.Contains(delegateProperty.Function)
				&& GeneratorUtilities.CanExportParameters(delegateProperty.Function);
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		string managedType = GetParamManagedType(property);
		return $"Delegate<{managedType}>";
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		UhtDelegateProperty delegateProperty = (UhtDelegateProperty)property;
		UhtFunction function = delegateProperty.Function;
		return $"{function.GetDelegateNamespace()}.F{function.StrippedFunctionName}";
	}

	public override string GetMarshaller(UhtProperty property)
	{
		string managedType = GetParamManagedType(property);
		return $"DelegateMarshaller<{managedType}>";
	}

	public override string GetNullValue(UhtProperty property)
	{
		// Delegate type properties do not have default values.
		throw new System.NotImplementedException();
	}

	public override string ConvertCppDefaultValue(string defaultValue, UhtProperty parameter)
	{
		// Delegate type properties do not have default values.
		throw new System.NotImplementedException();
	}

	public override void ExportStaticConstructor(CodeBuilder codeBuilder, UhtProperty property)
	{
		base.ExportStaticConstructor(codeBuilder, property);

		UhtDelegateProperty delegateProperty = (UhtDelegateProperty)property;
		UhtFunction function = delegateProperty.Function;
		string invokerName = $"{function.GetNamespace()}.{function.GetDelegateInvokerName()}";
		string propEngineName = property.EngineName;
		codeBuilder.AppendLine($"{invokerName}.Initialize({propEngineName}_NativeProp);");
	}

	public override void ExportPropertyGetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass)
	{
		string nativePtr = forClass ? "NativeObject" : "nativePtr";
		string propEngineName = property.EngineName;
		codeBuilder.Append($"new({nativePtr} + {propEngineName}_Offset);");
	}

	public override void ExportParamToNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName)
	{
		string marshaller = GetMarshaller(property);
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		codeBuilder.AppendLine($"{marshaller}.ToNative({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset, {paramName});");
	}

	public override void ExportParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName, bool needDeclaration)
	{
		string marshaller = GetMarshaller(property);
		string funcEngineName = function.StrippedFunctionName;
		string paramEngineName = property.EngineName;
		string declaration = needDeclaration ? $"{GetParamManagedType(property)} " : "";
		codeBuilder.AppendLine($"{declaration}{paramName} = {marshaller}.FromNative({nativeBufferName} + {funcEngineName}_{paramEngineName}_Offset);");
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		// Delegate type properties do not have default values.
		throw new System.NotImplementedException();
	}

	public override void ExportStructPropertyFromManaged(CodeBuilder codeBuilder, UhtProperty property)
	{
		string propertyName = property.GetPropertyName();
		codeBuilder.AppendLine($"{propertyName}.FromManaged(value.{propertyName});");
	}
}
