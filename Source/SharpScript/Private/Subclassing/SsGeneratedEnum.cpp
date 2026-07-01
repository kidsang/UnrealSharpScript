#include "SsGeneratedEnum.h"
#include "SsCommon.h"
#include "SsSubclassingUtils.h"
#include "UObject/Package.h"

class FSsGeneratedEnumBuilder
{
public:
	FSsGeneratedEnumBuilder(const FName& EnumName);

	~FSsGeneratedEnumBuilder();

	USsGeneratedEnum* Finalize(const FSsEnumValueDef* ValueDefines, int ValueCount, bool bIsFlags);

	bool HasOldEnum() const
	{
		return OldEnum != nullptr;
	}

private:
	FName EnumName;
	USsGeneratedEnum* OldEnum;
	USsGeneratedEnum* NewEnum;
	USsGeneratedEnum* FinalEnum;
};

FSsGeneratedEnumBuilder::FSsGeneratedEnumBuilder(const FName& EnumName)
	: EnumName(EnumName)
{
	UPackage* EnumOuter = USsSubclassingUtils::GetGeneratedPackage();

	// Find any existing enum with the name we want to use
	OldEnum = FindObject<USsGeneratedEnum>(EnumOuter, *EnumName.ToString());

	// Create a new enum with a temporary name; we will rename it as part of Finalize
	const FName NewEnumName = MakeUniqueObjectName(EnumOuter, USsGeneratedEnum::StaticClass(), *FString::Printf(TEXT("%s_NEWINST"), *EnumName.ToString()));
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

USsGeneratedEnum* FSsGeneratedEnumBuilder::Finalize(const FSsEnumValueDef* ValueDefines, int ValueCount, bool bIsFlags)
{
	if (!OldEnum)
	{
		FinalEnum->Rename(*EnumName.ToString(), nullptr, REN_DontCreateRedirectors);
	}

	// Build the list of enum names in the form "<EnumName>::<ValueName>", which matches
	// how a C++ "enum class" declares its entries (UEnum::ECppForm::EnumClass).
	const FString EnumNameStr = EnumName.ToString();
	TArray<TPair<FName, int64>> Names;
	Names.Reserve(ValueCount);
	for (int i = 0; i < ValueCount; ++i)
	{
		const FSsEnumValueDef& ValueDef = ValueDefines[i];
		const FString FullName = FString::Printf(TEXT("%s::%s"), *EnumNameStr, *ValueDef.Name.ToString());
		Names.Emplace(FName(*FullName), ValueDef.Value);
	}

	// A C# [Flags] enum maps to a UEnum tagged with EEnumFlags::Flags (matches how UHT marks
	// a UENUM(meta=(Bitflags)) / "enum class : uint8 { }" bitmask enum).
	const EEnumFlags EnumFlags = bIsFlags ? EEnumFlags::Flags : EEnumFlags::None;

	// SetEnums resets the internal Names array, so it works for both fresh creation and reload.
	FinalEnum->SetEnums(Names, UEnum::ECppForm::Namespaced, EnumFlags);

	if (!OldEnum)
	{
		// Null the NewEnum pointer so the destructor doesn't kill it
		NewEnum = nullptr;
	}
	return FinalEnum;
}

USsGeneratedEnum* USsGeneratedEnum::GenerateEnum(const FName& EnumName, const FSsEnumValueDef* ValueDefines,
                                                 int ValueCount, bool bIsFlags)
{
	// Builder used to generate the enum
	FSsGeneratedEnumBuilder EnumBuilder(EnumName);

#if !WITH_EDITOR
	if (EnumBuilder.HasOldEnum())
	{
		// The Subclassing enum only supports reload in editor mode.
		UE_LOG(LogSharpScript, Error, TEXT("Regenerate subclassing enum '%s' is not allowed in standalone build"),
			   *EnumName.ToString());
		return nullptr;
	}
#endif

	// Finalize the enum with its values
	return EnumBuilder.Finalize(ValueDefines, ValueCount, bIsFlags);
}
