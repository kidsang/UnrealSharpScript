using static UnrealEngine.Globals;
using SharpScriptUnitTest.Types;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test the lifecycle of C# exported objects.
/// </summary>
[RecordFilePath]
public class LifeCycleTest : IUnitTestInterface
{
	public bool RunTest()
	{
		SsTestObject obj = NewObject<SsTestObject>();
		SsTestObject cdo = SsTestObject.StaticClass.GetDefaultObject()!;

		// Newly created objects should be valid.
		Utils.Assert(obj.IsValid());
		Utils.Assert(cdo.IsValid());

		CollectGarbage();
		// Since C# objects have weak references to UE objects, after triggering UE garbage collection, C# objects should become invalid.
		Utils.Assert(!obj.IsValid());
		// CDO will not be collected, so it remains valid.
		Utils.Assert(cdo.IsValid());

		return true;
	}
}
