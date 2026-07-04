using System.Text;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Emits the UFUNCTION machinery for a subclassing [UCLASS], mirroring the hand-written
/// reference in SsTestGenFunctionManual.generated.cs:
/// <list type="bullet">
/// <item>per-function static fields (_NativeFunc / _ParamsSize / per-param _Offset and,
/// for container/wrapper params, _NativeProp);</item>
/// <item>the <c>FunctionParamDef[]</c> arrays + <c>FunctionDef[]</c> built in the static
/// constructor and passed to <c>SubclassingUtils.GenerateClass</c>;</item>
/// <item>the post-generate function/offset resolution;</item>
/// <item>the <c>[UnmanagedCallersOnly]</c> <c>Dispatch_*</c> stubs (UE → C#).</item>
/// </list>
/// The C#-side <c>Call*</c> helpers that drive UE's ProcessEvent are intentionally NOT emitted
/// here — the user (or the SourceGenerator test file) writes those against the generated fields,
/// exactly as the hand-written SubclassingFunctionManualTest does.
/// </summary>
internal static class FunctionEmitter
{
	/// <summary>Emits the private static fields backing every function.</summary>
	public static void EmitStaticFields(StringBuilder sb, FunctionModel func)
	{
		sb.AppendLine($"\tprivate static readonly IntPtr {func.Name}_NativeFunc;");
		sb.AppendLine($"\tprivate static readonly int {func.Name}_ParamsSize;");
		foreach (FunctionParamModel p in func.AllParams())
		{
			sb.AppendLine($"\tprivate static readonly int {func.Name}_{p.Name}_Offset;");
			if (p.NeedsNativeProp)
			{
				sb.AppendLine($"\tprivate static readonly IntPtr {func.Name}_{p.Name}_NativeProp;");
			}
		}
	}

	/// <summary>
	/// Emits the <c>FunctionParamDef[] _funcXxxParams = [...]</c> array declaration for a function
	/// (return / out / in order). Returns the local variable name used for the array.
	/// </summary>
	public static string EmitParamsArray(StringBuilder sb, FunctionModel func)
	{
		string local = ParamsArrayLocal(func);
		sb.AppendLine($"\t\tFunctionParamDef[] {local} =");
		sb.AppendLine("\t\t[");
		foreach (FunctionParamModel p in func.NativeParamOrder())
		{
			EmitParamDef(sb, p);
		}
		sb.AppendLine("\t\t];");
		sb.AppendLine();
		return local;
	}

	/// <summary>Emits a single <c>FunctionParamDef</c> initializer.</summary>
	private static void EmitParamDef(StringBuilder sb, FunctionParamModel p)
	{
		PropertyModel t = p.Type;
		sb.AppendLine("\t\t\tnew()");
		sb.AppendLine("\t\t\t{");
		sb.AppendLine($"\t\t\t\tParamName = \"{p.Name}\",");
		sb.AppendLine($"\t\t\t\tPropType = {t.PropTypeClass}.StaticClass.NativeClass,");

		if (t.UnderlyingTypeExpr != null)
		{
			sb.AppendLine($"\t\t\t\tUnderlyingType = {t.UnderlyingTypeExpr},");
		}
		if (t.Inner != null)
		{
			sb.AppendLine($"\t\t\t\tInnerPropType = {t.Inner.PropTypeClass}.StaticClass.NativeClass,");
			if (t.Inner.UnderlyingTypeExpr != null)
			{
				sb.AppendLine($"\t\t\t\tInnerUnderlyingType = {t.Inner.UnderlyingTypeExpr},");
			}
		}
		if (t.Key != null)
		{
			sb.AppendLine($"\t\t\t\tKeyPropType = {t.Key.PropTypeClass}.StaticClass.NativeClass,");
			if (t.Key.UnderlyingTypeExpr != null)
			{
				sb.AppendLine($"\t\t\t\tKeyUnderlyingType = {t.Key.UnderlyingTypeExpr},");
			}
		}

		sb.AppendLine($"\t\t\t\tParamFlags = {p.ParamFlagsExpr},");
		sb.AppendLine("\t\t\t},");
	}

	/// <summary>
	/// Emits one <c>FunctionDef</c> initializer line inside the <c>FunctionDef[]</c> collection
	/// expression. <paramref name="paramsPtr"/> is the pinned pointer local for this function's
	/// params array; <paramref name="paramsLocal"/> is the params array local (for <c>.Length</c>).
	/// </summary>
	public static void EmitFunctionDef(StringBuilder sb, FunctionModel func, string paramsPtr, string paramsLocal)
	{
		string flags = func.IsStatic ? ", FunctionFlags = SsFunctionFlags.Static" : "";
		sb.AppendLine(
			$"\t\t\t\tnew() {{ FuncName = \"{func.Name}\", Params = (IntPtr){paramsPtr}, ParamCount = {paramsLocal}.Length, " +
			$"ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_{func.Name}{flags} }},");
	}

