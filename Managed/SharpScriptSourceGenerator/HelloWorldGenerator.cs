using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace SharpScriptSourceGenerator;

[Generator]
public class HelloWorldGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
			"HelloWorld.g.cs",
			SourceText.From("// this is a test", Encoding.UTF8)));
	}
}
