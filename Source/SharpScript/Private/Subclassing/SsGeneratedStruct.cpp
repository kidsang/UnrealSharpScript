#include "SsGeneratedStruct.h"
#include "SsCommon.h"
#include "SsSubclassingUtils.h"
#include "UObject/Package.h"
#include "UserDefinedStructure/UserDefinedStructEditorData.h"

class FSsGeneratedStructBuilder
{
public:
	FSsGeneratedStructBuilder(const FName& StructName);

	~FSsGeneratedStructBuilder();

	USsGeneratedStruct* Finalize();

	bool HasOldStruct() const
	{
		return OldStruct != nullptr;
	}

	bool CreatePropertyFromDefinition(const FSsPropertyDef& PropDef);

private:
	/**
	 * If reloading, transfer newly generated struct members to old struct.
	 */
	void TransferClassMembers();

private:
	FName StructName;
	USsGeneratedStruct* OldStruct;
	USsGeneratedStruct* NewStruct;
	USsGeneratedStruct* FinalStruct;
};

FSsGeneratedStructBuilder::FSsGeneratedStructBuilder(const FName& StructName)
	: StructName(StructName)
{
	UPackage* StructOuter = USsSubclassingUtils::GetGeneratedPackage();

	// Find any existing struct with the name we want to use
	OldStruct = FindObject<USsGeneratedStruct>(StructOuter, *StructName.ToString());

	// Create a new struct with a temporary name; we will rename it as part of Finalize
	const FName NewStructName = MakeUniqueObjectName(StructOuter, USsGeneratedStruct::StaticClass(),
	                                                 *FString::Printf(TEXT("%s_NEWINST"), *StructName.ToString()));
	NewStruct = NewObject<USsGeneratedStruct>(StructOuter, *NewStructName.ToString(),
	                                          RF_Public | RF_MarkAsNative | RF_Transient);

	// If there are old struct, reuse the old struct as the final generated struct.
	// In this way, we don't need to fix the references to the old struct.
	FinalStruct = OldStruct ? OldStruct : NewStruct;
}

FSsGeneratedStructBuilder::~FSsGeneratedStructBuilder()
{
	// If NewStruct is still set at this point, if means Finalize wasn't called, and we should destroy the partially built struct
	if (NewStruct)
	{
		NewStruct->ClearFlags(RF_AllFlags);
		NewStruct->ClearInternalFlags(EInternalObjectFlags::Native);
		NewStruct = nullptr;

		CollectGarbage(GARBAGE_COLLECTION_KEEPFLAGS);
	}
}

USsGeneratedStruct* FSsGeneratedStructBuilder::Finalize()
{
	if (OldStruct)
	{
		check(FinalStruct == OldStruct);
		TransferClassMembers();
	}
	else
	{
		check(FinalStruct == NewStruct);
		NewStruct->Rename(*StructName.ToString(), nullptr, REN_DontCreateRedirectors);
	}

	// Finalize the struct
	FinalStruct->Bind();
	FinalStruct->StaticLink(true);
	FinalStruct->GenerateDefaultInstance();

	FinalStruct->Status = UDSS_UpToDate;
	if (!FinalStruct->Guid.IsValid())
	{
		FinalStruct->Guid = FGuid::NewGuid();
	}

	if (!OldStruct)
	{
		// Null the NewStruct pointer so the destructor doesn't kill it
		NewStruct = nullptr;
	}
	return FinalStruct;
}

bool FSsGeneratedStructBuilder::CreatePropertyFromDefinition(const FSsPropertyDef& PropDef)
{
	// Create the property from its definition
	FProperty* Prop = USsSubclassingUtils::CreateProperty(NewStruct, PropDef);
	if (!Prop)
	{
		UE_LOG(LogSharpScript, Error, TEXT("%s: Failed to create property for %s"),
			   *StructName.ToString(), *PropDef.GetFriendlyName());
		return false;
	}

	NewStruct->AddCppProperty(Prop);
	if (Prop->HasAnyPropertyFlags(CPF_ContainsInstancedReference | CPF_InstancedReference))
	{
		NewStruct->StructFlags = (EStructFlags)(NewStruct->StructFlags | STRUCT_HasInstancedReference);
	}

	return true;
}

void FSsGeneratedStructBuilder::TransferClassMembers()
{
	// Purge Struct
	// Copy from: FUserDefinedStructureCompilerInner::CleanAndSanitizeStruct
	{
#if WITH_EDITORONLY_DATA
		if (UUserDefinedStructEditorData* EditorData = Cast<UUserDefinedStructEditorData>(OldStruct->EditorData))
		{
			EditorData->CleanDefaultInstance();
		}
#endif

		OldStruct->SetSuperStruct(nullptr);
		OldStruct->Children = nullptr;
		OldStruct->DestroyChildPropertiesAndResetPropertyLinks();
		OldStruct->Script.Empty();
		OldStruct->MinAlignment = 0;
		OldStruct->ScriptAndPropertyObjectReferences.Empty();
#if WITH_EDITORONLY_DATA
		OldStruct->ErrorMessage.Empty();
#endif
		OldStruct->SetStructTrashed(true);
	}

	OldStruct->SetSuperStruct(NewStruct->GetSuperStruct());
	OldStruct->StructFlags = NewStruct->StructFlags;

	check(!OldStruct->ChildProperties);
	OldStruct->ChildProperties = NewStruct->ChildProperties;
	NewStruct->ChildProperties = nullptr;

	FField* LastFField = OldStruct->ChildProperties;
	while (LastFField)
	{
		check(LastFField->Owner == NewStruct);
		LastFField->Owner = OldStruct;
		LastFField = LastFField->Next;
	}
}

USsGeneratedStruct* USsGeneratedStruct::GenerateStruct(const FName& StructName, const FSsPropertyDef* PropertyDefines,
                                                       int PropertyCount)
{
	// Builder used to generate the struct
	FSsGeneratedStructBuilder StructBuilder(StructName);

#if !WITH_EDITOR
	if (StructBuilder.HasOldStruct())
	{
		// The Subclassing struct only supports reload in editor mode.
		UE_LOG(LogSharpScript, Error, TEXT("Regenerate subclassing struct '%s' is not allowed in standalone build"),
			   *StructName.ToString());
		return nullptr;
	}
#endif

	for (int i = 0; i < PropertyCount; ++i)
	{
		const FSsPropertyDef& PropDef = PropertyDefines[i];
		if (!StructBuilder.CreatePropertyFromDefinition(PropDef))
		{
			return nullptr;
		}
	}

	// Finalize the struct with its post-init function
	return StructBuilder.Finalize();
}

void USsGeneratedStruct::GenerateDefaultInstance()
{
	DefaultStructInstance.Recreate(this);
}
