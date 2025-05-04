using System;
using System.Collections.Generic;
using EpicGames.Core;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.PropertyTranslators;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.Exporters;

public enum EOverloadMode
{
	AllowOverloads,
	SuppressOverloads,
	SuppressDefaultValues, // this will also supress overloads
}

public enum EFunctionProtectionMode
{
	UseUFunctionProtection,
	OverrideWithInternal,
	OverrideWithPublic,
}

public enum EBlueprintVisibility
{
	Call,
	Event,
	GetterSetter,
}

public struct FunctionOverload
{
	public string ParamString;
	public string ParamStringCall;
	public string CSharpParamName;
	public string CppDefaultValue;
	public PropertyTranslator Translator;
	public UhtProperty Parameter;
}

public enum EExtensionMethodType
{
	Normal,
	Autocast,
	Operator,
}

public struct ExtensionMethod
{
	public EExtensionMethodType MethodType;
	public string MethodName;
	public UhtFunction Function;
}

public class FunctionExporter
{
	protected UhtFunction Function;
	protected EFunctionProtectionMode ProtectionMode;
	protected EOverloadMode OverloadMode;
	protected EBlueprintVisibility BlueprintVisibility;
	protected bool SuppressGeneric;

	protected string FunctionName;
	protected string Protection;
	protected string Modifiers;
	protected string ParamStringCall;
	protected string ParamStringWithDefault;
	protected string InvokeFunction;
	protected string InvokeArguments;

	protected bool IsBlueprintEvent => BlueprintVisibility == EBlueprintVisibility.Event;

	protected List<PropertyTranslator> ParamTranslators = new();

	protected List<KeyValuePair<UhtClass, string>>? ParamGenericConstraints;

	protected Dictionary<UhtProperty, string>? ParamGenericTypeArguments;

	protected string? OutputGenericTypeArgument;

	protected HashSet<string>? DynamicOutputParams;

	protected List<FunctionOverload> Overloads = new();

