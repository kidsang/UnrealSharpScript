using System.Text;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Emits the <c>&lt;Class&gt;.generated.cs</c> partial class for a [UCLASS],
/// mirroring the hand-written reference in SsTestGenClassManual.generated.cs.
/// </summary>
internal static class ClassEmitter
{
	public static string Emit(ClassModel model)
	{
		StringBuilder sb = new();

		sb.AppendLine("#nullable enable");
		sb.AppendLine("using System.Runtime.InteropServices;");
		sb.AppendLine("using SharpScript;");
		sb.AppendLine("using SharpScript.Interop;");
		sb.AppendLine("using SharpScript.Subclassing;");
		sb.AppendLine("using UnrealEngine.CoreUObject;");
		sb.AppendLine("using UnrealEngine.Intrinsic;");
		sb.AppendLine();

		EmitUtils.EmitNamespace(sb, model.Namespace);

		sb.AppendLine($"public partial class {model.ClassName} : IStaticClass<{model.ClassName}>");
		sb.AppendLine("{");

		EmitStaticFields(sb, model);
		sb.AppendLine();
		EmitStaticConstructor(sb, model);

		foreach (PropertyModel prop in model.Properties)
		{
			sb.AppendLine();
			EmitAccessor(sb, prop);
		}

		if (model.Functions.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("\t// ------------------------------------------------------------------");
			sb.AppendLine("\t// Native dispatch stubs (UE -> C#).");
			sb.AppendLine("\t// ------------------------------------------------------------------");
			foreach (FunctionModel func in model.Functions)
			{
				sb.AppendLine();
				FunctionEmitter.EmitDispatchStub(sb, model.ClassName, func);
			}
		}

		sb.AppendLine("}");
		return sb.ToString();
	}

	private static void EmitStaticFields(StringBuilder sb, ClassModel model)
	{
		sb.AppendLine($"\tpublic new static TSubclassOf<{model.ClassName}> StaticClass {{ get; }}");
		sb.AppendLine();
		sb.AppendLine("\tprivate new static readonly IntPtr NativeType;");
		foreach (PropertyModel prop in model.Properties)
		{
			sb.AppendLine($"\tprivate static readonly IntPtr {prop.Name}_NativeProp;");
			sb.AppendLine($"\tprivate static readonly int {prop.Name}_Offset;");
		}
		foreach (FunctionModel func in model.Functions)
		{
			FunctionEmitter.EmitStaticFields(sb, func);
		}
	}

	private static void EmitStaticConstructor(StringBuilder sb, ClassModel model)
	{
		bool hasFunctions = model.Functions.Count > 0;

		// A function-carrying class needs 'unsafe' on the ctor itself (dispatch pointers via &).
		sb.AppendLine($"\tstatic {(hasFunctions ? "unsafe " : "")}{model.ClassName}()");
		sb.AppendLine("\t{");

		sb.AppendLine("\t\tPropertyDef[] _propertyDefs =");
		sb.AppendLine("\t\t[");
		foreach (PropertyModel prop in model.Properties)
		{
			EmitUtils.EmitPropertyDef(sb, prop);
		}
		sb.AppendLine("\t\t];");
		sb.AppendLine();

		if (!hasFunctions)
		{
			EmitGenerateNoFunctions(sb, model);
		}
		else
		{
			EmitGenerateWithFunctions(sb, model);
		}

		sb.AppendLine();
		sb.AppendLine($"\t\tStaticClass = new TSubclassOf<{model.ClassName}>(NativeType);");
		sb.AppendLine($"\t\tHouseKeeper.AddBindedUnrealClass(StaticClass.Class!, typeof({model.ClassName}));");

		if (model.Properties.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("\t\tPropertyIterator propIter = new PropertyIterator(NativeType);");
			foreach (PropertyModel prop in model.Properties)
			{
				sb.AppendLine($"\t\t{prop.Name}_NativeProp = propIter.FindNext(\"{prop.Name}\");");
				sb.AppendLine($"\t\t{prop.Name}_Offset = TypeInterop.GetPropertyOffset({prop.Name}_NativeProp);");
			}
		}

		foreach (FunctionModel func in model.Functions)
		{
			sb.AppendLine();
			FunctionEmitter.EmitResolution(sb, func);
		}

		sb.AppendLine("\t}");
	}

