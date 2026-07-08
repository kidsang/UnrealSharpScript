#include "SsPropertySpecifiers.h"
#include "SsSubclassingUtils.h"

void FSsPropertySpecifiers::Apply(FProperty* Prop, uint64 SpecifierBits, const FSsMetaDataEntry* MetaEntries, int MetaCount)
{
	check(Prop);

	const ESsPropertySpecifier Specifiers = static_cast<ESsPropertySpecifier>(SpecifierBits);

	// Accumulate the property flags, then OR them onto the property in one shot.
	EPropertyFlags Flags = Prop->GetPropertyFlags();

	// --- Config. ---
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::Config))
	{
		Flags |= CPF_Config;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::GlobalConfig))
	{
		Flags |= CPF_GlobalConfig | CPF_Config;
	}

	// --- Transient family. ---
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::Transient))
	{
		Flags |= CPF_Transient;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::DuplicateTransient))
	{
		Flags |= CPF_DuplicateTransient;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::NonPieDuplicateTransient))
	{
		Flags |= CPF_NonPIEDuplicateTransient;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::TextExportTransient))
	{
		Flags |= CPF_TextExportTransient;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::NonTransactional))
	{
		Flags |= CPF_NonTransactional;
	}

	// --- Edit / visibility group (UHT allows only one; the C# generator enforces that at compile time). ---
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::EditAnywhere))
	{
		Flags |= CPF_Edit;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::EditInstanceOnly))
	{
		Flags |= CPF_Edit | CPF_DisableEditOnTemplate;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::EditDefaultsOnly))
	{
		Flags |= CPF_Edit | CPF_DisableEditOnInstance;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::VisibleAnywhere))
	{
		Flags |= CPF_Edit | CPF_EditConst;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::VisibleInstanceOnly))
	{
		Flags |= CPF_Edit | CPF_EditConst | CPF_DisableEditOnTemplate;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::VisibleDefaultsOnly))
	{
		Flags |= CPF_Edit | CPF_EditConst | CPF_DisableEditOnInstance;
	}

	// --- Blueprint visibility (BlueprintReadOnly and BlueprintReadWrite are mutually exclusive). ---
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::BlueprintReadWrite))
	{
		Flags |= CPF_BlueprintVisible;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::BlueprintReadOnly))
	{
		Flags |= CPF_BlueprintVisible | CPF_BlueprintReadOnly;
	}

	// --- Export / editing helpers. ---
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::Export))
	{
		Flags |= CPF_ExportObject;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::NoClear))
	{
		Flags |= CPF_NoClear;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::EditFixedSize))
	{
		Flags |= CPF_EditFixedSize;
	}

	// --- Interp implies Edit + BlueprintVisible + Interp. ---
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::Interp))
	{
		Flags |= CPF_Edit | CPF_BlueprintVisible | CPF_Interp;
	}

	// --- Instanced implies PersistentInstance + ExportObject + InstancedReference (+ EditInline meta). ---
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::Instanced))
	{
		Flags |= CPF_PersistentInstance | CPF_ExportObject | CPF_InstancedReference;
#if WITH_EDITORONLY_DATA
		Prop->SetMetaData(TEXT("EditInline"), TEXT("true"));
#endif
	}

	// --- Detail-panel display. ---
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::SimpleDisplay))
	{
		Flags |= CPF_SimpleDisplay;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::AdvancedDisplay))
	{
		Flags |= CPF_AdvancedDisplay;
	}

	// --- Misc serialization. ---
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::AssetRegistrySearchable))
	{
		Flags |= CPF_AssetRegistrySearchable;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::SaveGame))
	{
		Flags |= CPF_SaveGame;
	}
	if (EnumHasAnyFlags(Specifiers, ESsPropertySpecifier::SkipSerialization))
	{
		Flags |= CPF_SkipSerialization;
	}

	Prop->SetPropertyFlags(Flags);

#if WITH_EDITORONLY_DATA
	// Free-form metadata (DisplayName / Category / user-supplied Meta).
	for (int i = 0; i < MetaCount; ++i)
	{
		const FSsMetaDataEntry& Entry = MetaEntries[i];
		if (Entry.Key.IsNone())
		{
			continue;
		}
		const TCHAR* Value = Entry.Value ? Entry.Value : TEXT("");
		Prop->SetMetaData(*Entry.Key.ToString(), Value);
	}
#endif
}
