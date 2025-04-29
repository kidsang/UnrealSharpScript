#pragma once
#include "CoreMinimal.h"
#include "UObject/Object.h"
#include "SsNativeFuncExporter.generated.h"

/**
 * @param NativeFunc Pointer to the C++ static function to be exported to C#.
 * @param BindPath Name of the C# delegate to bind to, in the format "[ClassName].[DelegateName]".
 * @param UnmanagedDelegates Dictionary containing the C# delegates that need to be bound.
 */
using FSsBindNativeCallback = void(*)(const void* /* NativeFunc */, const TCHAR* /* BindPath */, const void* /* UnmanagedDelegates */);

/**
 * Function object to assist in passing the UnmanagedDelegates pointer.
 */
class FSsBindNativeCallbackFunc
{
public:
	FSsBindNativeCallbackFunc(FSsBindNativeCallback FuncPtr, const void* UnmanagedDelegates)
		: FuncPtr(FuncPtr), UnmanagedDelegates(UnmanagedDelegates)
	{
	}

	void operator ()(const void* NativeFunc, const TCHAR* BindPath)
	{
		FuncPtr(NativeFunc, BindPath, UnmanagedDelegates);
	}

private:
	FSsBindNativeCallback FuncPtr;
	const void* UnmanagedDelegates;
};

/**
 * A helper class for binding C++ static functions with C# delegates.
 * To export C++ static methods for use in C#, inherit from this class and implement the `DoExportFunctions` function.
 * C# delegates that can be bound must be static class members, the C# class containing the delegate needs to be marked with [NativeCallbacks], and the delegate itself needs to be declared as unmanaged.
 */
UCLASS(Abstract, NotBlueprintable, NotBlueprintType)
class SHARPSCRIPT_API USsNativeFuncExporter : public UObject
{
	GENERATED_BODY()

public:
	static void ExportFunctions(FSsBindNativeCallback BindNativeCallback, const void* UnmanagedDelegates);

protected:
	//~ Begin USsNativeFuncExporter Interface
	virtual void DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc) { PURE_VIRTUAL() }
	//~ End USsNativeFuncExporter Interface
};
