using System.Text;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Emits the <c>&lt;Struct&gt;.generated.cs</c> for a [USTRUCT], mirroring the hand-written
/// references (SsTestGenStructManual.generated.cs and SsTestBlittableGenStructManual.generated.cs).
///
/// <para>Two shapes are produced depending on whether the struct is fully blittable:</para>
/// <list type="bullet">
/// <item>Blittable: NativeRef overlays the managed struct directly and marshals via
/// <c>BlittableMarshaller&lt;T&gt;</c>; the partial struct gets <c>[StructLayout(Sequential)]</c>.</item>
/// <item>Non-blittable: NativeRef caches per-field offsets and exposes marshaller/wrapper
/// accessors, with field-by-field <c>ToManaged</c>/<c>FromManaged</c>.</item>
/// </list>
/// </summary>
internal static class StructEmitter
{
	public static string Emit(StructModel model)
	{
		return model.IsBlittable ? EmitBlittable(model) : EmitNonBlittable(model);
	}

	// ---------------------------------------------------------------------------------------
	// Non-blittable struct
	// ---------------------------------------------------------------------------------------

	private static string EmitNonBlittable(StructModel model)
	{
		StringBuilder sb = new();

		sb.AppendLine("#nullable enable");
		sb.AppendLine("using SharpScript;");
		sb.AppendLine("using SharpScript.Interop;");
		sb.AppendLine("using SharpScript.Subclassing;");
		sb.AppendLine("using UnrealEngine.CoreUObject;");
		sb.AppendLine("using UnrealEngine.Intrinsic;");
		sb.AppendLine();
		EmitUtils.EmitNamespace(sb, model.Namespace);

		// NativeRef class.
		sb.AppendLine($"public class {model.NativeRefName}(IntPtr nativePtr)");
		sb.AppendLine($"\t: IStructNativeRef<{model.StructName}>");
		sb.AppendLine("{");
		sb.AppendLine("\tpublic static readonly IntPtr NativeType;");
		sb.AppendLine("\tpublic static readonly int NativeDataSize;");
		foreach (PropertyModel prop in model.Properties)
		{
			sb.AppendLine($"\tprivate static readonly IntPtr {prop.Name}_NativeProp;");
			sb.AppendLine($"\tprivate static readonly int {prop.Name}_Offset;");
		}
		sb.AppendLine();

		EmitNonBlittableStaticCtor(sb, model);

		foreach (PropertyModel prop in model.Properties)
		{
			sb.AppendLine();
			EmitAccessor(sb, prop);
		}

		sb.AppendLine();
		EmitToManaged(sb, model);
		sb.AppendLine();
		EmitFromManaged(sb, model);
		sb.AppendLine();
		EmitNativeRefTail(sb, model, blittable: false);
		sb.AppendLine("}");
		sb.AppendLine();

		// Partial struct marshaller helper.
		EmitMarshallerHelperStruct(sb, model, structLayout: false);

		return sb.ToString();
	}

