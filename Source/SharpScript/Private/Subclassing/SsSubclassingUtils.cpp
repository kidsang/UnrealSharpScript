#include "SsSubclassingUtils.h"
#include "SsCommon.h"
#include "SsGeneratedClass.h"
#include "SsGeneratedStruct.h"
#include "SsGeneratedEnum.h"
#include "SsPropertySpecifiers.h"
#include "SsTypeRegistry.h"
#include "UObject/Package.h"
#include "UObject/Class.h"
#include "UObject/TextProperty.h"

FString FSsPropertyDef::GetFriendlyName() const
{
	const FString PropTypeName = GetFriendlyTypeName(*this);
	return FString::Printf(TEXT("'%s'(%s)"), *PropName.ToString(), *PropTypeName);
}

FString FSsPropertyDef::GetFriendlyTypeName(const FSsPropertyDef& PropDef)
{
	const UClass* PropType = PropDef.PropType;
	if (PropType->IsChildOf(USsSubclassingUtils::ObjectPropertyBaseClass))
	{
		return FString::Printf(TEXT("%s(%s)"), *PropType->GetName(), *PropDef.UnderlyingType->GetName());
	}

	if (PropType == USsSubclassingUtils::ArrayPropertyClass || PropType == USsSubclassingUtils::SetPropertyClass)
	{
		FSsPropertyDef InnerPropDef;
		InnerPropDef.PropType = PropDef.InnerPropType;
		InnerPropDef.UnderlyingType = PropDef.InnerUnderlyingType;

		const FString InnerTypeName = GetFriendlyTypeName(InnerPropDef);
		return FString::Printf(TEXT("%s(%s)"), *PropType->GetName(), *InnerTypeName);
	}

	if (PropType == USsSubclassingUtils::MapPropertyClass)
	{
		FSsPropertyDef KeyPropDef;
		KeyPropDef.PropType = PropDef.KeyPropType;
		KeyPropDef.UnderlyingType = PropDef.KeyUnderlyingType;

		FSsPropertyDef ValuePropDef;
		ValuePropDef.PropType = PropDef.InnerPropType;
		ValuePropDef.UnderlyingType = PropDef.InnerUnderlyingType;

		const FString KeyTypeName = GetFriendlyTypeName(KeyPropDef);
		const FString ValueTypeName = GetFriendlyTypeName(ValuePropDef);
		return FString::Printf(TEXT("%s(%s, %s)"), *PropType->GetName(), *KeyTypeName, *ValueTypeName);
	}

	return PropType->GetName();
}

