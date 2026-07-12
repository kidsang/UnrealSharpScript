#include "SsFunctionSpecifiers.h"
#include "SsSubclassingUtils.h"

void FSsFunctionSpecifiers::Apply(UFunction* Func, uint64 SpecifierBits, const FSsMetaDataEntry* MetaEntries, int MetaCount)
{
	check(Func);

	const ESsFunctionSpecifier Specifiers = static_cast<ESsFunctionSpecifier>(SpecifierBits);

	// Accumulate the function flags, then OR them onto the function in one shot.
	EFunctionFlags Flags = Func->FunctionFlags;

	if (EnumHasAnyFlags(Specifiers, ESsFunctionSpecifier::BlueprintEvent))
	{
		Flags |= FUNC_Event | FUNC_BlueprintEvent;
	}

	if (EnumHasAnyFlags(Specifiers, ESsFunctionSpecifier::Exec))
	{
		Flags |= FUNC_Exec;
	}

	if (EnumHasAnyFlags(Specifiers, ESsFunctionSpecifier::BlueprintCallable))
	{
		Flags |= FUNC_BlueprintCallable;
	}

	if (EnumHasAnyFlags(Specifiers, ESsFunctionSpecifier::BlueprintPure))
	{
		Flags |= FUNC_BlueprintCallable | FUNC_BlueprintPure;
	}

	Func->FunctionFlags = Flags;

#if WITH_EDITORONLY_DATA
	if (EnumHasAnyFlags(Specifiers, ESsFunctionSpecifier::CallInEditor))
	{
		Func->SetMetaData(TEXT("CallInEditor"), TEXT("true"));
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
		Func->SetMetaData(*Entry.Key.ToString(), Value);
	}
#endif
}

bool FSsFunctionSpecifiers::IsBlueprintEvent(uint64 SpecifierBits)
{
	const ESsFunctionSpecifier Specifiers = static_cast<ESsFunctionSpecifier>(SpecifierBits);
	return EnumHasAnyFlags(Specifiers, ESsFunctionSpecifier::BlueprintEvent);
}
