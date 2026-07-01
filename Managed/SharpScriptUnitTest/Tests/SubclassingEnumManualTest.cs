using SharpScriptUnitTest.Types;
using UnrealEngine.Intrinsic;
using static SharpScriptUnitTest.Tests.MapTest;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Tests byte-backed UENUM values used as container elements on a subclassed UCLASS with
/// hand-written bindings (<see cref="USsTestGenEnumContainerClassManual"/>): enum as TArray
/// element, TSet element, and both TMap key and TMap value. Paired with
/// <see cref="SubclassingEnumSourceGeneratorTest"/>, which asserts identical behaviour against
/// the source-generator-produced bindings.
/// </summary>
[RecordFilePath]
public class SubclassingEnumManualTest : IUnitTestInterface
{
	public bool RunTest()
	{
		USsTestGenEnumContainerClassManual obj = NewObject<USsTestGenEnumContainerClassManual>();

		// Default values: every enum container starts empty.
		Utils.Assert(obj.EnumArray.SequenceEqual([]));
		Utils.Assert(obj.EnumSet.SequenceEqual([]));
		Utils.Assert(DictEquals(obj.EnumKeyMap, []));
		Utils.Assert(DictEquals(obj.EnumValueMap, []));

		// Enum as TArray element.
		obj.EnumArray.CopyFrom([ESsTestGenEnumManual.Two, ESsTestGenEnumManual.Three]);
		Utils.Assert(obj.EnumArray.SequenceEqual([ESsTestGenEnumManual.Two, ESsTestGenEnumManual.Three]));

		// Enum as TSet element.
		obj.EnumSet.CopyFrom([ESsTestGenEnumManual.Two, ESsTestGenEnumManual.Four]);
		Utils.Assert(obj.EnumSet.SetEquals([ESsTestGenEnumManual.Two, ESsTestGenEnumManual.Four]));

		// Enum as TMap key.
		Dictionary<ESsTestGenEnumManual, int> enumKeyDict = new()
		{
			{ ESsTestGenEnumManual.One, 1 },
			{ ESsTestGenEnumManual.Three, 3 },
		};
		obj.EnumKeyMap.CopyFrom(enumKeyDict);
		Utils.Assert(DictEquals(obj.EnumKeyMap, enumKeyDict));

		// Enum as TMap value.
		Dictionary<int, ESsTestGenEnumManual> enumValueDict = new()
		{
			{ 1, ESsTestGenEnumManual.One },
			{ 3, ESsTestGenEnumManual.Three },
		};
		obj.EnumValueMap.CopyFrom(enumValueDict);
		Utils.Assert(DictEquals(obj.EnumValueMap, enumValueDict));

		// Overwrite / clear semantics.
		obj.EnumArray.CopyFrom([]);
		Utils.Assert(obj.EnumArray.SequenceEqual([]));
		obj.EnumSet.CopyFrom([]);
		Utils.Assert(obj.EnumSet.SequenceEqual([]));
		obj.EnumKeyMap.CopyFrom([]);
		Utils.Assert(DictEquals(obj.EnumKeyMap, []));
		obj.EnumValueMap.CopyFrom([]);
		Utils.Assert(DictEquals(obj.EnumValueMap, []));

		return true;
	}
}
