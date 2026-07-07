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
	// ReSharper restore InconsistentNaming

	public bool RunTest()
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

		return true;
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