	/// <summary>
	/// Emits the <c>fixed (char* metaValueN = "...")</c> pin lines for each metadata value, one per line,
	/// at the given indent, wrapped in <c>#if WITH_EDITOR</c> / <c>#endif</c>. These join the surrounding
	/// fixed cascade so the pinned pointers stay valid through the GenerateClass call.
	/// Returns the pin variable names (aligned with model.Metadata order).
	/// </summary>
	private static string[] EmitMetaValueFixeds(StringBuilder sb, ClassModel model, string indent)
	{
		string[] names = new string[model.Metadata.Count];
		for (int i = 0; i < model.Metadata.Count; i++)
		{
			names[i] = $"_metaValue{i}";
			sb.AppendLine($"{indent}#if WITH_EDITOR");
			sb.AppendLine($"{indent}fixed (char* {names[i]} = {EmitUtils.ToLiteral(model.Metadata[i].Value)})");
			sb.AppendLine($"{indent}#endif");
		}
		return names;
	}

	/// <summary>
	/// Emits the <c>fixed (char* ...)</c> pin lines for every metadata value on every property
	/// (WITH_EDITOR guarded), joining the surrounding fixed cascade. Returns a jagged array of pin
	/// variable names indexed by [propertyIndex][metaIndex]; properties without metadata get an empty row.
	/// </summary>
	private static string[][] EmitPropMetaValueFixeds(StringBuilder sb, ClassModel model, string indent)
	{
		string[][] names = new string[model.Properties.Count][];
		for (int p = 0; p < model.Properties.Count; p++)
		{
			PropertyModel prop = model.Properties[p];
			names[p] = new string[prop.Metadata.Count];
			for (int m = 0; m < prop.Metadata.Count; m++)
			{
				names[p][m] = $"_propMetaValue{p}_{m}";
				sb.AppendLine($"{indent}#if WITH_EDITOR");
				sb.AppendLine($"{indent}fixed (char* {names[p][m]} = {EmitUtils.ToLiteral(prop.Metadata[m].Value)})");
				sb.AppendLine($"{indent}#endif");
			}
		}
		return names;
	}

	/// <summary>
	/// Emits the <c>fixed (char* ...)</c> pin lines for every metadata value on every function
	/// (WITH_EDITOR guarded), joining the surrounding fixed cascade. Returns a jagged array of pin
	/// variable names indexed by [functionIndex][metaIndex]; functions without metadata get an empty row.
	/// </summary>
	private static string[][] EmitFuncMetaValueFixeds(StringBuilder sb, ClassModel model, string indent)
	{
		string[][] names = new string[model.Functions.Count][];
		for (int f = 0; f < model.Functions.Count; f++)
		{
			FunctionModel func = model.Functions[f];
			names[f] = new string[func.Metadata.Count];
			for (int m = 0; m < func.Metadata.Count; m++)
			{
				names[f][m] = $"_funcMetaValue{f}_{m}";
				sb.AppendLine($"{indent}#if WITH_EDITOR");
				sb.AppendLine($"{indent}fixed (char* {names[f][m]} = {EmitUtils.ToLiteral(func.Metadata[m].Value)})");
				sb.AppendLine($"{indent}#endif");
			}
		}
		return names;
	}