	/// <summary>
	/// Emits the post-generate resolution block for a function: FindFunction + params-size +
	/// a PropertyIterator that walks each param to capture its offset (and native-prop for wrappers).
	/// </summary>
	public static void EmitResolution(StringBuilder sb, FunctionModel func)
	{
		string iter = $"_{LowerFirst(func.Name)}Iter";
		sb.AppendLine($"\t\t{func.Name}_NativeFunc = TypeInterop.FindFunction(NativeType, \"{func.Name}\");");
		sb.AppendLine($"\t\t{func.Name}_ParamsSize = TypeInterop.GetFunctionParamsSize({func.Name}_NativeFunc);");
		sb.AppendLine($"\t\tPropertyIterator {iter} = new({func.Name}_NativeFunc);");
		foreach (FunctionParamModel p in func.AllParams())
		{
			if (p.NeedsNativeProp)
			{
				sb.AppendLine($"\t\t{func.Name}_{p.Name}_NativeProp = {iter}.FindNext(\"{p.Name}\");");
				sb.AppendLine($"\t\t{func.Name}_{p.Name}_Offset = TypeInterop.GetPropertyOffset({func.Name}_{p.Name}_NativeProp);");
			}
			else
			{
				sb.AppendLine($"\t\t{func.Name}_{p.Name}_Offset = {iter}.FindNextAndGetOffset(\"{p.Name}\");");
			}
		}
	}

	/// <summary>Emits the [UnmanagedCallersOnly] dispatch stub (UE → C#) for a function.</summary>
	public static void EmitDispatchStub(StringBuilder sb, string className, FunctionModel func)
	{
		sb.AppendLine("\t[UnmanagedCallersOnly]");
		sb.AppendLine($"\tprivate static void Dispatch_{func.Name}(IntPtr objectHandle, IntPtr paramsBuffer)");
		sb.AppendLine("\t{");

		if (!func.IsStatic)
		{
			sb.AppendLine($"\t\tvar self = SubclassingUtils.ResolveManagedObject<{className}>(objectHandle);");
		}

		// Read in-params into locals.
		foreach (FunctionParamModel p in func.Parameters)
		{
			if (p.Role == ParamRole.In)
			{
				string local = LowerFirst(p.Name);
				string offset = $"paramsBuffer + {func.Name}_{p.Name}_Offset";
				sb.AppendLine($"\t\t{ParamDeclType(p)} {local} = {ReadExpr(func, p, offset)};");
			}
		}

		// Build the argument list: in-params by value, out-params as 'out <type> <local>'.
		List<string> args = new();
		foreach (FunctionParamModel p in func.Parameters)
		{
			if (p.Role == ParamRole.In)
			{
				args.Add(LowerFirst(p.Name));
			}
			else
			{
				args.Add($"out {ParamDeclType(p)} {LowerFirst(p.Name)}");
			}
		}
		string argList = string.Join(", ", args);
		string target = func.IsStatic ? func.Name : $"self.{func.Name}";

		if (func.ReturnParam != null)
		{
			sb.AppendLine($"\t\t{ParamDeclType(func.ReturnParam)} returnValue = {target}({argList});");
		}
		else
		{
			sb.AppendLine($"\t\t{target}({argList});");
		}

		// Write out-params back.
		foreach (FunctionParamModel p in func.Parameters)
		{
			if (p.Role == ParamRole.Out)
			{
				string offset = $"paramsBuffer + {func.Name}_{p.Name}_Offset";
				sb.AppendLine($"\t\t{WriteStmt(func, p, offset, LowerFirst(p.Name))}");
			}
		}

		// Write the return value.
		if (func.ReturnParam != null)
		{
			string offset = $"paramsBuffer + {func.Name}_{func.ReturnParam.Name}_Offset";
			sb.AppendLine($"\t\t{WriteStmt(func, func.ReturnParam, offset, "returnValue")}");
		}

		sb.AppendLine("\t}");
	}

	// ------------------------------------------------------------------
	// Marshalling expression helpers (shared shape with the class property accessors,
	// but rendered for the function dispatch/call path).
	// ------------------------------------------------------------------

