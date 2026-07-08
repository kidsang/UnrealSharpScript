using System.Text;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Emits the <c>&lt;Enum&gt;.generated.cs</c> for a [UENUM], mirroring the hand-written
/// reference in SsTestGenEnumManual.generated.cs.
///
/// <para>The generated static class <c>&lt;Enum&gt;NativeRef</c> holds the native <c>UEnum</c>
/// (<c>NativeType</c>), built once in its static constructor from an <c>EnumValueDef[]</c>
/// via <c>SubclassingUtils.GenerateEnum</c>. The enum is passed as a single <c>EnumDef</c> bundle
/// (name, values, flags, specifiers, metadata), aligning with the UClass/UStruct paths. The
/// <c>NativeType</c> is referenced as the underlying <c>UEnum</c> of byte-backed enum properties.</para>
/// </summary>
internal static class EnumEmitter
{
	public static string Emit(EnumModel model)
	{
		StringBuilder sb = new();

		sb.AppendLine("#nullable enable");
		sb.AppendLine("using SharpScript.Subclassing;");
		sb.AppendLine("using UnrealEngine.Intrinsic;");
		sb.AppendLine();

		EmitUtils.EmitNamespace(sb, model.Namespace);

		sb.AppendLine($"public static class {model.NativeRefName}");
		sb.AppendLine("{");
		sb.AppendLine("\tpublic static readonly IntPtr NativeType;");
		sb.AppendLine();
		sb.AppendLine($"\tstatic {model.NativeRefName}()");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tEnumValueDef[] _valueDefs =");
		sb.AppendLine("\t\t[");
		foreach (EnumValueModel value in model.Values)
		{
			sb.AppendLine("\t\t\tnew()");
			sb.AppendLine("\t\t\t{");
			sb.AppendLine($"\t\t\t\tName = \"{value.Name}\",");
			sb.AppendLine($"\t\t\t\tValue = (long){model.EnumName}.{value.Name},");
			sb.AppendLine("\t\t\t},");
		}
		sb.AppendLine("\t\t];");
		sb.AppendLine();
		sb.AppendLine("\t\tunsafe");
		sb.AppendLine("\t\t{");
		EmitEnumDefAndCall(sb, model, "\t\t\t");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");
		sb.AppendLine("}");

		return sb.ToString();
	}

	/// <summary>
	/// Emits, inside an already-open <c>unsafe { }</c> block at <paramref name="indent"/>, the fixed
	/// cascade that pins the enum metadata value strings (WITH_EDITOR guarded) and the value defs,
	/// builds the enum-level <c>MetaDataEntry[]</c>, constructs the <c>EnumDef</c> and calls
	/// <c>SubclassingUtils.GenerateEnum</c>. Mirrors <c>StructEmitter.EmitStructDefAndCall</c>. No heap
	/// marshalling — every string is pinned by the fixed cascade for the duration of the native call.
	/// </summary>
	private static void EmitEnumDefAndCall(StringBuilder sb, EnumModel model, string indent)
	{
		// Pin each metadata value char* (WITH_EDITOR guarded) so the MetaDataEntry* stays valid across
		// the GenerateEnum call.
		string[] metaVarNames = new string[model.Metadata.Count];
		for (int i = 0; i < model.Metadata.Count; i++)
		{
			metaVarNames[i] = $"_metaValue{i}";
			sb.AppendLine($"{indent}#if WITH_EDITOR");
			sb.AppendLine($"{indent}fixed (char* {metaVarNames[i]} = {EmitUtils.ToLiteral(model.Metadata[i].Value)})");
			sb.AppendLine($"{indent}#endif");
		}

		sb.AppendLine($"{indent}fixed (EnumValueDef* _valueDefsPtr = _valueDefs)");
		sb.AppendLine($"{indent}{{");

		string body = indent + "\t";

		// Build the enum-level MetaDataEntry[] (from the pinned metadata value char*s).
		sb.AppendLine($"{body}MetaDataEntry[] _metaEntries =");
		sb.AppendLine($"{body}[");
		for (int i = 0; i < model.Metadata.Count; i++)
		{
			sb.AppendLine($"{body}#if WITH_EDITOR");
			sb.AppendLine($"{body}\tnew() {{ Key = \"{model.Metadata[i].Key}\", Value = {metaVarNames[i]} }},");
			sb.AppendLine($"{body}#endif");
		}
		sb.AppendLine($"{body}];");
		sb.AppendLine();

		sb.AppendLine($"{body}fixed (MetaDataEntry* _metaEntriesPtr = _metaEntries)");
		sb.AppendLine($"{body}{{");
		sb.AppendLine($"{body}\tEnumDef _enumDef = new()");
		sb.AppendLine($"{body}\t{{");
		sb.AppendLine($"{body}\t\tEnumName = \"{model.UnrealName}\",");
		sb.AppendLine($"{body}\t\tValueDefines = (IntPtr)_valueDefsPtr,");
		sb.AppendLine($"{body}\t\tValueCount = _valueDefs.Length,");
		sb.AppendLine($"{body}\t\tIsFlags = {(model.IsFlags ? "1" : "0")},");
		sb.AppendLine($"{body}\t\tSpecifiers = {model.SpecifiersExpr},");
		sb.AppendLine($"{body}\t\tMetaEntries = (IntPtr)_metaEntriesPtr,");
		sb.AppendLine($"{body}\t\tMetaCount = _metaEntries.Length,");
		sb.AppendLine($"{body}\t}};");
		sb.AppendLine($"{body}\tNativeType = SubclassingUtils.GenerateEnum((IntPtr)(&_enumDef));");
		sb.AppendLine($"{body}}}");
		sb.AppendLine($"{indent}}}");
	}
}
