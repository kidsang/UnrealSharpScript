#include "SsTypeInterop.h"

UClass* USsTypeInterop::FindClass(const TCHAR* InClassName)
{
	return FindFirstObject<UClass>(InClassName);
}

UScriptStruct* USsTypeInterop::FindStruct(const TCHAR* InStructName)
{
	return FindFirstObject<UScriptStruct>(InStructName);
}

void* USsTypeInterop::CreateStructInstance(const UScriptStruct* InStruct)
{
	void* Dest = FMemory::Malloc(InStruct->GetStructureSize() ? InStruct->GetStructureSize() : 1, InStruct->GetMinAlignment());
	InStruct->InitializeStruct(Dest);
	return Dest;
}

void* USsTypeInterop::CloneStructInstance(const UScriptStruct* InStruct, const void* SrcInstance)
{
	void* Dest = FMemory::Malloc(InStruct->GetStructureSize() ? InStruct->GetStructureSize() : 1, InStruct->GetMinAlignment());
	InStruct->InitializeStruct(Dest);
	InStruct->CopyScriptStruct(Dest, SrcInstance);
	return Dest;
}

void USsTypeInterop::CopyStructInstance(const UScriptStruct* InStruct, const void* SrcInstance, void* DestInstance)
{
	InStruct->CopyScriptStruct(DestInstance, SrcInstance);
}

void USsTypeInterop::DestroyStructInstance(const UScriptStruct* InStruct, void* Instance)
{
	InStruct->DestroyStruct(Instance);
	FMemory::Free(Instance);
}

int USsTypeInterop::GetStructureSize(const UStruct* InType)
{
	return InType->GetStructureSize();
}

void USsTypeInterop::InitializeStruct(const UStruct* InType, void* Buffer)
{
	InType->InitializeStruct(Buffer);
}

void USsTypeInterop::DeinitializeStruct(const UStruct* InType, void* Buffer)
{
	InType->DestroyStruct(Buffer);
}

FName USsTypeInterop::GetTypeName(const UStruct* InType)
{
	return InType->GetFName();
}

UEnum* USsTypeInterop::FindEnum(const TCHAR* InEnumName)
{
	return FindFirstObject<UEnum>(InEnumName);
}

UFunction* USsTypeInterop::FindFunction(const UClass* InClass, FName InFuncName)
{
	return InClass->FindFunctionByName(InFuncName);
}

int USsTypeInterop::GetFunctionParamsSize(const UFunction* InFunc)
{
	return InFunc->ParmsSize;
}

void USsTypeInterop::InitializeFunctionParams(const UFunction* InFunc, void* Buffer)
{
	// see: UObject::CallFunctionByNameWithArguments
	for (FProperty* Property = InFunc->PropertyLink; Property && Property->HasAnyPropertyFlags(CPF_Parm); Property = Property->PropertyLinkNext)
	{
		if (Property->HasAnyPropertyFlags(CPF_ZeroConstructor))
		{
			// Buffer has been zero initialized.
			continue;
		}

		Property->InitializeValue_InContainer(Buffer);
	}
}

void USsTypeInterop::DeinitializeFunctionParams(const UFunction* InFunc, void* Buffer)
{
	// see: UObject::CallFunctionByNameWithArguments
	for (FProperty* Property = InFunc->PropertyLink; Property && Property->HasAnyPropertyFlags(CPF_Parm); Property = Property->PropertyLinkNext)
	{
		Property->DestroyValue_InContainer(Buffer);
	}
}

FProperty* USsTypeInterop::GetFirstProperty(const UStruct* InStruct)
{
	return InStruct->PropertyLink;
}

FProperty* USsTypeInterop::GetNextProperty(const FProperty* InProp)
{
	if (!InProp)
	{
		return nullptr;
	}
	return InProp->PropertyLinkNext;
}

FProperty* USsTypeInterop::FindProperty(const UStruct* InStruct, FName InPropName)
{
	return InStruct->FindPropertyByName(InPropName);
}

FName USsTypeInterop::GetPropertyName(const FProperty* InProp)
{
	return InProp->GetFName();
}

int32 USsTypeInterop::GetPropertyOffset(const FProperty* InProp)
{
	return InProp->GetOffset_ForInternal();
}

int32 USsTypeInterop::GetPropertyOffsetFromName(const UStruct* InStruct, FName InPropName)
{
	return InStruct->FindPropertyByName(InPropName)->GetOffset_ForInternal();
}

int32 USsTypeInterop::GetPropertySize(const FProperty* InProp)
{
	return InProp->GetSize();
}

uint8_t USsTypeInterop::GetBoolPropertyFieldMask(const FBoolProperty* InProp)
{
	return InProp->GetFieldMask();
}

const FProperty* USsTypeInterop::GetMapKeyProperty(const FMapProperty* InProp)
{
	return InProp->KeyProp;
}

