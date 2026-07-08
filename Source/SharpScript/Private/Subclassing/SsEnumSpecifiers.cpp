#include "SsEnumSpecifiers.h"
#include "SsCommon.h"
#include "SsSubclassingUtils.h"

void FSsEnumSpecifiers::Apply(UEnum* Enum, uint64 SpecifierBits, const FSsMetaDataEntry* MetaEntries, int MetaCount)
{
	check(Enum);

	const ESsEnumSpecifier Specifiers = static_cast<ESsEnumSpecifier>(SpecifierBits);

#if WITH_EDITORONLY_DATA
	if (EnumHasAnyFlags(Specifiers, ESsEnumSpecifier::BlueprintType))
	{
		Enum->SetMetaData(TEXT("BlueprintType"), TEXT("true"));
	}
#endif

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
		Enum->SetMetaData(*Entry.Key.ToString(), Value);
	}
#endif
}
