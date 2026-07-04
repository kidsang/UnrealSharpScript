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
}

/// <summary>
/// Shared code-emission helpers used by both the class and struct emitters.
/// </summary>
internal static class EmitUtils
{
	/// <summary>
	/// Emits a single <c>PropertyDef</c> initializer into the static-constructor array.
	/// Every member line carries a trailing comma (a trailing comma is legal C# in an
	/// object/collection initializer), which keeps the emitter branches simple and the
	/// output identical between the class and struct paths.
	/// </summary>
	public static void EmitPropertyDef(StringBuilder sb, PropertyModel prop)
	{
		sb.AppendLine("\t\t\tnew()");
		sb.AppendLine("\t\t\t{");
		sb.AppendLine($"\t\t\t\tPropName = \"{prop.Name}\",");
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

		sb.AppendLine("\t\t\t},");
	}
}
