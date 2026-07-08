#include "SsGeneratedClass.h"
#include "SsCommon.h"
#include "SsHouseKeeper.h"
#include "SsSubclassingUtils.h"
#include "SsClassSpecifiers.h"
#include "Runtime/Launch/Resources/Version.h"

class FSsGeneratedClassBuilder
{
public:
	explicit FSsGeneratedClassBuilder(const FSsClassDef& ClassDef);

	~FSsGeneratedClassBuilder();

	USsGeneratedClass* Finalize();

	bool HasOldClass() const
	{
		return OldClass != nullptr;
	}

	bool CreatePropertyFromDefinition(const FSsPropertyDef& PropDef) const;

	bool CreateFunctionFromDefinition(const FSsFunctionDef& FuncDef) const;

private:
	static UPackage* GetGenClassOuter(const UClass* SuperClass);

	static USsGeneratedClass* FindOldClass(const FName ClassName);

	/**
	 * Resolve class config name (mirrors UhtClass::SetAndValidateConfigName)
	 */
	void SetAndValidateConfigName() const;

	/**
	 * If reloading, transfer newly generated class members to old class.
	 */
	void TransferClassMembers();

private:
	const FSsClassDef& ClassDef;
	USsGeneratedClass* OldClass;
	USsGeneratedClass* NewClass;
	USsGeneratedClass* FinalClass;
};

FSsGeneratedClassBuilder::FSsGeneratedClassBuilder(const FSsClassDef& InClassDef)
	: ClassDef(InClassDef)
{
	check(ClassDef.SuperClass);
	UPackage* ClassOuter = GetGenClassOuter(ClassDef.SuperClass);

	// Find any existing class with the name we want to use
	OldClass = FindOldClass(ClassDef.ClassName);

	// Create a new class with a temporary name; we will rename it as part of Finalize
	const FName NewClassName = MakeUniqueObjectName(ClassOuter, USsGeneratedClass::StaticClass(), *FString::Printf(TEXT("%s_NEWINST"), *ClassDef.ClassName.ToString()));
	NewClass = NewObject<USsGeneratedClass>(ClassOuter, *NewClassName.ToString(), RF_Public | RF_Standalone | RF_Transient);
	NewClass->AddToRoot();
	NewClass->SetSuperStruct(ClassDef.SuperClass);
	NewClass->ClassFlags = (ClassDef.SuperClass->ClassFlags & CLASS_ScriptInherit);

	// If there are old class, reuse the old class as the final generated class.
	// In this way, we don't need to fix the references to the old class.
	FinalClass = OldClass ? OldClass : NewClass;
}