	private static void EmitNonBlittableStaticCtor(StringBuilder sb, StructModel model)
	{
		sb.AppendLine($"\tstatic {model.NativeRefName}()");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tPropertyDef[] _propertyDefs =");
		sb.AppendLine("\t\t[");
		foreach (PropertyModel prop in model.Properties)
		{
			EmitUtils.EmitPropertyDef(sb, prop);
		}
		sb.AppendLine("\t\t];");
		sb.AppendLine();
		sb.AppendLine("\t\tunsafe");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tfixed (PropertyDef* _propertyDefsPtr = _propertyDefs)");
		sb.AppendLine("\t\t\t{");
		sb.AppendLine("\t\t\t\tNativeType = SubclassingUtils.GenerateStruct(");
		sb.AppendLine($"\t\t\t\t\t\"{model.UnrealName}\",");
		sb.AppendLine("\t\t\t\t\t(IntPtr)_propertyDefsPtr, _propertyDefs.Length);");
		sb.AppendLine("\t\t\t}");
		sb.AppendLine("\t\t}");
		sb.AppendLine();
		sb.AppendLine("\t\tNativeDataSize = TypeInterop.GetStructureSize(NativeType);");
		sb.AppendLine("\t\tPropertyIterator propIter = new PropertyIterator(NativeType);");
		foreach (PropertyModel prop in model.Properties)
		{
			sb.AppendLine($"\t\t{prop.Name}_NativeProp = propIter.FindNext(\"{prop.Name}\");");
			sb.AppendLine($"\t\t{prop.Name}_Offset = TypeInterop.GetPropertyOffset({prop.Name}_NativeProp);");
		}
		sb.AppendLine("\t}");
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
		}
	}

	/// <summary>
	/// Simple value property with get + set via a marshaller's static FromNative/ToNative.
	/// </summary>
	private static void EmitValueAccessor(StringBuilder sb, PropertyModel prop, string marshaller, bool nullable = false)
	{
		string returnType = nullable ? $"{prop.ManagedType}?" : prop.ManagedType;
		sb.AppendLine($"\tpublic {returnType} {prop.Name}");
		sb.AppendLine("\t{");
		sb.AppendLine($"\t\tget => {marshaller}.FromNative(nativePtr + {prop.Name}_Offset);");
		sb.AppendLine($"\t\tset => {marshaller}.ToNative(nativePtr + {prop.Name}_Offset, value);");
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
		string ctor = string.IsNullOrEmpty(constructorArgs)
			? $"new(nativePtr + {prop.Name}_Offset)"
			: $"new(nativePtr + {prop.Name}_Offset, {constructorArgs})";
		sb.AppendLine($"\tpublic {typeName} {prop.Name} => {field} ??= {ctor};");
	}

	private static void EmitToManaged(StringBuilder sb, StructModel model)
	{
		sb.AppendLine($"\tpublic {model.StructName} ToManaged()");
		sb.AppendLine("\t{");
		sb.AppendLine($"\t\treturn new {model.StructName}()");
		sb.AppendLine("\t\t{");
		for (int i = 0; i < model.Properties.Count; i++)
		{
			PropertyModel prop = model.Properties[i];
			string comma = i < model.Properties.Count - 1 ? "," : "";
			// Wrapper kinds (containers, struct ref) rely on implicit conversion to the
			// managed collection / value type; plain values assign directly.
			sb.AppendLine($"\t\t\t{prop.Name} = {prop.Name}{comma}");
		}
		sb.AppendLine("\t\t};");
		sb.AppendLine("\t}");
	}

	private static void EmitFromManaged(StringBuilder sb, StructModel model)
	{
		sb.AppendLine($"\tpublic void FromManaged(in {model.StructName} value)");
		sb.AppendLine("\t{");
		foreach (PropertyModel prop in model.Properties)
		{
			switch (prop.Kind)
			{
				case PropertyKind.Array:
				case PropertyKind.Set:
				case PropertyKind.Map:
				case PropertyKind.StructMap:
				case PropertyKind.StructArray:
					sb.AppendLine($"\t\t{prop.Name}.CopyFrom(value.{prop.Name});");
					break;
				case PropertyKind.StructNativeRef:
					sb.AppendLine($"\t\t{prop.Name}.FromManaged(value.{prop.Name});");
					break;
				default:
					sb.AppendLine($"\t\t{prop.Name} = value.{prop.Name};");
					break;
			}
		}
		sb.AppendLine("\t}");
	}

	// ---------------------------------------------------------------------------------------
	// Blittable struct
	// ---------------------------------------------------------------------------------------

	private static string EmitBlittable(StructModel model)
	{
		StringBuilder sb = new();

		sb.AppendLine("using System.Runtime.InteropServices;");
		sb.AppendLine("using SharpScript;");
		sb.AppendLine("using SharpScript.Interop;");
		sb.AppendLine("using SharpScript.Subclassing;");
		sb.AppendLine("using UnrealEngine.CoreUObject;");
		sb.AppendLine();
		EmitUtils.EmitNamespace(sb, model.Namespace);

		sb.AppendLine($"public class {model.NativeRefName}(IntPtr nativePtr) : IStructNativeRef<{model.StructName}>");
		sb.AppendLine("{");
		sb.AppendLine("\tpublic static readonly IntPtr NativeType;");
		sb.AppendLine();
		sb.AppendLine($"\tstatic {model.NativeRefName}()");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tPropertyDef[] _propertyDefs =");
		sb.AppendLine("\t\t[");
		foreach (PropertyModel prop in model.Properties)
		{
			EmitUtils.EmitPropertyDef(sb, prop);
		}
		sb.AppendLine("\t\t];");
		sb.AppendLine();
		sb.AppendLine("\t\tunsafe");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tfixed (PropertyDef* _propertyDefsPtr = _propertyDefs)");
		sb.AppendLine("\t\t\t{");
		sb.AppendLine("\t\t\t\tNativeType = SubclassingUtils.GenerateStruct(");
		sb.AppendLine($"\t\t\t\t\t\"{model.UnrealName}\",");
		sb.AppendLine("\t\t\t\t\t(IntPtr)_propertyDefsPtr, _propertyDefs.Length);");
		sb.AppendLine("\t\t\t}");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");

		foreach (PropertyModel prop in model.Properties)
		{
			sb.AppendLine();
			sb.AppendLine($"\tpublic unsafe {prop.ManagedType} {prop.Name}");
			sb.AppendLine("\t{");
			sb.AppendLine($"\t\tget => (*({model.StructName}*)nativePtr).{prop.Name};");
			sb.AppendLine($"\t\tset => (*({model.StructName}*)nativePtr).{prop.Name} = value;");
			sb.AppendLine("\t}");
		}

		sb.AppendLine();
		sb.AppendLine($"\tpublic {model.StructName} ToManaged()");
		sb.AppendLine("\t{");
		sb.AppendLine($"\t\treturn BlittableMarshaller<{model.StructName}>.FromNative(nativePtr);");
		sb.AppendLine("\t}");
		sb.AppendLine();
		sb.AppendLine($"\tpublic void FromManaged(in {model.StructName} value)");
		sb.AppendLine("\t{");
		sb.AppendLine($"\t\tBlittableMarshaller<{model.StructName}>.ToNative(nativePtr, value);");
		sb.AppendLine("\t}");
		sb.AppendLine();
		EmitNativeRefTail(sb, model, blittable: true);
		sb.AppendLine("}");
		sb.AppendLine();

		EmitMarshallerHelperStruct(sb, model, structLayout: true);

		return sb.ToString();
	}

	// ---------------------------------------------------------------------------------------
	// Shared
	// ---------------------------------------------------------------------------------------

	/// <summary>Emits CreateInstance / GetNativeDataSize / implicit operator on the NativeRef.</summary>
	private static void EmitNativeRefTail(StringBuilder sb, StructModel model, bool blittable)
	{
		sb.AppendLine($"\tpublic static IStructNativeRef<{model.StructName}> CreateInstance(IntPtr valuePtr)");
		sb.AppendLine("\t{");
		sb.AppendLine($"\t\treturn new {model.NativeRefName}(valuePtr);");
		sb.AppendLine("\t}");
		sb.AppendLine();
		if (blittable)
		{
			sb.AppendLine("\tpublic static unsafe int GetNativeDataSize()");
			sb.AppendLine("\t{");
			sb.AppendLine($"\t\treturn sizeof({model.StructName});");
			sb.AppendLine("\t}");
		}
		else
		{
			sb.AppendLine("\tpublic static int GetNativeDataSize()");
			sb.AppendLine("\t{");
			sb.AppendLine("\t\treturn NativeDataSize;");
			sb.AppendLine("\t}");
		}
		sb.AppendLine();
		sb.AppendLine($"\tpublic static implicit operator {model.StructName}({model.NativeRefName} nativeRef)");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\treturn nativeRef.ToManaged();");
		sb.AppendLine("\t}");
	}

	private static void EmitMarshallerHelperStruct(StringBuilder sb, StructModel model, bool structLayout)
	{
		if (structLayout)
		{
			sb.AppendLine("[StructLayout(LayoutKind.Sequential)]");
		}
		sb.AppendLine($"public partial struct {model.StructName} : IStructMarshallerHelper<{model.StructName}>");
		sb.AppendLine("{");
		sb.AppendLine("\tpublic static int GetNativeDataSize()");
		sb.AppendLine("\t{");
		sb.AppendLine($"\t\treturn {model.NativeRefName}.GetNativeDataSize();");
		sb.AppendLine("\t}");
		sb.AppendLine();
		sb.AppendLine($"\tpublic static IStructNativeRef<{model.StructName}> CreateStructNativeRef(IntPtr valuePtr)");
		sb.AppendLine("\t{");
		sb.AppendLine($"\t\treturn new {model.NativeRefName}(valuePtr);");
		sb.AppendLine("\t}");
		sb.AppendLine("}");
	}

}
