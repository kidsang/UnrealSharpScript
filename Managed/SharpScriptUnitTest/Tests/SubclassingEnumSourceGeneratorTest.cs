using SharpScriptUnitTest.Types;
using UnrealEngine.Intrinsic;
using static SharpScriptUnitTest.Tests.MapTest;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Tests byte-backed UENUM values used as container elements on a subclassed UCLASS whose
/// bindings are produced by the SharpScript source generator
/// (<see cref="USsTestGenEnumContainerClassSourceGenerator"/>): enum as TArray element, TSet
/// element, and both TMap key and TMap value. Asserts the same behaviour as
/// <see cref="SubclassingEnumManualTest"/> to prove the generator output is equivalent.
/// </summary>
[RecordFilePath]
public class SubclassingEnumSourceGeneratorTest : IUnitTestInterface
{
	public bool RunTest()
	{
		USsTestGenEnumContainerClassSourceGenerator obj = NewObject<USsTestGenEnumContainerClassSourceGenerator>();

		// Default values: every enum container starts empty.
		Utils.Assert(obj.EnumArray.SequenceEqual([]));
		Utils.Assert(obj.EnumSet.SequenceEqual([]));
		Utils.Assert(DictEquals(obj.EnumKeyMap, []));
		Utils.Assert(DictEquals(obj.EnumValueMap, []));

		// Enum as TArray element.
		obj.EnumArray.CopyFrom([ESsTestGenEnumSourceGenerator.Two, ESsTestGenEnumSourceGenerator.Three]);
		Utils.Assert(obj.EnumArray.SequenceEqual([ESsTestGenEnumSourceGenerator.Two, ESsTestGenEnumSourceGenerator.Three]));

		// Enum as TSet element.
		obj.EnumSet.CopyFrom([ESsTestGenEnumSourceGenerator.Two, ESsTestGenEnumSourceGenerator.Four]);
		Utils.Assert(obj.EnumSet.SetEquals([ESsTestGenEnumSourceGenerator.Two, ESsTestGenEnumSourceGenerator.Four]));

		// Enum as TMap key.
		Dictionary<ESsTestGenEnumSourceGenerator, int> enumKeyDict = new()
		{
			{ ESsTestGenEnumSourceGenerator.One, 1 },
			{ ESsTestGenEnumSourceGenerator.Three, 3 },
		};
		obj.EnumKeyMap.CopyFrom(enumKeyDict);
		Utils.Assert(DictEquals(obj.EnumKeyMap, enumKeyDict));

		// Enum as TMap value.
		Dictionary<int, ESsTestGenEnumSourceGenerator> enumValueDict = new()
		{
			{ 1, ESsTestGenEnumSourceGenerator.One },
			{ 3, ESsTestGenEnumSourceGenerator.Three },
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