	public FunctionExporter(UhtFunction function, EFunctionProtectionMode protectionMode, EOverloadMode overloadMode, EBlueprintVisibility blueprintVisibility, bool suppressGeneric = false)
	{
		Function = function;
		ProtectionMode = protectionMode;
		OverloadMode = overloadMode;
		BlueprintVisibility = blueprintVisibility;
		SuppressGeneric = suppressGeneric;

		FunctionName = function.GetScriptName();
		Protection = CalculateProtection();

		Modifiers = "";
		if (function.HasAnyFlags(EFunctionFlags.Static))
		{
			Modifiers += "static ";
			InvokeFunction = "InvokeStaticFunctionCall";
			InvokeArguments = $"NativeType, {Function.StrippedFunctionName}_NativeFunc, ";
		}
		else
		{
			if (IsBlueprintEvent)
			{
				Modifiers += "virtual ";
			}

			if (function.IsInterfaceFunction())
			{
				Protection = "public ";
				Modifiers = "";
			}

			InvokeFunction = "InvokeFunctionCall";
			InvokeArguments = $"{Function.StrippedFunctionName}_NativeFunc, ";
		}

		// handle conflict functions
		switch (FunctionName)
		{
			// override object.ToString
			case "ToString":
			{
				if (function.Children.Count == 1)
				{
					if (function.ReturnProperty is UhtStrProperty
						&& !Modifiers.Contains("static")
						&& !Modifiers.Contains("virtual"))
					{
						Modifiers += "override ";
					}
					else
					{
						Modifiers += "new ";
					}
				}

				break;
			}
			// override ObjectBase.IsValid()
			case "IsValid":
			{
				if (function is { Outer: UhtClass, Children.Count: 1, ReturnProperty: UhtBoolProperty })
				{
					Modifiers += "new ";
				}

				break;
			}
		}

		CollectGenericInfos();

		bool hasDefaultParameters = false;
		string paramString = "";
		ParamStringCall = "";
		ParamStringWithDefault = "";

		foreach (var uhtType in function.Children)
		{
			var param = (UhtProperty)uhtType;
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(param);
			ParamTranslators.Add(translator);

			if (param.HasAnyFlags(EPropertyFlags.ReturnParm))
			{
				continue;
			}

			if (paramString.Length > 0)
			{
				paramString = $"{paramString}, ";
				ParamStringCall = $"{ParamStringCall}, ";
			}

			string refQualifier = GetRefQualifier(param);
			string paramName = param.GetParameterName();
			string paramManagedType = translator.GetParamManagedType(param);
			if (!SuppressGeneric
				&& ParamGenericTypeArguments != null
				&& ParamGenericTypeArguments.TryGetValue(param, out var typeArgument))
			{
				paramManagedType = translator.GetGenericParamManagedType(param, typeArgument);
			}

			ParamStringCall = $"{ParamStringCall}{refQualifier}{paramName}";
			paramString = $"{paramString}{refQualifier}{paramManagedType} {paramName}";

			string? cppDefaultValue = OverloadMode != EOverloadMode.SuppressDefaultValues
				? translator.GetCppDefaultValue(function, param)
				: null;
			hasDefaultParameters = hasDefaultParameters || cppDefaultValue != null;

			string? csharpDefaultValue = null;
			if (hasDefaultParameters && Overloads.Count == 0)
			{
				if (string.IsNullOrEmpty(cppDefaultValue) || cppDefaultValue == "None")
				{
					csharpDefaultValue = translator.GetNullValue(param);
				}
				else if (translator.ExportDefaultParameter)
				{
					csharpDefaultValue = translator.ConvertCppDefaultValue(cppDefaultValue, param);
				}
			}

			if (!string.IsNullOrEmpty(csharpDefaultValue))
			{
				if (ParamStringWithDefault.Length > 0)
				{
					ParamStringWithDefault = $"{ParamStringWithDefault}, ";
				}

				ParamStringWithDefault = $"{ParamStringWithDefault}{refQualifier}{paramManagedType} {paramName} = {csharpDefaultValue}";
			}
			else if (hasDefaultParameters && OverloadMode == EOverloadMode.AllowOverloads)
			{
				FunctionOverload overload = new FunctionOverload
				{
					ParamString = ParamStringWithDefault,
					ParamStringCall = ParamStringCall,
					CSharpParamName = paramName,
					CppDefaultValue = cppDefaultValue ?? string.Empty,
					Translator = translator,
					Parameter = param,
				};

				Overloads.Add(overload);
				ParamStringWithDefault = paramString;
			}
			else
			{
				ParamStringWithDefault = paramString;
			}
		}
	}

	private string CalculateProtection()
	{
		switch (ProtectionMode)
		{
			case EFunctionProtectionMode.UseUFunctionProtection:
				if (Function.HasAnyFlags(EFunctionFlags.Public))
				{
					return "public ";
				}

				if (Function.HasAnyFlags(EFunctionFlags.Protected) || Function.HasMetadata("BlueprintProtected"))
				{
					return "protected ";
				}

				return "public ";
			case EFunctionProtectionMode.OverrideWithInternal:
				return "internal ";
			case EFunctionProtectionMode.OverrideWithPublic:
			default:
				return "public ";
		}
	}

