using System;
using System.Collections.Generic;
using System.Text;
using EpicGames.Core;
using EpicGames.UHT.Types;
using EpicGames.UHT.Utils;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator;

public class CodeBuilder : IDisposable
{
	private int _indent;

	private readonly List<string> _usings = new();

	private BorrowStringBuilder _borrower = new(StringBuilderCache.Big);

	private StringBuilder Builder => _borrower.StringBuilder;

	public void Dispose()
	{
		_borrower.Dispose();
	}

	public override string ToString()
	{
		string text = Builder.ToString();
		if (_usings.Count > 0)
		{
			StringBuilder newBuilder = StringBuilderCache.Big.Borrow();
			foreach (string line in _usings)
			{
				newBuilder.AppendLine(line);
			}

			newBuilder.AppendLine();
			newBuilder.Append(text);
			text = newBuilder.ToString();
			StringBuilderCache.Big.Return(newBuilder);
		}

		return text;
	}

	public void AddUsing(string text)
	{
		if (!_usings.Contains(text))
		{
			_usings.Add(text);
		}
	}

	public void Append(string text)
	{
		Builder.Append(text);
	}

	public void AppendLine()
	{
		if (Builder.Length > 0)
		{
			Builder.AppendLine();
		}
	}

	public void AppendLineAndIndent()
	{
		AppendLine();
		for (int i = 0; i < _indent; ++i)
		{
			Builder.Append('\t');
		}
	}

	public void AppendLine(string line)
	{
		AppendLineAndIndent();
		Builder.Append(line);
	}

	public void OpenBrace()
	{
		AppendLine("{");
		Indent();
	}

	public void CloseBrace()
	{
		UnIndent();
		AppendLine("}");
	}

	public void Indent()
	{
		++_indent;
	}

	public void UnIndent()
	{
		--_indent;
	}

	public void AppendNamespace(UhtType typeObj)
	{
		AppendLine($"namespace {typeObj.GetNamespace()};");
		AppendLine();
	}

	public void AppendTypeDeclare(string typeName, string declaredTypeName, string? primaryConstructor = null, string? baseTypeName = null, bool isPartial = true, List<UhtClass>? nativeInterfaces = null, List<string>? csInterfaces = null)
	{
		string partialSpecifier = isPartial ? "partial " : string.Empty;
		primaryConstructor ??= string.Empty;

		List<string> inheritingFrom = new List<string>();
		if (!string.IsNullOrEmpty(baseTypeName))
		{
			inheritingFrom.Add(baseTypeName);
		}

		if (nativeInterfaces != null)
		{
			foreach (UhtClass @interface in nativeInterfaces)
			{
				string fullInterfaceName = @interface.GetInterfaceFullManagedName();
				inheritingFrom.Add(fullInterfaceName);
			}
		}

		if (csInterfaces != null)
		{
			foreach (string @interface in csInterfaces)
			{
				inheritingFrom.Add(@interface);
			}
		}

		string inheritanceSpecifier = inheritingFrom.Count > 0 ? $" : {string.Join(", ", inheritingFrom)}" : string.Empty;

		AppendLine($"public {partialSpecifier}{typeName} {declaredTypeName}{primaryConstructor}{inheritanceSpecifier}");
	}
}

/// <summary>
/// Indent and then unindent.
/// </summary>
public struct IndentBlock : IDisposable
{
	private CodeBuilder? _builder;

	public IndentBlock()
	{
		throw new InvalidOperationException("Use the constructor with the CodeBuilder parameter");
	}

	public IndentBlock(CodeBuilder builder)
	{
		_builder = builder;
		_builder.Indent();
	}

	public void Dispose()
	{
		if (_builder != null)
		{
			_builder.UnIndent();
			_builder = null;
		}
	}
}

/// <summary>
/// Code block area with braces.
/// </summary>
public struct CodeBlock : IDisposable
{
	private CodeBuilder? _builder;

	public CodeBlock()
	{
		throw new InvalidOperationException("Use the constructor with the CodeBuilder parameter");
	}

	public CodeBlock(CodeBuilder builder)
	{
		_builder = builder;
		_builder.OpenBrace();
	}

	public void Dispose()
	{
		if (_builder != null)
		{
			_builder.CloseBrace();
			_builder = null;
		}
	}
}

/// <summary>
/// Conditional macro block (#if WITH_EDITOR #endif)
/// </summary>
public struct WithEditorBlock : IDisposable
{
	private CodeBuilder? _builder = null;

	public WithEditorBlock()
	{
		throw new InvalidOperationException("Use the constructor with the CodeBuilder parameter");
	}

	public WithEditorBlock(CodeBuilder builder, UhtProperty property)
	{
		if (property.HasAnyFlags(EPropertyFlags.EditorOnly))
		{
			_builder = builder;
			_builder.AppendLine("#if WITH_EDITOR");
		}
	}

	public WithEditorBlock(CodeBuilder builder, UhtFunction function)
	{
		if (function.HasAnyFlags(EFunctionFlags.EditorOnly))
		{
			_builder = builder;
			_builder.AppendLine("#if WITH_EDITOR");
		}
	}

	public void Dispose()
	{
		if (_builder != null)
		{
			_builder.AppendLine("#endif");
			_builder = null;
		}
	}
}

/// <summary>
/// Comment out unsupported properties or methods (/** unsupported */)
/// </summary>
public struct UnsupportedBlock : IDisposable
{
	private CodeBuilder? _builder = null;

	public UnsupportedBlock()
	{
		throw new InvalidOperationException("Use the constructor with the CodeBuilder parameter");
	}

	public UnsupportedBlock(CodeBuilder builder, bool unsupported = true)
	{
		if (unsupported)
		{
			_builder = builder;
			_builder.AppendLine("/** unsupported");
		}
	}

	public void Dispose()
	{
		if (_builder != null)
		{
			_builder.AppendLine("*/");
			_builder = null;
		}
	}
}
