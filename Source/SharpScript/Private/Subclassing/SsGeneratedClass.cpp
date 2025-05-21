#include "SsGeneratedClass.h"
#include "SsCommon.h"
#include "SsSubclassingUtils.h"
#include "Runtime/Launch/Resources/Version.h"

class FSsGeneratedClassBuilder
{
public:
	FSsGeneratedClassBuilder(const FName& ClassName, UClass* SuperClass);

	~FSsGeneratedClassBuilder();

	USsGeneratedClass* Finalize();

	bool HasOldClass() const
	{
		return OldClass != nullptr;
	}

	bool CreatePropertyFromDefinition(const FSsPropertyDef& PropDef);

private:
	static UPackage* GetGenClassOuter(const UClass* SuperClass);

	static USsGeneratedClass* FindOldClass(const FName ClassName);

	/**
	 * If reloading, transfer newly generated class members to old class.
	 */
	void TransferClassMembers();

	FName ClassName;
	UClass* SuperClass;
	USsGeneratedClass* OldClass;
	USsGeneratedClass* NewClass;
	USsGeneratedClass* FinalClass;
};

FSsGeneratedClassBuilder::~FSsGeneratedClassBuilder()
{
	// If NewClass is still set at this point, it means Finalize wasn't called, and we should destroy the partially built class.
	if (NewClass)
	{
		NewClass->ClearFlags(RF_AllFlags);
		NewClass->ClearInternalFlags(EInternalObjectFlags::Native);
		NewClass = nullptr;

		CollectGarbage(GARBAGE_COLLECTION_KEEPFLAGS);
	}
}

USsGeneratedClass* FSsGeneratedClassBuilder::Finalize()
{
	if (OldClass)
	{
		check(FinalClass == OldClass);
		TransferClassMembers();
	}
	else
	{
		check(FinalClass == NewClass);
		NewClass->Rename(*ClassName.ToString(), nullptr, REN_DontCreateRedirectors);
	}

	// Records the most derived native super class.
	if (USsGeneratedClass* GeneratedSuperClass = Cast<USsGeneratedClass>(SuperClass))
	{
		FinalClass->NativeSuperClass = GeneratedSuperClass->NativeSuperClass;
	}
	else
	{
		FinalClass->NativeSuperClass = SuperClass;
	}
	check(FinalClass->NativeSuperClass);
	check(FinalClass->NativeSuperClass->HasAnyClassFlags(CLASS_Native));

	FinalClass->ClassConstructor = USsGeneratedClass::StaticObjectConstructor;

	// Finalize the class
	FinalClass->ClassConfigName = SuperClass->ClassConfigName;
	FinalClass->Bind();
	FinalClass->StaticLink(true);
	FinalClass->AssembleReferenceTokenStream(true);

	// Initialize class default object
	auto _ = FinalClass->GetDefaultObject(true);
	FinalClass->UpdateCustomPropertyListForPostConstruction();

#if ENGINE_MAJOR_VERSION >= 5 && ENGINE_MINOR_VERSION >= 3 || ENGINE_MAJOR_VERSION >= 6
	FinalClass->InitializeFieldNotifies();
#endif

	if (!OldClass)
	{
		// Null the NewClass pointer so the destructor doesn't kill it
		NewClass = nullptr;
	}

	return FinalClass;
}

FSsGeneratedClassBuilder::FSsGeneratedClassBuilder(const FName& ClassName, UClass* SuperClass)
	: ClassName(ClassName)
	  , SuperClass(SuperClass)
{
	check(SuperClass);
	UPackage* ClassOuter = GetGenClassOuter(SuperClass);

	// Find any existing class with the name we want to use
	OldClass = FindOldClass(ClassName);

	// Create a new class with a temporary name; we will rename it as part of Finalize
	const FName NewClassName = MakeUniqueObjectName(ClassOuter, USsGeneratedClass::StaticClass(),
	                                                *FString::Printf(TEXT("%s_NEWINST"), *ClassName.ToString()));
	NewClass = NewObject<USsGeneratedClass>(ClassOuter, *NewClassName.ToString(),
	                                        RF_Public | RF_MarkAsNative | RF_Transient);
	NewClass->SetSuperStruct(SuperClass);
	NewClass->ClassFlags = (SuperClass->ClassFlags & CLASS_ScriptInherit);

	// If there are old classes, reuse the old classes as the final generated classes.
	// In this way, we don't need to fix the references to the old classes.
	FinalClass = OldClass ? OldClass : NewClass;
}

