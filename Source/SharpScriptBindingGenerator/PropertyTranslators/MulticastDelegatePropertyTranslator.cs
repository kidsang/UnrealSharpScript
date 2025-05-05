using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

public class MulticastDelegatePropertyTranslator : NativeReferencePropertyTranslator
{
	public MulticastDelegatePropertyTranslator() : base(EPropertyUsageFlags.Property)
	{
	}

	public override bool ParamNeedInitialize => true;

	public override bool CanExport(UhtProperty property)
	{
		UhtMulticastDelegateProperty delegateProperty = (UhtMulticastDelegateProperty)property;
		return BindingGenerator.ShouldExportTypes.Contains(delegateProperty.Function)
				&& GeneratorUtilities.CanExportParameters(delegateProperty.Function);
	}

	public override string GetPropManagedType(UhtProperty property)
	{
		string managedType = GetParamManagedType(property);
		return $"MulticastDelegate<{managedType}>";
	}

	public override string GetParamManagedType(UhtProperty property)
	{
		UhtMulticastDelegateProperty delegateProperty = (UhtMulticastDelegateProperty)property;
		UhtFunction function = delegateProperty.Function;
		return $"{function.GetDelegateNamespace()}.F{function.StrippedFunctionName}";
	}

	public override string GetMarshaller(UhtProperty property)
	{
		// Multicast delegates do not have a Marshaller.
		throw new System.NotImplementedException();
	}

	public override string GetNullValue(UhtProperty property)
	{
		// Multicast delegates do not have a default value.
		throw new System.NotImplementedException();
	}

	public override string ConvertCppDefaultValue(string defaultValue, UhtProperty parameter)
	{
		// Multicast delegates do not have a default value.
		throw new System.NotImplementedException();
	}

	public override void ExportStaticConstructor(CodeBuilder codeBuilder, UhtProperty property)
	{
		base.ExportStaticConstructor(codeBuilder, property);

		UhtMulticastDelegateProperty delegateProperty = (UhtMulticastDelegateProperty)property;
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
		// Multicast delegates cannot be used as function parameters.
		throw new System.NotImplementedException();
	}

	public override void ExportParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName, bool needDeclaration)
	{
		// Multicast delegates cannot be used as function parameters.
		throw new System.NotImplementedException();
	}

	public override void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue)
	{
		// Multicast delegates do not have default values.
		throw new System.NotImplementedException();
	}
}
