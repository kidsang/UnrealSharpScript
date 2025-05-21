#include "SsSubclassingUtils.h"
#include "SsCommon.h"
#include "SsGeneratedClass.h"
#include "SsTypeRegistry.h"
#include "UObject/Package.h"
#include "UObject/TextProperty.h"

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

USsGeneratedClass* USsSubclassingUtils::GenerateClass(const void* ManagedType, const FName& ClassName,
                                                      UClass* SuperClass,
                                                      const FSsPropertyDef* PropertyDefines, int PropertyCount)
{
	check(IsInGameThread());
	USsGeneratedClass* GenClass = USsGeneratedClass::GenerateClass(ClassName, SuperClass, PropertyDefines,
	                                                               PropertyCount);
	if (GenClass)
	{
		USsTypeRegistry::RegisterClassType(GenClass, ManagedType);
	}
	return GenClass;
}

FString USsSubclassingUtils::GetFriendlyName(const FSsPropertyDef& PropDef)
{
	// todo: twx Support container types.
	return FString::Printf(TEXT("'%s'(%s)"), *PropDef.PropName.ToString(), *PropDef.PropType->GetName());
}

FProperty* USsSubclassingUtils::CreateProperty(FFieldVariant Owner, const FSsPropertyDef& PropDef)
{
	// todo: twx Support container types.
	using ConverterFun = FProperty* (*)(FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags);
	static TMap<UClass*, ConverterFun> ConverterMap = {
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("Int8Property")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FInt8Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("Int16Property")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FInt16Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("IntProperty")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FIntProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("Int64Property")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FInt64Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("ByteProperty")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FByteProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("UInt16Property")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FUInt16Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("UInt32Property")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FUInt32Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("UInt64Property")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FUInt64Property(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("FloatProperty")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FFloatProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("DoubleProperty")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FDoubleProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("BoolProperty")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FBoolProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("StrProperty")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FStrProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("NameProperty")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FNameProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
		{
			FindObject<UClass>(CoreUObjectPackage, TEXT("TextProperty")),
			[](FFieldVariant Owner, const FSsPropertyDef& PropDef, EObjectFlags InObjectFlags) -> FProperty*
			{
				return new FTextProperty(Owner, PropDef.PropName, InObjectFlags);
			}
		},
	};

	ConverterFun* ConverterPtr = ConverterMap.Find(PropDef.PropType);
	if (!ConverterPtr)
	{
		UE_LOG(LogSharpScript, Error, TEXT("%s: Can't find converter when creating property %s"),
		       *Owner.GetName(), *GetFriendlyName(PropDef));
		return nullptr;
	}

	auto Converter = *ConverterPtr;
	FProperty* NewProp = Converter(Owner, PropDef, RF_Public | RF_MarkAsNative);
	return NewProp;
}

void USsSubclassingUtils::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&GenerateClass, TEXT("SubclassingUtils.GenerateClass"));
}

UPackage* USsSubclassingUtils::CoreUObjectPackage = nullptr;
UPackage* USsSubclassingUtils::GeneratedPackage = nullptr;
#if WITH_EDITOR
UPackage* USsSubclassingUtils::GeneratedPackageEditorOnly = nullptr;
#endif
