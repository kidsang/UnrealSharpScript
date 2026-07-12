using SharpScriptUnitTest.Types;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Validates the subclassing <c>BlueprintEvent</c> support (the "single-method" design) end to end in a
/// pure-C# environment.
/// </summary>
[RecordFilePath]
public class SubclassingBlueprintEventTest : IUnitTestInterface
{
	public bool RunTest()
	{
		USsTestGenBlueprintEvent testObj = NewObject<USsTestGenBlueprintEvent>();

		// --- Case 1: direct source call site -> intercepted -> virtual dispatch -> C# body ------------
		// If interception silently failed and the body ran directly, the value would still be correct,
		// so we additionally require the body-ran flag to be set (proves a single dispatch through
		// ProcessEvent into the body, with no recursion / no double execution).
		testObj.DoubleScoreBodyRan = false;
		int scored = testObj.DoubleScoreEvent(21);
		Utils.Assert(scored == 42);              // body computes score * 2
		Utils.Assert(testObj.DoubleScoreBodyRan); // the C# default implementation actually ran

		// --- Case 2: out param + non-blittable (string) return through the ProcessEvent boundary ------
		string greeting = testObj.FormatGreetingEvent("World", out int length);
		Utils.Assert(greeting == "Hello, World!");
		Utils.Assert(length == "Hello, World!".Length);

		// --- Case 3: void BlueprintEvent ---------------------------------------------------------------
		testObj.RecordedNotifyValue = 0;
		testObj.RecordNotifyEvent(5);
		Utils.Assert(testObj.RecordedNotifyValue == 15); // body computes amount * 3

		// --- Case 4: repeated direct calls stay correct and never recurse ------------------------------
		Utils.Assert(testObj.DoubleScoreEvent(0) == 0);
		Utils.Assert(testObj.DoubleScoreEvent(-4) == -8);
		Utils.Assert(testObj.DoubleScoreEvent(1000) == 2000);

		// --- Case 5: indirect (delegate) call is NOT intercepted and hits the C# body directly ---------
		testObj.DoubleScoreBodyRan = false;
		Func<int, int> scoreDelegate = testObj.DoubleScoreEvent;
		int viaDelegate = scoreDelegate(21);
		Utils.Assert(viaDelegate == 42);
		Utils.Assert(testObj.DoubleScoreBodyRan); // body ran via the direct (non-intercepted) path

		// --- Case 6: C# override of a C++ BlueprintNativeEvent, WITHOUT calling base --------------------
		testObj.CallBaseNativeScore = false;
		testObj.BaseNativeScoreBodyRan = false;
		int overrideOnly = testObj.BaseNativeScore(21);
		Utils.Assert(overrideOnly == 42);          // 21 * 2, not the C++ default 21 + 100
		Utils.Assert(testObj.BaseNativeScoreBodyRan);

		// --- Case 7: C# override calling base.BaseNativeScore() into the C++ _Implementation ------------
		testObj.CallBaseNativeScore = true;
		testObj.BaseNativeScoreBodyRan = false;
		testObj.LastBaseNativeResult = 0;
		int withBase = testObj.BaseNativeScore(10);
		Utils.Assert(testObj.LastBaseNativeResult == 110); // C++ _Implementation: 10 + 100
		Utils.Assert(withBase == 111);                     // 110 + 1
		Utils.Assert(testObj.BaseNativeScoreBodyRan);

		// --- Case 8: C# override of a C++ BlueprintImplementableEvent -----------------------------------
		testObj.BaseImplScoreBodyRan = false;
		int implScored = testObj.BaseImplScore(5);
		Utils.Assert(implScored == 50);
		Utils.Assert(testObj.BaseImplScoreBodyRan);

		// --- Case 9: repeated / mixed base and non-base calls stay correct and never recurse -----------
		testObj.CallBaseNativeScore = false;
		Utils.Assert(testObj.BaseNativeScore(0) == 0);
		testObj.CallBaseNativeScore = true;
		Utils.Assert(testObj.BaseNativeScore(1) == 102); // (1 + 100) + 1
		testObj.CallBaseNativeScore = false;
		Utils.Assert(testObj.BaseNativeScore(-3) == -6);

		return true;
	}
}
