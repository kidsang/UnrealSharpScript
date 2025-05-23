#pragma once
#include "CoreMinimal.h"
#include "UObject/Field.h"
#include "SsNativeFuncExporter.h"
#include "SsSubclassingUtils.generated.h"

class USsGeneratedClass;
class USsGeneratedStruct;

/**
 * Meta info collected from csharp property definitions to create unreal property.
 * <br/> See: SubclassingUtils.cs PropertyDef
 */
struct FSsPropertyDef
{
	/** Name of property. */
	FName PropName;

	/** Class of UProperty, eg. UObjectProperty::StaticClass. */
	UClass* PropType = nullptr;

	/** Underlying type of this property, e.g. UObjectProperty::PropertyClass. */
	UField* UnderlyingType = nullptr;

	/** For UArrayProperty or USetProperty, this represents the type of inner property. */
	UClass* InnerPropType = nullptr;

	/** For UArrayProperty or USetProperty, this represents the underlying type of inner property. */
	UField* InnerUnderlyingType = nullptr;

	/** For UMapProperty, this represents the type of map key property. */
	UClass* KeyPropType = nullptr;

	/** For UMapProperty, this represents the underlying type of map key property. */
	UField* KeyUnderlyingType = nullptr;

	/** Return display string in the form of "'PropName'(PropType)" */
	FString GetFriendlyName() const;

private:
	static FString GetFriendlyTypeName(const FSsPropertyDef& PropDef);
};

/**
 * Provides functionality to register and create subclassing types in C#.
 */
UCLASS()
class USsSubclassingUtils : public USsNativeFuncExporter
{
	GENERATED_BODY()

public:
	static bool Initialize();

	static void Finalize();

	static UPackage* GetGeneratedPackage();

#if WITH_EDITOR
	static UPackage* GetGeneratedPackageEditorOnly();
#endif

	/**
	 * Called by C#, generate a new unreal class from given infos.
	 * @param ManagedType The C# class which the new class will bind to.
	 * @param ClassName Name of the new class.
	 * @param SuperClass Base class of the new class.
	 * @param PropertyDefines Array of property defines.
	 * @param PropertyCount Count of property array.
	 * @return Newly generated class if success, otherwise nullptr.
	 */
	static USsGeneratedClass* GenerateClass(const void* ManagedType, const FName& ClassName, UClass* SuperClass,
	                                        const FSsPropertyDef* PropertyDefines, int PropertyCount);

	/**
	 * Called by C#, generate a new unreal struct from given infos.
	 * @param StructName Name of the new struct.
	 * @param PropertyDefines Array of property defines.
	 * @param PropertyCount Count of property array.
	 * @return Newly generated struct if success, otherwise nullptr.
	 */
	static USsGeneratedStruct* GenerateStruct(const FName& StructName, const FSsPropertyDef* PropertyDefines,
	                                          int PropertyCount);

	/**
	 * Create new property by definition.
	 * @param Owner Property owner, UClass or UScriptStruct.
	 * @param PropDef Property definition.
	 * @return Returns newly created property if success, nullptr if failed.
	 */
	static FProperty* CreateProperty(FFieldVariant Owner, const FSsPropertyDef& PropDef);

private:
	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;

private:
	static UPackage* CoreUObjectPackage;
	static UPackage* GeneratedPackage;
#if WITH_EDITOR
	static UPackage* GeneratedPackageEditorOnly;
#endif
	static UClass* Int8PropertyClass;
	static UClass* Int16PropertyClass;
	static UClass* IntPropertyClass;
	static UClass* Int64PropertyClass;
	static UClass* BytePropertyClass;
	static UClass* UInt16PropertyClass;
	static UClass* UInt32PropertyClass;
	static UClass* UInt64PropertyClass;
	static UClass* FloatPropertyClass;
	static UClass* DoublePropertyClass;
	static UClass* BoolPropertyClass;
	static UClass* StrPropertyClass;
	static UClass* NamePropertyClass;
	static UClass* TextPropertyClass;
	static UClass* ObjectPropertyBaseClass;
	static UClass* ObjectPropertyClass;
	static UClass* SoftObjectPropertyClass;
	static UClass* LazyObjectPropertyClass;
	static UClass* ClassPropertyClass;
	static UClass* SoftClassPropertyClass;
	static UClass* ArrayPropertyClass;
	static UClass* SetPropertyClass;
	static UClass* MapPropertyClass;

	friend struct FSsPropertyDef;
};
