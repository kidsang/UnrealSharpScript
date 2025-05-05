using UnrealEngine.Intrinsic;
using UnrealEngine.SharpScriptUnitTest;
using static UnrealEngine.Globals;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test auto generated code for blueprint getter & setter.
/// </summary>
public class BindingGetSetTest : IUnitTestInterface
{
	public bool RunTest()
	{
		SsBindingGetSetTest obj = NewObject<SsBindingGetSetTest>();
		SsBindingTestObject objValue = NewObject<SsBindingTestObject>();

		Utils.Assert(obj.Bool == false);
		obj.Bool = true;
		Utils.Assert(obj.Bool);

		Utils.Assert(obj.BitfieldBool == false);
		obj.BitfieldBool = true;
		Utils.Assert(obj.BitfieldBool);

		Utils.Assert(obj.Int == 0);
		obj.Int = 1;
		Utils.Assert(obj.Int == 1);

		Utils.Assert(obj.Float == 0);
		obj.Float = 1;
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		Utils.Assert(obj.Float == 1);

		Utils.Assert(obj.Enum == ESsBindingTestEnum.One);
		obj.Enum = ESsBindingTestEnum.Two;
		Utils.Assert(obj.Enum == ESsBindingTestEnum.Two);

		Utils.Assert(obj.String == String.Empty);
		obj.String = "String";
		Utils.Assert(obj.String == "String");

		Utils.Assert(obj.Name == Name.None);
		obj.Name = "Name";
		Utils.Assert(obj.Name == "Name");

		Utils.Assert(obj.Text == Text.None);
		obj.Text = new Text("Text");
		Utils.Assert(obj.Text == new Text("Text"));

		Utils.Assert(obj.FieldPath.Path == String.Empty);
		obj.FieldPath = new FieldPath("/Script/SharpScriptUnitTest.SsTestStruct:StringArray");
		Utils.Assert(obj.FieldPath.Path == "/Script/SharpScriptUnitTest.SsTestStruct:StringArray");

		Utils.Assert(obj.StringArray.Count == 0);
		obj.StringArray = ["String", "Array"];
		Utils.Assert(obj.StringArray.SequenceEqual(["String", "Array"]));

		Utils.Assert(obj.StringSet.Count == 0);
		obj.StringSet = ["String", "Set"];
		Utils.Assert(obj.StringSet.SetEquals(["String", "Set"]));

		Utils.Assert(obj.StringIntMap.Count == 0);
		obj.StringIntMap = new() { { "A", 1 }, { "B", 2 } };
		Utils.Assert(obj.StringIntMap["A"] == 1);
		Utils.Assert(obj.StringIntMap["B"] == 2);

		Utils.Assert(obj.Struct.Int == 0);
		Utils.Assert(obj.Struct.String == String.Empty);
		obj.Struct = new SsBindingTestStruct
		{
			Int = 1,
			String = "String",
		};
		Utils.Assert(obj.Struct.Int == 1);
		Utils.Assert(obj.Struct.String == "String");

		Utils.Assert(obj.Object == null);
		obj.Object = objValue;
		Utils.Assert(obj.Object == objValue);

		Utils.Assert(obj.Class == null);
		obj.Class = objValue.GetClass();
		Utils.Assert(obj.Class == objValue.GetClass());

		Utils.Assert(obj.Interface == null);
		obj.Interface = objValue;
		Utils.Assert(obj.Interface == objValue);

		return true;
	}
}
