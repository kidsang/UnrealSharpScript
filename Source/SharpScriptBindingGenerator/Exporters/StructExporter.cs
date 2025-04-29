using System.Collections.Generic;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.PropertyTranslators;
using SharpScriptBindingGenerator.Utilities;

namespace SharpScriptBindingGenerator.Exporters;

public static class StructExporter
{
	public static void ExportStruct(UhtScriptStruct structObj)
	{
		// C# does not support struct inheritance, so base structs of UStruct needs to be "flattened" during export.
		List<UhtScriptStruct> inheritedChain = new() { structObj };
		var superStructObj = structObj.SuperScriptStruct;
		while (superStructObj != null)
		{
			inheritedChain.Add(superStructObj);
			superStructObj = superStructObj.SuperScriptStruct;
		}

		// Sort the inheritance chain from parent to child.
		inheritedChain.Reverse();

		// Collect property definitions.
		List<UhtProperty> exportedProperties = new List<UhtProperty>();
		foreach (var scriptStruct in inheritedChain)
		{
			GeneratorUtilities.GetExportedProperties(scriptStruct, exportedProperties);
		}

		using var codeBuilder = new CodeBuilder();
		codeBuilder.AddUsing("#nullable enable");
		codeBuilder.AddUsing("#pragma warning disable CS9113"); // CS9113: Parameter is unread.
		codeBuilder.AddUsing("using System.Runtime.InteropServices;");
		codeBuilder.AddUsing("using UnrealEngine.Intrinsic;");
		codeBuilder.AddUsing("using SharpScript;");
		codeBuilder.AddUsing("using SharpScript.Interop;");
		codeBuilder.AppendNamespace(structObj);

		// Export struct reference.
		bool isBlittable = structObj.IsBlittable();
		if (!isBlittable)
		{
			ExportStructNativeRef(codeBuilder, structObj, exportedProperties);
		}
		else
		{
			ExportBlittableStructNativeRef(codeBuilder, structObj, exportedProperties);
		}

		// Export struct body.
		ExportManagedStruct(codeBuilder, structObj, exportedProperties, isBlittable);

		FileExporter.SaveGeneratedToDisk(structObj, codeBuilder);
	}

	private static void ExportStructNativeRef(CodeBuilder codeBuilder, UhtScriptStruct structObj, List<UhtProperty> exportedProperties)
	{
		string structName = structObj.EngineName;
		string nativeRefName = $"{structName}NativeRef";
		string baseTypeName = $"IStructNativeRef<{structName}>";
		string primaryConstructor = "(IntPtr nativePtr)";
		codeBuilder.AppendTypeDeclare("class", nativeRefName, primaryConstructor: primaryConstructor, baseTypeName: baseTypeName);
		using (new CodeBlock(codeBuilder)) // struct native body
		{
			StaticConstructorUtilities.ExportStaticConstructor(
				codeBuilder,
				structObj,
				exportedProperties);

			ExportNativeRefProterties(codeBuilder, exportedProperties);

			if (exportedProperties.Count > 0)
			{
				codeBuilder.AppendLine();
			}

			// IStructNativeRef.ToManaged
			codeBuilder.AppendLine($"public {structName} ToManaged()");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine($"return new {structName}");
				using (new CodeBlock(codeBuilder))
				{
					ExportStructProtertiesToManaged(codeBuilder, exportedProperties);
				}

				codeBuilder.Append(";");
			}

			// IStructNativeRef.FromManaged
			codeBuilder.AppendLine();
			codeBuilder.AppendLine($"public void FromManaged(in {structName} value)");
			using (new CodeBlock(codeBuilder))
			{
				ExportStructProtertiesFromManaged(codeBuilder, exportedProperties);
			}

			// IStructNativeRef.CreateInstance
			codeBuilder.AppendLine();
			codeBuilder.AppendLine($"public static IStructNativeRef<{structName}> CreateInstance(IntPtr valuePtr)");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine($"return new {nativeRefName}(valuePtr);");
			}

