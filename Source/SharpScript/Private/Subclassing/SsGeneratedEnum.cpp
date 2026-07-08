#include "SsGeneratedEnum.h"
#include "SsCommon.h"
#include "SsSubclassingUtils.h"
#include "SsEnumSpecifiers.h"
#include "UObject/Package.h"

class FSsGeneratedEnumBuilder
{
public:
	explicit FSsGeneratedEnumBuilder(const FSsEnumDef& EnumDef);

	~FSsGeneratedEnumBuilder();

	USsGeneratedEnum* Finalize();

	bool HasOldEnum() const
	{
		return OldEnum != nullptr;
	}

private:
	const FSsEnumDef& EnumDef;
	USsGeneratedEnum* OldEnum;
	USsGeneratedEnum* NewEnum;
	USsGeneratedEnum* FinalEnum;
};

FSsGeneratedEnumBuilder::FSsGeneratedEnumBuilder(const FSsEnumDef& InEnumDef)
	: EnumDef(InEnumDef)
{
	UPackage* EnumOuter = USsSubclassingUtils::GetGeneratedPackage();

	// Find any existing enum with the name we want to use
	OldEnum = FindObject<USsGeneratedEnum>(EnumOuter, *EnumDef.EnumName.ToString());

	// Create a new enum with a temporary name; we will rename it as part of Finalize
	const FName NewEnumName = MakeUniqueObjectName(EnumOuter, USsGeneratedEnum::StaticClass(), *FString::Printf(TEXT("%s_NEWINST"), *EnumDef.EnumName.ToString()));
	NewEnum = NewObject<USsGeneratedEnum>(EnumOuter, *NewEnumName.ToString(), RF_Public | RF_Standalone | RF_Transient);
	NewEnum->AddToRoot();

	// If there are old enum, reuse the old enum as the final generated enum.
	// In this way, we don't need to fix the references to the old enum.
	FinalEnum = OldEnum ? OldEnum : NewEnum;
}

FSsGeneratedEnumBuilder::~FSsGeneratedEnumBuilder()
{
	// If NewEnum is still set at this point, it means Finalize wasn't called, and we should destroy the partially built enum
	if (NewEnum)
	{
		NewEnum->ClearFlags(RF_AllFlags);
		NewEnum->RemoveFromRoot();
		NewEnum = nullptr;

		CollectGarbage(GARBAGE_COLLECTION_KEEPFLAGS);
	}
}

USsGeneratedEnum* FSsGeneratedEnumBuilder::Finalize()
{
	if (!OldEnum)
	{
		FinalEnum->Rename(*EnumDef.EnumName.ToString(), nullptr, REN_DontCreateRedirectors);
	}

	// Build the list of enum names in the form "<EnumName>::<ValueName>", which matches
	// how a C++ "enum class" declares its entries (UEnum::ECppForm::EnumClass).
	const FString EnumNameStr = EnumDef.EnumName.ToString();
	TArray<TPair<FName, int64>> Names;
	Names.Reserve(EnumDef.ValueCount);
	for (int i = 0; i < EnumDef.ValueCount; ++i)
	{
		const FSsEnumValueDef& ValueDef = EnumDef.ValueDefines[i];
		const FString FullName = FString::Printf(TEXT("%s::%s"), *EnumNameStr, *ValueDef.Name.ToString());
		Names.Emplace(FName(*FullName), ValueDef.Value);
	}

	// A C# [Flags] enum maps to a UEnum tagged with EEnumFlags::Flags (matches how UHT marks
	// a UENUM(meta=(Bitflags)) / "enum class : uint8 { }" bitmask enum).
	const EEnumFlags EnumFlags = EnumDef.IsFlags ? EEnumFlags::Flags : EEnumFlags::None;

	// SetEnums resets the internal Names array, so it works for both fresh creation and reload.
	FinalEnum->SetEnums(Names, UEnum::ECppForm::Namespaced, EnumFlags);

	// Expand the C# enum specifiers (editor-only metadata) onto the final enum. On the reload
	// path SetEnums only resets the value list; metadata is not reset, so we (re)apply directly
	// to FinalEnum here to cover both fresh and reused-enum cases.
	FSsEnumSpecifiers::Apply(FinalEnum, EnumDef.Specifiers, EnumDef.MetaEntries, EnumDef.MetaCount);

	if (!OldEnum)
	{
		// Null the NewEnum pointer so the destructor doesn't kill it
		NewEnum = nullptr;
	}
	return FinalEnum;
}

USsGeneratedEnum* USsGeneratedEnum::GenerateEnum(const FSsEnumDef& EnumDef)
{
	// Builder used to generate the enum
	FSsGeneratedEnumBuilder EnumBuilder(EnumDef);

#if !WITH_EDITOR
	if (EnumBuilder.HasOldEnum())
	{
		// The Subclassing enum only supports reload in editor mode.
		UE_LOG(LogSharpScript, Error, TEXT("Regenerate subclassing enum '%s' is not allowed in standalone build"),
			   *EnumDef.EnumName.ToString());
		return nullptr;
	}
#endif

	// Finalize the enum with its values
	return EnumBuilder.Finalize();
}