	/// <summary>
	/// Emits, inside an already-open unsafe/fixed scope (with the property/function def pointers and the
	/// metadata value char* pins already fixed), the inline <c>MetaDataEntry[]</c> (built from the
	/// <paramref name="metaVarNames"/> pins with <c>#if WITH_EDITOR</c> guards), then pins it, constructs
	/// the <c>ClassDef</c> and calls <c>SubclassingUtils.GenerateClass</c>. No heap marshalling — every
	/// string is pinned by the caller's fixed cascade. <paramref name="indent"/> is the leading tab string;
	/// <paramref name="funcPtrExpr"/>/<paramref name="funcCountExpr"/> supply the function-def slot;
	/// <paramref name="configExpr"/> is <c>(IntPtr)_configName</c> or <c>IntPtr.Zero</c>.
	/// </summary>
	private static void EmitClassDefAndCall(StringBuilder sb, ClassModel model, string indent,
		string funcPtrExpr, string funcCountExpr, string[] metaVarNames, string[][] propMetaVarNames, string configExpr,
		string[][]? funcMetaVarNames = null)
	{
		// Build the class-level MetaDataEntry[] (from the pinned class metadata value char*s).
		sb.AppendLine($"{indent}MetaDataEntry[] _metaEntries =");
		sb.AppendLine($"{indent}[");
		for (int i = 0; i < model.Metadata.Count; i++)
		{
			sb.AppendLine($"{indent}#if WITH_EDITOR");
			sb.AppendLine($"{indent}\tnew() {{ Key = \"{model.Metadata[i].Key}\", Value = {metaVarNames[i]} }},");
			sb.AppendLine($"{indent}#endif");
		}
		sb.AppendLine($"{indent}];");
		sb.AppendLine();

		// Build one MetaDataEntry[] per property that carries metadata (from the pinned per-property
		// value char*s). Named _propMetaEntries{p}. Properties without metadata are skipped.
		for (int p = 0; p < model.Properties.Count; p++)
		{
			PropertyModel prop = model.Properties[p];
			if (prop.Metadata.Count == 0)
			{
				continue;
			}
			sb.AppendLine($"{indent}MetaDataEntry[] _propMetaEntries{p} =");
			sb.AppendLine($"{indent}[");
			for (int m = 0; m < prop.Metadata.Count; m++)
			{
				sb.AppendLine($"{indent}#if WITH_EDITOR");
				sb.AppendLine($"{indent}\tnew() {{ Key = \"{prop.Metadata[m].Key}\", Value = {propMetaVarNames[p][m]} }},");
				sb.AppendLine($"{indent}#endif");
			}
			sb.AppendLine($"{indent}];");
		}
		sb.AppendLine();

		// Build one MetaDataEntry[] per function that carries metadata (from the pinned per-function
		// value char*s). Named _funcMetaEntries{f}. Functions without metadata are skipped. Only the
		// with-functions path passes funcMetaVarNames; the property-only path leaves it null.
		if (funcMetaVarNames != null)
		{
			for (int f = 0; f < model.Functions.Count; f++)
			{
				FunctionModel func = model.Functions[f];
				if (func.Metadata.Count == 0)
				{
					continue;
				}
				sb.AppendLine($"{indent}MetaDataEntry[] _funcMetaEntries{f} =");
				sb.AppendLine($"{indent}[");
				for (int m = 0; m < func.Metadata.Count; m++)
				{
					sb.AppendLine($"{indent}#if WITH_EDITOR");
					sb.AppendLine($"{indent}\tnew() {{ Key = \"{func.Metadata[m].Key}\", Value = {funcMetaVarNames[f][m]} }},");
					sb.AppendLine($"{indent}#endif");
				}
				sb.AppendLine($"{indent}];");
			}
			sb.AppendLine();
		}

		// Pin the class metadata array and every per-property metadata array in one fixed cascade so
		// the MetaDataEntry* pointers stay valid across the GenerateClass call.
		sb.AppendLine($"{indent}fixed (MetaDataEntry* _metaEntriesPtr = _metaEntries)");
		for (int p = 0; p < model.Properties.Count; p++)
		{
			if (model.Properties[p].Metadata.Count > 0)
			{
				sb.AppendLine($"{indent}fixed (MetaDataEntry* _propMetaEntriesPtr{p} = _propMetaEntries{p})");
			}
		}
		if (funcMetaVarNames != null)
		{
			for (int f = 0; f < model.Functions.Count; f++)
			{
				if (model.Functions[f].Metadata.Count > 0)
				{
					sb.AppendLine($"{indent}fixed (MetaDataEntry* _funcMetaEntriesPtr{f} = _funcMetaEntries{f})");
				}
			}
		}
		sb.AppendLine($"{indent}{{");

		// Attach each property's metadata to its PropertyDef entry (the array is pinned, so writing
		// into its elements before the native call is safe).
		for (int p = 0; p < model.Properties.Count; p++)
		{
			if (model.Properties[p].Metadata.Count > 0)
			{
				sb.AppendLine($"{indent}\t_propertyDefs[{p}].MetaEntries = (IntPtr)_propMetaEntriesPtr{p};");
				sb.AppendLine($"{indent}\t_propertyDefs[{p}].MetaCount = _propMetaEntries{p}.Length;");
			}
		}

		// Attach each function's metadata to its FunctionDef entry (same pinned-array reasoning).
		if (funcMetaVarNames != null)
		{
			for (int f = 0; f < model.Functions.Count; f++)
			{
				if (model.Functions[f].Metadata.Count > 0)
				{
					sb.AppendLine($"{indent}\t_functionDefs[{f}].MetaEntries = (IntPtr)_funcMetaEntriesPtr{f};");
					sb.AppendLine($"{indent}\t_functionDefs[{f}].MetaCount = _funcMetaEntries{f}.Length;");
				}
			}
		}

		sb.AppendLine($"{indent}\tClassDef _classDef = new()");
		sb.AppendLine($"{indent}\t{{");
		sb.AppendLine($"{indent}\t\tClassName = \"{model.UnrealName}\",");
		sb.AppendLine($"{indent}\t\tSuperClass = {model.SuperClass}.StaticClass.NativeClass,");
		sb.AppendLine($"{indent}\t\tPropertyDefines = (IntPtr)_propertyDefsPtr,");
		sb.AppendLine($"{indent}\t\tPropertyCount = _propertyDefs.Length,");
		sb.AppendLine($"{indent}\t\tFunctionDefines = {funcPtrExpr},");
		sb.AppendLine($"{indent}\t\tFunctionCount = {funcCountExpr},");
		sb.AppendLine($"{indent}\t\tSpecifiers = {model.SpecifiersExpr},");
		sb.AppendLine($"{indent}\t\tMetaEntries = (IntPtr)_metaEntriesPtr,");
		sb.AppendLine($"{indent}\t\tMetaCount = _metaEntries.Length,");
		sb.AppendLine($"{indent}\t\tConfigName = {configExpr},");
		sb.AppendLine($"{indent}\t}};");
		sb.AppendLine($"{indent}\tNativeType = SubclassingUtils.GenerateClass(");
		sb.AppendLine($"{indent}\t\tRuntimeTypeHandle.ToIntPtr(typeof({model.ClassName}).TypeHandle),");
		sb.AppendLine($"{indent}\t\t(IntPtr)(&_classDef));");
		sb.AppendLine($"{indent}}}");
	}