	private void CollectGenericInfos()
	{
		if (SuppressGeneric)
		{
			return;
		}

		// Collect parameters of type "TSubclassOf" in the function parameters and convert them into type arguments.
		List<UhtClassProperty>? genericParams = null;
		foreach (var uhtType in Function.Children)
		{
			var param = (UhtProperty)uhtType;
			if (param.HasAnyFlags(EPropertyFlags.ReturnParm)
				|| param.HasAnyFlags(EPropertyFlags.OutParm) && !param.HasAnyFlags(EPropertyFlags.ReferenceParm))
			{
				continue;
			}

			if (param is UhtClassProperty classProperty)
			{
				genericParams ??= new();
				genericParams.Add(classProperty);
			}
		}

		if (genericParams == null)
		{
			return;
		}

		ParamGenericConstraints ??= new();
		ParamGenericTypeArguments ??= new();
		if (genericParams.Count == 1)
		{
			var param = genericParams[0];
			ParamGenericConstraints.Add(new(param.MetaClass!, "T"));
			ParamGenericTypeArguments[param] = "T";
		}
		else
		{
			for (int i = 0; i < genericParams.Count; i++)
			{
				var param = genericParams[i];
				ParamGenericConstraints.Add(new(param.MetaClass!, $"T{i + 1}"));
				ParamGenericTypeArguments[param] = $"T{i + 1}";
			}
		}

		// Based on the "DeterminesOutputType" and "DynamicOutputParam" metadata,
		// change output parameters and the return value into the corresponding type argument.
		if (Function.TryGetMetadata("DeterminesOutputType", out string? determinesOutputType))
		{
			foreach (var pair in ParamGenericTypeArguments)
			{
				if (pair.Key.EngineName == determinesOutputType)
				{
					OutputGenericTypeArgument = pair.Value;
					break;
				}
			}

			if (OutputGenericTypeArgument != null)
			{
				if (Function.TryGetMetadata("DynamicOutputParam", out string? dynamicOutputParam))
				{
					DynamicOutputParams ??= new();
					foreach (var paramName in dynamicOutputParam.Split(","))
					{
						DynamicOutputParams.Add(paramName.Trim());
					}

					foreach (var uhtType in Function.Children)
					{
						var param = (UhtProperty)uhtType;
						if (param.HasAnyFlags(EPropertyFlags.OutParm)
							&& !param.HasAnyFlags(EPropertyFlags.ReferenceParm | EPropertyFlags.ReturnParm)
							&& DynamicOutputParams.Contains(param.EngineName))
						{
							ParamGenericTypeArguments[param] = OutputGenericTypeArgument;
						}
					}
				}
				else if (Function.ReturnProperty is not null)
				{
					ParamGenericTypeArguments[Function.ReturnProperty] = OutputGenericTypeArgument;
				}
			}
		}
	}

	public static void ExportFunction(CodeBuilder codeBuilder, UhtFunction function)
	{
		EFunctionProtectionMode protectionMode = EFunctionProtectionMode.UseUFunctionProtection;
		EOverloadMode overloadMode = EOverloadMode.AllowOverloads;
		EBlueprintVisibility blueprintVisibility = EBlueprintVisibility.Call;

		FunctionExporter exporter = new FunctionExporter(function, protectionMode, overloadMode, blueprintVisibility);
		exporter.ExportOverloads(codeBuilder);
		exporter.ExportFunction(codeBuilder);
	}

	public static FunctionExporter ExportDelegateSignature(CodeBuilder codeBuilder, UhtFunction function)
	{
		EFunctionProtectionMode protectionMode = EFunctionProtectionMode.UseUFunctionProtection;
		EOverloadMode overloadMode = EOverloadMode.SuppressOverloads;
		EBlueprintVisibility blueprintVisibility = EBlueprintVisibility.Call;
		bool suppressGeneric = true;

		FunctionExporter exporter = new FunctionExporter(function, protectionMode, overloadMode, blueprintVisibility, suppressGeneric)
		{
			FunctionName = $"F{function.GetScriptName()}",
			InvokeArguments = "",
			InvokeFunction = function.HasAnyFlags(EFunctionFlags.MulticastDelegate)
				? "instance.ProcessMulticastDelegate"
				: "instance.ProcessDelegate",
		};

		exporter.ExportDelegateSignature(codeBuilder);
		return exporter;
	}

	public static void ExportInterfaceFunction(CodeBuilder codeBuilder, UhtFunction function)
	{
		EFunctionProtectionMode protectionMode = EFunctionProtectionMode.OverrideWithPublic;
		EOverloadMode overloadMode = EOverloadMode.SuppressOverloads;
		EBlueprintVisibility blueprintVisibility = EBlueprintVisibility.Call;

		FunctionExporter exporter = new FunctionExporter(function, protectionMode, overloadMode, blueprintVisibility);
		exporter.ExportInterfaceSignature(codeBuilder);
	}

