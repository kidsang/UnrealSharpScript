#include "SsStructSpecifiers.h"
#include "SsCommon.h"
#include "SsSubclassingUtils.h"

void FSsStructSpecifiers::Apply(UScriptStruct* Struct, uint64 SpecifierBits, const FSsMetaDataEntry* MetaEntries, int MetaCount)
{
	check(Struct);

	const ESsStructSpecifier Specifiers = static_cast<ESsStructSpecifier>(SpecifierBits);

#if WITH_EDITORONLY_DATA
	if (EnumHasAnyFlags(Specifiers, ESsStructSpecifier::BlueprintType))
	{
		Struct->SetMetaData(TEXT("BlueprintType"), TEXT("true"));
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
		Struct->SetMetaData(*Entry.Key.ToString(), Value);
	}
#endif
}
