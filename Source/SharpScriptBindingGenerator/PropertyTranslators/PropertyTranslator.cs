using System;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.PropertyTranslators;

[Flags]
public enum EPropertyUsageFlags : byte
{
	None = 0x00,
	Property = 0x01,
	Parameter = 0x02,
	ReturnValue = 0x04,
	InnerKey = 0x08,
	InnerValue = 0x10,
	Any = 0xFF,
};

public abstract class PropertyTranslator
{
	private readonly EPropertyUsageFlags _supportedPropertyUsage;

	protected const EPropertyUsageFlags ContainerSupportedUsages = EPropertyUsageFlags.Property
																	| EPropertyUsageFlags.Parameter
																	| EPropertyUsageFlags.ReturnValue;

	public bool IsSupportedAsProperty => _supportedPropertyUsage.HasFlag(EPropertyUsageFlags.Property);
	public bool IsSupportedAsParameter => _supportedPropertyUsage.HasFlag(EPropertyUsageFlags.Parameter);
	public bool IsSupportedAsReturnValue => _supportedPropertyUsage.HasFlag(EPropertyUsageFlags.ReturnValue);
	public bool IsSupportedAsInnerKey => _supportedPropertyUsage.HasFlag(EPropertyUsageFlags.InnerKey);
	public bool IsSupportedAsInnerValue => _supportedPropertyUsage.HasFlag(EPropertyUsageFlags.InnerValue);

	// Can we set this property?
	public virtual bool SupportsSetter => true;

	// Can we export this property as function default parameter?
	public virtual bool ExportDefaultParameter => true;

	// Is this property nullable?
	public virtual bool IsNullable => false;

	// Can we export this property?
	public abstract bool CanExport(UhtProperty property);

	// Get the managed type for this property
	// Example: "int" for a property of type "int32"
	public abstract string GetPropManagedType(UhtProperty property);

	// Get the managed type for this property when using as a parameter.
	public abstract string GetParamManagedType(UhtProperty property);

	// Get the generic managed type for this property when using as a parameter.
	public virtual string GetGenericParamManagedType(UhtProperty property, string typeArgument)
	{
		return GetParamManagedType(property);
	}

	// Get the marshaller for this property to marshal back and forth between C++ and C#
	public abstract string GetMarshaller(UhtProperty property);

	// Get the null value for this property
	public abstract string GetNullValue(UhtProperty property);

	// Convert a C++ default value to a C# default value
	// Example: "0.0f" for a float property
	public abstract string ConvertCppDefaultValue(string defaultValue, UhtProperty parameter);

	protected PropertyTranslator(EPropertyUsageFlags supportedPropertyUsage)
	{
		_supportedPropertyUsage = supportedPropertyUsage;
	}

	/// <summary>
	/// Export static variables required for the property.
	/// </summary>
	public virtual void ExportStaticField(CodeBuilder codeBuilder, UhtProperty property)
	{
		string propEngineName = property.EngineName;
		codeBuilder.AppendLine($"internal static readonly IntPtr {propEngineName}_NativeProp;");
		codeBuilder.AppendLine($"internal static readonly int {propEngineName}_Offset;");
	}

	/// <summary>
	/// Export code for the property in the static constructor.
	/// </summary>
	public virtual void ExportStaticConstructor(CodeBuilder codeBuilder, UhtProperty property)
	{
		string propEngineName = property.EngineName;
		codeBuilder.AppendLine($"{propEngineName}_NativeProp = propIter.FindNext(\"{propEngineName}\");");
		codeBuilder.AppendLine($"{propEngineName}_Offset = TypeInterop.GetPropertyOffset({propEngineName}_NativeProp);");
	}

	/// <summary>
	/// Export property Getter/Setter.
	/// </summary>
	public virtual void ExportProperty(CodeBuilder codeBuilder, UhtProperty property, bool forClass)
	{
		string protection = property.GetProtection();
		string managedType = GetPropManagedType(property);
		string propertyName = property.GetPropertyName();

		bool isReadWrite = property.IsReadWrite() || !forClass;
		bool isEditDefaultsOnly = property.IsEditDefaultsOnly();
		bool hasSetter = SupportsSetter && (isReadWrite || isEditDefaultsOnly);
		string setterOperation = isEditDefaultsOnly && !isReadWrite ? "init" : "set";

		codeBuilder.AppendTooltip(property);
		codeBuilder.AppendLine($"{protection}{managedType} {propertyName}");
		using (new CodeBlock(codeBuilder)) // property body
		{
			codeBuilder.AppendLine("get");
			if (forClass)
			{
				using (new CodeBlock(codeBuilder)) // getter body
				{
					codeBuilder.AppendLine("ThrowIfNotValid();");
					codeBuilder.AppendLine("return ");
					ExportPropertyGetter(codeBuilder, property, propertyName, forClass);
				}
			}
			else
			{
				codeBuilder.Append($" => ");
				ExportPropertyGetter(codeBuilder, property, propertyName, forClass);
			}

			if (hasSetter)
			{
				codeBuilder.AppendLine(setterOperation);
				if (forClass)
				{
					using var setterBody = new CodeBlock(codeBuilder);
					codeBuilder.AppendLine("ThrowIfNotValid();");
					codeBuilder.AppendLineAndIndent();
					ExportPropertySetter(codeBuilder, property, propertyName, forClass);
				}
				else
				{
					codeBuilder.Append($" => ");
					ExportPropertySetter(codeBuilder, property, propertyName, forClass);
				}
			}
		}
	}