	/// <summary>
	/// Property-only GenerateClass call: pin the metadata value strings (WITH_EDITOR guarded), the
	/// config name (if any), and property defs in one fixed cascade, build the MetaDataEntry[] /
	/// ClassDef and call GenerateClass.
	/// </summary>
	private static void EmitGenerateNoFunctions(StringBuilder sb, ClassModel model)
	{
		sb.AppendLine("\t\tunsafe");
		sb.AppendLine("\t\t{");
		string configExpr = EmitConfigFixed(sb, model, "\t\t\t");
		string[] metaVars = EmitMetaValueFixeds(sb, model, "\t\t\t");
		string[][] propMetaVars = EmitPropMetaValueFixeds(sb, model, "\t\t\t");
		sb.AppendLine("\t\t\tfixed (PropertyDef* _propertyDefsPtr = _propertyDefs)");
		sb.AppendLine("\t\t\t{");
		EmitClassDefAndCall(sb, model, "\t\t\t\t", "IntPtr.Zero", "0", metaVars, propMetaVars, configExpr);
		sb.AppendLine("\t\t\t}");
		sb.AppendLine("\t\t}");
	}

	/// <summary>
	/// Emits a <c>fixed (char* _configName = "...")</c> line when <c>model.ConfigName</c> is non-null,
	/// and returns the expression to use for <c>ClassDef.ConfigName</c> (<c>(IntPtr)_configName</c> or
	/// <c>IntPtr.Zero</c>). Unlike metadata, Config is NOT wrapped in <c>#if WITH_EDITOR</c> because
	/// configuration classes must function in non-editor builds as well.
	/// <paramref name="indent"/> is the leading tab string for the fixed line.
	/// </summary>
	private static string EmitConfigFixed(StringBuilder sb, ClassModel model, string indent)
	{
		if (model.ConfigName != null)
		{
			sb.AppendLine($"{indent}fixed (char* _configName = {EmitUtils.ToLiteral(model.ConfigName)})");
			return "(IntPtr)_configName";
		}
		return "IntPtr.Zero";
	}

