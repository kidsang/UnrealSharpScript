using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Shared helpers for analysing Roslyn symbols: attribute detection, inheritance
/// checks and 'partial' validation. Extracted from the per-kind generators so the
/// [UCLASS] and [USTRUCT] paths share a single implementation.
/// </summary>
internal static class SymbolUtils
{
	private const string UPropertyAttributeName = "UPROPERTYAttribute";
	private const string UFunctionAttributeName = "UFUNCTIONAttribute";

	/// <summary>
	/// True when the symbol carries a <c>[UPROPERTY]</c> attribute.
	/// </summary>
	public static bool HasUPropertyAttribute(ISymbol symbol)
	{
		foreach (AttributeData attr in symbol.GetAttributes())
		{
			if (attr.AttributeClass?.Name == UPropertyAttributeName)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// True when the symbol carries a <c>[UFUNCTION]</c> attribute.
	/// </summary>
	public static bool HasUFunctionAttribute(ISymbol symbol)
	{
		foreach (AttributeData attr in symbol.GetAttributes())
		{
			if (attr.AttributeClass?.Name == UFunctionAttributeName)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// True when the type derives from <c>UObject</c>.
	/// </summary>
	/// <param name="type">The type to inspect.</param>
	/// <param name="includeSelf">
	/// When true, the type itself is considered (and <c>UObjectBase</c> also matches);
	/// this mirrors the classifier's element check. When false, the walk starts at the
	/// base type and only matches <c>UObject</c>; this mirrors the class-generator check.
	/// </param>
	public static bool IsUObjectDerived(ITypeSymbol type, bool includeSelf)
	{
		for (ITypeSymbol? t = includeSelf ? type : type.BaseType; t != null; t = t.BaseType)
		{
			if (t.Name == "UObject")
			{
				return true;
			}
			if (includeSelf && t.Name == "UObjectBase")
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// True when any declaring syntax of the type carries the <c>partial</c> modifier.
	/// Works for both class and struct declarations (both derive from
	/// <see cref="BaseTypeDeclarationSyntax"/>).
	/// </summary>
	public static bool IsPartial(INamedTypeSymbol symbol)
	{
		foreach (SyntaxReference reference in symbol.DeclaringSyntaxReferences)
		{
			if (reference.GetSyntax() is BaseTypeDeclarationSyntax decl
				&& decl.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
			{
				return true;
			}
		}
		return false;
	}
}

/// <summary>
/// Shared naming helpers: UE type-prefix stripping and backing-field naming.
/// Extracted so the class/struct generators and emitters share one implementation.
/// </summary>
internal static class NamingUtils
{
	/// <summary>
	/// Strips a leading UE type prefix (e.g. <c>U</c>/<c>A</c> for classes, <c>F</c> for
	/// structs) when it is followed by an upper-case letter.
	/// e.g. "USsTestGenClassManual" → "SsTestGenClassManual".
	/// </summary>
	/// <param name="name">The C# type name.</param>
	/// <param name="prefixes">The set of accepted leading prefix characters.</param>
	public static string StripTypePrefix(string name, params char[] prefixes)
	{
		if (name.Length > 1 && char.IsUpper(name[1]) && Array.IndexOf(prefixes, name[0]) >= 0)
		{
			return name.Substring(1);
		}
		return name;
	}

	/// <summary>
	/// Produces the private backing-field name for a property: an underscore followed by
	/// the property name with a lower-cased first letter. e.g. "Items" → "_items".
	/// </summary>
	public static string BackingFieldName(string propName)
	{
		if (propName.Length == 0)
		{
			return "_";
		}
		return "_" + char.ToLowerInvariant(propName[0]) + propName.Substring(1);
	}

	/// <summary>
	/// Lower-cases the first character of <paramref name="s"/> for use as a local variable
	/// name, e.g. "FuncName" → "funcName". Empty string returns as-is.
	/// </summary>
	public static string LowerFirst(string s)
	{
		return s.Length == 0 ? s : char.ToLowerInvariant(s[0]) + s.Substring(1);
	}
}

/// <summary>
/// Shared code-emission helpers used by all emitters.
/// </summary>
internal static class EmitUtils
{
	/// <summary>
	/// Emits a <c>namespace Xxx;</c> file-scoped declaration when <paramref name="ns"/>
	/// is non-empty, followed by a blank line. A no-op for global namespace types.
	/// </summary>
	public static void EmitNamespace(StringBuilder sb, string ns)
	{
		if (!string.IsNullOrEmpty(ns))
		{
			sb.AppendLine($"namespace {ns};");
			sb.AppendLine();
		}
	}

	/// <summary>
	/// Renders a string as a C# double-quoted string literal (including the surrounding quotes),
	/// escaping backslashes, quotes and common control characters. Used to embed metadata values.
	/// </summary>
	public static string ToLiteral(string value)
	{
		StringBuilder sb = new();
		sb.Append('"');
		foreach (char c in value)
		{
			switch (c)
			{
				case '\\': sb.Append("\\\\"); break;
				case '"': sb.Append("\\\""); break;
				case '\n': sb.Append("\\n"); break;
				case '\r': sb.Append("\\r"); break;
				case '\t': sb.Append("\\t"); break;
				case '\0': sb.Append("\\0"); break;
				default: sb.Append(c); break;
			}
		}
		sb.Append('"');
		return sb.ToString();
	}

	/// <summary>
	/// True when the <see cref="PropertyKind"/> is a simple value marshalled through
	/// a single static <c>XMarshaller.FromNative/ToNative</c> pair (i.e. not a container
	/// or struct-native-ref wrapper).
	/// </summary>
	public static bool IsSimpleValueKind(PropertyKind kind)
	{
		return kind switch
		{
			PropertyKind.Bool
				or PropertyKind.Blittable
				or PropertyKind.Enum
				or PropertyKind.String
				or PropertyKind.Name
				or PropertyKind.Text
				or PropertyKind.Object
				or PropertyKind.SoftObjectPtr
				or PropertyKind.LazyObjectPtr
				or PropertyKind.SubclassOf
				or PropertyKind.SoftClassPtr
				=> true,
			_ => false,
		};
	}

	/// <summary>
	/// Returns the marshaller type name for a simple value <see cref="PropertyModel"/>,
	/// e.g. <c>"BoolMarshaller"</c>, <c>"ObjectMarshaller&lt;UObject&gt;"</c>.
	/// Must only be called when <see cref="IsSimpleValueKind"/> returns true.
	/// </summary>
	public static string GetValueMarshallerName(PropertyModel prop)
	{
		return prop.Kind switch
		{
			PropertyKind.Bool => "BoolMarshaller",
			PropertyKind.Blittable => $"BlittableMarshaller<{prop.ManagedType}>",
			PropertyKind.Enum => $"EnumMarshaller<{prop.ManagedType}>",
			PropertyKind.String => "StringMarshaller",
			PropertyKind.Name => "NameMarshaller",
			PropertyKind.Text => "TextMarshaller",
			PropertyKind.Object => $"ObjectMarshaller<{prop.TargetType}>",
			PropertyKind.SoftObjectPtr => $"SoftObjectPtrMarshaller<{prop.TargetType}>",
			PropertyKind.LazyObjectPtr => $"LazyObjectPtrMarshaller<{prop.TargetType}>",
			PropertyKind.SubclassOf => $"SubclassOfMarshaller<{prop.TargetType}>",
			PropertyKind.SoftClassPtr => $"SoftClassPtrMarshaller<{prop.TargetType}>",
			_ => throw new ArgumentException($"PropertyKind.{prop.Kind} is not a simple value kind."),
		};
	}

	/// <summary>
	/// Shared PropType / UnderlyingType / Inner / Key emission for both
	/// <c>PropertyDef</c> and <c>FunctionParamDef</c> initializers.
	/// The caller is responsible for emitting <c>new() {{</c> and the
	/// name line (<c>PropName</c> or <c>ParamName</c>) before calling this.
	/// </summary>
	public static void EmitTypeDefBlock(StringBuilder sb, PropertyModel prop)
	{
		sb.AppendLine($"\t\t\t\tPropType = {prop.PropTypeClass}.StaticClass.NativeClass,");

		if (prop.UnderlyingTypeExpr != null)
		{
			sb.AppendLine($"\t\t\t\tUnderlyingType = {prop.UnderlyingTypeExpr},");
		}

		if (prop.Inner != null)
		{
			sb.AppendLine($"\t\t\t\tInnerPropType = {prop.Inner.PropTypeClass}.StaticClass.NativeClass,");
			if (prop.Inner.UnderlyingTypeExpr != null)
			{
				sb.AppendLine($"\t\t\t\tInnerUnderlyingType = {prop.Inner.UnderlyingTypeExpr},");
			}
		}

		if (prop.Key != null)
		{
			sb.AppendLine($"\t\t\t\tKeyPropType = {prop.Key.PropTypeClass}.StaticClass.NativeClass,");
			if (prop.Key.UnderlyingTypeExpr != null)
			{
				sb.AppendLine($"\t\t\t\tKeyUnderlyingType = {prop.Key.UnderlyingTypeExpr},");
			}
		}
	}

	/// <summary>
	/// Emits a single <c>PropertyDef</c> initializer into the static-constructor array. The
	/// <c>Specifiers</c> bit set is emitted inline; per-property metadata (<c>MetaEntries</c> /
	/// <c>MetaCount</c>) is NOT set here — the emitter assigns it inside the pinned fixed cascade
	/// (see <c>ClassEmitter</c>) because the metadata value strings must stay pinned across the
	/// GenerateClass call.
	/// </summary>
	public static void EmitPropertyDef(StringBuilder sb, PropertyModel prop)
	{
		sb.AppendLine("\t\t\tnew()");
		sb.AppendLine("\t\t\t{");
		sb.AppendLine($"\t\t\t\tPropName = \"{prop.Name}\",");
		EmitTypeDefBlock(sb, prop);
		if (prop.SpecifierNames.Count > 0)
		{
			sb.AppendLine($"\t\t\t\tSpecifiers = {prop.SpecifiersExpr},");
		}
		sb.AppendLine("\t\t\t},");
	}
}

/// <summary>
/// Shared parsing of the flag-enum + metadata attribute shape used by both <c>[UCLASS]</c> and
/// <c>[UPROPERTY]</c> (constructor <c>params TSpec[] specifiers</c> plus named
/// <c>DisplayName</c> / <c>Category</c> / <c>Meta</c>). Purely a transport step: specifier bits are
/// decomposed into their single-bit member names and metadata is collected verbatim; no bit is
/// interpreted here (the C++ layer expands them).
/// </summary>
internal static class AttributeParsing
{
	/// <summary>
	/// Collects the single-bit specifier member names from the attribute's <c>params TSpec[]</c>
	/// constructor argument(s) into <paramref name="specifierNames"/> (deduplicated, order preserved).
	/// </summary>
	public static void CollectSpecifierNames(AttributeData attr, List<string> specifierNames)
	{
		foreach (TypedConstant ctorArg in attr.ConstructorArguments)
		{
			if (ctorArg.Kind == TypedConstantKind.Array)
			{
				foreach (TypedConstant element in ctorArg.Values)
				{
					AddSpecifierName(specifierNames, element);
				}
			}
			else if (ctorArg.Kind == TypedConstantKind.Enum)
			{
				AddSpecifierName(specifierNames, ctorArg);
			}
		}
	}

	/// <summary>
	/// Reads the named <c>DisplayName</c> / <c>Category</c> / <c>Meta</c> arguments into
	/// <paramref name="metadata"/>. DisplayName / Category map to their well-known metadata keys;
	/// each <c>Meta</c> entry is "Key=Value" (a bare "Key" becomes "Key=true").
	/// </summary>
	public static void CollectMetadata(AttributeData attr, List<(string Key, string Value)> metadata)
	{
		foreach (KeyValuePair<string, TypedConstant> named in attr.NamedArguments)
		{
			switch (named.Key)
			{
				case "DisplayName":
					AddMetadataIfNonEmpty(metadata, "DisplayName", named.Value.Value as string);
					break;
				case "Category":
					AddMetadataIfNonEmpty(metadata, "Category", named.Value.Value as string);
					break;
				case "Meta" when named.Value.Kind == TypedConstantKind.Array:
					foreach (TypedConstant entry in named.Value.Values)
					{
						AddMetaEntry(metadata, entry.Value as string);
					}
					break;
			}
		}
	}

	/// <summary>
	/// Resolves the enum member name(s) for a single specifier constant and records them. A single
	/// named flag is the common case; a combined constant is decomposed into its single-bit members.
	/// </summary>
	private static void AddSpecifierName(List<string> specifierNames, TypedConstant specifier)
	{
		if (specifier.Type is not INamedTypeSymbol enumType || specifier.Value == null)
		{
			return;
		}

		ulong bits = Convert.ToUInt64(specifier.Value);
		if (bits == 0)
		{
			return;
		}

		foreach (ISymbol member in enumType.GetMembers())
		{
			if (member is not IFieldSymbol { HasConstantValue: true, ConstantValue: { } cv })
			{
				continue;
			}

			ulong memberBits = Convert.ToUInt64(cv);
			if (memberBits != 0 && (bits & memberBits) == memberBits)
			{
				if (!specifierNames.Contains(member.Name))
				{
					specifierNames.Add(member.Name);
				}
			}
		}
	}

	/// <summary>Adds a "Key=Value" (or bare "Key" =&gt; "true") metadata entry, skipping blanks.</summary>
	private static void AddMetaEntry(List<(string Key, string Value)> metadata, string? raw)
	{
		if (string.IsNullOrWhiteSpace(raw))
		{
			return;
		}

		int eq = raw!.IndexOf('=');
		if (eq < 0)
		{
			AddMetadataIfNonEmpty(metadata, raw.Trim(), "true");
		}
		else
		{
			string key = raw.Substring(0, eq).Trim();
			string value = raw.Substring(eq + 1);
			AddMetadataIfNonEmpty(metadata, key, value);
		}
	}

	/// <summary>Appends a metadata pair when the key is non-empty; a null value is normalized to "".</summary>
	private static void AddMetadataIfNonEmpty(List<(string Key, string Value)> metadata, string key, string? value)
	{
		if (!string.IsNullOrEmpty(key))
		{
			metadata.Add((key, value ?? ""));
		}
	}

	/// <summary>
	/// Validates that <paramref name="specifierNames"/> contains no two members of any mutually
	/// exclusive group in <paramref name="groups"/>. For each violated group, invokes
	/// <paramref name="reportConflict"/> with the comma-joined conflicting members (already prefixed).
	/// Returns false if any conflict was found.
	/// </summary>
	public static bool ValidateMutuallyExclusive(
		IReadOnlyList<string> specifierNames,
		string enumPrefix,
		string[][] groups,
		Action<string> reportConflict)
	{
		if (specifierNames.Count < 2)
		{
			return true;
		}

		HashSet<string> present = new(specifierNames);
		bool valid = true;

		foreach (string[] group in groups)
		{
			List<string> conflicting = new();
			foreach (string member in group)
			{
				if (present.Contains(member))
				{
					conflicting.Add(member);
				}
			}

			if (conflicting.Count >= 2)
			{
				string joined = string.Join(", ", conflicting.Select(name => $"{enumPrefix}.{name}"));
				reportConflict(joined);
				valid = false;
			}
		}

		return valid;
	}
}

/// <summary>
/// Thin index-able wrapper over an immutable array of type arguments to keep
/// the classifier code readable without taking a hard dependency on the exact
/// ImmutableArray API surface in this file.
/// </summary>
internal readonly struct ImmutableArrayLike<T>(System.Collections.Immutable.ImmutableArray<T> items)
{
	public T this[int index] => items[index];

	public int Length => items.Length;
}
