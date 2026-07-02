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
		sb.AppendLine("using SharpScript;");
		sb.AppendLine("using SharpScript.Interop;");
		sb.AppendLine("using SharpScript.Subclassing;");
		sb.AppendLine("using UnrealEngine.CoreUObject;");
		sb.AppendLine("using UnrealEngine.Intrinsic;");
		sb.AppendLine();

		bool hasNamespace = !string.IsNullOrEmpty(model.Namespace);
		if (hasNamespace)
		{
			sb.AppendLine($"namespace {model.Namespace};");
			sb.AppendLine();
		}

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
	}

	private static void EmitStaticConstructor(StringBuilder sb, ClassModel model)
	{
		sb.AppendLine($"\tstatic {model.ClassName}()");
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
		sb.AppendLine("\t\t\t\tNativeType = SubclassingUtils.GenerateClass(");
		sb.AppendLine($"\t\t\t\t\tRuntimeTypeHandle.ToIntPtr(typeof({model.ClassName}).TypeHandle),");
		sb.AppendLine($"\t\t\t\t\t\"{model.UnrealName}\",");
		sb.AppendLine($"\t\t\t\t\t{model.SuperClass}.StaticClass.NativeClass,");
		sb.AppendLine("\t\t\t\t\t(IntPtr)_propertyDefsPtr, _propertyDefs.Length);");
		sb.AppendLine("\t\t\t}");
		sb.AppendLine("\t\t}");
		sb.AppendLine();
		sb.AppendLine($"\t\tStaticClass = new TSubclassOf<{model.ClassName}>(NativeType);");
		sb.AppendLine($"\t\tHouseKeeper.AddBindedUnrealClass(StaticClass.Class!, typeof({model.ClassName}));");
		sb.AppendLine();
		sb.AppendLine("\t\tPropertyIterator propIter = new PropertyIterator(NativeType);");
		foreach (PropertyModel prop in model.Properties)
		{
			sb.AppendLine($"\t\t{prop.Name}_NativeProp = propIter.FindNext(\"{prop.Name}\");");
			sb.AppendLine($"\t\t{prop.Name}_Offset = TypeInterop.GetPropertyOffset({prop.Name}_NativeProp);");
		}
		sb.AppendLine("\t}");
	}

	private static void EmitAccessor(StringBuilder sb, PropertyModel prop)
	{
		switch (prop.Kind)
		{
			case PropertyKind.Bool:
				EmitValueAccessor(sb, prop, "BoolMarshaller");
				break;
			case PropertyKind.Blittable:
				EmitValueAccessor(sb, prop, $"BlittableMarshaller<{prop.ManagedType}>");
				break;
			case PropertyKind.Enum:
				EmitValueAccessor(sb, prop, $"EnumMarshaller<{prop.ManagedType}>");
				break;
			case PropertyKind.String:
				EmitValueAccessor(sb, prop, "StringMarshaller");
				break;
			case PropertyKind.Name:
				EmitValueAccessor(sb, prop, "NameMarshaller");
				break;
			case PropertyKind.Text:
				EmitValueAccessor(sb, prop, "TextMarshaller");
				break;
			case PropertyKind.Object:
				EmitValueAccessor(sb, prop, $"ObjectMarshaller<{prop.TargetType}>", prop.IsNullable);
				break;
			case PropertyKind.SoftObjectPtr:
				EmitValueAccessor(sb, prop, $"SoftObjectPtrMarshaller<{prop.TargetType}>");
				break;
			case PropertyKind.LazyObjectPtr:
				EmitValueAccessor(sb, prop, $"LazyObjectPtrMarshaller<{prop.TargetType}>");
				break;
			case PropertyKind.SubclassOf:
				EmitValueAccessor(sb, prop, $"SubclassOfMarshaller<{prop.TargetType}>");
				break;
			case PropertyKind.SoftClassPtr:
				EmitValueAccessor(sb, prop, $"SoftClassPtrMarshaller<{prop.TargetType}>");
				break;
			case PropertyKind.Array:
				EmitArrayAccessor(sb, prop);
				break;
			case PropertyKind.StructArray:
				EmitStructArrayAccessor(sb, prop);
				break;
			case PropertyKind.Set:
				EmitSetAccessor(sb, prop);
				break;
			case PropertyKind.Map:
				EmitMapAccessor(sb, prop);
				break;
			case PropertyKind.StructMap:
				EmitStructMapAccessor(sb, prop);
				break;
			case PropertyKind.StructNativeRef:
				EmitStructNativeRefAccessor(sb, prop);
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

	private static void EmitArrayAccessor(StringBuilder sb, PropertyModel prop)
	{
		string field = NamingUtils.BackingFieldName(prop.Name);
		sb.AppendLine($"\tprivate {prop.ManagedType}? {field};");
		sb.AppendLine();
		sb.AppendLine($"\tpublic partial {prop.ManagedType} {prop.Name}");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tget");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tThrowIfNotValid();");
		sb.AppendLine($"\t\t\treturn {field} ??= new(NativeObject + {prop.Name}_Offset, {prop.Name}_NativeProp, {prop.Inner!.MarshallerInstanceExpr});");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");
	}

	private static void EmitStructArrayAccessor(StringBuilder sb, PropertyModel prop)
	{
		string field = NamingUtils.BackingFieldName(prop.Name);
		sb.AppendLine($"\tprivate {prop.ManagedType}? {field};");
		sb.AppendLine();
		sb.AppendLine($"\tpublic partial {prop.ManagedType} {prop.Name}");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tget");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tThrowIfNotValid();");
		sb.AppendLine($"\t\t\treturn {field} ??= new(NativeObject + {prop.Name}_Offset, {prop.Name}_NativeProp);");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");
	}

	private static void EmitSetAccessor(StringBuilder sb, PropertyModel prop)
	{
		string field = NamingUtils.BackingFieldName(prop.Name);
		sb.AppendLine($"\tprivate {prop.ManagedType}? {field};");
		sb.AppendLine();
		sb.AppendLine($"\tpublic partial {prop.ManagedType} {prop.Name}");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tget");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tThrowIfNotValid();");
		sb.AppendLine($"\t\t\treturn {field} ??= new(NativeObject + {prop.Name}_Offset, {prop.Name}_NativeProp, {prop.Inner!.MarshallerInstanceExpr});");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");
	}

	private static void EmitMapAccessor(StringBuilder sb, PropertyModel prop)
	{
		string field = NamingUtils.BackingFieldName(prop.Name);
		sb.AppendLine($"\tprivate {prop.ManagedType}? {field};");
		sb.AppendLine();
		sb.AppendLine($"\tpublic partial {prop.ManagedType} {prop.Name}");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tget");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tThrowIfNotValid();");
		sb.AppendLine($"\t\t\treturn {field} ??= new(NativeObject + {prop.Name}_Offset, {prop.Name}_NativeProp, {prop.Key!.MarshallerInstanceExpr}, {prop.Inner!.MarshallerInstanceExpr});");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");
	}

	private static void EmitStructMapAccessor(StringBuilder sb, PropertyModel prop)
	{
		// Struct-value map: TMap<K, V, VRef>. The wrapper marshals the struct value through
		// its native-ref, so the constructor takes only the key marshaller.
		string field = NamingUtils.BackingFieldName(prop.Name);
		sb.AppendLine($"\tprivate {prop.ManagedType}? {field};");
		sb.AppendLine();
		sb.AppendLine($"\tpublic partial {prop.ManagedType} {prop.Name}");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tget");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tThrowIfNotValid();");
		sb.AppendLine($"\t\t\treturn {field} ??= new(NativeObject + {prop.Name}_Offset, {prop.Name}_NativeProp, {prop.Key!.MarshallerInstanceExpr});");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");
	}

	private static void EmitStructNativeRefAccessor(StringBuilder sb, PropertyModel prop)
	{
		string field = NamingUtils.BackingFieldName(prop.Name);
		sb.AppendLine($"\tprivate {prop.NativeRefType}? {field};");
		sb.AppendLine();
		sb.AppendLine($"\tpublic partial {prop.NativeRefType} {prop.Name}");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tget");
		sb.AppendLine("\t\t{");
		sb.AppendLine("\t\t\tThrowIfNotValid();");
		sb.AppendLine($"\t\t\treturn {field} ??= new(NativeObject + {prop.Name}_Offset);");
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
