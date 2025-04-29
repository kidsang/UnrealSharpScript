#include "SharpScriptUnitTest.h"
#include "SharpScript.h"
#include "SharpScriptHost.h"
#include "SsAssemblyManager.h"
#include "HAL/PlatformProcess.h"
#include "Interfaces/IPluginManager.h"
#include "Misc/AutomationTest.h"
#include "Misc/Paths.h"

DEFINE_LOG_CATEGORY(LogSharpScript);

#define LOCTEXT_NAMESPACE "FSharpScriptUnitTest"

/**
* 单元测试上下文
* 用于和C#虚拟机通信，触发单元测试并获取结果。
*/
class FSharpScriptUnitTestContext
{
public:
	/** 获取单元测试信息列表 */
	void GetAllTestInfos(TArray<FString>& OutBeautifiedNames, TArray<FString>& OutTestCommands);

	/** 执行单元测试 */
	static bool RunTest(const FString& TestCommand);

private:
	/** 获取单元测试信息列表的C#回调 */
	static void OnGetOneTestInfos(FSharpScriptUnitTestContext* Context, const TCHAR* TestName, const TCHAR* FilePath, int LineNo);

	/** 编译构建C#测试用例程序集 */
	static bool BuildUnitTestAssemblies();

	/** 加载C#测试用例程序集 */
	static void LoadUnitTestAssemblies();

	/** 卸载C#测试用例程序集 */
	static void UnloadUnitTestAssemblies();

	/** 从测试用例程序集里获取C#函数指针 */
	static void GetManagedFunction(const TCHAR* FuncName, void** OutManagedFunc);

public:
	static constexpr const TCHAR* UnitTestAssemblyName = TEXT("SharpScriptUnitTest");
	static constexpr const TCHAR* FrameworkTypeName = TEXT("SharpScriptUnitTest.UnitTestFramework");

private:
	TArray<FString> BeautifiedNames;
	TArray<FString> TestCommands;
};

void FSharpScriptUnitTestContext::GetAllTestInfos(TArray<FString>& OutBeautifiedNames, TArray<FString>& OutTestCommands)
{
	// 获取测试用例信息之前重新构建测试用例程序集
	if (!BuildUnitTestAssemblies())
	{
		UE_LOG(LogSharpScript, Error, TEXT("Build unit test assemblies failed!"));
		return;
	}

	LoadUnitTestAssemblies();

	// 获取单元测试信息列表
	void (*ManagedGetAllTestInfos)(FSharpScriptUnitTestContext* Context, decltype(OnGetOneTestInfos)* Callback);
	GetManagedFunction(TEXT("GetAllTestInfos"), (void**)&ManagedGetAllTestInfos);
	ManagedGetAllTestInfos(this, OnGetOneTestInfos);

	OutBeautifiedNames = MoveTemp(BeautifiedNames);
	OutTestCommands = MoveTemp(TestCommands);

	UnloadUnitTestAssemblies();
}

bool FSharpScriptUnitTestContext::RunTest(const FString& TestCommand)
{
	LoadUnitTestAssemblies();

	int32 StartIndex;
	bool bResult = TestCommand.FindChar(TEXT('|'), StartIndex);
	check(bResult);

	FString TestName = TestCommand.LeftChop(TestCommand.Len() - StartIndex);
	int (*ManagedRunTest)(const TCHAR*);
	GetManagedFunction(TEXT("RunTest"), (void**)&ManagedRunTest);
	const TCHAR* TestNameStr = *TestName;
	bResult = ManagedRunTest(TestNameStr) == 0;

	UnloadUnitTestAssemblies();
	return bResult;
}

void FSharpScriptUnitTestContext::OnGetOneTestInfos(FSharpScriptUnitTestContext* Context, const TCHAR* TestName, const TCHAR* FilePath, int LineNo)
{
	FString BeautifiedName = TestName;
	int32 DotIndex;
	if (BeautifiedName.FindLastChar(TEXT('.'), DotIndex))
	{
		BeautifiedName.RightChopInline(DotIndex + 1);
	}
	BeautifiedName = "SharpScript." + BeautifiedName;
	Context->BeautifiedNames.Add(BeautifiedName);

	FString TestCommand = FString::Printf(TEXT("%s|%s|%d"), TestName, FilePath, LineNo);
	Context->TestCommands.Add(TestCommand);
}

