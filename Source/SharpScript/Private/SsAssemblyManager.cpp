#include "SsAssemblyManager.h"
#include "SsCommon.h"
#include "SsFrameworkUtils.h"
#include "Misc/FileHelper.h"

FSsAssemblyManager* FSsAssemblyManager::Get()
{
	return Instance;
}

bool FSsAssemblyManager::Initialize()
{
	static TPair<const TCHAR*, void**> FuncInfos[] = {
		{TEXT("RegisterAssembly"), (void**)&ManagedRegisterAssembly},
		{TEXT("UnregisterAssembly"), (void**)&ManagedUnregisterAssembly},
		{TEXT("IsAssemblyRegistered"), (void**)&ManagedIsAssemblyRegistered},
		{TEXT("LoadAssembly"), (void**)&ManagedLoadAssembly},
		{TEXT("UnloadAssembly"), (void**)&ManagedUnloadAssembly},
		{TEXT("ReloadAssembly"), (void**)&ManagedReloadAssembly},
		{TEXT("IsAssemblyLoaded"), (void**)&ManagedIsAssemblyLoaded},
		{TEXT("InternalLoadAssemblyBytes"), (void**)&ManagedInternalLoadAssemblyBytes},
		{TEXT("GetAssemblyFunctionPointer"), (void**)&ManagedGetAssemblyFunctionPointer},
	};

	for (auto& Pair : FuncInfos)
	{
		if (!SsInternal::BindManagedFrameworkFunction(Pair.Key, Pair.Value))
		{
			return false;
		}
	}

	return true;
}

void FSsAssemblyManager::Finalize()
{
}

bool FSsAssemblyManager::RegisterAssembly(const TCHAR* AssemblyName, const TCHAR* AssemblyPath)
{
	return ManagedRegisterAssembly(AssemblyName, AssemblyPath) == 0;
}

void FSsAssemblyManager::UnregisterAssembly(const TCHAR* AssemblyName)
{
	ManagedUnregisterAssembly(AssemblyName);
}

bool FSsAssemblyManager::IsAssemblyRegistered(const TCHAR* AssemblyName)
{
	return ManagedIsAssemblyRegistered(AssemblyName) == 0;
}

bool FSsAssemblyManager::LoadAssembly(const TCHAR* AssemblyName)
{
	return ManagedLoadAssembly(AssemblyName) == 0;
}

bool FSsAssemblyManager::UnloadAssembly(const TCHAR* AssemblyName)
{
	return ManagedUnloadAssembly(AssemblyName) == 0;
}

bool FSsAssemblyManager::ReloadAssembly(const TCHAR* AssemblyName)
{
	return ManagedReloadAssembly(AssemblyName) == 0;
}

bool FSsAssemblyManager::IsAssemblyLoaded(const TCHAR* AssemblyName)
{
	return ManagedIsAssemblyLoaded(AssemblyName) == 0;
}

bool FSsAssemblyManager::GetAssemblyFunctionPointer(const TCHAR* AssemblyName, const TCHAR* TypeName, const TCHAR* FuncName, void** OutFuncPointer)
{
	return ManagedGetAssemblyFunctionPointer(AssemblyName, TypeName, FuncName, OutFuncPointer) == 0;
}

int FSsAssemblyManager::InternalLoadAssembly(void* AssemblyLoadContext, const TCHAR* AssemblyPath)
{
	TArray<uint8> AssemblyBytes;
	if (!FFileHelper::LoadFileToArray(AssemblyBytes, AssemblyPath))
	{
		UE_LOG(LogSharpScript, Error, TEXT("failed to read assembly bytes data from '%s'"), AssemblyPath);
		return 1;
	}

#if WITH_EDITOR || !UE_BUILD_SHIPPING
	// In editor or non-shipping builds, attempt to load pdb for debugging
	FString SymbolePath(AssemblyPath);
	SymbolePath.ReplaceInline(TEXT(".dll"), TEXT(".pdb"));
	TArray<uint8> SymbolBytes;
	FFileHelper::LoadFileToArray(SymbolBytes, *SymbolePath);
	int SymbolBytesLength = SymbolBytes.Num();
	const uint8* SymbolBytesPtr = SymbolBytesLength > 0 ? SymbolBytes.GetData() : nullptr;
#else
	int SymbolBytesLength = 0;
	const uint8* SymbolBytesPtr = nullptr;
#endif

	int result = ManagedInternalLoadAssemblyBytes(AssemblyLoadContext, AssemblyBytes.GetData(), AssemblyBytes.Num(), SymbolBytesPtr, SymbolBytesLength);
	if (result != 0)
	{
		UE_LOG(LogSharpScript, Error, TEXT("failed to load assembly bytes from '%s'"), AssemblyPath);
	}

	return result;
}

FSsAssemblyManager* FSsAssemblyManager::Instance = nullptr;

int (*FSsAssemblyManager::ManagedRegisterAssembly)(const TCHAR* AssemblyName, const TCHAR* AssemblyPath) = nullptr;
void (*FSsAssemblyManager::ManagedUnregisterAssembly)(const TCHAR* AssemblyName) = nullptr;
int (*FSsAssemblyManager::ManagedIsAssemblyRegistered)(const TCHAR* AssemblyName) = nullptr;
int (*FSsAssemblyManager::ManagedLoadAssembly)(const TCHAR* AssemblyName) = nullptr;
int (*FSsAssemblyManager::ManagedUnloadAssembly)(const TCHAR* AssemblyName) = nullptr;
int (*FSsAssemblyManager::ManagedReloadAssembly)(const TCHAR* AssemblyName) = nullptr;
int (*FSsAssemblyManager::ManagedIsAssemblyLoaded)(const TCHAR* AssemblyName) = nullptr;
int (*FSsAssemblyManager::ManagedInternalLoadAssemblyBytes)(void* AssemblyLoadContext, const uint8* AssemblyBytes, uint32 AssemblyBytesLength, const uint8* SymbolBytes, uint32 SymbolBytesLength) = nullptr;
int (*FSsAssemblyManager::ManagedGetAssemblyFunctionPointer)(const TCHAR* AssemblyName, const TCHAR* TypeName, const TCHAR* FuncName, void** OutFuncPointer) = nullptr;

void USsAssemblyManagerExporter::DoExportFunctions(FSsBindNativeCallbackFunc BindNativeCallbackFunc)
{
	BindNativeCallbackFunc(&FSsAssemblyManager::InternalLoadAssembly, TEXT("AssemblyManager.NativeLoadAssembly"));
}
