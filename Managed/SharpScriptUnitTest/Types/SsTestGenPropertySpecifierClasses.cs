using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

[UCLASS]
public partial class USsTestGenPropSpec : UObject
{
	// --- Config family. ---
	[UPROPERTY(PropSpecs.Config)]
	public partial int Config { get; set; }

	[UPROPERTY(PropSpecs.GlobalConfig)]
	public partial int GlobalConfig { get; set; }

	// --- Transient family. ---
	[UPROPERTY(PropSpecs.Transient)]
	public partial int Transient { get; set; }

	[UPROPERTY(PropSpecs.DuplicateTransient)]
	public partial int DuplicateTransient { get; set; }

	[UPROPERTY(PropSpecs.NonPieDuplicateTransient)]
	public partial int NonPieDuplicateTransient { get; set; }

	[UPROPERTY(PropSpecs.TextExportTransient)]
	public partial int TextExportTransient { get; set; }

	[UPROPERTY(PropSpecs.NonTransactional)]
	public partial int NonTransactional { get; set; }

	// --- Edit / visibility group (mutually exclusive; one per property). ---
	[UPROPERTY(PropSpecs.EditAnywhere)]
	public partial int EditAnywhere { get; set; }

	[UPROPERTY(PropSpecs.EditInstanceOnly)]
	public partial int EditInstanceOnly { get; set; }

	[UPROPERTY(PropSpecs.EditDefaultsOnly)]
	public partial int EditDefaultsOnly { get; set; }

	[UPROPERTY(PropSpecs.VisibleAnywhere)]
	public partial int VisibleAnywhere { get; set; }

	[UPROPERTY(PropSpecs.VisibleInstanceOnly)]
	public partial int VisibleInstanceOnly { get; set; }

	[UPROPERTY(PropSpecs.VisibleDefaultsOnly)]
	public partial int VisibleDefaultsOnly { get; set; }

	// --- Blueprint visibility (BlueprintReadOnly and BlueprintReadWrite are mutually exclusive). ---
	[UPROPERTY(PropSpecs.BlueprintReadWrite)]
	public partial int BlueprintReadWrite { get; set; }

	[UPROPERTY(PropSpecs.BlueprintReadOnly)]
	public partial int BlueprintReadOnly { get; set; }

	// --- Export / editing helpers. ---
	[UPROPERTY(PropSpecs.Export)]
	public partial UObject? Export { get; set; }

	[UPROPERTY(PropSpecs.NoClear)]
	public partial UObject? NoClear { get; set; }

	[UPROPERTY(PropSpecs.EditFixedSize)]
	public partial TArray<int> EditFixedSize { get; }

	// --- Interp (implies Edit + BlueprintVisible + Interp). ---
	[UPROPERTY(PropSpecs.Interp)]
	public partial int Interp { get; set; }

	// --- Instanced (implies PersistentInstance + ExportObject + InstancedReference + EditInline meta). ---
	[UPROPERTY(PropSpecs.Instanced)]
	public partial UObject? Instanced { get; set; }

	// --- Detail-panel display. ---
	[UPROPERTY(PropSpecs.SimpleDisplay)]
	public partial int SimpleDisplay { get; set; }

	[UPROPERTY(PropSpecs.AdvancedDisplay)]
	public partial int AdvancedDisplay { get; set; }

	// --- Misc serialization. ---
	[UPROPERTY(PropSpecs.AssetRegistrySearchable)]
	public partial int AssetRegistrySearchable { get; set; }

	[UPROPERTY(PropSpecs.SaveGame)]
	public partial int SaveGame { get; set; }

	[UPROPERTY(PropSpecs.SkipSerialization)]
	public partial int SkipSerialization { get; set; }

	// --- A property with no specifiers: baseline for "no CPF_* bits added". ---
	[UPROPERTY]
	public partial int Plain { get; set; }

	// --- Combined specifiers: every requested bit must OR-fold in together. ---
	[UPROPERTY(PropSpecs.EditAnywhere, PropSpecs.BlueprintReadWrite, PropSpecs.Transient, PropSpecs.SaveGame)]
	public partial int Combined { get; set; }

	// --- Metadata-only specifiers: DisplayName / Category / free-form Meta. ---
	[UPROPERTY(PropSpecs.EditAnywhere,
		DisplayName = "Property Metadata Test",
		Category = "CSharp|Internal",
		Meta = ["ToolTip=Generated for SubclassingSpecifierTest", "CustomFlag"])]
	public partial int Metadata { get; set; }
}
