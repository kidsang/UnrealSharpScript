#pragma once
#include "CoreMinimal.h"
#include "UObject/Field.h"
#include "SsNativeFuncExporter.h"
#include "SsSubclassingUtils.generated.h"

class USsGeneratedClass;

/**
 * Meta info collected from csharp property definitions to create unreal property.
 * <br/> See: SubclassingUtils.cs PropertyDef
 */
struct FSsPropertyDef
{
	/** Name of property. */
	FName PropName;

	/** Class of UProperty, eg. UObjectProperty::StaticClass. */
	UClass* PropType;

	/** Underlying type of this property, e.g. UObjectProperty::PropertyClass. */
	UField* UnderlyingType;

	/** For UArrayProperty or USetProperty, this represents the type of inner property. */
	UClass* InnerPropType;

	/** For UArrayProperty or USetProperty, this represents the underlying type of inner property. */
	UField* InnerUnderlyingType;
	
	/** For UMapProperty, this represents the type of map key property. */
	UClass* KeyPropType;

	/** For UMapProperty, this represents the underlying type of map key property. */
	UField* KeyUnderlyingType;
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
	 * Called from by C#, generate a new unreal class from given infos.
	 * @param ManagedType The C# class which the new class will bind to.
	 * @param ClassName Name of the new class.
	 * @param SuperClass Base class of the new class.
	 * @param PropertyDefines Array of property defines.
	 * @param PropertyCount Count of property array.
	 * @return Newly generated class if success, otherwise nullptr.
	 */
	static USsGeneratedClass* GenerateClass(const void* ManagedType, const FName& ClassName, UClass* SuperClass,
	                                        const FSsPropertyDef* PropertyDefines, int PropertyCount);

	/** Return display string in the form of "'PropName'(PropType)" */
	static FString GetFriendlyName(const FSsPropertyDef& PropDef);

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
};
