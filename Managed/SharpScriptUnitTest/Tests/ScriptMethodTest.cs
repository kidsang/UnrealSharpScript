using TestStruct = UnrealEngine.SharpScriptUnitTest.SsTestNumericStruct;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test struct extension methods exported through "ScriptMethod" metadata.
/// </summary>
[RecordFilePath]
public class ScriptMethodTest : IUnitTestInterface
{
	public bool RunTest()
	{
		TestStruct testStruct = new TestStruct
		{
			X = 10,
			Y = 20
		};

		Utils.Assert(testStruct.ToString() == "X=10 Y=20");

		return true;
	}
}