	/// <summary>
	/// Export the getter function body for the property.
	/// </summary>
	public abstract void ExportPropertyGetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass);

	/// <summary>
	/// Export the setter function body for the property.
	/// </summary>
	public abstract void ExportPropertySetter(CodeBuilder codeBuilder, UhtProperty property, string propertyManagedName, bool forClass);

	/// <summary>
	/// Wrap C# function parameters into C++ function parameters.
	/// </summary>
	public abstract void ExportParamToNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName);

	/// <summary>
	/// Wrap C++ function return value into C# function return value.
	/// </summary>
	public abstract void ExportParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string nativeBufferName, bool needDeclaration);

	/// <summary>
	/// Wrap generic C# function parameters into C++ function parameters.
	/// </summary>
	public virtual void ExportGenericParamToNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string typeArgument, string nativeBufferName)
	{
		ExportParamToNative(codeBuilder, function, property, paramName, nativeBufferName);
	}

	/// <summary>
	/// Wrap C++ function return value into generic C# function return value.
	/// </summary>
	public virtual void ExportGenericParamFromNative(CodeBuilder codeBuilder, UhtFunction function, UhtProperty property, string paramName, string typeArgument, string nativeBufferName, bool needDeclaration)
	{
		ExportParamFromNative(codeBuilder, function, property, paramName, nativeBufferName, needDeclaration);
	}

	/// <summary>
	/// Export C++ function parameter default value as C# variable.
	/// </summary>
	public abstract void ExportCppDefaultParameterAsLocalVariable(CodeBuilder codeBuilder, UhtProperty property, string paramName, string defaultValue);

	/// <summary>
	/// Export Blittable struct member variable.
	/// </summary>
	public virtual void ExportBlittableStructProperty(CodeBuilder codeBuilder, UhtProperty property, string structName)
	{
		string protection = property.GetProtection();
		string managedType = GetPropManagedType(property);
		string propertyName = property.GetPropertyName();

		codeBuilder.AppendTooltip(property);
		codeBuilder.AppendLine($"{protection}unsafe {managedType} {propertyName}");
		using (new CodeBlock(codeBuilder)) // property body
		{
			codeBuilder.AppendLine($"get => (*({structName}*)nativePtr).{propertyName};");
			codeBuilder.AppendLine($"set => (*({structName}*)nativePtr).{propertyName} = value;");
		}
	}

	/// <summary>
	/// Export struct member variables.
	/// </summary>
	public virtual void ExportStructProperty(CodeBuilder codeBuilder, UhtProperty property)
	{
		string protection = property.GetProtection();
		string managedType = GetPropManagedType(property);
		string propertyName = property.GetPropertyName();

		codeBuilder.AppendTooltip(property);
		codeBuilder.AppendLine($"{protection}{managedType} {propertyName};");
	}

	/// <summary>
	/// Export property assignment in the ToManaged method of struct reference.
	/// </summary>
	public virtual void ExportStructPropertyToManaged(CodeBuilder codeBuilder, UhtProperty property)
	{
		string propertyName = property.GetPropertyName();
		codeBuilder.AppendLine($"{propertyName} = {propertyName},");
	}

	/// <summary>
	/// Export property assignment in the FromManaged method of struct reference.
	/// </summary>
	public virtual void ExportStructPropertyFromManaged(CodeBuilder codeBuilder, UhtProperty property)
	{
		string propertyName = property.GetPropertyName();
		codeBuilder.AppendLine($"{propertyName} = value.{propertyName};");
	}

	/// <summary>
	/// Export static variables required for function parameters.
	/// </summary>
	public virtual void ExportParamsStaticField(CodeBuilder codeBuilder, UhtProperty property, UhtFunction function, string modifier)
	{
		string funcName = function.StrippedFunctionName;
		string propName = property.EngineName;
		codeBuilder.AppendLine($"{modifier} int {funcName}_{propName}_Offset;");
	}

	/// <summary>
	/// Export code for function parameters in the static constructor of the class.
	/// </summary>
	public virtual void ExportParamsStaticConstructor(CodeBuilder codeBuilder, UhtProperty property, UhtFunction function)
	{
		string funcName = function.StrippedFunctionName;
		string propName = property.EngineName;
		codeBuilder.AppendLine($"{funcName}_{propName}_Offset = {funcName}_PropIter.FindNextAndGetOffset(\"{propName}\");");
	}

	/// <summary>
	/// Get the default value of C++ function parameters from meta data.
	/// </summary>
	/// <returns>Returns the default value of C++ function parameters from meta data, returns null if there is no default value.</returns>
	public string? GetCppDefaultValue(UhtFunction function, UhtProperty parameter)
	{
		string metaDataKey = $"CPP_Default_{parameter.EngineName}";
		function.TryGetMetadata(metaDataKey, out var defaultValue);
		return defaultValue;
	}
}
