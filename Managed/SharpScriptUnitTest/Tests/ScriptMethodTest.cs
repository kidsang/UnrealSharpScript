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

		// override ToString
		Utils.Assert(testStruct.ToString() == "X=10 Y=20");

		// test default value
		testStruct.TestDefaultValue();
		Utils.Assert(testStruct.X == 1);
		Utils.Assert(testStruct.Y == 0);

		testStruct.TestDefaultValue(new Vector { X = 10, Y = 20 });
		Utils.Assert(testStruct.X == 10);
		Utils.Assert(testStruct.Y == 20);

		// test auto cast
		IntPoint testPoint = testStruct;
		Utils.Assert(testPoint.X == 10);
		Utils.Assert(testPoint.Y == 20);

		// test equality
		TestStruct structA = testStruct;
		TestStruct structB = new();
		Utils.Assert(testStruct.Equals(structA));
		Utils.Assert(!testStruct.Equals(structB));
		Utils.Assert(testStruct.Equals(structA as object));
		Utils.Assert(!testStruct.Equals(structB as object));
		Utils.Assert(!testStruct.Equals(null));
		Utils.Assert(testStruct == structA);
		Utils.Assert(testStruct != structB);

		return true;
	}
}
