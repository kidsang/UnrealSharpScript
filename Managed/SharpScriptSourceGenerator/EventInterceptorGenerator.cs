using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Implements the "single-method" half of BlueprintEvent support: rewrites source-level calls of a
/// <c>[UFUNCTION(FuncSpecs.BlueprintEvent)]</c> method (via C# interceptors) to the generated
/// <c>Invoke_&lt;Name&gt;</c> virtual-dispatch entry, so blueprint overrides are respected. The call inside
/// the native dispatch stub is deliberately left unintercepted, keeping "source call" and "run body" as two
/// physically distinct paths (no recursion). Interceptors are emitted into the opt-in namespace
/// <c>SharpScript.Generated.Interceptors</c>, one file per declaring class.
/// </summary>
[Generator]
public sealed class EventInterceptorGenerator : IIncrementalGenerator
{
	private const string InterceptorNamespace = "SharpScript.Generated.Interceptors";
	private const string UFunctionAttributeName = "UFUNCTIONAttribute";
	private const string UnmanagedCallersOnlyAttributeName = "UnmanagedCallersOnlyAttribute";
	private const string BlueprintEventMember = "BlueprintEvent";
	private const string BlueprintEventGlueAttributeName = "BlueprintEventGlueAttribute";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValuesProvider<EventCallSite?> callSites = context.SyntaxProvider
			.CreateSyntaxProvider(
				predicate: static (node, _) => node is InvocationExpressionSyntax,
				transform: static (ctx, ct) => TryGetCallSite(ctx, ct))
			.Where(static x => x is not null);