const FProperty* USsTypeInterop::GetMapValueProperty(const FMapProperty* InProp)
{
	return InProp->ValueProp;
}

const FProperty* USsTypeInterop::GetSetElementProperty(const FSetProperty* InProp)
{
	return InProp->ElementProp;
}

const UFunction* USsTypeInterop::GetDelegateSignatureFunction(const FDelegateProperty* InProp)
{
	return InProp->SignatureFunction;
}

int32 USsTypeInterop::PropertyIdentical(const FProperty* InProp, const void* A, const void* B)
{
	bool bResult = InProp->Identical(A, B);
	return bResult ? 1 : 0;
}

void USsTypeInterop::InitializePropertyValue(const FProperty* InProp, void* Value)
{
	InProp->InitializeValue(Value);
}

void USsTypeInterop::CopyPropertyValue(const FProperty* InProp, void* Dest, const void* Src)
{
	InProp->CopyCompleteValue(Dest, Src);
}

void USsTypeInterop::DestroyPropertyValue(const FProperty* InProp, void* Value)
{
	InProp->DestroyValue(Value);
}

void USsTypeInterop::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&FindClass, TEXT("TypeInterop.NativeFindClass"));
	BindNativeCallbackFunc(&FindStruct, TEXT("TypeInterop.NativeFindStruct"));
	BindNativeCallbackFunc(&CreateStructInstance, TEXT("TypeInterop.NativeCreateStructInstance"));
	BindNativeCallbackFunc(&CloneStructInstance, TEXT("TypeInterop.NativeCloneStructInstance"));
	BindNativeCallbackFunc(&DestroyStructInstance, TEXT("TypeInterop.NativeDestroyStructInstance"));
	BindNativeCallbackFunc(&GetStructureSize, TEXT("TypeInterop.NativeGetStructureSize"));
	BindNativeCallbackFunc(&InitializeStruct, TEXT("TypeInterop.NativeInitializeStruct"));
	BindNativeCallbackFunc(&DeinitializeStruct, TEXT("TypeInterop.NativeDeinitializeStruct"));
	BindNativeCallbackFunc(&GetTypeName, TEXT("TypeInterop.NativeGetTypeName"));
	BindNativeCallbackFunc(&FindEnum, TEXT("TypeInterop.NativeFindEnum"));
	BindNativeCallbackFunc(&FindFunction, TEXT("TypeInterop.NativeFindFunction"));
	BindNativeCallbackFunc(&GetFunctionParamsSize, TEXT("TypeInterop.NativeGetFunctionParamsSize"));
	BindNativeCallbackFunc(&InitializeFunctionParams, TEXT("TypeInterop.NativeInitializeFunctionParams"));
	BindNativeCallbackFunc(&DeinitializeFunctionParams, TEXT("TypeInterop.NativeDeinitializeFunctionParams"));
	BindNativeCallbackFunc(&GetFirstProperty, TEXT("TypeInterop.NativeGetFirstProperty"));
	BindNativeCallbackFunc(&GetNextProperty, TEXT("TypeInterop.NativeGetNextProperty"));
	BindNativeCallbackFunc(&FindProperty, TEXT("TypeInterop.NativeFindProerty"));
	BindNativeCallbackFunc(&GetPropertyName, TEXT("TypeInterop.NativeGetPropertyName"));
	BindNativeCallbackFunc(&GetPropertyOffset, TEXT("TypeInterop.NativeGetPropertyOffset"));
	BindNativeCallbackFunc(&GetPropertyOffsetFromName, TEXT("TypeInterop.NativeGetPropertyOffsetFromName"));
	BindNativeCallbackFunc(&GetPropertySize, TEXT("TypeInterop.NativeGetPropertySize"));
	BindNativeCallbackFunc(&GetBoolPropertyFieldMask, TEXT("TypeInterop.NativeGetBoolPropertyFieldMask"));
	BindNativeCallbackFunc(&GetMapKeyProperty, TEXT("TypeInterop.NativeGetMapKeyProperty"));
	BindNativeCallbackFunc(&GetMapValueProperty, TEXT("TypeInterop.NativeGetMapValueProperty"));
	BindNativeCallbackFunc(&GetSetElementProperty, TEXT("TypeInterop.NativeGetSetElementProperty"));
	BindNativeCallbackFunc(&GetDelegateSignatureFunction, TEXT("TypeInterop.NativeGetDelegateSignatureFunction"));
	BindNativeCallbackFunc(&PropertyIdentical, TEXT("TypeInterop.NativePropertyIdentical"));
	BindNativeCallbackFunc(&InitializePropertyValue, TEXT("TypeInterop.NativeInitializePropertyValue"));
	BindNativeCallbackFunc(&CopyPropertyValue, TEXT("TypeInterop.NativeCopyPropertyValue"));
	BindNativeCallbackFunc(&DestroyPropertyValue, TEXT("TypeInterop.NativeDestroyPropertyValue"));
}