	public static void ExportExtensionMethod(CodeBuilder codeBuilder, string hostClassName, ExtensionMethod extensionMethod)
	{
		EFunctionProtectionMode protectionMode = EFunctionProtectionMode.OverrideWithPublic;
		EOverloadMode overloadMode = extensionMethod.MethodType switch
		{
			EExtensionMethodType.Normal => EOverloadMode.AllowOverloads,
			EExtensionMethodType.Operator => EOverloadMode.SuppressDefaultValues,
			_ => EOverloadMode.SuppressOverloads
		};
		EBlueprintVisibility blueprintVisibility = EBlueprintVisibility.Call;
		bool suppressGeneric = true;

		FunctionExporter exporter = new FunctionExporter(extensionMethod.Function, protectionMode, overloadMode, blueprintVisibility, suppressGeneric);
		exporter.ExportExtensionOverloads(codeBuilder, hostClassName, extensionMethod);
		exporter.ExportExtensionFunction(codeBuilder, hostClassName, extensionMethod);
	}

	public void ExportDelegateInvoker(CodeBuilder codeBuilder)
	{
		ExportDelegateInvokerSignature(codeBuilder);
		using (new CodeBlock(codeBuilder)) // function body
		{
			ExportInvoke(codeBuilder);
		}
	}

	private void ExportOverloads(CodeBuilder codeBuilder)
	{
		foreach (FunctionOverload overload in Overloads)
		{
			ExportDeprecation(codeBuilder);

			string returnType = "void";
			string returnStatement = "";
			if (Function.ReturnProperty != null)
			{
				PropertyTranslator returnValueTranslator = PropertyTranslatorManager.GetTranslator(Function.ReturnProperty);
				returnType = returnValueTranslator.GetParamManagedType(Function.ReturnProperty);
				returnStatement = "return ";
			}

			codeBuilder.AppendLine($"{Protection}{Modifiers}unsafe {returnType} {FunctionName}({overload.ParamString})");

			using (new CodeBlock(codeBuilder)) // function body
			{
				overload.Translator.ExportCppDefaultParameterAsLocalVariable(codeBuilder, overload.Parameter, overload.CSharpParamName, overload.CppDefaultValue);
				codeBuilder.AppendLine($"{returnStatement}{FunctionName}({overload.ParamStringCall});");
			}

			codeBuilder.AppendLine();
		}
	}

	private void ExportExtensionOverloads(CodeBuilder codeBuilder, string hostClassName, ExtensionMethod extensionMethod)
	{
		foreach (FunctionOverload overload in Overloads)
		{
			ExportDeprecation(codeBuilder);

			string returnType = "void";
			string returnStatement = "";
			if (Function.ReturnProperty != null)
			{
				PropertyTranslator returnValueTranslator = PropertyTranslatorManager.GetTranslator(Function.ReturnProperty);
				returnType = returnValueTranslator.GetParamManagedType(Function.ReturnProperty);
				returnStatement = "return ";
			}

			string paramString = GetExtensionMethodParamString(overload.ParamString);
			codeBuilder.AppendLine($"{Protection}{returnType} {extensionMethod.MethodName}({paramString})");

			using (new CodeBlock(codeBuilder)) // function body
			{
				string paramStringCall = GetExtensionMethodParamStringCall(overload.ParamStringCall);
				int commaIndex = paramStringCall.LastIndexOf(",", StringComparison.Ordinal);
				if (commaIndex >= 0)
				{
					paramStringCall = paramStringCall.Substring(0, commaIndex); // remove the last param
				}

				codeBuilder.AppendLine($"{returnStatement}{hostClassName}.{FunctionName}({paramStringCall});");
			}

			codeBuilder.AppendLine();
		}
	}