bool USsSubclassingUtils::Initialize()
{
	CoreUObjectPackage = FindPackage(nullptr, TEXT("/Script/CoreUObject"));
	check(CoreUObjectPackage);

	auto CreatePackage = [](const TCHAR* PackageName, EPackageFlags ExtraPackageFlags)
	{
		check(!FindPackage(nullptr, PackageName));

		EObjectFlags ObjectFlags =
			RF_Public;

		EPackageFlags PackageFlags =
			PKG_ContainsScript |
			PKG_RuntimeGenerated |
			PKG_CompiledIn |
			ExtraPackageFlags;

		UPackage* Package = NewObject<UPackage>(nullptr, PackageName, ObjectFlags);
		Package->SetPackageFlags(PackageFlags);
		Package->AddToRoot();
		return Package;
	};

	check(!GeneratedPackage);
	constexpr const TCHAR* GeneratedPackageName = TEXT("/Script/SharpScriptGenerated");
	GeneratedPackage = CreatePackage(GeneratedPackageName, PKG_None);

#if WITH_EDITOR
	check(!GeneratedPackageEditorOnly);
	constexpr const TCHAR* GeneratedEditorOnlyPackageName = TEXT("/Script/SharpScriptGeneratedEditorOnly");
	GeneratedPackageEditorOnly = CreatePackage(GeneratedEditorOnlyPackageName, PKG_EditorOnly);
#endif

	Int8PropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("Int8Property"));
	check(Int8PropertyClass);
	Int16PropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("Int16Property"));
	check(Int16PropertyClass);
	IntPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("IntProperty"));
	check(IntPropertyClass);
	Int64PropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("Int64Property"));
	check(Int64PropertyClass);
	BytePropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("ByteProperty"));
	check(BytePropertyClass);
	UInt16PropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("UInt16Property"));
	check(UInt16PropertyClass);
	UInt32PropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("UInt32Property"));
	check(UInt32PropertyClass);
	UInt64PropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("UInt64Property"));
	check(UInt64PropertyClass);
	FloatPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("FloatProperty"));
	check(FloatPropertyClass);
	DoublePropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("DoubleProperty"));
	check(DoublePropertyClass);
	BoolPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("BoolProperty"));
	check(BoolPropertyClass);
	StrPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("StrProperty"));
	check(StrPropertyClass);
	NamePropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("NameProperty"));
	check(NamePropertyClass);
	TextPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("TextProperty"));
	check(TextPropertyClass);
	StructPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("StructProperty"));
	check(StructPropertyClass);
	ObjectPropertyBaseClass = FindObject<UClass>(CoreUObjectPackage, TEXT("ObjectPropertyBase"));
	check(ObjectPropertyBaseClass);
	ObjectPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("ObjectProperty"));
	check(ObjectPropertyClass);
	SoftObjectPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("SoftObjectProperty"));
	check(SoftObjectPropertyClass);
	LazyObjectPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("LazyObjectProperty"));
	check(LazyObjectPropertyClass);
	ClassPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("ClassProperty"));
	check(ClassPropertyClass);
	SoftClassPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("SoftClassProperty"));
	check(SoftClassPropertyClass);
	ArrayPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("ArrayProperty"));
	check(ArrayPropertyClass);
	SetPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("SetProperty"));
	check(SetPropertyClass);
	MapPropertyClass = FindObject<UClass>(CoreUObjectPackage, TEXT("MapProperty"));
	check(MapPropertyClass);

	return true;
}

void USsSubclassingUtils::Finalize()
{
}

UPackage* USsSubclassingUtils::GetGeneratedPackage()
{
	return GeneratedPackage;
}

#if WITH_EDITOR
UPackage* USsSubclassingUtils::GetGeneratedPackageEditorOnly()
{
	return GeneratedPackageEditorOnly;
}
#endif

USsGeneratedClass* USsSubclassingUtils::GenerateClass(const void* ManagedType, const FSsClassDef* ClassDef)
{
	check(IsInGameThread());
	check(ClassDef);
	USsGeneratedClass* GenClass = USsGeneratedClass::GenerateClass(*ClassDef);
	if (GenClass)
	{
		USsTypeRegistry::RegisterClassType(GenClass, ManagedType);
	}
	return GenClass;
}

uint64 USsSubclassingUtils::TranslateParamFlags(ESsFunctionParamFlags ParamFlags)
{
	// Every parameter is at least CPF_Parm.
	uint64 Result = CPF_Parm;
	if (EnumHasAnyFlags(ParamFlags, ESsFunctionParamFlags::InParam))
	{
		// do nothing
	}
	else if (EnumHasAnyFlags(ParamFlags, ESsFunctionParamFlags::OutParam))
	{
		Result |= CPF_OutParm | CPF_ReferenceParm;
	}
	else if (EnumHasAnyFlags(ParamFlags, ESsFunctionParamFlags::ReturnParam))
	{
		Result |= CPF_OutParm | CPF_ReturnParm;
	}
	return Result;
}

uint32 USsSubclassingUtils::TranslateFunctionFlags(ESsFunctionFlags FunctionFlags)
{
	uint32 Result = 0;
	if (EnumHasAnyFlags(FunctionFlags, ESsFunctionFlags::Static))
	{
		Result |= FUNC_Static;
	}
	return Result;
}

USsGeneratedStruct* USsSubclassingUtils::GenerateStruct(const FName& StructName, const FSsPropertyDef* PropertyDefines, int PropertyCount)
{
	check(IsInGameThread());
	return USsGeneratedStruct::GenerateStruct(StructName, PropertyDefines, PropertyCount);
}

