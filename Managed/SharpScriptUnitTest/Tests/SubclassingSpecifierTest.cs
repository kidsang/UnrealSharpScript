using SharpScript.Interop;
using SharpScriptUnitTest.Types;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Tests;

[RecordFilePath]
public class SubclassingSpecifierTest : IUnitTestInterface
{
	// ReSharper disable InconsistentNaming
	// EClassFlags bit values (Engine/Source/Runtime/CoreUObject/Public/UObject/ObjectMacros.h).
	private const uint CLASS_Abstract = 0x00000001u;
	private const uint CLASS_DefaultConfig = 0x00000002u;
	private const uint CLASS_Config = 0x00000004u;
	private const uint CLASS_Transient = 0x00000008u;
	private const uint CLASS_ProjectUserConfig = 0x00000040u;
	private const uint CLASS_NotPlaceable = 0x00000200u;
	private const uint CLASS_PerObjectConfig = 0x00000400u;
	private const uint CLASS_EditInlineNew = 0x00001000u;
	private const uint CLASS_Const = 0x00010000u;
	private const uint CLASS_DefaultToInstanced = 0x00200000u;
	private const uint CLASS_Deprecated = 0x02000000u;
	private const uint CLASS_HideDropDown = 0x04000000u;
	private const uint CLASS_GlobalUserConfig = 0x08000000u;
	private const uint CLASS_ConfigDoNotCheckDefaults = 0x40000000u;

	// EPropertyFlags (CPF_*) bit values (Engine/Source/Runtime/CoreUObject/Public/UObject/ObjectMacros.h).
	private const ulong CPF_Edit = 0x0000000000000001u;
	private const ulong CPF_BlueprintVisible = 0x0000000000000004u;
	private const ulong CPF_ExportObject = 0x0000000000000008u;
	private const ulong CPF_BlueprintReadOnly = 0x0000000000000010u;
	private const ulong CPF_EditFixedSize = 0x0000000000000040u;
	private const ulong CPF_DisableEditOnTemplate = 0x0000000000000800u;
	private const ulong CPF_Transient = 0x0000000000002000u;
	private const ulong CPF_Config = 0x0000000000004000u;
	private const ulong CPF_DisableEditOnInstance = 0x0000000000010000u;
	private const ulong CPF_EditConst = 0x0000000000020000u;
	private const ulong CPF_GlobalConfig = 0x0000000000040000u;
	private const ulong CPF_InstancedReference = 0x0000000000080000u;
	private const ulong CPF_DuplicateTransient = 0x0000000000200000u;
	private const ulong CPF_SaveGame = 0x0000000001000000u;
	private const ulong CPF_NoClear = 0x0000000002000000u;
	private const ulong CPF_Interp = 0x0000000200000000u;
	private const ulong CPF_NonTransactional = 0x0000000400000000u;
	private const ulong CPF_AssetRegistrySearchable = 0x0000010000000000u;
	private const ulong CPF_SimpleDisplay = 0x0000020000000000u;
	private const ulong CPF_AdvancedDisplay = 0x0000040000000000u;
	private const ulong CPF_TextExportTransient = 0x0000400000000000u;
	private const ulong CPF_NonPIEDuplicateTransient = 0x0000800000000000u;
	private const ulong CPF_PersistentInstance = 0x0002000000000000u;
	private const ulong CPF_SkipSerialization = 0x0080000000000000u;
	// ReSharper restore InconsistentNaming

	public bool RunTest()
	{
		TestClassSpecifiers();
		TestPropertySpecifiers();

		return true;
	}