	private void ExportFunction(CodeBuilder codeBuilder)
	{
		codeBuilder.AppendTooltip(Function);
		ExportDeprecation(codeBuilder);
		ExportSignature(codeBuilder);
		using (new CodeBlock(codeBuilder)) // function body
		{
			ExportInvoke(codeBuilder);
		}
	}

	private void ExportExtensionFunction(CodeBuilder codeBuilder, string hostClassName, ExtensionMethod extensionMethod)
	{
		codeBuilder.AppendTooltip(Function);
		ExportDeprecation(codeBuilder);
		ExportExtensionMethodSignature(codeBuilder, extensionMethod);
		using (new CodeBlock(codeBuilder)) // function body
		{
			ExportExtensionMethodInvoke(codeBuilder, hostClassName, extensionMethod);
		}
	}

	private void ExportDeprecation(CodeBuilder codeBuilder)
	{
		if (Function.HasMetadata("DeprecatedFunction"))
		{
			string deprecationMessage = Function.GetMetadata("DeprecationMessage");
			deprecationMessage = deprecationMessage.Length == 0
				? "This function is deprecated."
				: deprecationMessage.Replace("\"", ""); // Remove nested quotes
			codeBuilder.AppendLine($"[Obsolete(\"{Function.EngineName} is deprecated: {deprecationMessage}\")]");
		}
	}

	private string GetReturnType()
	{
		string returnType = "void";
		if (Function.ReturnProperty != null)
		{
			PropertyTranslator returnValueTranslator = PropertyTranslatorManager.GetTranslator(Function.ReturnProperty);
			if (!string.IsNullOrEmpty(OutputGenericTypeArgument)
				&& (DynamicOutputParams == null || DynamicOutputParams.Count == 0))
			{
				returnType = returnValueTranslator.GetGenericParamManagedType(Function.ReturnProperty, OutputGenericTypeArgument);
			}
			else
			{
				returnType = returnValueTranslator.GetParamManagedType(Function.ReturnProperty);
			}
		}

		return returnType;
	}

	private string GetTypeParams()
	{
		string typeParams = "";
		if (ParamGenericConstraints != null)
		{
			if (ParamGenericConstraints.Count > 1)
			{
				typeParams = "<";
				for (int i = 0; i < ParamGenericConstraints.Count; i++)
				{
					if (i > 0)
					{
						typeParams += ", ";
					}

					typeParams += $"T{i + 1}";
				}

				typeParams += ">";
			}
			else if (ParamGenericConstraints.Count > 0)
			{
				typeParams = "<T>";
			}
		}

		return typeParams;
	}

	private void ExportSignature(CodeBuilder codeBuilder)
	{
		string returnType = GetReturnType();
		string typeParams = GetTypeParams();
		codeBuilder.AppendLine($"{Protection}{Modifiers}unsafe {returnType} {FunctionName}{typeParams}({ParamStringWithDefault})");
		ExportGenericConstraints(codeBuilder);
	}

	public void ExportDelegateSignature(CodeBuilder codeBuilder)
	{
		string returnType = GetReturnType();
		codeBuilder.AppendLine($"public delegate {returnType} {FunctionName}({ParamStringWithDefault});");
	}

	public void ExportDelegateInvokerSignature(CodeBuilder codeBuilder)
	{
		string outerClassPrefix = "";
		if (Function.Outer is UhtClass outerClass)
		{
			outerClassPrefix = $"{outerClass.EngineName}.";
		}

		string functionName;
		string thisParam;
		if (Function.HasAnyFlags(EFunctionFlags.MulticastDelegate))
		{
			functionName = "Broadcast";
			thisParam = $"this MulticastDelegate<{outerClassPrefix}{FunctionName}> instance";
		}
		else
		{
			functionName = "Execute";
			thisParam = $"this Delegate<{outerClassPrefix}{FunctionName}> instance";
		}

		string returnType = GetReturnType();
		string paramString = ParamStringWithDefault.Length > 0 ? $"{thisParam}, {ParamStringWithDefault}" : thisParam;
		codeBuilder.AppendLine($"public static unsafe {returnType} {functionName}({paramString})");
	}

