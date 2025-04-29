using System.Reflection;
using System.Runtime.InteropServices;
using SharpScript;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test various interfaces on AssemblyManager
/// </summary>
[RecordFilePath]
public class AssemblyManagerTest : IUnitTestInterface
{
	public bool RunTest()
	{
		// The framework is loaded into the default ALC, does not support dynamic loading/unloading, and should not exist in AssemblyManager
		string frameworkAssemblyName = typeof(Logger).Assembly.GetName().Name!;
		Utils.Assert(!AssemblyManager.IsAssemblyRegistered(frameworkAssemblyName));
		Utils.Assert(!AssemblyManager.IsAssemblyLoaded(frameworkAssemblyName));

		// The unit test case itself supports dynamic loading and unloading
		string unitTestAssemblyName = Assembly.GetExecutingAssembly().GetName().Name!;
		Utils.Assert(AssemblyManager.IsAssemblyRegistered(unitTestAssemblyName));
		Utils.Assert(AssemblyManager.IsAssemblyLoaded(unitTestAssemblyName));

		// Ensure that the unit test assembly has not been corrupted by Reload
		Assembly? unitTestAssembly = AssemblyManager.GetLoadedAssembly(unitTestAssemblyName);
		Utils.Assert(unitTestAssembly != null);
		Utils.Assert(unitTestAssembly == Assembly.GetExecutingAssembly());

		// Ensure that the function pointer address obtained from C# is correct
		unsafe
		{
			string typeName = GetType().FullName!;
			nint funcAddr = 0;
			bool getFuncResult = AssemblyManager.GetAssemblyFunctionPointer(unitTestAssemblyName, typeName, "DummyFunction", (IntPtr)(&funcAddr));
			Utils.Assert(getFuncResult);
			Utils.Assert(funcAddr != 0);

			delegate* unmanaged<void> funcPtr = &DummyFunction;
			Utils.Assert((nint)funcPtr == funcAddr);
		}

		return true;
	}

	[UnmanagedCallersOnly]
	public static void DummyFunction()
	{
	}
}
