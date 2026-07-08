#pragma once
#include "CoreMinimal.h"
#include "UObject/Field.h"
#include "SsNativeFuncExporter.h"
#include "SsSubclassingUtils.generated.h"

class USsGeneratedClass;
class USsGeneratedStruct;
class USsGeneratedEnum;

/**
 * A single metadata key/value pair applied to a generated type.
 * <br/> See: SubclassingUtils.cs MetaDataEntry
 */
struct FSsMetaDataEntry
{
	/** Metadata key. */
	FName Key;

	/** Null-terminated metadata value string (TCHAR). Owned by the caller for the call duration. */
	const TCHAR* Value = nullptr;
};

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

	/** The raw C# PropSpecs bit set, passed through unchanged. Expanded by FSsPropertySpecifiers. */
	uint64 Specifiers = 0;

	/** Array of metadata entries (may be null when MetaCount is 0). */
	const FSsMetaDataEntry* MetaEntries = nullptr;

	/** Count of metadata entry array. */
	int MetaCount = 0;

	/** Return display string in the form of "'PropName'(PropType)" */
	FString GetFriendlyName() const;

private:
	static FString GetFriendlyTypeName(const FSsPropertyDef& PropDef);
};

/**
 * Meta info collected from csharp enum value definitions to create unreal enum.
 * <br/> See: SubclassingUtils.cs EnumValueDef
 */
struct FSsEnumValueDef
{
	/** Name of enum value. */
	FName Name;

	/** Underlying integer value of enum value. */
	int64 Value = 0;
};

/**
 * Engine-agnostic UFUNCTION parameter role flags supplied by the C# layer.
 * <br/> The C# side must NOT hardcode UE EPropertyFlags (CPF_*), because those values may differ
 * across engine versions. C# passes these stable values, and C++ translates them to CPF_* in
 * TranslateParamFlags().
 * <br/> See: SubclassingUtils.cs ESsFunctionParamFlags
 */
enum class ESsFunctionParamFlags : uint32
{
	None = 0,
	/** Normal input parameter. */
	InParam = 1 << 0,
	/** Value is copied out after the call (out parameter). */
	OutParam = 1 << 1,
	/** The return value of the function. */
	ReturnParam = 1 << 2,
};

ENUM_CLASS_FLAGS(ESsFunctionParamFlags);

/**
 * Engine-agnostic UFUNCTION role flags supplied by the C# layer.
 * <br/> As with ESsFunctionParamFlags, the C# side must NOT hardcode UE EFunctionFlags (FUNC_*),
 * because those values may differ across engine versions. C# passes these stable values, and C++
 * translates them to FUNC_* in TranslateFunctionFlags().
 * <br/> See: SubclassingUtils.cs SsFunctionFlags
 */
enum class ESsFunctionFlags : uint32
{
	None = 0,
	/** The function is a C# static method (generated UFunction gets FUNC_Static, no "this" is resolved). */
	Static = 1 << 0,
};

ENUM_CLASS_FLAGS(ESsFunctionFlags);

/**
 * Meta info collected from a csharp UFUNCTION parameter to create a UFunction parameter property.
 * <br/> Shares the same shape as FSsPropertyDef, plus a flags field describing the parameter role
 * (input / out / return / by-ref).
 * <br/> See: SubclassingUtils.cs FunctionParamDef
 */
struct FSsFunctionParamDef
{
	/** Name of the parameter. */
	FName ParamName;

	/** Class of UProperty, eg. UIntProperty::StaticClass. */
	UClass* PropType = nullptr;

	/** Underlying type of this parameter, e.g. object property class / struct / enum. */
	UField* UnderlyingType = nullptr;

	/** For array/set inner or map value: the inner property class. */
	UClass* InnerPropType = nullptr;

	/** For array/set inner or map value: the inner underlying type. */
	UField* InnerUnderlyingType = nullptr;

	/** For map key: the key property class. */
	UClass* KeyPropType = nullptr;

	/** For map key: the key underlying type. */
	UField* KeyUnderlyingType = nullptr;

	/** Engine-agnostic parameter role flags. Translated to CPF_* by TranslateParamFlags(). */
	ESsFunctionParamFlags ParamFlags = ESsFunctionParamFlags::None;
};

/**
 * Meta info collected from a csharp UFUNCTION definition to create a UFunction on a generated class.
 * <br/> See: SubclassingUtils.cs FunctionDef
 */
