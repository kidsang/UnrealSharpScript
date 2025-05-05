#pragma once
#include "CoreMinimal.h"
#include "SsNativeFuncExporter.h"
#include "SsTypeInterop.generated.h"

UCLASS()
class USsTypeInterop : public USsNativeFuncExporter
{
	GENERATED_BODY()

	static UClass* FindClass(const TCHAR* InClassName);

	static UScriptStruct* FindStruct(const TCHAR* InStructName);

	static void* CreateStructInstance(const UScriptStruct* InStruct);

	static void* CloneStructInstance(const UScriptStruct* InStruct, const void* SrcInstance);

	static void CopyStructInstance(const UScriptStruct* InStruct, const void* SrcInstance, void* DestInstance);

	static void DestroyStructInstance(const UScriptStruct* InStruct, void* Instance);

	static int GetStructureSize(const UStruct* InType);

	static void InitializeStruct(const UStruct* InType, void* Buffer);

	static void DeinitializeStruct(const UStruct* InType, void* Buffer);

	static FName GetTypeName(const UStruct* InType);

	static UEnum* FindEnum(const TCHAR* InEnumName);

	static UFunction* FindFunction(const UClass* InClass, FName InFuncName);

	static int GetFunctionParamsSize(const UFunction* InFunc);

	static void InitializeFunctionParams(const UFunction* InFunc, void* Buffer);

	static void DeinitializeFunctionParams(const UFunction* InFunc, void* Buffer);

	static FProperty* GetFirstProperty(const UStruct* InStruct);

	static FProperty* GetNextProperty(const FProperty* InProp);

	static FProperty* FindProperty(const UStruct* InStruct, FName InPropName);

	static FName GetPropertyName(const FProperty* InProp);

	static int32 GetPropertyOffset(const FProperty* InProp);

	static int32 GetPropertyOffsetFromName(const UStruct* InStruct, FName InPropName);

	static int32 GetPropertySize(const FProperty* InProp);

	static uint8_t GetBoolPropertyFieldMask(const FBoolProperty* InProp);

	static const FProperty* GetMapKeyProperty(const FMapProperty* InProp);

	static const FProperty* GetMapValueProperty(const FMapProperty* InProp);

	static const FProperty* GetSetElementProperty(const FSetProperty* InProp);

	static const UFunction* GetDelegateSignatureFunction(const FDelegateProperty* InProp);

	static int32 PropertyIdentical(const FProperty* InProp, const void* A, const void* B);

	static void InitializePropertyValue(const FProperty* InProp, void* Value);

	static void CopyPropertyValue(const FProperty* InProp, void* Dest, const void* Src);

	static void DestroyPropertyValue(const FProperty* InProp, void* Value);

	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) override;
};