	public void ExportInterfaceSignature(CodeBuilder codeBuilder)
	{
		string returnType = GetReturnType();
		string typeParams = GetTypeParams();
		codeBuilder.AppendLine($"public {returnType} {FunctionName}{typeParams}({ParamStringWithDefault})");
		ExportGenericConstraints(codeBuilder);
		codeBuilder.Append(";");
	}

	private string GetExtensionMethodParamString(string paramStringWithDefault)
	{
		int commaIndex = paramStringWithDefault.IndexOf(",", StringComparison.Ordinal);
		string paramString = commaIndex >= 0 ? paramStringWithDefault.Substring(commaIndex + 2) : "";
		return paramString;
	}

	public void ExportExtensionMethodSignature(CodeBuilder codeBuilder, ExtensionMethod extensionMethod)
	{
		string methodName = extensionMethod.MethodName;
		string returnType = GetReturnType();
		if (extensionMethod.MethodType == EExtensionMethodType.Normal)
		{
			string modifiers = "";
			string paramString = GetExtensionMethodParamString(ParamStringWithDefault);

			if (methodName == "ToString")
			{
				// override object.ToString
				modifiers = "override ";
			}
			else if (methodName == "Equals")
			{
				// parameter with "in" modifier does not match function signature of "IEquatable".
				if (paramString.StartsWith("in "))
				{
					paramString = paramString[3..];
				}
			}

			codeBuilder.AppendLine($"{Protection}{modifiers}{returnType} {methodName}({paramString})");
		}
		else if (extensionMethod.MethodType == EExtensionMethodType.Autocast)
		{
			codeBuilder.AppendLine($"{Protection}static implicit operator {returnType}({ParamStringWithDefault})");
		}
		else if (extensionMethod.MethodType == EExtensionMethodType.Operator)
		{
			if (methodName == "!(==)")
			{
				methodName = "!=";
			}

			codeBuilder.AppendLine($"{Protection}static {returnType} operator {methodName}({ParamStringWithDefault})");
		}
	}

	private void ExportGenericConstraints(CodeBuilder codeBuilder)
	{
		if (ParamGenericConstraints != null)
		{
			using (new IndentBlock(codeBuilder))
			{
				foreach (var pair in ParamGenericConstraints)
				{
					UhtClass clsObj = pair.Key;
					string typeArgument = pair.Value;
					codeBuilder.AppendLine($"where {typeArgument} : {clsObj.GetFullManagedName()}");
				}
			}
		}
	}

	private void ExportInvoke(CodeBuilder codeBuilder)
	{
		string nativeFunctionPtr = $"{Function.StrippedFunctionName}_NativeFunc";
		if (!Function.HasParametersOrReturnValue())
		{
			codeBuilder.AppendLine($"{InvokeFunction}({InvokeArguments}IntPtr.Zero);");
			return;
		}

		if (FunctionName == "IsValid" && Modifiers.Contains("new"))
		{
			codeBuilder.AppendLine("if (!base.IsValid()) { return false; }");
		}

		codeBuilder.AppendLine($"byte* _paramsBuffer = stackalloc byte[{Function.StrippedFunctionName}_ParamsSize];");
		codeBuilder.AppendLine($"using ScopedFuncParams _params = new ScopedFuncParams({nativeFunctionPtr}, _paramsBuffer);");

		ForEachParameter((translator, param) =>
		{
			if (param.HasAnyFlags(EPropertyFlags.ReturnParm)
				|| param.HasAnyFlags(EPropertyFlags.OutParm) && !param.HasAnyFlags(EPropertyFlags.ReferenceParm))
			{
				return;
			}

			string paramName = param.GetParameterName();
			if (ParamGenericTypeArguments != null && param is UhtClassProperty)
			{
				string typeArgument = ParamGenericTypeArguments[param];
				translator.ExportGenericParamToNative(codeBuilder, Function, param, paramName, typeArgument, "_params.Buffer");
			}
			else
			{
				translator.ExportParamToNative(codeBuilder, Function, param, paramName, "_params.Buffer");
			}
		});

		codeBuilder.AppendLine($"{InvokeFunction}({InvokeArguments}_params.Buffer);");

		ForEachParameter((translator, param) =>
		{
			string paramName;
			bool needDeclaration;
			if (param.HasAnyFlags(EPropertyFlags.ReturnParm))
			{
				paramName = "returnValue";
				needDeclaration = true;
			}
			else if (param.HasAnyFlags(EPropertyFlags.ReferenceParm | EPropertyFlags.OutParm) && !param.HasAnyFlags(EPropertyFlags.ConstParm))
			{
				paramName = param.GetParameterName();
				needDeclaration = false;
			}
			else
			{
				return;
			}

			if (ParamGenericTypeArguments != null && ParamGenericTypeArguments.TryGetValue(param, out var typeArgument))
			{
				translator.ExportGenericParamFromNative(codeBuilder, Function, param, paramName, typeArgument, "_params.Buffer", needDeclaration);
			}
			else
			{
				translator.ExportParamFromNative(codeBuilder, Function, param, paramName, "_params.Buffer", needDeclaration);
			}
		});

		UhtProperty? returnProp = Function.ReturnProperty;
		if (returnProp != null)
		{
			codeBuilder.AppendLine("return returnValue;");
		}
	}