struct FSsFunctionDef
{
	/** Name of the function. */
	FName FuncName;

	/** Array of parameter defines (input params first, then out params, then return value). */
	const FSsFunctionParamDef* Params = nullptr;

	/** Count of parameter array. */
	int ParamCount = 0;

	/**
	 * The managed dispatch function pointer for this function.
	 * A csharp static [UnmanagedCallersOnly] stub with signature
	 * void(IntPtr objectHandle, void* paramsBuffer) that reads params, calls the user method and writes back.
	 * For static functions the objectHandle argument is unused (nullptr is passed).
	 */
	const void* ManagedDispatch = nullptr;

	/** Engine-agnostic function role flags. Translated to FUNC_* by TranslateFunctionFlags(). */
	ESsFunctionFlags FunctionFlags = ESsFunctionFlags::None;
};

/**
 * Meta info collected from a csharp [UCLASS] definition to create a unreal class.
 * <br/> See: SubclassingUtils.cs ClassDef
 */
struct FSsClassDef
{
	/** Name of the class. */
	FName ClassName;

	/** Native super UClass pointer. */
	UClass* SuperClass = nullptr;

	/** Array of property defines. */
	const FSsPropertyDef* PropertyDefines = nullptr;

	/** Count of property array. */
	int PropertyCount = 0;

	/** Array of function defines (may be null when FunctionCount is 0). */
	const FSsFunctionDef* FunctionDefines = nullptr;

	/** Count of function array. */
	int FunctionCount = 0;

	/** The raw C# ClassSpecs bit set, passed through unchanged. Expanded by SsClassSpecifiers. */
	uint64 Specifiers = 0;

	/** Array of metadata entries (may be null when MetaCount is 0). */
	const FSsMetaDataEntry* MetaEntries = nullptr;

	/** Count of metadata entry array. */
	int MetaCount = 0;

	/**
	 * Configuration file name for this class (equivalent to UCLASS(Config=X)).
	 * May be null when no Config was specified; in that case the class inherits
	 * ClassConfigName from its super class.
	 */
	const TCHAR* ConfigName = nullptr;
};

/**
 * Provides functionality to register and create subclassing types in C#.
 */
UCLASS()
class USsSubclassingUtils final : public USsNativeFuncExporter
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
	 * Called by C#, generate a new unreal class from given infos. Optionally declares UFunctions
	 * implemented in C#.
	 * @param ManagedType The C# class which the new class will bind to.
	 * @param ClassDef The class definition bundle (name, super, properties, functions, specifiers, metadata).
	 * @return Newly generated class if success, otherwise nullptr.
	 */
	static USsGeneratedClass* GenerateClass(const void* ManagedType, const FSsClassDef* ClassDef);

	/** Translate engine-agnostic C# parameter flags to UE EPropertyFlags for the current engine version. */
	static uint64 TranslateParamFlags(ESsFunctionParamFlags ParamFlags);

	/** Translate engine-agnostic C# function flags to UE EFunctionFlags for the current engine version. */
	static uint32 TranslateFunctionFlags(ESsFunctionFlags FunctionFlags);

	/**
	 * Called by C#, generate a new unreal struct from given infos.
	 * @param StructName Name of the new struct.
	 * @param PropertyDefines Array of property defines.
	 * @param PropertyCount Count of property array.
	 * @return Newly generated struct if success, otherwise nullptr.
	 */
	static USsGeneratedStruct* GenerateStruct(const FName& StructName, const FSsPropertyDef* PropertyDefines, int PropertyCount);

	/**
	 * Called by C#, generate a new unreal enum from given infos.
	 * @param EnumName Name of the new enum.
	 * @param ValueDefines Array of enum value defines.
	 * @param ValueCount Count of enum value array.
	 * @param bIsFlags Whether the C# enum was declared with [Flags]; sets EEnumFlags::Flags on the generated UEnum.
	 * @return Newly generated enum if success, otherwise nullptr.
	 */
	static USsGeneratedEnum* GenerateEnum(const FName& EnumName, const FSsEnumValueDef* ValueDefines, int ValueCount, bool bIsFlags);

	/**
	 * Create new property by definition.
	 * @param Owner Property owner, UClass or UScriptStruct.
	 * @param PropDef Property definition.
	 * @return Returns newly created property if success, nullptr if failed.
	 */
	static FProperty* CreateProperty(FFieldVariant Owner, const FSsPropertyDef& PropDef);

protected:
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
	static UClass* StructPropertyClass;
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