bool FSharpScriptUnitTestContext::BuildUnitTestAssemblies()
{
	const FString PluginDir = FPaths::ConvertRelativePathToFull(
		IPluginManager::Get().FindPlugin(UE_PLUGIN_NAME)->GetBaseDir());

	const FString DotnetExe = FSharpScriptModule::GetDotnetExePath();
	const FString ProjectPath = PluginDir / "Managed" / "SharpScriptUnitTest";
	const FString OutputPath = PluginDir / "Managed" / "Assemblies";
	const FString Params = FString("build -c Release --no-dependencies ") + ProjectPath + " --output " + OutputPath;
	UE_LOG(LogSharpScript, Log, TEXT("building unit test assembly ..."));
	UE_LOG(LogSharpScript, Log, TEXT("%s %s"), *DotnetExe, *Params);

	void* PipeRead = nullptr;
	void* PipeWrite = nullptr;
	verify(FPlatformProcess::CreatePipe(PipeRead, PipeWrite));

	bool bLaunchHidden = true;
	FProcHandle ProcHandle = FPlatformProcess::CreateProc(
		*DotnetExe, *Params, false, bLaunchHidden, bLaunchHidden,
		nullptr, 0, nullptr, PipeWrite, PipeRead);
	if (!ProcHandle.IsValid())
	{
		return false;
	}

	while (FPlatformProcess::IsProcRunning(ProcHandle))
	{
		FString ProcOutput = FPlatformProcess::ReadPipe(PipeRead);
		if (!ProcOutput.IsEmpty())
		{
			UE_LOG(LogSharpScript, Log, TEXT("%s"), *ProcOutput);
		}
		FPlatformProcess::Sleep(0.1f);
	}

	int32 ReturnCode = -1;
	bool bGotReturnCode = FPlatformProcess::GetProcReturnCode(ProcHandle, &ReturnCode);
	check(bGotReturnCode);

	FPlatformProcess::CloseProc(ProcHandle);
	FPlatformProcess::ClosePipe(PipeRead, PipeWrite);
	return ReturnCode == 0;
}

void FSharpScriptUnitTestContext::LoadUnitTestAssemblies()
{
	bool bResult = FSsAssemblyManager::LoadAssembly(UnitTestAssemblyName);
	check(bResult);
}

void FSharpScriptUnitTestContext::UnloadUnitTestAssemblies()
{
	bool bResult = FSsAssemblyManager::UnloadAssembly(UnitTestAssemblyName);
	check(bResult || FSharpScriptHostModule::IsUsingMonoRuntime()); // NOTE: 至少在8.0.6版本，Mono运行时依旧不支持卸载程序集。
}

void FSharpScriptUnitTestContext::GetManagedFunction(const TCHAR* FuncName, void** OutManagedFunc)
{
	bool bResult = FSsAssemblyManager::GetAssemblyFunctionPointer(
		UnitTestAssemblyName, FrameworkTypeName, FuncName, OutManagedFunc);
	check(bResult);
}

class FSharpScriptUnitTestBase : public FAutomationTestBase
{
public:
	FSharpScriptUnitTestBase(const FString& InName, bool bInComplexTask)
		: FAutomationTestBase(InName, bInComplexTask)
	{
	}

	virtual FString GetTestSourceFileName() const override { return __FILE__; }

	virtual FString GetTestSourceFileName(const FString& InTestName) const override
	{
		int32 StartIndex;
		bool bResult = InTestName.FindChar(TEXT('|'), StartIndex);
		check(bResult);

		int32 EndIndex;
		bResult = InTestName.FindLastChar(TEXT('|'), EndIndex);
		check(bResult);

		FString FilePath = InTestName.LeftChop(InTestName.Len() - EndIndex);
		FilePath.RightChopInline(StartIndex + 1);
		return FilePath;
	}

	virtual int32 GetTestSourceFileLine() const override { return __LINE__; }

	virtual int32 GetTestSourceFileLine(const FString& InTestName) const override
	{
		int32 EndIndex;
		bool bResult = InTestName.FindLastChar(TEXT('|'), EndIndex);
		check(bResult);

		FString LineNoStr = InTestName.RightChop(EndIndex + 1);
		return FCString::Atoi(*LineNoStr);
	}
};

IMPLEMENT_CUSTOM_COMPLEX_AUTOMATION_TEST(
	FSharpScriptUnitTest,
	FSharpScriptUnitTestBase,
	"Functional Tests",
	EAutomationTestFlags::EditorContext | EAutomationTestFlags::EngineFilter)

void FSharpScriptUnitTest::GetTests(TArray<FString>& OutBeautifiedNames, TArray<FString>& OutTestCommands) const
{
	FSharpScriptUnitTestContext Context;
	Context.GetAllTestInfos(OutBeautifiedNames, OutTestCommands);
}

bool FSharpScriptUnitTest::RunTest(const FString& Parameters)
{
	return FSharpScriptUnitTestContext::RunTest(Parameters);
}

#undef LOCTEXT_NAMESPACE


IMPLEMENT_MODULE(FSharpScriptUnitTestModule, SharpScriptUnitTest)

void FSharpScriptUnitTestModule::StartupModule()
{
	const FString PluginDir = FPaths::ConvertRelativePathToFull(IPluginManager::Get().FindPlugin(UE_PLUGIN_NAME)->GetBaseDir());
	const FString UnitTestAssemblyPath = PluginDir / "Managed/Assemblies" / FSharpScriptUnitTestContext::UnitTestAssemblyName + ".dll";
	FSsAssemblyManager::RegisterAssembly(FSharpScriptUnitTestContext::UnitTestAssemblyName, *UnitTestAssemblyPath);
}

void FSharpScriptUnitTestModule::ShutdownModule()
{
}