	/// <summary>
	/// Replace first parameter to "this".
	/// </summary>
	private string GetExtensionMethodParamStringCall(string paramStringCall)
	{
		UhtProperty selfParam = (UhtProperty)Function.Children[0];
		string refQualifier = GetRefQualifier(selfParam);

		string paramString = $"{refQualifier}this";
		if (selfParam is UhtClassProperty classProperty)
		{
			UhtClass metaClass = classProperty.MetaClass!;
			paramString = $"new SubclassOf<{metaClass.GetFullManagedName()}>(this)";
		}

		int commaIndex = paramStringCall.IndexOf(",", StringComparison.Ordinal);
		if (commaIndex >= 0)
		{
			paramString += paramStringCall.Substring(commaIndex);
		}

		return paramString;
	}

	private void ExportExtensionMethodInvoke(CodeBuilder codeBuilder, string hostClassName, ExtensionMethod extensionMethod)
	{
		if (extensionMethod.MethodType == EExtensionMethodType.Normal)
		{
			string returnStatement = Function.ReturnProperty != null ? "return " : "";
			string paramStringCall = GetExtensionMethodParamStringCall(ParamStringCall);
			codeBuilder.AppendLine($"{returnStatement}{hostClassName}.{FunctionName}({paramStringCall});");
		}
		else if (extensionMethod.MethodType == EExtensionMethodType.Autocast)
		{
			codeBuilder.AppendLine($"return {hostClassName}.{FunctionName}({ParamStringCall});");
		}
		else if (extensionMethod.MethodType == EExtensionMethodType.Operator)
		{
			codeBuilder.AppendLine(extensionMethod.MethodName == "!(==)"
				? $"return !{hostClassName}.{FunctionName}({ParamStringCall});"
				: $"return {hostClassName}.{FunctionName}({ParamStringCall});");
		}
	}

	public void ForEachParameter(Action<PropertyTranslator, UhtProperty> action)
	{
		for (int i = 0; i < Function.Children.Count; i++)
		{
			UhtProperty parameter = (UhtProperty)Function.Children[i];
			PropertyTranslator translator = ParamTranslators[i];
			action(translator, parameter);
		}
	}

	static string GetRefQualifier(UhtProperty parameter)
	{
		if (parameter.HasAnyFlags(EPropertyFlags.ConstParm))
		{
			return parameter.HasAnyFlags(EPropertyFlags.ReferenceParm) ? "in " : "";
		}

		if (parameter.HasAnyFlags(EPropertyFlags.ReferenceParm))
		{
			return "ref ";
		}

		if (parameter.HasAnyFlags(EPropertyFlags.OutParm))
		{
			return "out ";
		}

		return "";
	}
}