USsGeneratedEnum* USsSubclassingUtils::GenerateEnum(const FName& EnumName, const FSsEnumValueDef* ValueDefines, int ValueCount, bool bIsFlags)
{
	check(IsInGameThread());
	return USsGeneratedEnum::GenerateEnum(EnumName, ValueDefines, ValueCount, bIsFlags);
}

FProperty* USsSubclassingUtils::CreateProperty(FFieldVariant Owner, const FSsPropertyDef& PropDef)
{
	using ConverterFun = FProperty* (*)(FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags);
	static TMap<UClass*, ConverterFun> ConverterMap = {
		{
			Int8PropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FInt8Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			Int16PropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FInt16Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			IntPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FIntProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			Int64PropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FInt64Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			BytePropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				auto Prop = new FByteProperty(Owner, PropDef.PropName, InObjectFlags);
				// A byte property may be backed by a UEnum (a byte-backed enum property).
				if (PropDef.UnderlyingType)
				{
					UEnum* Enum = Cast<UEnum>(PropDef.UnderlyingType);
					if (!Enum)
					{
						UE_LOG(LogSharpScript, Error,
							TEXT("Subclassing error! UnderlyingType of byte enum property %s must be a UEnum. Owner is %s"),
							*PropDef.PropName.ToString(), *Owner.GetName());
						delete Prop;
						return nullptr;
					}
					Prop->Enum = Enum;
				}
				return Prop;
			}
		},
		{
			UInt16PropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FUInt16Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			UInt32PropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FUInt32Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			UInt64PropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FUInt64Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FloatPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FFloatProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			DoublePropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FDoubleProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			BoolPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FBoolProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			StrPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FStrProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			NamePropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FNameProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			TextPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FTextProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			StructPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				UScriptStruct* ScriptStruct = Cast<UScriptStruct>(PropDef.UnderlyingType);
				check(IsValid(ScriptStruct));
				auto Prop = new FStructProperty(Owner, PropDef.PropName, InObjectFlags);
				Prop->Struct = ScriptStruct;

				if (ScriptStruct->StructFlags & STRUCT_HasInstancedReference)
				{
					Prop->SetPropertyFlags(CPF_ContainsInstancedReference);
				}

				return Prop;
			}
		},
		{
			ObjectPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				UClass* PropertyClass = Cast<UClass>(PropDef.UnderlyingType);
				check(IsValid(PropertyClass));
				auto Prop = new FObjectProperty(Owner, PropDef.PropName, InObjectFlags);
				Prop->SetPropertyClass(PropertyClass);
				return Prop;
			}
		},
		{
			SoftObjectPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				UClass* PropertyClass = Cast<UClass>(PropDef.UnderlyingType);
				check(IsValid(PropertyClass));
				auto Prop = new FSoftObjectProperty(Owner, PropDef.PropName, InObjectFlags);
				Prop->SetPropertyClass(PropertyClass);
				return Prop;
			}
		},
		{
			LazyObjectPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				UClass* PropertyClass = Cast<UClass>(PropDef.UnderlyingType);
				check(IsValid(PropertyClass));
				auto Prop = new FLazyObjectProperty(Owner, PropDef.PropName, InObjectFlags);
				Prop->SetPropertyClass(PropertyClass);
				return Prop;
			}
		},
		{
			ClassPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				UClass* MetaClass = Cast<UClass>(PropDef.UnderlyingType);
				check(IsValid(MetaClass));
				auto Prop = new FClassProperty(Owner, PropDef.PropName, InObjectFlags);
				Prop->SetPropertyClass(UClass::StaticClass());
				Prop->SetMetaClass(MetaClass);
				return Prop;
			}
		},
		{
			SoftClassPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				UClass* MetaClass = Cast<UClass>(PropDef.UnderlyingType);
				check(IsValid(MetaClass));
				auto Prop = new FSoftClassProperty(Owner, PropDef.PropName, InObjectFlags);
				Prop->SetPropertyClass(UClass::StaticClass());
				Prop->SetMetaClass(MetaClass);
				return Prop;
			}
		},
		{
			ArrayPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				FSsPropertyDef InnerPropDef;
				InnerPropDef.PropName = FName(PropDef.PropName.ToString() + TEXT("_Inner"));
				InnerPropDef.PropType = PropDef.InnerPropType;
				InnerPropDef.UnderlyingType = PropDef.InnerUnderlyingType;

				auto Prop = new FArrayProperty(Owner, PropDef.PropName, InObjectFlags);
				Prop->Inner = CreateProperty(Prop, InnerPropDef);
				if (!Prop->Inner)
				{
					UE_LOG(LogSharpScript, Error,
					       TEXT("Subclassing error! Types in TArray for %s is not supported. Owner is %s"),
					       *PropDef.PropName.ToString(), *Owner.GetName());
					// There is no null testing in destructor ...
					Prop->Inner = new FProperty(Prop, NAME_None, RF_NoFlags);
					delete Prop;
					return nullptr;
				}

				if (Prop->Inner->HasAnyPropertyFlags(CPF_ContainsInstancedReference | CPF_InstancedReference))
				{
					Prop->SetPropertyFlags(CPF_ContainsInstancedReference);
				}

				return Prop;
			}
		},
		{
			SetPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				FSsPropertyDef InnerPropDef;
				InnerPropDef.PropName = FName(PropDef.PropName.ToString() + TEXT("_Elem"));
				InnerPropDef.PropType = PropDef.InnerPropType;
				InnerPropDef.UnderlyingType = PropDef.InnerUnderlyingType;

				auto Prop = new FSetProperty(Owner, PropDef.PropName, InObjectFlags);
				Prop->ElementProp = CreateProperty(Prop, InnerPropDef);
				if (!Prop->ElementProp)
				{
					UE_LOG(LogSharpScript, Error,
					       TEXT("Subclassing error! Elem type in TSet for %s is not supported. Owner is %s"),
					       *PropDef.PropName.ToString(), *Owner.GetName());
					// There is no null testing in destructor ...
					Prop->ElementProp = new FProperty(Prop, NAME_None, RF_NoFlags);
					delete Prop;
					return nullptr;
				}

				if (Prop->ElementProp->HasAnyPropertyFlags(CPF_ContainsInstancedReference | CPF_InstancedReference))
				{
					Prop->SetPropertyFlags(CPF_ContainsInstancedReference);
				}

				return Prop;
			}
		},
		{
			MapPropertyClass,
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				FSsPropertyDef KeyPropDef;
				KeyPropDef.PropName = FName(PropDef.PropName.ToString() + TEXT("_Key"));
				KeyPropDef.PropType = PropDef.KeyPropType;
				KeyPropDef.UnderlyingType = PropDef.KeyUnderlyingType;

				FSsPropertyDef ValuePropDef;
				ValuePropDef.PropName = FName(PropDef.PropName.ToString() + TEXT("_Value"));
				ValuePropDef.PropType = PropDef.InnerPropType;
				ValuePropDef.UnderlyingType = PropDef.InnerUnderlyingType;

				auto Prop = new FMapProperty(Owner, PropDef.PropName, InObjectFlags);
				Prop->KeyProp = CreateProperty(Prop, KeyPropDef);
				Prop->ValueProp = CreateProperty(Prop, ValuePropDef);

				if (!Prop->KeyProp || !Prop->ValueProp)
				{
					if (!Prop->KeyProp)
					{
						UE_LOG(LogSharpScript, Error,
						       TEXT("Subclassing error! Key type in TMap for %s is not supported. Owner is %s"),
						       *PropDef.PropName.ToString(), *Owner.GetName());
						// There is no null testing in destructor ...
						Prop->KeyProp = new FProperty(Prop, NAME_None, RF_NoFlags);
					}

					if (!Prop->KeyProp)
					{
						UE_LOG(LogSharpScript, Error,
						       TEXT("Subclassing error! Value type in TMap for %s is not supported. Owner is %s"),
						       *PropDef.PropName.ToString(), *Owner.GetName());
						// There is no null testing in destructor ...
						Prop->ValueProp = new FProperty(Prop, NAME_None, RF_NoFlags);
					}

					delete Prop;
					return nullptr;
				}

				if (Prop->KeyProp->HasAnyPropertyFlags(CPF_ContainsInstancedReference | CPF_InstancedReference)
					|| Prop->ValueProp->HasAnyPropertyFlags(CPF_ContainsInstancedReference | CPF_InstancedReference))
				{
					Prop->SetPropertyFlags(CPF_ContainsInstancedReference);
				}

				return Prop;
			}
		},
	};

	ConverterFun* ConverterPtr = ConverterMap.Find(PropDef.PropType);
	if (!ConverterPtr)
	{
		UE_LOG(LogSharpScript, Error, TEXT("%s: Can't find converter when creating property %s"),
		       *Owner.GetName(), *PropDef.GetFriendlyName());
		return nullptr;
	}

	auto Converter = *ConverterPtr;
	FProperty* NewProp = Converter(Owner, PropDef, RF_Public | RF_MarkAsNative);
	if (!NewProp)
	{
		return nullptr;
	}

	// Expand the C# UPROPERTY specifiers (EPropertyFlags + editor-only metadata) onto the property.
	FSsPropertySpecifiers::Apply(NewProp, PropDef.Specifiers, PropDef.MetaEntries, PropDef.MetaCount);

	return NewProp;
}

