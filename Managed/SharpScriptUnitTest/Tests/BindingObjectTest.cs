using static UnrealEngine.Globals;
using static SharpScriptUnitTest.Tests.MapTest;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using UnrealEngine.SharpScriptUnitTest;
using Object = UnrealEngine.CoreUObject.Object;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test automatically generated static export properties and methods of UObject.
/// </summary>
[RecordFilePath]
public class BindingObjectTest : IUnitTestInterface
{
	public bool RunTest()
	{
		SsBindingTestObject obj = NewObject<SsBindingTestObject>();
		SsBindingTestObject objValue = NewObject<SsBindingTestObject>();
		Class clsValue = SsBindingTestObject.StaticClass!;

		// Test Object base properties
		Utils.Assert(obj.GetName() == obj.ToString());
		Utils.Assert(obj.GetClass() == SsBindingTestObject.StaticClass.Class);

		// Test type information
		Utils.Assert(obj.GetClass().IsChildOf(Object.StaticClass!));
		Utils.Assert(obj.GetClass().GetSuperClass() == Object.StaticClass.Class);
		Utils.Assert(obj.GetClass().GetSuperClass()!.GetSuperClass() == null);

		// Test package properties
		Package package = obj.GetPackage();
		Utils.Assert(obj.GetOuter() == package);
		Utils.Assert(package.GetName() == "/Engine/Transient");
		Utils.Assert(package.GetPackageName() == Name.None);

		// Test member default values
		Utils.Assert(!obj.Bool);
		Utils.Assert(!obj.BitfieldBoolA);
		Utils.Assert(!obj.BitfieldBoolB);
		Utils.Assert(obj.Int == 0);
		Utils.Assert(obj.Float == 0);
		Utils.Assert(obj.Enum == 0);
		Utils.Assert(obj.LongEnum == 0);
		Utils.Assert(obj.String == string.Empty);
		Utils.Assert(obj.Name == Name.None);
		Utils.Assert(obj.Text == string.Empty);
		Utils.Assert(obj.FieldPath.Path == string.Empty);
		Utils.Assert(obj.StructFieldPath.Path == string.Empty);
		Utils.Assert(obj.StringArray.SequenceEqual([]));
		Utils.Assert(obj.StringSet.SequenceEqual([]));
		Utils.Assert(DictEquals(obj.StringIntMap, []));
		Utils.Assert(obj.Object == null);
		Utils.Assert(obj.ObjectPtr == null);
		Utils.Assert(obj.SoftObjectPtr == null);
		Utils.Assert(obj.WeakObjectPtr == null);
		Utils.Assert(obj.LazyObjectPtr == null);
		Utils.Assert(obj.Class == null);
		Utils.Assert(obj.ClassPtr == null);
		Utils.Assert(obj.SoftClassPtr == null);
		Utils.Assert(obj.Interface == null);

		// Test member assignment
		obj.Bool = true;
		Utils.Assert(obj.Bool);
		obj.BitfieldBoolA = true;
		Utils.Assert(obj.BitfieldBoolA);
		Utils.Assert(!obj.BitfieldBoolB);
		obj.BitfieldBoolB = true;
		Utils.Assert(obj.BitfieldBoolA);
		Utils.Assert(obj.BitfieldBoolB);
		obj.Int = 123;
		Utils.Assert(obj.Int == 123);
		obj.Float = 2;
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		Utils.Assert(obj.Float == 2);
		obj.Enum = ESsBindingTestEnum.Two;
		Utils.Assert(obj.Enum == ESsBindingTestEnum.Two);
		obj.LongEnum = ESsBindingTestLongEnum.Two;
		Utils.Assert(obj.LongEnum == ESsBindingTestLongEnum.Two);
		obj.String = "String";
		Utils.Assert(obj.String == "String");
		obj.Name = "Name";
		Utils.Assert(obj.Name == "Name");
		obj.Text = new Text("Text");
		Utils.Assert(obj.Text == "Text");
		obj.FieldPath = new FieldPath("/Script/SharpScriptUnitTest.SsTestStruct:StringArray");
		Utils.Assert(obj.FieldPath.Path == "/Script/SharpScriptUnitTest.SsTestStruct:StringArray");
		obj.StructFieldPath = new FieldPath("/Script/SharpScriptUnitTest.SsBindingTestObject:Struct");
		Utils.Assert(obj.StructFieldPath.Path == "/Script/SharpScriptUnitTest.SsBindingTestObject:Struct");
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
		obj.ObjectPtr = objValue;
		Utils.Assert(obj.ObjectPtr == objValue);
		obj.ObjectPtr = null;
		Utils.Assert(obj.ObjectPtr == null);
		obj.SoftObjectPtr = objValue;
		Utils.Assert(obj.SoftObjectPtr == objValue);
		obj.SoftObjectPtr = null;
		Utils.Assert(obj.SoftObjectPtr == null);
		obj.WeakObjectPtr = objValue;
		Utils.Assert(obj.WeakObjectPtr == objValue);
		obj.WeakObjectPtr = null;
		Utils.Assert(obj.WeakObjectPtr == null);
		obj.LazyObjectPtr = objValue;
		Utils.Assert(obj.LazyObjectPtr == objValue);
		obj.LazyObjectPtr = null;
		Utils.Assert(obj.LazyObjectPtr == null);
		obj.Class = clsValue;
		Utils.Assert(obj.Class == clsValue);
		obj.Class = null as Class;
		Utils.Assert(obj.Class == null);
		obj.ClassPtr = clsValue;
		Utils.Assert(obj.ClassPtr == clsValue);
		obj.ClassPtr = null;
		Utils.Assert(obj.ClassPtr == null);
		obj.SoftClassPtr = clsValue;
		Utils.Assert(obj.SoftClassPtr == clsValue);
		obj.SoftClassPtr = null;
		Utils.Assert(obj.SoftClassPtr == null);
		obj.Interface = objValue;
		Utils.Assert(obj.Interface == objValue);
		obj.Interface = null;
		Utils.Assert(obj.Interface == null);

		// Test struct reference default values
		Utils.Assert(!obj.Struct.Bool);
		Utils.Assert(obj.Struct.Int == 0);
		Utils.Assert(obj.Struct.Float == 0);
		Utils.Assert(obj.Struct.Enum == 0);
		Utils.Assert(obj.Struct.LongEnum == 0);
		Utils.Assert(obj.Struct.String == string.Empty);
		Utils.Assert(obj.Struct.Name == Name.None);
		Utils.Assert(obj.Struct.Text == string.Empty);
		Utils.Assert(obj.Struct.StringArray.SequenceEqual([]));
		Utils.Assert(obj.Struct.StringSet.SequenceEqual([]));
		Utils.Assert(DictEquals(obj.Struct.StringIntMap, []));
		Utils.Assert(obj.Struct.Object == null);
		Utils.Assert(obj.Struct.ObjectPtr == null);
		Utils.Assert(obj.Struct.SoftObjectPtr == null);
		Utils.Assert(obj.Struct.WeakObjectPtr == null);
		Utils.Assert(obj.Struct.LazyObjectPtr == null);
		Utils.Assert(obj.Struct.Class == null);
		Utils.Assert(obj.Struct.ClassPtr == null);
		Utils.Assert(obj.Struct.SoftClassPtr == null);
		Utils.Assert(obj.Struct.Interface == null);

		// Test struct reference member assignment
		obj.Struct.Bool = true;
		Utils.Assert(obj.Struct.Bool);
		obj.Struct.Int = 123;
		Utils.Assert(obj.Struct.Int == 123);
		obj.Struct.Float = 2;
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		Utils.Assert(obj.Struct.Float == 2);
		obj.Struct.Enum = ESsBindingTestEnum.Two;
		Utils.Assert(obj.Struct.Enum == ESsBindingTestEnum.Two);
		obj.Struct.LongEnum = ESsBindingTestLongEnum.Two;
		Utils.Assert(obj.Struct.LongEnum == ESsBindingTestLongEnum.Two);
		obj.Struct.String = "String";
		Utils.Assert(obj.Struct.String == "String");
		obj.Struct.Name = "Name";
		Utils.Assert(obj.Struct.Name == "Name");
		obj.Struct.Text = new Text("Text");
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
		obj.Struct.ObjectPtr = objValue;
		Utils.Assert(obj.Struct.ObjectPtr == objValue);
		obj.Struct.ObjectPtr = null;
		Utils.Assert(obj.Struct.ObjectPtr == null);
		obj.Struct.SoftObjectPtr = objValue;
		Utils.Assert(obj.Struct.SoftObjectPtr == objValue);
		obj.Struct.SoftObjectPtr = null;
		Utils.Assert(obj.Struct.SoftObjectPtr == null);
		obj.Struct.WeakObjectPtr = objValue;
		Utils.Assert(obj.Struct.WeakObjectPtr == objValue);
		obj.Struct.WeakObjectPtr = null;
		Utils.Assert(obj.Struct.WeakObjectPtr == null);
		obj.Struct.LazyObjectPtr = objValue;
		Utils.Assert(obj.Struct.LazyObjectPtr == objValue);
		obj.Struct.LazyObjectPtr = null;
		Utils.Assert(obj.Struct.LazyObjectPtr == null);
		obj.Struct.Class = clsValue;
		Utils.Assert(obj.Struct.Class == clsValue);
		obj.Struct.Class = null as Class;
		Utils.Assert(obj.Struct.Class == null);
		obj.Struct.ClassPtr = clsValue;
		Utils.Assert(obj.Struct.ClassPtr == clsValue);
		obj.Struct.ClassPtr = null;
		Utils.Assert(obj.Struct.ClassPtr == null);
		obj.Struct.SoftClassPtr = clsValue;
		Utils.Assert(obj.Struct.SoftClassPtr == clsValue);
		obj.Struct.SoftClassPtr = null;
		Utils.Assert(obj.Struct.SoftClassPtr == null);
		obj.Struct.Interface = objValue;
		Utils.Assert(obj.Struct.Interface == objValue);
		obj.Struct.Interface = null;
		Utils.Assert(obj.Struct.Interface == null);

		// Test struct array reference default values
		Utils.Assert(obj.StructArray.Count == 0);
		obj.StructArray.Add(default);
		Utils.Assert(obj.StructArray.Count == 1);
		Utils.Assert(!obj.StructArray[0].Bool);
		Utils.Assert(obj.StructArray[0].Int == 0);
		Utils.Assert(obj.StructArray[0].Float == 0);
		Utils.Assert(obj.StructArray[0].Enum == 0);
		Utils.Assert(obj.StructArray[0].LongEnum == 0);
		Utils.Assert(obj.StructArray[0].String == string.Empty);
		Utils.Assert(obj.StructArray[0].Name == Name.None);
		Utils.Assert(obj.StructArray[0].Text == string.Empty);
		Utils.Assert(obj.StructArray[0].StringArray.SequenceEqual([]));
		Utils.Assert(obj.StructArray[0].StringSet.SequenceEqual([]));
		Utils.Assert(DictEquals(obj.StructArray[0].StringIntMap, []));
		Utils.Assert(obj.StructArray[0].Object == null);
		Utils.Assert(obj.StructArray[0].ObjectPtr == null);
		Utils.Assert(obj.StructArray[0].SoftObjectPtr == null);
		Utils.Assert(obj.StructArray[0].WeakObjectPtr == null);
		Utils.Assert(obj.StructArray[0].LazyObjectPtr == null);
		Utils.Assert(obj.StructArray[0].Class == null);
		Utils.Assert(obj.StructArray[0].ClassPtr == null);
		Utils.Assert(obj.StructArray[0].SoftClassPtr == null);
		Utils.Assert(obj.StructArray[0].Interface == null);

		// Test ref struct
		Utils.Assert(obj.BlittableStruct.X == 0);
		Utils.Assert(obj.BlittableStruct.Y == 0);
		obj.BlittableStruct.X = 10;
		Utils.Assert(obj.BlittableStruct.X == 10);
		obj.BlittableStruct.Y = 20;
		Utils.Assert(obj.BlittableStruct.Y == 20);
		obj.BlittableStruct = new SsBindingTestBlittableStruct { X = 30, Y = 30 };
		Utils.Assert(obj.BlittableStruct.X == 30);
		Utils.Assert(obj.BlittableStruct.Y == 30);

		// Test struct array reference member assignment
		obj.StructArray[0].Bool = true;
		Utils.Assert(obj.StructArray[0].Bool);
		obj.StructArray[0].Int = 123;
		Utils.Assert(obj.StructArray[0].Int == 123);
		obj.StructArray[0].Float = 2;
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		Utils.Assert(obj.StructArray[0].Float == 2);
		obj.StructArray[0].Enum = ESsBindingTestEnum.Two;
		Utils.Assert(obj.StructArray[0].Enum == ESsBindingTestEnum.Two);
		obj.StructArray[0].LongEnum = ESsBindingTestLongEnum.Two;
		Utils.Assert(obj.StructArray[0].LongEnum == ESsBindingTestLongEnum.Two);
		obj.StructArray[0].String = "String";
		Utils.Assert(obj.StructArray[0].String == "String");
		obj.StructArray[0].Name = "Name";
		Utils.Assert(obj.StructArray[0].Name == "Name");
		obj.StructArray[0].Text = new Text("Text");
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
		obj.StructArray[0].ObjectPtr = objValue;
		Utils.Assert(obj.StructArray[0].ObjectPtr == objValue);
		obj.StructArray[0].ObjectPtr = null;
		Utils.Assert(obj.StructArray[0].ObjectPtr == null);
		obj.StructArray[0].SoftObjectPtr = objValue;
		Utils.Assert(obj.StructArray[0].SoftObjectPtr == objValue);
		obj.StructArray[0].SoftObjectPtr = null;
		Utils.Assert(obj.StructArray[0].SoftObjectPtr == null);
		obj.StructArray[0].WeakObjectPtr = objValue;
		Utils.Assert(obj.StructArray[0].WeakObjectPtr == objValue);
		obj.StructArray[0].WeakObjectPtr = null;
		Utils.Assert(obj.StructArray[0].WeakObjectPtr == null);
		obj.StructArray[0].LazyObjectPtr = objValue;
		Utils.Assert(obj.StructArray[0].LazyObjectPtr == objValue);
		obj.StructArray[0].LazyObjectPtr = null;
		Utils.Assert(obj.StructArray[0].LazyObjectPtr == null);
		obj.StructArray[0].Class = clsValue;
		Utils.Assert(obj.StructArray[0].Class == clsValue);
		obj.StructArray[0].Class = null as Class;
		Utils.Assert(obj.StructArray[0].Class == null);
		obj.StructArray[0].ClassPtr = clsValue;
		Utils.Assert(obj.StructArray[0].ClassPtr == clsValue);
		obj.StructArray[0].ClassPtr = null;
		Utils.Assert(obj.StructArray[0].ClassPtr == null);
		obj.StructArray[0].SoftClassPtr = clsValue;
		Utils.Assert(obj.StructArray[0].SoftClassPtr == clsValue);
		obj.StructArray[0].SoftClassPtr = null;
		Utils.Assert(obj.StructArray[0].SoftClassPtr == null);
		obj.StructArray[0].Interface = objValue;
		Utils.Assert(obj.StructArray[0].Interface == objValue);
		obj.StructArray[0].Interface = null;
		Utils.Assert(obj.StructArray[0].Interface == null);

		// Test function calls
		SsBindingTestStruct testStruct = new SsBindingTestStruct
		{
			Int = 10,
			String = "test"
		};
		// Utils.Assert(obj.FuncBlueprintImplementable(2) == 0); // todo: twx
		// Utils.Assert(obj.FuncBlueprintNative(2) == 2); // todo: twx
		// obj.FuncBlueprintNativeRef(ref testStruct); // todo: twx
		Utils.Assert(testStruct.Int == 10);
		Utils.Assert(testStruct.String == "test");
		Utils.Assert(obj.CallFuncBlueprintImplementable(2) == 0);
		Utils.Assert(obj.CallFuncBlueprintNative(2) == 2);
		obj.CallFuncBlueprintNativeRef(ref testStruct);
		Utils.Assert(testStruct.Int == 10);
		Utils.Assert(testStruct.String == "test");
		Utils.Assert(obj.FuncTakingSsBindingTestDelegate(obj.CallFuncBlueprintNative, 2) == 2);
		obj.FuncTakingFieldPath(new FieldPath("/Script/SharpScriptUnitTest.SsTestStruct:Int"));
		Utils.Assert(obj.FieldPath.Path == "/Script/SharpScriptUnitTest.SsTestStruct:Int");
		Utils.Assert(obj.FuncInterface(123) == 123);
		Utils.Assert(obj.FuncInterfaceChild(123) == 123);
		Utils.Assert(obj.FuncInterfaceOther(123) == 123);

		List<int> list = SsBindingTestObject.ReturnArray();
		Utils.Assert(list.Count == 1);
		Utils.Assert(list[0] == 10);

		HashSet<int> set = SsBindingTestObject.ReturnSet();
		Utils.Assert(set.Count == 1);
		Utils.Assert(set.Contains(10));

		Dictionary<int, bool> dict = SsBindingTestObject.ReturnMap();
		Utils.Assert(dict.Count == 1);
		Utils.Assert(dict.ContainsKey(10));
		Utils.Assert(dict[10]);

		FieldPath fieldPath = SsBindingTestObject.ReturnFieldPath();
		Utils.Assert(fieldPath.Path == "/Script/SharpScriptUnitTest.SsBindingTestObject:FieldPath");

		obj.SetInt(42);
		Utils.Assert(obj.Int == 42);
		Utils.Assert(obj.GetInt() == 42);

		// Test Delegate property
		Utils.Assert(!obj.Delegate.IsBound());
		obj.Delegate.Bind(obj.CallFuncBlueprintNative);
		Utils.Assert(obj.Delegate.IsBound());
		Utils.Assert(obj.Delegate.IsBoundToObject(obj));
		Utils.Assert(obj.Delegate.Execute(2) == 2);
		obj.Delegate.Unbind();
		Utils.Assert(!obj.Delegate.IsBound());

		// Test MulticastDelegate property
		Utils.Assert(!obj.MulticastDelegate.IsBound());
		obj.String = string.Empty;
		obj.MulticastDelegate.Broadcast("multicast");
		Utils.Assert(obj.String == string.Empty);
		obj.MulticastDelegate.Add(obj.MulticastDelegatePropertyCallback);
		Utils.Assert(obj.MulticastDelegate.IsBound());
		obj.MulticastDelegate.Broadcast("multicast");
		Utils.Assert(obj.String == "multicast");
		obj.MulticastDelegate.Remove(obj.MulticastDelegatePropertyCallback);
		Utils.Assert(!obj.MulticastDelegate.IsBound());
		obj.MulticastDelegate.Add(obj.MulticastDelegatePropertyCallback);
		obj.MulticastDelegate.RemoveAll(obj);
		Utils.Assert(!obj.MulticastDelegate.IsBound());
		obj.MulticastDelegate.Add(obj.MulticastDelegatePropertyCallback);
		obj.MulticastDelegate.Clear();
		Utils.Assert(!obj.MulticastDelegate.IsBound());

		return true;
	}
}
