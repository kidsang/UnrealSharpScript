using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Tests;

[RecordFilePath]
public class EmptyTest : IUnitTestInterface
{
	public bool RunTest()
	{
		Logger.Display("EmptyTest ok!");
		return true;
	}
}