	private static void TestClassSpecifiers()
	{
		// BlueprintType -> BlueprintType=true.
		UClass bpType = GetClass<USsTestGenSpecBlueprintType>();
		Utils.Assert(bpType.HasMetaData("BlueprintType"));
		Utils.Assert(bpType.GetMetaData("BlueprintType") == "true");

		// NotBlueprintType -> NotBlueprintType=false, and BlueprintType is not present.
		UClass notBpType = GetClass<USsTestGenSpecNotBlueprintType>();
		Utils.Assert(notBpType.HasMetaData("NotBlueprintType"));
		Utils.Assert(notBpType.GetMetaData("NotBlueprintType") == "false");
		Utils.Assert(!notBpType.HasMetaData("BlueprintType"));

		// Blueprintable -> IsBlueprintBase=true and (implicitly) BlueprintType=true.
		UClass bpable = GetClass<USsTestGenSpecBlueprintable>();
		Utils.Assert(bpable.HasMetaData("IsBlueprintBase"));
		Utils.Assert(bpable.GetMetaData("IsBlueprintBase") == "true");
		Utils.Assert(bpable.HasMetaData("BlueprintType"));
		Utils.Assert(bpable.GetMetaData("BlueprintType") == "true");

		// NotBlueprintable -> IsBlueprintBase=false, and BlueprintType is not present.
		UClass notBpable = GetClass<USsTestGenSpecNotBlueprintable>();
		Utils.Assert(notBpable.HasMetaData("IsBlueprintBase"));
		Utils.Assert(notBpable.GetMetaData("IsBlueprintBase") == "false");
		Utils.Assert(!notBpable.HasMetaData("BlueprintType"));

		// None of these specifiers should introduce EClassFlags bits.
		Utils.Assert(bpType.GetClassFlags() == notBpType.GetClassFlags());
		Utils.Assert(bpable.GetClassFlags() == notBpable.GetClassFlags());

		// --- Single-bit flag specifiers: each flag must be present on its class. ---
		AssertFlagSet<USsTestGenSpecNotPlaceable>(CLASS_NotPlaceable);
		AssertFlagSet<USsTestGenSpecDefaultToInstanced>(CLASS_DefaultToInstanced);
		AssertFlagSet<USsTestGenSpecConst>(CLASS_Const);
		AssertFlagSet<USsTestGenSpecAbstract>(CLASS_Abstract);
		AssertFlagSet<USsTestGenSpecTransient>(CLASS_Transient);
		AssertFlagSet<USsTestGenSpecPerObjectConfig>(CLASS_PerObjectConfig);
		AssertFlagSet<USsTestGenSpecDefaultConfig>(CLASS_DefaultConfig);
		AssertFlagSet<USsTestGenSpecGlobalUserConfig>(CLASS_GlobalUserConfig);
		AssertFlagSet<USsTestGenSpecProjectUserConfig>(CLASS_ProjectUserConfig);
		AssertFlagSet<USsTestGenSpecConfigDoNotCheckDefaults>(CLASS_ConfigDoNotCheckDefaults);
		AssertFlagSet<USsTestGenSpecEditInlineNew>(CLASS_EditInlineNew);
		AssertFlagSet<USsTestGenSpecHideDropdown>(CLASS_HideDropDown);

		// Deprecated additionally forces NotPlaceable (matches UHT behaviour).
		AssertFlagSet<USsTestGenSpecDeprecated>(CLASS_Deprecated | CLASS_NotPlaceable);

		// Config named argument sets CLASS_Config and the class config name.
		AssertFlagSet<USsTestGenSpecConfig>(CLASS_Config);
		Utils.Assert(GetClass<USsTestGenSpecConfig>().GetClassConfigName() == "Game");

		// --- Cancel-out specifiers: since UObject carries neither flag, the result must lack it. ---
		AssertFlagClear<USsTestGenSpecNonTransient>(CLASS_Transient);
		AssertFlagClear<USsTestGenSpecNotEditInlineNew>(CLASS_EditInlineNew);

		// --- Combined specifiers: every requested bit must OR-fold in together. ---
		AssertFlagSet<USsTestGenSpecCombined>(
			CLASS_NotPlaceable | CLASS_Const | CLASS_Abstract | CLASS_HideDropDown);

		// --- Metadata-only specifiers: verify the editor-only metadata written by the runtime
		//     (FSsClassSpecifiers::Apply) is queryable via the new UClass.GetMetaData/HasMetaData. ---
		UClass metadataClass = GetClass<USsTestGenSpecMetadata>();

		// BlueprintType and Blueprintable both write BlueprintType=true; Blueprintable also writes
		// IsBlueprintBase=true.
		Utils.Assert(metadataClass.HasMetaData("BlueprintType"));
		Utils.Assert(metadataClass.GetMetaData("BlueprintType") == "true");
		Utils.Assert(metadataClass.HasMetaData("IsBlueprintBase"));
		Utils.Assert(metadataClass.GetMetaData("IsBlueprintBase") == "true");

		// DisplayName / Category named arguments map to the well-known metadata keys.
		Utils.Assert(metadataClass.GetMetaData("DisplayName") == "Specifier Metadata Test");
		Utils.Assert(metadataClass.GetMetaData("Category") == "CSharp|Internal");

		// Free-form Meta entries: "Key=Value" and a bare "Key" (treated as "Key=true").
		Utils.Assert(metadataClass.GetMetaData("ToolTip") == "Generated for SubclassingSpecifierTest");
		Utils.Assert(metadataClass.GetMetaData("CustomFlag") == "true");

		// Absent keys report false / empty string.
		Utils.Assert(!metadataClass.HasMetaData("NoSuchMetaKey"));
		Utils.Assert(metadataClass.GetMetaData("NoSuchMetaKey") == string.Empty);

		// Metadata handling must not leak flag bits into the class.
		Utils.Assert((metadataClass.GetClassFlags() & (CLASS_Config | CLASS_Abstract | CLASS_Deprecated)) == 0);
	}