	/// <summary>
	/// The C# type used to declare a param local and its <c>out</c> argument. This is the type
	/// exactly as written in the user method signature (e.g. "List&lt;string&gt;", "UObject?"), so
	/// the call binds to the user method; the wrapper read expression converts into it.
	/// </summary>
	private static string ParamDeclType(FunctionParamModel p) => p.DeclaredType;

	/// <summary>Reads a value from the params buffer at <paramref name="offset"/>.</summary>
	private static string ReadExpr(FunctionModel func, FunctionParamModel p, string offset)
	{
		PropertyModel t = p.Type;
		switch (t.Kind)
		{
			case PropertyKind.Array:
				return $"new TArray<{t.Inner!.ManagedType}>({offset}, {NativeProp(func, p)}, {t.Inner.MarshallerInstanceExpr})";
			case PropertyKind.Set:
				return $"new TSet<{t.Inner!.ManagedType}>({offset}, {NativeProp(func, p)}, {t.Inner.MarshallerInstanceExpr})";
			case PropertyKind.Map:
				return $"new TMap<{t.Key!.ManagedType}, {t.Inner!.ManagedType}>({offset}, {NativeProp(func, p)}, {t.Key.MarshallerInstanceExpr}, {t.Inner.MarshallerInstanceExpr})";
			case PropertyKind.StructNativeRef when !t.IsBlittable:
				return $"new {t.NativeRefType}({offset}).ToManaged()";
			default:
				return $"{ValueMarshaller(t)}.FromNative({offset})";
		}
	}

	/// <summary>Writes <paramref name="value"/> to the params buffer at <paramref name="offset"/>.</summary>
	private static string WriteStmt(FunctionModel func, FunctionParamModel p, string offset, string value)
	{
		PropertyModel t = p.Type;
		switch (t.Kind)
		{
			case PropertyKind.Array:
				return $"new TArray<{t.Inner!.ManagedType}>({offset}, {NativeProp(func, p)}, {t.Inner.MarshallerInstanceExpr}).CopyFrom({value});";
			case PropertyKind.Set:
				return $"new TSet<{t.Inner!.ManagedType}>({offset}, {NativeProp(func, p)}, {t.Inner.MarshallerInstanceExpr}).CopyFrom({value});";
			case PropertyKind.Map:
				return $"new TMap<{t.Key!.ManagedType}, {t.Inner!.ManagedType}>({offset}, {NativeProp(func, p)}, {t.Key.MarshallerInstanceExpr}, {t.Inner.MarshallerInstanceExpr}).CopyFrom({value});";
			case PropertyKind.StructNativeRef when !t.IsBlittable:
				return $"new {t.NativeRefType}({offset}).FromManaged({value});";
			default:
				return $"{ValueMarshaller(t)}.ToNative({offset}, {value});";
		}
	}

	/// <summary>
	/// The static marshaller type used for simple value / object / blittable-struct params.
	/// </summary>
	private static string ValueMarshaller(PropertyModel t)
	{
		return t.Kind switch
		{
			PropertyKind.Bool => "BoolMarshaller",
			PropertyKind.Blittable => $"BlittableMarshaller<{t.ManagedType}>",
			PropertyKind.Enum => $"EnumMarshaller<{t.ManagedType}>",
			PropertyKind.String => "StringMarshaller",
			PropertyKind.Name => "NameMarshaller",
			PropertyKind.Text => "TextMarshaller",
			PropertyKind.Object => $"ObjectMarshaller<{t.TargetType}>",
			PropertyKind.SoftObjectPtr => $"SoftObjectPtrMarshaller<{t.TargetType}>",
			PropertyKind.LazyObjectPtr => $"LazyObjectPtrMarshaller<{t.TargetType}>",
			PropertyKind.SubclassOf => $"SubclassOfMarshaller<{t.TargetType}>",
			PropertyKind.SoftClassPtr => $"SoftClassPtrMarshaller<{t.TargetType}>",
			// Blittable USTRUCT passed by value.
			PropertyKind.StructNativeRef => $"BlittableMarshaller<{t.ValueStructType}>",
			_ => $"BlittableMarshaller<{t.ManagedType}>",
		};
	}

	private static string NativeProp(FunctionModel func, FunctionParamModel p)
		=> $"{func.Name}_{p.Name}_NativeProp";

	/// <summary>The params-array local name for a function, e.g. "_funcInt32Params".</summary>
	public static string ParamsArrayLocal(FunctionModel func) => $"_{LowerFirst(func.Name)}Params";

	private static string LowerFirst(string s)
		=> s.Length == 0 ? s : char.ToLowerInvariant(s[0]) + s.Substring(1);
}