		context.RegisterSourceOutput(callSites.Collect(), static (spc, sites) => Emit(spc, sites));
	}

	private static EventCallSite? TryGetCallSite(GeneratorSyntaxContext ctx, CancellationToken ct)
	{
		var invocation = (InvocationExpressionSyntax)ctx.Node;

		if (ctx.SemanticModel.GetSymbolInfo(invocation, ct).Symbol is not IMethodSymbol method)
		{
			return null;
		}

		// Only instance calls to a BlueprintEvent are intercepted (a receiver is needed for 'this'). This
		// covers both subclassing events ([UFUNCTION(BlueprintEvent)]) and C++ binding glue ([BlueprintEventGlue]).
		if (method.IsStatic || !IsBlueprintEvent(method))
		{
			return null;
		}

		// Skip 'base.<Name>()' call sites. In IL these are non-virtual 'call' to the base's fixed-pointer body
		// (the Super path -> C++ _Implementation). Redirecting them to Invoke_<Name> would re-enter virtual
		// dispatch and recurse. Only 'base.' is exempt; 'this.'/'obj.' calls still get intercepted.
		if (invocation.Expression is MemberAccessExpressionSyntax { Expression: BaseExpressionSyntax })
		{
			return null;
		}

		// Skip the dispatch stub's own call to the body — intercepting it would cause infinite recursion.
		if (IsInsideNativeDispatchStub(invocation, ctx.SemanticModel, ct))
		{
			return null;
		}

		var loc = ctx.SemanticModel.GetInterceptableLocation(invocation, ct);
		if (loc is null)
		{
			return null;
		}

		return new EventCallSite(
			attributeSyntax: loc.GetInterceptsLocationAttributeSyntax(),
			methodName: method.Name,
			returnType: method.ReturnType.ToDisplayString(),
			containingType: method.ContainingType.ToDisplayString(),
			containingTypeName: method.ContainingType.Name,
			parameters: [..method.Parameters.Select(BuildParam)]);
	}

	private static EventParam BuildParam(IParameterSymbol p)
	{
		// ref is treated as out on the subclassing side.
		string modifier = p.RefKind switch
		{
			RefKind.Out => "out ",
			RefKind.Ref => "ref ",
			_ => "",
		};
		return new EventParam(modifier, p.Type.ToDisplayString(), p.Name);
	}

	/// <summary>
	/// Whether the method is an interceptable BlueprintEvent. True when it carries either
	/// <c>[UFUNCTION(FuncSpecs.BlueprintEvent)]</c> (subclassing declaration) or <c>[BlueprintEventGlue]</c>
	/// (C++ binding glue emitted by the SharpScriptBindingGenerator).
	/// </summary>
	private static bool IsBlueprintEvent(IMethodSymbol method)
	{
		if (method.GetAttributes().Any(a => a.AttributeClass?.Name == BlueprintEventGlueAttributeName))
		{
			return true;
		}

		AttributeData? attr = method.GetAttributes()
			.FirstOrDefault(a => a.AttributeClass?.Name == UFunctionAttributeName);
		if (attr == null)
		{
			return false;
		}

		foreach (TypedConstant ctorArg in attr.ConstructorArguments)
		{
			if (ctorArg.Kind == TypedConstantKind.Array)
			{
				foreach (TypedConstant element in ctorArg.Values)
				{
					if (SpecifierContainsBlueprintEvent(element))
					{
						return true;
					}
				}
			}
			else if (ctorArg.Kind == TypedConstantKind.Enum && SpecifierContainsBlueprintEvent(ctorArg))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>Whether a single FuncSpecs constant includes the BlueprintEvent bit.</summary>
	private static bool SpecifierContainsBlueprintEvent(TypedConstant specifier)
	{
		if (specifier.Type is not INamedTypeSymbol enumType || specifier.Value == null)
		{
			return false;
		}

		ulong bits = Convert.ToUInt64(specifier.Value);
		if (bits == 0)
		{
			return false;
		}

		foreach (ISymbol member in enumType.GetMembers())
		{
			if (member.Name != BlueprintEventMember
				|| member is not IFieldSymbol { HasConstantValue: true, ConstantValue: { } cv })
			{
				continue;
			}
			ulong memberBits = Convert.ToUInt64(cv);
			return memberBits != 0 && (bits & memberBits) == memberBits;
		}
		return false;
	}

	/// <summary>Whether the invocation is inside a generated <c>[UnmanagedCallersOnly]</c> dispatch stub.</summary>
	private static bool IsInsideNativeDispatchStub(InvocationExpressionSyntax invocation, SemanticModel model, CancellationToken ct)
	{
		for (SyntaxNode? node = invocation.Parent; node != null; node = node.Parent)
		{
			if (node is MethodDeclarationSyntax methodDecl)
			{
				return model.GetDeclaredSymbol(methodDecl, ct) is { } enclosing
					&& enclosing.GetAttributes().Any(a => a.AttributeClass?.Name == UnmanagedCallersOnlyAttributeName);
			}
		}
		return false;
	}

	/// <summary>
	/// Emits one interceptor file per declaring class (<c>&lt;ClassName&gt;Interceptor.g.cs</c>), each
	/// forwarding its call sites to the generated <c>Invoke_&lt;Name&gt;</c> entry.
	/// </summary>
	private static void Emit(SourceProductionContext spc, ImmutableArray<EventCallSite?> sites)
	{
		if (sites.IsDefaultOrEmpty)
		{
			return;
		}

		// Group by declaring class so each class gets its own interceptor file/class (avoids name clashes).
		foreach (IGrouping<string, EventCallSite> group in sites
			         .Where(s => s is not null)
			         .Select(s => s!.Value)
			         .GroupBy(s => s.ContainingType))
		{
			EmitClassInterceptors(spc, group.First().ContainingTypeName, [..group]);
		}
	}

	private static void EmitClassInterceptors(SourceProductionContext spc, string className, ImmutableArray<EventCallSite> sites)
	{
		string interceptorClass = $"__{className}Interceptor";

		StringBuilder sb = new();
		sb.AppendLine("#nullable enable");
		sb.AppendLine($"namespace {InterceptorNamespace}");
		sb.AppendLine("{");
		sb.AppendLine($"\tfile static class {interceptorClass}");
		sb.AppendLine("\t{");

		for (int i = 0; i < sites.Length; i++)
		{
			EventCallSite s = sites[i];
			string sigParams = string.Join(", ", s.Parameters.Select(p => $"{p.Modifier}{p.Type} {p.Name}"));
			string forwardArgs = string.Join(", ", s.Parameters.Select(p => $"{p.Modifier}{p.Name}"));
			string sep = s.Parameters.Length > 0 ? ", " : "";

			sb.AppendLine($"\t\t{s.AttributeSyntax}");
			sb.AppendLine($"\t\tpublic static {s.ReturnType} Intercept_{i}(this {s.ContainingType} self{sep}{sigParams})");
			sb.AppendLine($"\t\t\t=> self.Invoke_{s.MethodName}({forwardArgs});");
		}

		sb.AppendLine("\t}");
		sb.AppendLine("}");

		// File-local InterceptsLocationAttribute (stable 2-arg form); scoped 'file' so per-class copies never clash.
		sb.AppendLine("namespace System.Runtime.CompilerServices");
		sb.AppendLine("{");
		sb.AppendLine("\t[global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]");
		sb.AppendLine("\tfile sealed class InterceptsLocationAttribute : global::System.Attribute");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tpublic InterceptsLocationAttribute(int version, string data) { }");
		sb.AppendLine("\t}");
		sb.AppendLine("}");

		spc.AddSource($"{className}Interceptor.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
	}

	/// <summary>A single parameter of an intercepted event method.</summary>
	private readonly struct EventParam(string modifier, string type, string name)
	{
		public readonly string Modifier = modifier;
		public readonly string Type = type;
		public readonly string Name = name;
	}

	/// <summary>A collected BlueprintEvent call site to be intercepted.</summary>
	private readonly struct EventCallSite(
		string attributeSyntax,
		string methodName,
		string returnType,
		string containingType,
		string containingTypeName,
		ImmutableArray<EventParam> parameters)
	{
		public readonly string AttributeSyntax = attributeSyntax;
		public readonly string MethodName = methodName;
		public readonly string ReturnType = returnType;
		public readonly string ContainingType = containingType;
		public readonly string ContainingTypeName = containingTypeName;
		public readonly ImmutableArray<EventParam> Parameters = parameters;
	}
}