	private static void TestPropertySpecifiers()
	{
		UClass cls = USsTestGenPropSpec.StaticClass.Class!;

		// --- Config family. ---
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.Config), CPF_Config);
		// GlobalConfig implies Config as well.
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.GlobalConfig), CPF_GlobalConfig | CPF_Config);

		// --- Transient family. ---
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.Transient), CPF_Transient);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.DuplicateTransient), CPF_DuplicateTransient);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.NonPieDuplicateTransient), CPF_NonPIEDuplicateTransient);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.TextExportTransient), CPF_TextExportTransient);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.NonTransactional), CPF_NonTransactional);

		// --- Edit / visibility group. ---
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.EditAnywhere), CPF_Edit);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.EditInstanceOnly), CPF_Edit | CPF_DisableEditOnTemplate);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.EditDefaultsOnly), CPF_Edit | CPF_DisableEditOnInstance);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.VisibleAnywhere), CPF_Edit | CPF_EditConst);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.VisibleInstanceOnly),
			CPF_Edit | CPF_EditConst | CPF_DisableEditOnTemplate);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.VisibleDefaultsOnly),
			CPF_Edit | CPF_EditConst | CPF_DisableEditOnInstance);

		// EditInstanceOnly / EditDefaultsOnly must differ only in the disable-edit bit they add.
		AssertPropFlagClear(cls, nameof(USsTestGenPropSpec.EditInstanceOnly), CPF_DisableEditOnInstance | CPF_EditConst);
		AssertPropFlagClear(cls, nameof(USsTestGenPropSpec.EditDefaultsOnly), CPF_DisableEditOnTemplate | CPF_EditConst);
		// EditAnywhere alone must not carry any disable/const bit.
		AssertPropFlagClear(cls, nameof(USsTestGenPropSpec.EditAnywhere),
			CPF_EditConst | CPF_DisableEditOnTemplate | CPF_DisableEditOnInstance);

		// --- Blueprint visibility. ---
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.BlueprintReadWrite), CPF_BlueprintVisible);
		AssertPropFlagClear(cls, nameof(USsTestGenPropSpec.BlueprintReadWrite), CPF_BlueprintReadOnly);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.BlueprintReadOnly),
			CPF_BlueprintVisible | CPF_BlueprintReadOnly);

		// --- Export / editing helpers. ---
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.Export), CPF_ExportObject);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.NoClear), CPF_NoClear);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.EditFixedSize), CPF_EditFixedSize);

		// --- Interp implies Edit + BlueprintVisible + Interp. ---
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.Interp),
			CPF_Edit | CPF_BlueprintVisible | CPF_Interp);

		// --- Instanced implies PersistentInstance + ExportObject + InstancedReference + "EditInline" meta. ---
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.Instanced),
			CPF_PersistentInstance | CPF_ExportObject | CPF_InstancedReference);
		IntPtr instancedProp = FindProp(cls, nameof(USsTestGenPropSpec.Instanced));
		Utils.Assert(TypeInterop.HasPropertyMetaData(instancedProp, "EditInline"));
		Utils.Assert(TypeInterop.GetPropertyMetaData(instancedProp, "EditInline") == "true");

		// --- Detail-panel display. ---
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.SimpleDisplay), CPF_SimpleDisplay);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.AdvancedDisplay), CPF_AdvancedDisplay);

		// --- Misc serialization. ---
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.AssetRegistrySearchable), CPF_AssetRegistrySearchable);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.SaveGame), CPF_SaveGame);
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.SkipSerialization), CPF_SkipSerialization);

		// --- Baseline: a specifier-less property must carry none of the specifier-driven bits. ---
		const ulong AllSpecifierBits =
			CPF_Edit | CPF_BlueprintVisible | CPF_ExportObject | CPF_BlueprintReadOnly | CPF_EditFixedSize |
			CPF_DisableEditOnTemplate | CPF_Transient | CPF_Config | CPF_DisableEditOnInstance | CPF_EditConst |
			CPF_GlobalConfig | CPF_InstancedReference | CPF_DuplicateTransient | CPF_SaveGame | CPF_NoClear |
			CPF_Interp | CPF_NonTransactional | CPF_AssetRegistrySearchable | CPF_SimpleDisplay |
			CPF_AdvancedDisplay | CPF_TextExportTransient | CPF_NonPIEDuplicateTransient | CPF_PersistentInstance |
			CPF_SkipSerialization;
		AssertPropFlagClear(cls, nameof(USsTestGenPropSpec.Plain), AllSpecifierBits);

		// --- Combined specifiers: every requested bit must OR-fold in together. ---
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.Combined),
			CPF_Edit | CPF_BlueprintVisible | CPF_Transient | CPF_SaveGame);

		// --- Metadata: DisplayName / Category / free-form Meta on a property. ---
		IntPtr metaProp = FindProp(cls, nameof(USsTestGenPropSpec.Metadata));
		AssertPropFlagSet(cls, nameof(USsTestGenPropSpec.Metadata), CPF_Edit);
		Utils.Assert(TypeInterop.GetPropertyMetaData(metaProp, "DisplayName") == "Property Metadata Test");
		Utils.Assert(TypeInterop.GetPropertyMetaData(metaProp, "Category") == "CSharp|Internal");
		Utils.Assert(TypeInterop.GetPropertyMetaData(metaProp, "ToolTip") == "Generated for SubclassingSpecifierTest");
		Utils.Assert(TypeInterop.GetPropertyMetaData(metaProp, "CustomFlag") == "true");
		// Absent keys report false / empty string.
		Utils.Assert(!TypeInterop.HasPropertyMetaData(metaProp, "NoSuchMetaKey"));
		Utils.Assert(TypeInterop.GetPropertyMetaData(metaProp, "NoSuchMetaKey") == string.Empty);
	}

	private static IntPtr FindProp(UClass cls, string propName)
	{
		IntPtr prop = TypeInterop.FindProperty(cls.NativeObject, propName);
		Utils.Assert(prop != IntPtr.Zero);
		return prop;
	}

	private static void AssertPropFlagSet(UClass cls, string propName, ulong expectedFlags)
	{
		ulong flags = TypeInterop.GetPropertyFlags(FindProp(cls, propName));
		Utils.Assert((flags & expectedFlags) == expectedFlags);
	}

	private static void AssertPropFlagClear(UClass cls, string propName, ulong clearedFlags)
	{
		ulong flags = TypeInterop.GetPropertyFlags(FindProp(cls, propName));
		Utils.Assert((flags & clearedFlags) == 0);
	}

	private static UClass GetClass<T>() where T : UObject, IStaticClass<T>
	{
		return T.StaticClass.Class!;
	}

	private static uint GetClassFlags<T>() where T : UObject, IStaticClass<T>
	{
		return GetClass<T>().GetClassFlags();
	}

	private static void AssertFlagSet<T>(uint expectedFlags)
		where T : UObject, IStaticClass<T>
	{
		uint flags = GetClassFlags<T>();
		Utils.Assert((flags & expectedFlags) == expectedFlags);
	}

	private static void AssertFlagClear<T>(uint clearedFlags)
		where T : UObject, IStaticClass<T>
	{
		uint flags = GetClassFlags<T>();
		Utils.Assert((flags & clearedFlags) == 0);
	}
}
