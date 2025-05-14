#if WITH_EDITOR
using UnrealEngine.SharpScriptUnitTest;
#endif

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test preprocessor macros defined in C#.
/// </summary>
[RecordFilePath]
public class BindingPreprocessorTest : IUnitTestInterface
{
	public bool RunTest()
	{
#if WITH_EDITOR
		int result = USsBindingPreprocessorTest.FuncWithEditor();
		Utils.Assert(result == 10);
#endif

		return true;
	}
}
