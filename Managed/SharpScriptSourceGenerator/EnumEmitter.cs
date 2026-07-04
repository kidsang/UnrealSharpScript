using System.Text;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Emits the <c>&lt;Enum&gt;.generated.cs</c> for a [UENUM], mirroring the hand-written
/// reference in SsTestGenEnumManual.generated.cs.
///
/// <para>The generated static class <c>&lt;Enum&gt;NativeRef</c> holds the native <c>UEnum</c>
/// (<c>NativeType</c>), built once in its static constructor from an <c>EnumValueDef[]</c>
/// via <c>SubclassingUtils.GenerateEnum</c>. The <c>NativeType</c> is referenced as the
/// underlying <c>UEnum</c> of byte-backed enum properties.</para>
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
		sb.AppendLine("\t\t\tfixed (EnumValueDef* _valueDefsPtr = _valueDefs)");
		sb.AppendLine("\t\t\t{");
		sb.AppendLine("\t\t\t\tNativeType = SubclassingUtils.GenerateEnum(");
		sb.AppendLine($"\t\t\t\t\t\"{model.UnrealName}\",");
		sb.AppendLine($"\t\t\t\t\t(IntPtr)_valueDefsPtr, _valueDefs.Length, {(model.IsFlags ? "1" : "0")});");
		sb.AppendLine("\t\t\t}");
		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");
		sb.AppendLine("}");

		return sb.ToString();
	}
}