FSsGeneratedClassBuilder::~FSsGeneratedClassBuilder()
{
	// If NewClass is still set at this point, it means Finalize wasn't called, and we should destroy the partially built class.
	if (NewClass)
	{
		NewClass->ClearFlags(RF_AllFlags);
		NewClass->RemoveFromRoot();
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
		NewClass->Rename(*ClassDef.ClassName.ToString(), nullptr, REN_DontCreateRedirectors);
	}

	// Records the most derived native super class.
	if (USsGeneratedClass* GeneratedSuperClass = Cast<USsGeneratedClass>(ClassDef.SuperClass))
	{
		FinalClass->NativeSuperClass = GeneratedSuperClass->NativeSuperClass;
	}
	else
	{
		FinalClass->NativeSuperClass = ClassDef.SuperClass;
	}
	check(FinalClass->NativeSuperClass);
	check(FinalClass->NativeSuperClass->HasAnyClassFlags(CLASS_Native));

	// Expand the C# class specifiers (EClassFlags + metadata) onto the final class. On the reload path
	// TransferClassMembers already copied NewClass->ClassFlags onto OldClass, but metadata is not carried
	// across, so we (re)apply directly to FinalClass here to cover both fresh and reused-class cases.
	FSsClassSpecifiers::Apply(FinalClass, ClassDef.Specifiers, ClassDef.MetaEntries, ClassDef.MetaCount);

	// A Const class forces all of its own members to be blueprint read-only (mirrors UHT UhtClass)
	if (FinalClass->HasAnyClassFlags(CLASS_Const))
	{
		for (TFieldIterator<FProperty> It(FinalClass, EFieldIteratorFlags::ExcludeSuper); It; ++It)
		{
			It->SetPropertyFlags(CPF_BlueprintReadOnly);
		}
	}

	FinalClass->ClassConstructor = USsGeneratedClass::StaticObjectConstructor;

	// Finalize the class
	SetAndValidateConfigName();
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

bool FSsGeneratedClassBuilder::CreatePropertyFromDefinition(const FSsPropertyDef& PropDef) const
{
	// Resolve the property name to match any previously exported properties from the parent type
	const FName& PropName = PropDef.PropName;
	if (ClassDef.SuperClass->FindPropertyByName(PropName))
	{
		UE_LOG(LogSharpScript, Error, TEXT("%s: Property %s cannot override a property from the base type"),
		       *ClassDef.ClassName.ToString(), *PropDef.GetFriendlyName());
		return false;
	}

	// Create the property from its definition
	FProperty* Prop = USsSubclassingUtils::CreateProperty(NewClass, PropDef);
	if (!Prop)
	{
		UE_LOG(LogSharpScript, Error, TEXT("%s: Failed to create property for %s"),
		       *ClassDef.ClassName.ToString(), *PropDef.GetFriendlyName());
		return false;
	}

	NewClass->AddCppProperty(Prop);

	if (Prop->HasAnyPropertyFlags(CPF_ContainsInstancedReference | CPF_InstancedReference))
	{
		NewClass->ClassFlags |= CLASS_HasInstancedReference;
	}

	if (Prop->HasAnyPropertyFlags(CPF_Config))
	{
		NewClass->ClassFlags |= CLASS_Config;
	}

	return true;
}

bool FSsGeneratedClassBuilder::CreateFunctionFromDefinition(const FSsFunctionDef& FuncDef) const
{
	const FName& FuncName = FuncDef.FuncName;

	// Overriding a base-class function is not supported by the subclassing UFunction feature yet.
	if (ClassDef.SuperClass->FindFunctionByName(FuncName))
	{
		UE_LOG(LogSharpScript, Error, TEXT("%s: Function %s cannot override a function from the base type"),
		       *ClassDef.ClassName.ToString(), *FuncName.ToString());
		return false;
	}

	if (!FuncDef.ManagedDispatch)
	{
		UE_LOG(LogSharpScript, Error, TEXT("%s: Function %s has no managed dispatch"),
		       *ClassDef.ClassName.ToString(), *FuncName.ToString());
		return false;
	}

	USsGeneratedFunction* Func = NewObject<USsGeneratedFunction>(NewClass, FuncName, RF_Public | RF_Transient | RF_MarkAsNative);
	Func->FunctionFlags |= (FUNC_Public | FUNC_Native);
	Func->FunctionFlags |= (EFunctionFlags)USsSubclassingUtils::TranslateFunctionFlags(FuncDef.FunctionFlags);
	Func->ManagedDispatch = (FSsManagedFunctionDispatch)FuncDef.ManagedDispatch;
	NewClass->GeneratedFunctions.Add(Func);

	// Insert into the class field linked list so that field iterators and FindFunction work.
	Func->Next = NewClass->Children;
	NewClass->Children = Func;

	// Create parameter properties.
	// The C# side must order params so that appending in that order yields the correct reflection layout:
	// return / out params first, then input params in reverse — matching AddCppProperty's list insertion.
	for (int i = 0; i < FuncDef.ParamCount; ++i)
	{
		const FSsFunctionParamDef& ParamDef = FuncDef.Params[i];

		FSsPropertyDef PropDef;
		PropDef.PropName = ParamDef.ParamName;
		PropDef.PropType = ParamDef.PropType;
		PropDef.UnderlyingType = ParamDef.UnderlyingType;
		PropDef.InnerPropType = ParamDef.InnerPropType;
		PropDef.InnerUnderlyingType = ParamDef.InnerUnderlyingType;
		PropDef.KeyPropType = ParamDef.KeyPropType;
		PropDef.KeyUnderlyingType = ParamDef.KeyUnderlyingType;

		FProperty* ParamProp = USsSubclassingUtils::CreateProperty(Func, PropDef);
		if (!ParamProp)
		{
			UE_LOG(LogSharpScript, Error, TEXT("%s: Failed to create parameter %s for function %s"),
			       *ClassDef.ClassName.ToString(), *ParamDef.ParamName.ToString(), *FuncName.ToString());
			return false;
		}

		ParamProp->SetPropertyFlags((EPropertyFlags)USsSubclassingUtils::TranslateParamFlags(ParamDef.ParamFlags));
		Func->AddCppProperty(ParamProp);

		if (ParamProp->HasAnyPropertyFlags(CPF_OutParm) && !ParamProp->HasAnyPropertyFlags(CPF_ReturnParm))
		{
			Func->FunctionFlags |= FUNC_HasOutParms;
		}
	}

	// USsGeneratedFunction::Bind() installs our shared native thunk (no NativeFunctionLookupTable needed),
	// then StaticLink computes ParmsSize / property offsets.
	Func->Bind();
	Func->StaticLink(true);

	NewClass->AddFunctionToFunctionMap(Func, Func->GetFName());
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

void FSsGeneratedClassBuilder::SetAndValidateConfigName() const
{
	if (ClassDef.ConfigName && ClassDef.ConfigName[0] != TEXT('\0'))
	{
		FinalClass->ClassFlags |= CLASS_Config;
		if (FCString::Stricmp(ClassDef.ConfigName, TEXT("inherit")) == 0)
		{
			FinalClass->ClassConfigName = ClassDef.SuperClass->ClassConfigName;	
		}
		else
		{
			FinalClass->ClassConfigName = ClassDef.ConfigName;
		}
	}
	else
	{
		FinalClass->ClassConfigName = ClassDef.SuperClass->ClassConfigName;
	}

	if (EnumHasAnyFlags(FinalClass->ClassFlags, CLASS_Config) && FinalClass->ClassConfigName == NAME_None)
	{
		UE_LOG(LogSharpScript, Error, TEXT("Classes '%s' with config / globalconfig member variables need to specify config file."),
			   *ClassDef.ClassName.ToString());
		FinalClass->ClassConfigName = "Engine";
	}
}

void FSsGeneratedClassBuilder::TransferClassMembers()
{
	// Move the previously generated UFunctions out to the transient package first. PurgeClass detaches
	// them from the Children list but does NOT rename or destroy them, so their names (e.g. "FuncInt32")
	// would still be taken on OldClass and collide when we rename the freshly generated functions in,
	// silently renaming the new ones and breaking FindFunctionByName.
	OldClass->MoveGeneratedFunctionsAside();

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
		// Capture Next before Rename/Bind/StaticLink in case any of them perturb the field chain.
		UField* NextUField = LastUField->Next;
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
		LastUField = NextUField;
	}

	// Take ownership of the generated-function tracking list so the next reload can move them aside.
	OldClass->GeneratedFunctions = MoveTemp(NewClass->GeneratedFunctions);
}

void USsGeneratedClass::MoveGeneratedFunctionsAside()
{
	for (USsGeneratedFunction* Func : GeneratedFunctions)
	{
		if (!Func || !IsValid(Func) || Func->HasAnyFlags(RF_BeginDestroyed | RF_FinishDestroyed))
		{
			continue;
		}

		Func->ClearFlags(RF_AllFlags);
		Func->ClearInternalFlags(EInternalObjectFlags::Native);
		// Rename into the transient package to free the name (e.g. "FuncInt32") on this class.
		Func->Rename(nullptr, GetTransientPackage(),
		             REN_DoNotDirty | REN_DontCreateRedirectors | REN_ForceNoResetLoaders);
	}
	GeneratedFunctions.Empty();
}

USsGeneratedClass* USsGeneratedClass::GenerateClass(const FSsClassDef& ClassDef)
{
	FSsGeneratedClassBuilder ClassBuilder(ClassDef);

#if !WITH_EDITOR
	if (ClassBuilder.HasOldClass())
	{
		// The Subclassing class only supports reload in editor mode.
		UE_LOG(LogSharpScript, Error, TEXT("Regenerate subclassing class '%s' is not allowed in standalone build"),
		       *ClassDef.ClassDef.ClassName.ToString());
		return nullptr;
	}
#endif

	for (int i = 0; i < ClassDef.PropertyCount; ++i)
	{
		const FSsPropertyDef& PropDef = ClassDef.PropertyDefines[i];
		if (!ClassBuilder.CreatePropertyFromDefinition(PropDef))
		{
			return nullptr;
		}
	}

	for (int i = 0; i < ClassDef.FunctionCount; ++i)
	{
		const FSsFunctionDef& FuncDef = ClassDef.FunctionDefines[i];
		if (!ClassBuilder.CreateFunctionFromDefinition(FuncDef))
		{
			return nullptr;
		}
	}

	// Finalize the class with its post-init function
	return ClassBuilder.Finalize();
}

void USsGeneratedFunction::Bind()
{
	SetNativeFunc(&USsGeneratedFunction::execCallManagedFunction);
}

DEFINE_FUNCTION(USsGeneratedFunction::execCallManagedFunction)
{
	// The function currently being executed by the script VM.
	USsGeneratedFunction* Func = CastChecked<USsGeneratedFunction>(Stack.CurrentNativeFunction);

	// Records (param, callerAddress) for out params so we can copy results back after dispatch.
	struct FOutParamCopy
	{
		FProperty* Param;
		void* CallerAddress;
	};
	TArray<FOutParamCopy, TInlineAllocator<8>> OutParamCopies;

	// The C# body always runs against a single contiguous params buffer, but where that buffer comes
	// from differs by call path:
	//   * ProcessEvent path: we reuse Stack.Locals directly as the params buffer, which already holds
	//     the incoming params. Both out params AND the return value are carried in Stack.OutParms,
	//     so we record every Stack.OutParms entry and copy them all back to the caller after dispatch.
	//   * Bytecode path: there is no params struct yet, so we allocate a transient local buffer and step
	//     each argument off the VM stack into it. The caller storage for out params (and RESULT_PARAM for
	//     the return value) is captured while stepping and written back after dispatch.
	// In both cases OutParamCopies drives the post-dispatch write-back of out/return values.
	void* ParamsBuffer;

	if (Stack.Code)
	{
		// Blueprint bytecode path: step each parameter off the VM stack into our local buffer.
		ParamsBuffer = FMemory_Alloca(FMath::Max<int32>(1, Func->GetStructureSize()));
		Func->InitializeStruct(ParamsBuffer);
		
		for (TFieldIterator<FProperty> It(Func); It && It->HasAnyPropertyFlags(CPF_Parm); ++It)
		{
			FProperty* Param = *It;

			// The return value never has data on the bytecode stack, so don't step it.
			if (Param->HasAnyPropertyFlags(CPF_ReturnParm))
			{
				OutParamCopies.Add({Param, RESULT_PARAM});
				continue;
			}

			// Step the property data to populate the local value
			Stack.MostRecentPropertyAddress = nullptr;
			void* LocalValue = Param->ContainerPtrToValuePtr<void>(ParamsBuffer);
			Stack.StepCompiledIn<FProperty>(LocalValue);

			// Out (non-return) params are written back to the caller after dispatch. Prefer the address
			// the VM resolved for the argument; fall back to the stepped value's own storage.
			if (Param->HasAnyPropertyFlags(CPF_OutParm))
			{
				void* CallerAddr = Stack.MostRecentPropertyAddress ? Stack.MostRecentPropertyAddress : LocalValue;
				OutParamCopies.Add({Param, CallerAddr});
			}
		}

		// Validate we reached the end of the parameters when stepping the bytecode stack
		checkSlow(*Stack.Code == EX_EndFunctionParms);
		++Stack.Code;
	}
	else
	{
		// ProcessEvent path: the params already sit in Stack.Locals (a copy).
		ParamsBuffer = Stack.Locals;	

		for (FOutParmRec* Out = Stack.OutParms; Out; Out = Out->NextOutParm)
		{
			OutParamCopies.Add({Out->Property, Out->PropAddr});
		}
	}

	const UObject* ThisObject = P_THIS_OBJECT;
	if (Func->ManagedDispatch)
	{
		if (Func->HasAnyFunctionFlags(FUNC_Static))
		{
			// Static functions have no "this": UE still routes the call through the CDO (see
			// USsObjectInterop::InvokeStaticFunctionCall), so P_THIS_OBJECT is a valid object here, but the
			// managed side is a C# static method that must not receive a self instance. Skip resolving a
			// wrapper and pass a null handle;
			// the generated static dispatch stub ignores its object-handle argument.
			Func->ManagedDispatch(nullptr, ParamsBuffer);
		}
		else
		{
			// Locate the managed wrapper (GCHandle IntPtr) for the calling object, then dispatch to C#.
			const void* ManagedObjectHandle = USsHouseKeeper::GetManagedObject(ThisObject);
			check(ManagedObjectHandle);
			Func->ManagedDispatch(ManagedObjectHandle, ParamsBuffer);
		}
	}
	else
	{
		UE_LOG(LogSharpScript, Error, TEXT("No managed dispatch found for function %s on %s"),
		       *Func->GetName(), *ThisObject->GetName());
	}

	// Copy out params back to the caller's storage.
	for (const FOutParamCopy& OutCopy : OutParamCopies)
	{
		void* LocalValue = OutCopy.Param->ContainerPtrToValuePtr<void>(ParamsBuffer);
		OutCopy.Param->CopyCompleteValue(OutCopy.CallerAddress, LocalValue);
	}

	if (Stack.Code)
	{
		// Destroy the local buffer.
		Func->DestroyStruct(ParamsBuffer);
	}
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
