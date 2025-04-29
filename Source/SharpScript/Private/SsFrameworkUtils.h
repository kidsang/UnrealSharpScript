#pragma once
#include "CoreMinimal.h"

namespace SsInternal
{
	/**
	 * Query functions in the C# framework and bind them to C++ function pointers for C++ side to call.
	 * @param FrameworkTypeName C# framework name
	 * @param FuncName Name of the C# function to be bound
	 * @param OutFuncPointer C++ function pointer to be bound
	 * @return Whether the binding was successful
	 * @remarks Can only bind static functions in the SharpScript.Main class, functions need to have the [UnmanagedCallersOnly] attribute.
	 */
	bool BindManagedFrameworkFunction(const TCHAR* FrameworkTypeName, const TCHAR* FuncName, void** OutFuncPointer);

	/**
	 * Query functions in the C# framework and bind them to C++ function pointers for C++ side to call.
	 * Omits the C# framework name, defaults to searching from "SharpScript.Main, SharpScript".
	 * @param FuncName Name of the C# function to be bound
	 * @param OutFuncPointer C++ function pointer to be bound
	 * @return Whether the binding was successful
	 * @remarks Can only bind static functions in the SharpScript.Main class, functions need to have the [UnmanagedCallersOnly] attribute.
	 */
	bool BindManagedFrameworkFunction(const TCHAR* FuncName, void** OutFuncPointer);
}
