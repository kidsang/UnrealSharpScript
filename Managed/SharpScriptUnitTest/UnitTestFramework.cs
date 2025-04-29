using System.Reflection;
using System.Runtime.InteropServices;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest;

public static class UnitTestFramework
{
	/// <summary>
	/// Get all unit test information
	/// </summary>
	/// <param name="nativeContext"></param>
	/// <param name="nativeCallback"></param>
	[UnmanagedCallersOnly]
	public static unsafe void GetAllTestInfos(void* nativeContext, delegate* unmanaged[Cdecl]<void*, void*, void*, int, void> nativeCallback)
	{
		var interfaceType = typeof(IUnitTestInterface);
		var types = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(s => s.GetTypes())
			.Where(p => interfaceType.IsAssignableFrom(p));


		foreach (var type in types)
		{
			if (type.FullName == null || type.IsAbstract)
			{
				continue;
			}


			string testName = type.FullName;
			string filePath = "";
			int lineNo = 0;
			foreach (var attr in type.GetCustomAttributes(typeof(RecordFilePathAttribute), false))
			{
				var filePathAttr = attr as RecordFilePathAttribute;
				if (filePathAttr == null)
				{
					continue;
				}

				filePath = filePathAttr.FilePath;
				lineNo = filePathAttr.LineNo;
				break;
			}

			fixed (char* testNamePointer = testName, filePathPointer = filePath)
			{
				nativeCallback(nativeContext, testNamePointer, filePathPointer, lineNo);
			}
		}
	}

	/// <summary>
	/// Execute unit test
	/// </summary>
	/// <param name="nativeTestName">Unit test class name</param>
	/// <returns>Whether the test passed</returns>
	[UnmanagedCallersOnly]
	public static int RunTest(IntPtr nativeTestName)
	{
		string testName = Marshal.PtrToStringUni(nativeTestName)!;
		Assembly currentAssembly = Assembly.GetExecutingAssembly();

		Type? unitTestType = currentAssembly.GetType(testName);
		if (unitTestType == null)
		{
			Logger.Error($"can't get unit test \"{testName}\" from assembly!");
			return 1;
		}

		object? unitTestObject = Activator.CreateInstance(unitTestType);
		if (unitTestObject == null)
		{
			Logger.Error($"can't create instance of \"{testName}\"!");
			return 2;
		}

		IUnitTestInterface? unitTest = unitTestObject as IUnitTestInterface;
		if (unitTest == null)
		{
			Logger.Error($"\"{testName}\" not implements \"UnitTestInterface\"!");
			return 3;
		}

		bool result = false;
		try
		{
			result = unitTest.RunTest();
		}
		catch (Exception e)
		{
			Logger.Error(e.ToString());
		}

		return result ? 0 : 1;
	}
}
