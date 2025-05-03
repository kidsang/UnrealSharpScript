using UnrealEngine.CoreUObject;
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

		testStruct.TestDefaultValue();
		Utils.Assert(testStruct.X == 1);
		Utils.Assert(testStruct.Y == 0);

		testStruct.TestDefaultValue(new Vector { X = 10, Y = 20 });
		Utils.Assert(testStruct.X == 10);
		Utils.Assert(testStruct.Y == 20);

		return true;
	}
}
