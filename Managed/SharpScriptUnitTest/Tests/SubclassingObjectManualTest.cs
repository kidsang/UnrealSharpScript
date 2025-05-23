using SharpScriptUnitTest.Types;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using static SharpScriptUnitTest.Tests.MapTest;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test binding properties and methods of C# generated class.
/// </summary>
[RecordFilePath]
public class SubclassingObjectManualTest : IUnitTestInterface
{
	public bool RunTest()
	{
		USsTestGenClassManual obj = NewObject<USsTestGenClassManual>();
		USsTestGenClassManual objValue = NewObject<USsTestGenClassManual>();
		UClass clsValue = USsTestGenClassManual.StaticClass!;

		// Test UObject base properties
		Utils.Assert(obj.GetName() == obj.ToString());
		Utils.Assert(obj.GetClass() == USsTestGenClassManual.StaticClass.Class);

		// Test type information
		Utils.Assert(obj.GetClass().IsChildOf(UObject.StaticClass!));
		Utils.Assert(obj.GetClass().GetSuperClass() == UObject.StaticClass.Class);
		Utils.Assert(obj.GetClass().GetSuperClass()!.GetSuperClass() == null);

		// Test package properties
		UPackage package = obj.GetPackage();
		Utils.Assert(obj.GetOuter() == package);
		Utils.Assert(package.GetName() == "/Engine/Transient");
		Utils.Assert(package.GetPackageName() == FName.None);

		// Test member default values
		Utils.Assert(!obj.Bool);
		Utils.Assert(obj.Int == 0);
		Utils.Assert(obj.Float == 0);
		// Utils.Assert(obj.Enum == 0);
		// Utils.Assert(obj.LongEnum == 0);
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
		// obj.Enum = ESsBindingTestEnum.Two;
		// Utils.Assert(obj.Enum == ESsBindingTestEnum.Two);
		// obj.LongEnum = ESsBindingTestLongEnum.Two;
		// Utils.Assert(obj.LongEnum == ESsBindingTestLongEnum.Two);
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

		return true;
	}
}
