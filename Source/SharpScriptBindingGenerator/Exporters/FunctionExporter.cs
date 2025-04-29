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
};

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
};

public struct FunctionOverload
{
	public string ParamString;
	public string ParamStringCall;
	public string CSharpParamName;
	public string CppDefaultValue;
	public PropertyTranslator Translator;
	public UhtProperty Parameter;
}

public class FunctionExporter
{
	protected UhtFunction Function;
	protected EFunctionProtectionMode ProtectionMode;
	protected EOverloadMode OverloadMode;
	protected EBlueprintVisibility BlueprintVisibility;

	protected string FunctionName;
	protected string Modifiers;
	protected string ParamStringWithDefault;
	protected string InvokeFunciton;
	protected string InvokeArguments;

	protected bool IsBlueprintEvent => BlueprintVisibility == EBlueprintVisibility.Event;

	protected List<PropertyTranslator> ParamTranslators = new();

	protected List<FunctionOverload> Overloads = new();

	public FunctionExporter(UhtFunction function, EFunctionProtectionMode protectionMode, EOverloadMode overloadMode, EBlueprintVisibility blueprintVisibility)
	{
		Function = function;
		ProtectionMode = protectionMode;
		OverloadMode = overloadMode;
		BlueprintVisibility = blueprintVisibility;

		FunctionName = function.StrippedFunctionName;

		Modifiers = "";
		switch (protectionMode)
		{
			case EFunctionProtectionMode.UseUFunctionProtection:
				if (function.HasAnyFlags(EFunctionFlags.Public))
				{
					Modifiers = "public ";
				}
				else if (function.HasAnyFlags(EFunctionFlags.Protected) || function.HasMetadata("BlueprintProtected"))
				{
					Modifiers = "protected ";
				}
				else
				{
					Modifiers = "public ";
				}

				break;
			case EFunctionProtectionMode.OverrideWithInternal:
				Modifiers = "internal ";
				break;
			case EFunctionProtectionMode.OverrideWithPublic:
				Modifiers = "public ";
				break;
		}

		if (function.HasAnyFlags(EFunctionFlags.Static))
		{
			Modifiers += "static ";
			InvokeFunciton = "InvokeStaticFunctionCall";
			InvokeArguments = $"NativeType, {FunctionName}_NativeFunc, ";
		}
		else
		{
			if (IsBlueprintEvent)
			{
				Modifiers += "virtual ";
			}

			if (function.IsInterfaceFunction())
			{
				Modifiers = "public ";
			}

			InvokeFunciton = "InvokeFunctionCall";
			InvokeArguments = $"{FunctionName}_NativeFunc, ";
		}

		// handle conflict functions
		switch (FunctionName)
		{
			// override ToString
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
			// never override System.Object.GetType()
			case "GetType":
			{
				FunctionName = "K2_" + FunctionName;
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

		bool hasDefaultParameters = false;
		string paramString = "";
		string paramStringCall = "";
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
				paramStringCall = $"{paramStringCall}, ";
			}

			string refQualifier = GetRefQualifier(param);
			string paramName = param.GetParameterName();
			string paramManagedType = translator.GetParamManagedType(param);

			// if (_selfParameter == param)
			// {
			// 	if (string.IsNullOrEmpty(paramsStringCallGenerics))
			// 	{
			// 		paramsStringCallGenerics += refQualifier + paramName;
			// 	}
			// 	else
			// 	{
			// 		paramsStringCallGenerics = $"{paramName},  " + _paramsStringCall.Substring(0, _paramsStringCall.Length - 2);
			// 	}
			//
			// 	if (string.IsNullOrEmpty(_paramsStringCall))
			// 	{
			// 		_paramsStringCall += refQualifier + paramName;
			// 	}
			// 	else
			// 	{
			// 		_paramsStringCall = $"{paramName},  " + _paramsStringCall.Substring(0, _paramsStringCall.Length - 2);
			// 	}
			// 	paramsStringCallNative += paramName;
			// }
			// else
			{
				paramStringCall = $"{paramStringCall}{refQualifier}{paramName}";
				paramString = $"{paramString}{refQualifier}{paramManagedType} {paramName}";

				string? cppDefaultValue = translator.GetCppDefaultValue(function, param);
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
						ParamStringCall = paramStringCall,
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

		FunctionExporter exporter = new FunctionExporter(function, protectionMode, overloadMode, blueprintVisibility)
		{
			FunctionName = $"F{function.StrippedFunctionName}",
			InvokeArguments = "",
			InvokeFunciton = function.HasAnyFlags(EFunctionFlags.MulticastDelegate)
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

			codeBuilder.AppendLine($"{Modifiers}unsafe {returnType} {FunctionName}({overload.ParamString})");

			using (new CodeBlock(codeBuilder)) // function body
			{
				overload.Translator.ExportCppDefaultParameterAsLocalVariable(codeBuilder, overload.Parameter, overload.CSharpParamName, overload.CppDefaultValue);
				codeBuilder.AppendLine($"{returnStatement}{FunctionName}({overload.ParamStringCall});");
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
			returnType = returnValueTranslator.GetParamManagedType(Function.ReturnProperty);
		}

		return returnType;
	}

	private void ExportSignature(CodeBuilder codeBuilder)
	{
		string returnType = GetReturnType();
		codeBuilder.AppendLine($"{Modifiers}unsafe {returnType} {FunctionName}({ParamStringWithDefault})");
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
		codeBuilder.AppendLine($"public {returnType} {FunctionName}({ParamStringWithDefault});");
	}

	private void ExportInvoke(CodeBuilder codeBuilder)
	{
		string nativeFunctionPtr = $"{Function.StrippedFunctionName}_NativeFunc";
		if (!Function.HasParametersOrReturnValue())
		{
			codeBuilder.AppendLine($"{InvokeFunciton}({InvokeArguments}IntPtr.Zero);");
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
			translator.ExportParamToNative(codeBuilder, Function, param, paramName, "_params.Buffer");
		});

		codeBuilder.AppendLine($"{InvokeFunciton}({InvokeArguments}_params.Buffer);");

		ForEachParameter((translator, param) =>
		{
			if (param.HasAnyFlags(EPropertyFlags.ReturnParm))
			{
				translator.ExportParamFromNative(codeBuilder, Function, param, "returnValue", "_params.Buffer", true);
			}
			else if (param.HasAnyFlags(EPropertyFlags.ReferenceParm | EPropertyFlags.OutParm) && !param.HasAnyFlags(EPropertyFlags.ConstParm))
			{
				string paramName = param.GetParameterName();
				translator.ExportParamFromNative(codeBuilder, Function, param, paramName, "_params.Buffer", false);
			}
		});

		UhtProperty? returnProp = Function.ReturnProperty;
		if (returnProp != null)
		{
			codeBuilder.AppendLine("return returnValue;");
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
