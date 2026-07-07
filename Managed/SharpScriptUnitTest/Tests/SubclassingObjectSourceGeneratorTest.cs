using SharpScriptUnitTest.Types;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using static SharpScriptUnitTest.Tests.MapTest;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test binding properties of a C# generated class whose binding code is produced by
/// the SharpScript source generator (SharpScriptSourceGenerator), as opposed to
/// <see cref="SubclassingObjectManualTest"/> which exercises hand-written bindings.
/// The two tests assert the same behaviour to prove the generator output is equivalent.
/// </summary>
[RecordFilePath]
public class SubclassingObjectSourceGeneratorTest : IUnitTestInterface
{
	public bool RunTest()
	{
		USsTestGenClassSourceGenerator obj = NewObject<USsTestGenClassSourceGenerator>();
		USsTestGenClassSourceGenerator objValue = NewObject<USsTestGenClassSourceGenerator>();
		UClass clsValue = USsTestGenClassSourceGenerator.StaticClass!;

		// Test UObject base properties
		Utils.Assert(obj.GetName() == obj.ToString());
		Utils.Assert(obj.GetClass() == USsTestGenClassSourceGenerator.StaticClass.Class);

		// Test type information
		Utils.Assert(obj.GetClass().IsChildOf(UObject.StaticClass!));
		Utils.Assert(obj.GetClass().GetSuperClass() == UObject.StaticClass.Class);
		Utils.Assert(obj.GetClass().GetSuperClass()!.GetSuperClass() == null);

		// Test class specifiers were expanded onto the generated UClass.
		// ReSharper disable once InconsistentNaming
		const uint CLASS_NotPlaceable = 0x00000200u;
		Utils.Assert((obj.GetClass().GetClassFlags() & CLASS_NotPlaceable) != 0);

		// Test package properties
		UPackage package = obj.GetPackage();
		Utils.Assert(obj.GetOuter() == package);
		Utils.Assert(package.GetName() == "/Engine/Transient");
		Utils.Assert(package.GetPackageName() == FName.None);

		// Test member default values
		Utils.Assert(!obj.Bool);
		Utils.Assert(obj.Int == 0);
		Utils.Assert(obj.Float == 0);
		Utils.Assert(obj.Enum == 0);
		Utils.Assert(obj.String == string.Empty);
		Utils.Assert(obj.Name == FName.None);
		Utils.Assert(obj.Text == string.Empty);
		Utils.Assert(obj.StringArray.SequenceEqual([]));
		Utils.Assert(obj.StringSet.SequenceEqual([]));
		Utils.Assert(DictEquals(obj.StringIntMap, []));
		Utils.Assert(obj.Object == null);
		Utils.Assert(obj.SoftObjectPtr == null);
		Utils.Assert(obj.LazyObjectPtr == null);
		Utils.Assert(obj.Class == null);
		Utils.Assert(obj.SoftClassPtr == null);
		// Utils.Assert(obj.Interface == null);

		// Test member assignment
		obj.Bool = true;
		Utils.Assert(obj.Bool);
		obj.Int = 123;
		Utils.Assert(obj.Int == 123);
		obj.Float = 2;
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		Utils.Assert(obj.Float == 2);
		obj.Enum = ESsTestGenEnumSourceGenerator.Two;
		Utils.Assert(obj.Enum == ESsTestGenEnumSourceGenerator.Two);
		obj.String = "String";
		Utils.Assert(obj.String == "String");
		obj.Name = "Name";
		Utils.Assert(obj.Name == "Name");
		obj.Text = new FText("Text");
		Utils.Assert(obj.Text == "Text");
		obj.StringArray.CopyFrom(["String", "Array"]);
		Utils.Assert(obj.StringArray.SequenceEqual(["String", "Array"]));
		obj.StringSet.CopyFrom(["String", "Set"]);
		Utils.Assert(obj.StringSet.SetEquals(["String", "Set"]));
		Dictionary<string, int> testDict = new() { { "A", 1 }, { "B", 2 } };
		obj.StringIntMap.CopyFrom(testDict);
		Utils.Assert(DictEquals(obj.StringIntMap, testDict));
		obj.Object = objValue;
		Utils.Assert(obj.Object == objValue);
		obj.Object = null;
		Utils.Assert(obj.Object == null);
		obj.SoftObjectPtr = objValue;
		Utils.Assert(obj.SoftObjectPtr == objValue);
		obj.SoftObjectPtr = null;
		Utils.Assert(obj.SoftObjectPtr == null);
		obj.LazyObjectPtr = objValue;
		Utils.Assert(obj.LazyObjectPtr == objValue);
		obj.LazyObjectPtr = null;
		Utils.Assert(obj.LazyObjectPtr == null);
		obj.Class = clsValue;
		Utils.Assert(obj.Class == clsValue);
		obj.Class = null;
		Utils.Assert(obj.Class == null);
		obj.SoftClassPtr = clsValue;
		Utils.Assert(obj.SoftClassPtr == clsValue);
		obj.SoftClassPtr = null;
		Utils.Assert(obj.SoftClassPtr == null);
		// obj.Interface = objValue;
		// Utils.Assert(obj.Interface == objValue);
		// obj.Interface = null;
		// Utils.Assert(obj.Interface == null);

		// Test struct reference default values
		Utils.Assert(!obj.Struct.Bool);
		Utils.Assert(obj.Struct.Int == 0);
		Utils.Assert(obj.Struct.Float == 0);
		Utils.Assert(obj.Struct.Enum == 0);
		Utils.Assert(obj.Struct.String == string.Empty);
		Utils.Assert(obj.Struct.Name == FName.None);
		Utils.Assert(obj.Struct.Text == string.Empty);
		Utils.Assert(obj.Struct.StringArray.SequenceEqual([]));
		Utils.Assert(obj.Struct.StringSet.SequenceEqual([]));
		Utils.Assert(DictEquals(obj.Struct.StringIntMap, []));
		Utils.Assert(obj.Struct.Object == null);
		Utils.Assert(obj.Struct.SoftObjectPtr == null);
		Utils.Assert(obj.Struct.LazyObjectPtr == null);
		Utils.Assert(obj.Struct.Class == null);
		Utils.Assert(obj.Struct.SoftClassPtr == null);
		// Utils.Assert(obj.Struct.Interface == null);

		// Test struct reference member assignment
		obj.Struct.Bool = true;
		Utils.Assert(obj.Struct.Bool);
		obj.Struct.Int = 123;
		Utils.Assert(obj.Struct.Int == 123);
		obj.Struct.Float = 2;
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		Utils.Assert(obj.Struct.Float == 2);
		obj.Struct.Enum = ESsTestGenEnumManual.Two;
		Utils.Assert(obj.Struct.Enum == ESsTestGenEnumManual.Two);
		obj.Struct.String = "String";
		Utils.Assert(obj.Struct.String == "String");
		obj.Struct.Name = "Name";
		Utils.Assert(obj.Struct.Name == "Name");
		obj.Struct.Text = new FText("Text");
		Utils.Assert(obj.Struct.Text == "Text");
		obj.Struct.StringArray.CopyFrom(["String", "Array"]);
		Utils.Assert(obj.Struct.StringArray.SequenceEqual(["String", "Array"]));
		obj.Struct.StringSet.CopyFrom(["String", "Set"]);
		Utils.Assert(obj.Struct.StringSet.SetEquals(["String", "Set"]));
		obj.Struct.StringIntMap.CopyFrom(testDict);
		Utils.Assert(DictEquals(obj.Struct.StringIntMap, testDict));
		obj.Struct.Object = objValue;
		Utils.Assert(obj.Struct.Object == objValue);
		obj.Struct.Object = null;
		Utils.Assert(obj.Struct.Object == null);
		obj.Struct.SoftObjectPtr = objValue;
		Utils.Assert(obj.Struct.SoftObjectPtr == objValue);
		obj.Struct.SoftObjectPtr = null;
		Utils.Assert(obj.Struct.SoftObjectPtr == null);
		obj.Struct.LazyObjectPtr = objValue;
		Utils.Assert(obj.Struct.LazyObjectPtr == objValue);
		obj.Struct.LazyObjectPtr = null;
		Utils.Assert(obj.Struct.LazyObjectPtr == null);
		obj.Struct.Class = clsValue;
		Utils.Assert(obj.Struct.Class == clsValue);
		obj.Struct.Class = null;
		Utils.Assert(obj.Struct.Class == null);
		obj.Struct.SoftClassPtr = clsValue;
		Utils.Assert(obj.Struct.SoftClassPtr == clsValue);
		obj.Struct.SoftClassPtr = null;
		Utils.Assert(obj.Struct.SoftClassPtr == null);
		// obj.Struct.Interface = objValue;
		// Utils.Assert(obj.Struct.Interface == objValue);
		// obj.Struct.Interface = null;
		// Utils.Assert(obj.Struct.Interface == null);

		// Test struct array reference default values
		Utils.Assert(obj.StructArray.Count == 0);
		obj.StructArray.Add(default);
		Utils.Assert(obj.StructArray.Count == 1);
		Utils.Assert(!obj.StructArray[0].Bool);
		Utils.Assert(obj.StructArray[0].Int == 0);
		Utils.Assert(obj.StructArray[0].Float == 0);
		Utils.Assert(obj.StructArray[0].Enum == 0);
		Utils.Assert(obj.StructArray[0].String == string.Empty);
		Utils.Assert(obj.StructArray[0].Name == FName.None);
		Utils.Assert(obj.StructArray[0].Text == string.Empty);
		Utils.Assert(obj.StructArray[0].StringArray.SequenceEqual([]));
		Utils.Assert(obj.StructArray[0].StringSet.SequenceEqual([]));
		Utils.Assert(DictEquals(obj.StructArray[0].StringIntMap, []));
		Utils.Assert(obj.StructArray[0].Object == null);
		Utils.Assert(obj.StructArray[0].SoftObjectPtr == null);
		Utils.Assert(obj.StructArray[0].LazyObjectPtr == null);
		Utils.Assert(obj.StructArray[0].Class == null);
		Utils.Assert(obj.StructArray[0].SoftClassPtr == null);
		// Utils.Assert(obj.StructArray[0].Interface == null);

		// Test struct array reference member assignment
		obj.StructArray[0].Bool = true;
		Utils.Assert(obj.StructArray[0].Bool);
		obj.StructArray[0].Int = 123;
		Utils.Assert(obj.StructArray[0].Int == 123);
		obj.StructArray[0].Float = 2;
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		Utils.Assert(obj.StructArray[0].Float == 2);
		obj.StructArray[0].Enum = ESsTestGenEnumManual.Two;
		Utils.Assert(obj.StructArray[0].Enum == ESsTestGenEnumManual.Two);
		obj.StructArray[0].String = "String";
		Utils.Assert(obj.StructArray[0].String == "String");
		obj.StructArray[0].Name = "Name";
		Utils.Assert(obj.StructArray[0].Name == "Name");
		obj.StructArray[0].Text = new FText("Text");
		Utils.Assert(obj.StructArray[0].Text == "Text");
		obj.StructArray[0].StringArray.CopyFrom(["String", "Array"]);
		Utils.Assert(obj.StructArray[0].StringArray.SequenceEqual(["String", "Array"]));
		obj.StructArray[0].StringSet.CopyFrom(["String", "Set"]);
		Utils.Assert(obj.StructArray[0].StringSet.SetEquals(["String", "Set"]));
		obj.StructArray[0].StringIntMap.CopyFrom(testDict);
		Utils.Assert(DictEquals(obj.StructArray[0].StringIntMap, testDict));
		obj.StructArray[0].Object = objValue;
		Utils.Assert(obj.StructArray[0].Object == objValue);
		obj.StructArray[0].Object = null;
		Utils.Assert(obj.StructArray[0].Object == null);
		obj.StructArray[0].SoftObjectPtr = objValue;
		Utils.Assert(obj.StructArray[0].SoftObjectPtr == objValue);
		obj.StructArray[0].SoftObjectPtr = null;
		Utils.Assert(obj.StructArray[0].SoftObjectPtr == null);
		obj.StructArray[0].LazyObjectPtr = objValue;
		Utils.Assert(obj.StructArray[0].LazyObjectPtr == objValue);
		obj.StructArray[0].LazyObjectPtr = null;
		Utils.Assert(obj.StructArray[0].LazyObjectPtr == null);
		obj.StructArray[0].Class = clsValue;
		Utils.Assert(obj.StructArray[0].Class == clsValue);
		obj.StructArray[0].Class = null;
		Utils.Assert(obj.StructArray[0].Class == null);
		obj.StructArray[0].SoftClassPtr = clsValue;
		Utils.Assert(obj.StructArray[0].SoftClassPtr == clsValue);
		obj.StructArray[0].SoftClassPtr = null;
		Utils.Assert(obj.StructArray[0].SoftClassPtr == null);
		// obj.StructArray[0].Interface = objValue;
		// Utils.Assert(obj.StructArray[0].Interface == objValue);
		// obj.StructArray[0].Interface = null;
		// Utils.Assert(obj.StructArray[0].Interface == null);
		// Test ref struct

		Utils.Assert(obj.BlittableStruct.X == 0);
		Utils.Assert(obj.BlittableStruct.Y == 0);
		obj.BlittableStruct.X = 10;
		Utils.Assert(obj.BlittableStruct.X == 10);
		obj.BlittableStruct.Y = 20;
		Utils.Assert(obj.BlittableStruct.Y == 20);
		obj.BlittableStruct = new FSsTestBlittableGenStructManual { X = 30, Y = 30 };
		Utils.Assert(obj.BlittableStruct.X == 30);
		Utils.Assert(obj.BlittableStruct.Y == 30);

		TestObjectAsContainerValue(obj, objValue);

		return true;
	}