	/// <summary>
	/// GenerateClass call with functions: build each function's FunctionParamDef[], pin them all
	/// along with metadata values (WITH_EDITOR guarded), config name and the function defs, build
	/// the FunctionDef[] (with managed dispatch pointers), then call GenerateClass with both blocks.
	/// Mirrors SsTestGenFunctionManual.generated.cs.
	/// </summary>
	private static void EmitGenerateWithFunctions(StringBuilder sb, ClassModel model)
	{
		// Emit each function's params array first (they are pinned below).
		foreach (FunctionModel func in model.Functions)
		{
			FunctionEmitter.EmitParamsArray(sb, func);
		}

		// Pin the config name (always, not editor-only), the metadata value strings (WITH_EDITOR
		// guarded), the property defs and every function's params array in a single fixed cascade
		// so all pointers stay valid through the GenerateClass call.
		string configExpr = EmitConfigFixed(sb, model, "\t\t");
		string[] metaVars = EmitMetaValueFixeds(sb, model, "\t\t");
		string[][] propMetaVars = EmitPropMetaValueFixeds(sb, model, "\t\t");
		string[][] funcMetaVars = EmitFuncMetaValueFixeds(sb, model, "\t\t");
		sb.AppendLine("\t\tfixed (PropertyDef* _propertyDefsPtr = _propertyDefs)");
		string[] ptrNames = new string[model.Functions.Count];
		for (int i = 0; i < model.Functions.Count; i++)
		{
			ptrNames[i] = $"_p{i}";
			string local = FunctionEmitter.ParamsArrayLocal(model.Functions[i]);
			sb.AppendLine($"\t\tfixed (FunctionParamDef* {ptrNames[i]} = {local})");
		}
		sb.AppendLine("\t\t{");

		sb.AppendLine("\t\t\tFunctionDef[] _functionDefs =");
		sb.AppendLine("\t\t\t[");
		for (int i = 0; i < model.Functions.Count; i++)
		{
			FunctionModel func = model.Functions[i];
			FunctionEmitter.EmitFunctionDef(sb, func, ptrNames[i], FunctionEmitter.ParamsArrayLocal(func));
		}
		sb.AppendLine("\t\t\t];");
		sb.AppendLine();

		sb.AppendLine("\t\t\tfixed (FunctionDef* _functionDefsPtr = _functionDefs)");
		sb.AppendLine("\t\t\t{");
		EmitClassDefAndCall(sb, model, "\t\t\t\t", "(IntPtr)_functionDefsPtr", "_functionDefs.Length", metaVars, propMetaVars, configExpr, funcMetaVars);
		sb.AppendLine("\t\t\t}");
		sb.AppendLine("\t\t}");
	}