void USsSubclassingUtils::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&GenerateClass, TEXT("SubclassingUtils.GenerateClass"));
	BindNativeCallbackFunc(&GenerateStruct, TEXT("SubclassingUtils.GenerateStruct"));
	BindNativeCallbackFunc(&GenerateEnum, TEXT("SubclassingUtils.GenerateEnum"));
}

UPackage* USsSubclassingUtils::CoreUObjectPackage = nullptr;
UPackage* USsSubclassingUtils::GeneratedPackage = nullptr;
#if WITH_EDITOR
UPackage* USsSubclassingUtils::GeneratedPackageEditorOnly = nullptr;
#endif
UClass* USsSubclassingUtils::Int8PropertyClass = nullptr;
UClass* USsSubclassingUtils::Int16PropertyClass = nullptr;
UClass* USsSubclassingUtils::IntPropertyClass = nullptr;
UClass* USsSubclassingUtils::Int64PropertyClass = nullptr;
UClass* USsSubclassingUtils::BytePropertyClass = nullptr;
UClass* USsSubclassingUtils::UInt16PropertyClass = nullptr;
UClass* USsSubclassingUtils::UInt32PropertyClass = nullptr;
UClass* USsSubclassingUtils::UInt64PropertyClass = nullptr;
UClass* USsSubclassingUtils::FloatPropertyClass = nullptr;
UClass* USsSubclassingUtils::DoublePropertyClass = nullptr;
UClass* USsSubclassingUtils::BoolPropertyClass = nullptr;
UClass* USsSubclassingUtils::StrPropertyClass = nullptr;
UClass* USsSubclassingUtils::NamePropertyClass = nullptr;
UClass* USsSubclassingUtils::TextPropertyClass = nullptr;
UClass* USsSubclassingUtils::StructPropertyClass = nullptr;
UClass* USsSubclassingUtils::ObjectPropertyBaseClass = nullptr;
UClass* USsSubclassingUtils::ObjectPropertyClass = nullptr;
UClass* USsSubclassingUtils::SoftObjectPropertyClass = nullptr;
UClass* USsSubclassingUtils::LazyObjectPropertyClass = nullptr;
UClass* USsSubclassingUtils::ClassPropertyClass = nullptr;
UClass* USsSubclassingUtils::SoftClassPropertyClass = nullptr;
UClass* USsSubclassingUtils::ArrayPropertyClass = nullptr;
UClass* USsSubclassingUtils::SetPropertyClass = nullptr;
UClass* USsSubclassingUtils::MapPropertyClass = nullptr;