			// IStructNativeRef.GetNativeDataSize
			codeBuilder.AppendLine();
			codeBuilder.AppendLine("public static int GetNativeDataSize()");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine("return NativeDataSize;");
			}

			// Implicit convertor from NativeRef to struct.
			codeBuilder.AppendLine();
			codeBuilder.AppendLine($"public static implicit operator {structName}({nativeRefName} nativeRef)");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine("return nativeRef.ToManaged();");
			}
		}
	}

	private static void ExportBlittableStructNativeRef(CodeBuilder codeBuilder, UhtScriptStruct structObj, List<UhtProperty> exportedProperties)
	{
		string structName = structObj.EngineName;
		string nativeRefName = $"{structName}NativeRef";
		string primaryConstructor = "(IntPtr nativePtr)";
		string baseTypeName = $"IStructNativeRef<{structName}>";
		codeBuilder.AppendTypeDeclare("class", nativeRefName, primaryConstructor: primaryConstructor, baseTypeName: baseTypeName);
		using (new CodeBlock(codeBuilder)) // struct native body
		{
			ExportNativeBlittableRefProterties(codeBuilder, exportedProperties, structName);

			if (exportedProperties.Count > 0)
			{
				codeBuilder.AppendLine();
			}

			// IStructNativeRef.ToManaged
			codeBuilder.AppendLine($"public {structName} ToManaged()");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine($"return BlittableMarshaller<{structName}>.FromNative(nativePtr);");
			}

			// IStructNativeRef.FromManaged
			codeBuilder.AppendLine();
			codeBuilder.AppendLine($"public void FromManaged(in {structName} value)");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine($"BlittableMarshaller<{structName}>.ToNative(nativePtr, value);");
			}

			// IStructNativeRef.CreateInstance
			codeBuilder.AppendLine();
			codeBuilder.AppendLine($"public static IStructNativeRef<{structName}> CreateInstance(IntPtr valuePtr)");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine($"return new {nativeRefName}(valuePtr);");
			}

			// IStructNativeRef.GetNativeDataSize
			codeBuilder.AppendLine();
			codeBuilder.AppendLine("public static unsafe int GetNativeDataSize()");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine($"return sizeof({structName});");
			}

			// Implicit convertor from NativeRef to struct.
			codeBuilder.AppendLine();
			codeBuilder.AppendLine($"public static implicit operator {structName}({nativeRefName} nativeRef)");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine("return nativeRef.ToManaged();");
			}
		}
	}

	private static void ExportManagedStruct(CodeBuilder codeBuilder, UhtScriptStruct structObj, List<UhtProperty> exportedProperties, bool isBlittable)
	{
		codeBuilder.AppendLine();
		codeBuilder.AppendTooltip(structObj);
		if (isBlittable)
		{
			codeBuilder.AppendLine("[StructLayout(LayoutKind.Sequential)]");
		}

		string structName = structObj.EngineName;
		string nativeRefName = $"{structName}NativeRef";
		string baseTypeName = $"IStructMarshallerHelper<{structName}>";
		codeBuilder.AppendTypeDeclare("struct", structName, baseTypeName: baseTypeName);
		using (new CodeBlock(codeBuilder)) // struct body
		{
			ExportStructProterties(codeBuilder, exportedProperties);

			// IStructMarshallerHelper.GetNativeDataSize
			codeBuilder.AppendLine();
			codeBuilder.AppendLine("public static unsafe int GetNativeDataSize()");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine($"return {nativeRefName}.GetNativeDataSize();");
			}

			// IStructMarshallerHelper.CreateStructNativeRef
			codeBuilder.AppendLine();
			codeBuilder.AppendLine($"public static IStructNativeRef<{structName}> CreateStructNativeRef(IntPtr valuePtr)");
			using (new CodeBlock(codeBuilder))
			{
				codeBuilder.AppendLine($"return new {nativeRefName}(valuePtr);");
			}
		}

		codeBuilder.AppendLine();
	}

	private static void ExportNativeRefProterties(CodeBuilder codeBuilder, List<UhtProperty> exportedProperties)
	{
		foreach (UhtProperty property in exportedProperties)
		{
			codeBuilder.AppendLine();
			using var withEditorBlock = new WithEditorBlock(codeBuilder, property);
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
			translator.ExportProperty(codeBuilder, property, forClass: false);
		}
	}

	private static void ExportNativeBlittableRefProterties(CodeBuilder codeBuilder, List<UhtProperty> exportedProperties, string structName)
	{
		bool newline = false;
		foreach (UhtProperty property in exportedProperties)
		{
			if (newline)
			{
				codeBuilder.AppendLine();
			}

			using var withEditorBlock = new WithEditorBlock(codeBuilder, property);
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
			translator.ExportBlittableStructProperty(codeBuilder, property, structName);
			newline = true;
		}
	}

	private static void ExportStructProterties(CodeBuilder codeBuilder, List<UhtProperty> exportedProperties)
	{
		bool newline = false;
		foreach (UhtProperty property in exportedProperties)
		{
			if (newline)
			{
				codeBuilder.AppendLine();
			}

			using var withEditorBlock = new WithEditorBlock(codeBuilder, property);
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
			translator.ExportStructProperty(codeBuilder, property);
			newline = true;
		}
	}

	private static void ExportStructProtertiesToManaged(CodeBuilder codeBuilder, List<UhtProperty> exportedProperties)
	{
		foreach (UhtProperty property in exportedProperties)
		{
			using var withEditorBlock = new WithEditorBlock(codeBuilder, property);
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
			translator.ExportStructPropertyToManaged(codeBuilder, property);
		}
	}

	private static void ExportStructProtertiesFromManaged(CodeBuilder codeBuilder, List<UhtProperty> exportedProperties)
	{
		foreach (UhtProperty property in exportedProperties)
		{
			using var withEditorBlock = new WithEditorBlock(codeBuilder, property);
			PropertyTranslator translator = PropertyTranslatorManager.GetTranslator(property);
			translator.ExportStructPropertyFromManaged(codeBuilder, property);
		}
	}
}
