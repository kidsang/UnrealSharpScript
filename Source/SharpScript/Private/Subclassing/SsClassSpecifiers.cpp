#include "SsClassSpecifiers.h"
#include "SsCommon.h"
#include "SsSubclassingUtils.h"

void FSsClassSpecifiers::Apply(UClass* Class, uint64 SpecifierBits, const FSsMetaDataEntry* MetaEntries, int MetaCount)
{
	check(Class);

	const ESsClassSpecifier Specifiers = static_cast<ESsClassSpecifier>(SpecifierBits);

#if WITH_EDITORONLY_DATA	
	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::BlueprintType))
	{
		Class->SetMetaData(TEXT("BlueprintType"), TEXT("true"));
	}
	else if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::NotBlueprintType))
	{
		Class->SetMetaData(TEXT("NotBlueprintType"), TEXT("false"));
		Class->RemoveMetaData(TEXT("BlueprintType"));
	}
#endif

#if WITH_EDITORONLY_DATA	
	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::Blueprintable))
	{
		// A Blueprintable class is implicitly a valid blueprint base and blueprint type.
		Class->SetMetaData(TEXT("IsBlueprintBase"), TEXT("true"));
		Class->SetMetaData(TEXT("BlueprintType"), TEXT("true"));
	}
	else if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::NotBlueprintable))
	{
		Class->SetMetaData(TEXT("IsBlueprintBase"), TEXT("false"));
		Class->RemoveMetaData(TEXT("BlueprintType"));
	}
#endif

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::NotPlaceable))
	{
		Class->ClassFlags |= CLASS_NotPlaceable;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::DefaultToInstanced))
	{
		Class->ClassFlags |= CLASS_DefaultToInstanced;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::Const))
	{
		Class->ClassFlags |= CLASS_Const;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::Abstract))
	{
		Class->ClassFlags |= CLASS_Abstract;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::Deprecated))
	{
		// Matches UHT: a deprecated class is also not placeable.
		Class->ClassFlags |= CLASS_Deprecated | CLASS_NotPlaceable;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::Transient))
	{
		Class->ClassFlags |= CLASS_Transient;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::NonTransient))
	{
		Class->ClassFlags &= ~CLASS_Transient;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::PerObjectConfig))
	{
		Class->ClassFlags |= CLASS_PerObjectConfig;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::DefaultConfig))
	{
		Class->ClassFlags |= CLASS_DefaultConfig;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::GlobalUserConfig))
	{
		Class->ClassFlags |= CLASS_GlobalUserConfig;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::ProjectUserConfig))
	{
		Class->ClassFlags |= CLASS_ProjectUserConfig;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::ConfigDoNotCheckDefaults))
	{
		Class->ClassFlags |= CLASS_ConfigDoNotCheckDefaults;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::EditInlineNew))
	{
		Class->ClassFlags |= CLASS_EditInlineNew;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::NotEditInlineNew))
	{
		Class->ClassFlags &= ~CLASS_EditInlineNew;
	}

	if (EnumHasAnyFlags(Specifiers, ESsClassSpecifier::HideDropdown))
	{
		Class->ClassFlags |= CLASS_HideDropDown;
	}

#if WITH_EDITORONLY_DATA
	// free-form metadata (DisplayName / Category / user-supplied Meta)
	for (int i = 0; i < MetaCount; ++i)
	{
		const FSsMetaDataEntry& Entry = MetaEntries[i];
		if (Entry.Key.IsNone())
		{
			continue;
		}
		const TCHAR* Value = Entry.Value ? Entry.Value : TEXT("");
		Class->SetMetaData(*Entry.Key.ToString(), Value);
	}
#endif
}