	private static void TestObjectAsContainerValue(USsTestGenClassSourceGenerator obj, USsTestGenClassSourceGenerator objValue)
	{
		USsTestGenClassSourceGenerator objOne = NewObject<USsTestGenClassSourceGenerator>();
		USsTestGenClassSourceGenerator objTwo = NewObject<USsTestGenClassSourceGenerator>();

		// Default values: every object container starts empty.
		Utils.Assert(obj.ObjectArray.SequenceEqual([]));
		Utils.Assert(obj.ObjectSet.SequenceEqual([]));
		Utils.Assert(DictEquals(obj.IntObjectMap, []));

		// Object as TArray element.
		obj.ObjectArray.CopyFrom([objOne, objTwo]);
		Utils.Assert(obj.ObjectArray.SequenceEqual([objOne, objTwo]));
		obj.ObjectArray[1] = objValue;
		Utils.Assert(obj.ObjectArray[1] == objValue);
		Utils.Assert(obj.ObjectArray.Contains(objOne));
		obj.ObjectArray.Clear();
		Utils.Assert(obj.ObjectArray.SequenceEqual([]));

		// Object as TSet element.
		obj.ObjectSet.CopyFrom([objOne, objTwo]);
		Utils.Assert(obj.ObjectSet.SetEquals([objOne, objTwo]));
		Utils.Assert(obj.ObjectSet.Contains(objOne));
		Utils.Assert(!obj.ObjectSet.Contains(objValue));
		obj.ObjectSet.Clear();
		Utils.Assert(obj.ObjectSet.SequenceEqual([]));

		// Object as TMap value.
		Dictionary<int, UObject?> objValueDict = new() { { 1, objOne }, { 2, objTwo } };
		obj.IntObjectMap.CopyFrom(objValueDict);
		Utils.Assert(DictEquals(obj.IntObjectMap, objValueDict));
		Utils.Assert(obj.IntObjectMap[1] == objOne);
		obj.IntObjectMap[1] = objValue;
		Utils.Assert(obj.IntObjectMap[1] == objValue);
		obj.IntObjectMap.Clear();
		Utils.Assert(DictEquals(obj.IntObjectMap, []));
	}
}