	/// <summary>Emits a single accessor on the NativeRef class (against nativePtr + offset).</summary>
	private static void EmitAccessor(StringBuilder sb, PropertyModel prop)
	{
		if (EmitUtils.IsSimpleValueKind(prop.Kind))
		{
			EmitValueAccessor(sb, prop, EmitUtils.GetValueMarshallerName(prop),
				prop is { Kind: PropertyKind.Object, IsNullable: true });
			return;
		}

		switch (prop.Kind)
		{
			case PropertyKind.Array:
				EmitCachedWrapperAccessor(sb, prop, prop.ManagedType,
					$"{prop.Name}_NativeProp, {prop.Inner!.MarshallerInstanceExpr}");
				break;
			case PropertyKind.StructArray:
				EmitCachedWrapperAccessor(sb, prop, prop.ManagedType, $"{prop.Name}_NativeProp");
				break;
			case PropertyKind.Set:
				EmitCachedWrapperAccessor(sb, prop, prop.ManagedType,
					$"{prop.Name}_NativeProp, {prop.Inner!.MarshallerInstanceExpr}");
				break;
			case PropertyKind.Map:
				EmitCachedWrapperAccessor(sb, prop, prop.ManagedType,
					$"{prop.Name}_NativeProp, {prop.Key!.MarshallerInstanceExpr}, {prop.Inner!.MarshallerInstanceExpr}");
				break;
			case PropertyKind.StructMap:
				EmitCachedWrapperAccessor(sb, prop, prop.ManagedType,
					$"{prop.Name}_NativeProp, {prop.Key!.MarshallerInstanceExpr}");
				break;
			case PropertyKind.StructNativeRef:
				EmitCachedWrapperAccessor(sb, prop, prop.NativeRefType!, "");
				break;
			case PropertyKind.BlittableStructRef:
				EmitBlittableStructRefAccessor(sb, prop);
				break;
		}
	}

	/// <summary>
	/// Simple value property with get + set via a marshaller's static FromNative/ToNative.
	/// </summary>
	private static void EmitValueAccessor(StringBuilder sb, PropertyModel prop, string marshaller, bool nullable = false)
	{
		string returnType = nullable ? $"{prop.ManagedType}?" : prop.ManagedType;
		sb.AppendLine($"\tpublic partial {returnType} {prop.Name}");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tget");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tThrowIfNotValid();");
		sb.AppendLine($"\t\t\treturn {marshaller}.FromNative(NativeObject + {prop.Name}_Offset);");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t\tset");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tThrowIfNotValid();");
		sb.AppendLine($"\t\t\t{marshaller}.ToNative(NativeObject + {prop.Name}_Offset, value);");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");
	}

	/// <summary>
	/// Emits a lazy-cached wrapper property accessor. <paramref name="constructorArgs"/> is
	/// the comma-separated additional arguments after the offset expression. Use
	/// <see cref="string.Empty"/> when the wrapper constructor takes only the offset
	/// (e.g. StructNativeRef).
	/// </summary>
	private static void EmitCachedWrapperAccessor(StringBuilder sb, PropertyModel prop, string typeName, string constructorArgs)
	{
		string field = NamingUtils.BackingFieldName(prop.Name);
		sb.AppendLine($"\tprivate {typeName}? {field};");
		sb.AppendLine();
		sb.AppendLine($"\tpublic partial {typeName} {prop.Name}");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tget");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tThrowIfNotValid();");
		string ctor = string.IsNullOrEmpty(constructorArgs)
			? $"new(NativeObject + {prop.Name}_Offset)"
			: $"new(NativeObject + {prop.Name}_Offset, {constructorArgs})";
		sb.AppendLine($"\t\t\treturn {field} ??= {ctor};");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");
	}

	private static void EmitBlittableStructRefAccessor(StringBuilder sb, PropertyModel prop)
	{
		sb.AppendLine($"\tpublic partial ref {prop.ValueStructType} {prop.Name}");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tget");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tunsafe");
		sb.AppendLine("\t\t\t{");
		sb.AppendLine("\t\t\t\tThrowIfNotValid();");
		sb.AppendLine($"\t\t\t\treturn ref *({prop.ValueStructType}*)(NativeObject + {prop.Name}_Offset);");
		sb.AppendLine("\t\t\t}");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");
	}
}