bool FSsGeneratedClassBuilder::CreatePropertyFromDefinition(const FSsPropertyDef& PropDef)
{
	// Resolve the property name to match any previously exported properties from the parent type
	const FName& PropName = PropDef.PropName;
	if (SuperClass->FindPropertyByName(PropName))
	{
		UE_LOG(LogSharpScript, Error, TEXT("%s: Property %s cannot override a property from the base type"),
		       *ClassName.ToString(), *USsSubclassingUtils::GetFriendlyName(PropDef));
		return false;
	}

	// Create the property from its definition
	FProperty* Prop = USsSubclassingUtils::CreateProperty(NewClass, PropDef);
	if (!Prop)
	{
		UE_LOG(LogSharpScript, Error, TEXT("%s: Failed to create property for %s"),
		       *ClassName.ToString(), *USsSubclassingUtils::GetFriendlyName(PropDef));
		return false;
	}

	NewClass->AddCppProperty(Prop);
	if (Prop->HasAnyPropertyFlags(CPF_ContainsInstancedReference | CPF_InstancedReference))
	{
		NewClass->ClassFlags |= CLASS_HasInstancedReference;
	}

	return true;
}

UPackage* FSsGeneratedClassBuilder::GetGenClassOuter(const UClass* SuperClass)
{
	UPackage* SuperClassPackage = SuperClass->GetPackage();
#if WITH_EDITOR
	if (SuperClassPackage->HasAnyPackageFlags(PKG_EditorOnly))
	{
		return USsSubclassingUtils::GetGeneratedPackageEditorOnly();
	}
#endif
	return USsSubclassingUtils::GetGeneratedPackage();
}

USsGeneratedClass* FSsGeneratedClassBuilder::FindOldClass(const FName ClassName)
{
	USsGeneratedClass* OldClass = FindObject<USsGeneratedClass>(USsSubclassingUtils::GetGeneratedPackage(),
	                                                            *ClassName.ToString());
#if WITH_EDITOR
	if (!OldClass)
	{
		OldClass = FindObject<USsGeneratedClass>(USsSubclassingUtils::GetGeneratedPackageEditorOnly(),
		                                         *ClassName.ToString());
	}
#endif
	return OldClass;
}

void FSsGeneratedClassBuilder::TransferClassMembers()
{
	OldClass->PurgeClass(true);

	OldClass->SetSuperStruct(NewClass->GetSuperClass());
	OldClass->ClassFlags = NewClass->ClassFlags;

#if ENGINE_MAJOR_VERSION >= 5 && ENGINE_MINOR_VERSION >= 3 || ENGINE_MAJOR_VERSION >= 6
	OldClass->FieldNotifies = MoveTemp(NewClass->FieldNotifies);
#endif

	check(!OldClass->ChildProperties);
	OldClass->ChildProperties = NewClass->ChildProperties;
	NewClass->ChildProperties = nullptr;

	FField* LastFField = OldClass->ChildProperties;
	while (LastFField)
	{
		check(LastFField->Owner == NewClass);
		LastFField->Owner = OldClass;
		LastFField = LastFField->Next;
	}

	check(!OldClass->Children);
	OldClass->Children = NewClass->Children;
	NewClass->Children = nullptr;

	UField* LastUField = OldClass->Children;
	while (LastUField)
	{
		check(LastUField->GetOuter() == NewClass);
		LastUField->Rename(nullptr, OldClass,
		                   REN_DoNotDirty | REN_DontCreateRedirectors | REN_ForceNoResetLoaders);
		if (UFunction* Func = Cast<UFunction>(LastUField))
		{
			Func->Bind();
			Func->StaticLink(true);
			if (!Func->HasAnyFunctionFlags(FUNC_Delegate | FUNC_MulticastDelegate))
			{
				OldClass->AddFunctionToFunctionMap(Func, Func->GetFName());
			}
		}
		LastUField = LastUField->Next;
	}
}

USsGeneratedClass* USsGeneratedClass::GenerateClass(const FName& ClassName, UClass* SuperClass,
                                                    const FSsPropertyDef* PropertyDefines, int PropertyCount)
{
	FSsGeneratedClassBuilder ClassBuilder(ClassName, SuperClass);

#if !WITH_EDITOR
	if (ClassBuilder.HasOldClass())
	{
		// The Subclassing class only supports reload in editor mode.
		UE_LOG(LogSharpScript, Error, TEXT("Regenerate subclassing class '%s' is not allowed in standalone build"),
		       *ClassName.ToString());
		return nullptr;
	}
#endif

	for (int i = 0; i < PropertyCount; ++i)
	{
		const FSsPropertyDef& PropDef = PropertyDefines[i];
		if (!ClassBuilder.CreatePropertyFromDefinition(PropDef))
		{
			return nullptr;
		}
	}

	// Finalize the class with its post-init function
	return ClassBuilder.Finalize();
}

void USsGeneratedClass::StaticObjectConstructor(const FObjectInitializer& Initializer)
{
	UObject* Object = Initializer.GetObj();
	const USsGeneratedClass* GenClass = GetFirstGeneratedClass(Object->GetClass());
	check(GenClass);
	
	// Call native constructor
	GenClass->NativeSuperClass->ClassConstructor(Initializer);
}

const USsGeneratedClass* USsGeneratedClass::GetFirstGeneratedClass(const UClass* InClass)
{
	while (InClass)
	{
		if (const USsGeneratedClass* Class = Cast<USsGeneratedClass>(InClass))
		{
			return Class;
		}
		InClass = InClass->GetSuperClass();
	}
	return nullptr;
}
