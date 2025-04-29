#include "SsFrameworkUtils.h"
#include "SsCommon.h"
#include "SharpScriptHostAPI.h"

namespace SsInternal
{
	bool BindManagedFrameworkFunction(const TCHAR* ManagedFrameworkTypeName, const TCHAR* FuncName, void** OutFuncPointer)
	{
		int result = HostfxrGetFunctionPointer(
			ManagedFrameworkTypeName,
			FuncName,
			UNMANAGEDCALLERSONLY_METHOD, nullptr, nullptr,
			OutFuncPointer);
		if (result != 0)
		{
			UE_LOG(LogSharpScript, Error, TEXT("failed to bind managed framework function '%s'"), FuncName);
			return false;
		}

		return true;
	}

	bool BindManagedFrameworkFunction(const TCHAR* FuncName, void** OutFuncPointer)
	{
		static constexpr const TCHAR* FrameworkTypeName = TEXT("SharpScript.Main, SharpScript");
		return BindManagedFrameworkFunction(FrameworkTypeName, FuncName, OutFuncPointer);
	}
}
