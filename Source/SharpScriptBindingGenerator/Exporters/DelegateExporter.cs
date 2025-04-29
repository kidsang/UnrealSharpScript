using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.Exporters;

public static class DelegateExporter
{
	public static void ExportDelegate(UhtFunction function)
	{
		using var codeBuilder = new CodeBuilder();
		codeBuilder.AddUsing("#nullable enable");
		codeBuilder.AddUsing("using UnrealEngine.Intrinsic;");
		codeBuilder.AddUsing("using SharpScript.Interop;");
		codeBuilder.AppendNamespace(function);

		{
			bool unsupported = !GeneratorUtilities.CanExportParameters(function);
			using var unsupportedBlock = new UnsupportedBlock(codeBuilder, unsupported);
			using var withEditorBlock = new WithEditorBlock(codeBuilder, function);

			FunctionExporter functionExporter;

			if (function.Outer is UhtClass outerClass)
			{
				codeBuilder.AppendLine($"public partial class {outerClass.EngineName}");
				using (new CodeBlock(codeBuilder))
				{
					functionExporter = FunctionExporter.ExportDelegateSignature(codeBuilder, function);
				}
			}
			else
			{
				functionExporter = FunctionExporter.ExportDelegateSignature(codeBuilder, function);
			}

			codeBuilder.AppendLine();
			codeBuilder.AppendLine($"public static class {function.GetDelegateInvokerName()}");
			using (new CodeBlock(codeBuilder)) // class body
			{
				StaticConstructorUtilities.ExportDelegateInvokerStaticConstructor(codeBuilder, function);
				codeBuilder.AppendLine();
				functionExporter.ExportDelegateInvoker(codeBuilder);
			}
		}

		codeBuilder.AppendLine();
		FileExporter.SaveGeneratedToDisk(function, codeBuilder);
	}
}
