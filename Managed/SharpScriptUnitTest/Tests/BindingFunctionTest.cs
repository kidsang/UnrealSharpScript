using static UnrealEngine.Globals;
using UnrealEngine.CoreUObject;
using UnrealEngine.Engine;
using UnrealEngine.Intrinsic;
using UnrealEngine.SharpScriptUnitTest;
using TestClass = UnrealEngine.SharpScriptUnitTest.SsBindingFunctionTest;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test UFunction static export.
/// </summary>
[RecordFilePath]
public class BindingFunctionTest : IUnitTestInterface
{
	public bool RunTest()
	{
		// sbyte inputInt8 = 123;
		// Utils.Assert(TestClass.FuncInt8(inputInt8, out var outputInt8) == inputInt8);
		// Utils.Assert(outputInt8 == inputInt8);
		//
		// short inputInt16 = 123;
		// Utils.Assert(TestClass.FuncInt16(inputInt16, out var outputInt16) == inputInt16);
		// Utils.Assert(outputInt16 == inputInt16);

		int inputInt32 = 123;
		Utils.Assert(TestClass.FuncInt32(inputInt32, out var outputInt32) == inputInt32);
		Utils.Assert(outputInt32 == inputInt32);

		long inputInt64 = 123;
		Utils.Assert(TestClass.FuncInt64(inputInt64, out var outputInt64) == inputInt64);
		Utils.Assert(outputInt64 == inputInt64);

		// byte inputUInt8 = 123;
		// Utils.Assert(TestClass.FuncUInt8(inputUInt8, out var outputUInt8) == inputUInt8);
		// Utils.Assert(outputUInt8 == inputUInt8);
		//
		// ushort inputUInt16 = 123;
		// Utils.Assert(TestClass.FuncUInt16(inputUInt16, out var outputUInt16) == inputUInt16);
		// Utils.Assert(outputUInt16 == inputUInt16);
		//
		// uint inputUInt32 = 123;
		// Utils.Assert(TestClass.FuncUInt32(inputUInt32, out var outputUInt32) == inputUInt32);
		// Utils.Assert(outputUInt32 == inputUInt32);
		//
		// ulong inputUInt64 = 123;
		// Utils.Assert(TestClass.FuncUInt64(inputUInt64, out var outputUInt64) == inputUInt64);
		// Utils.Assert(outputUInt64 == inputUInt64);

		// ReSharper disable CompareOfFloatsByEqualityOperator
		float inputFloat = 128.0f;
		Utils.Assert(TestClass.FuncFloat(inputFloat, out var outputFloat) == inputFloat);
		Utils.Assert(outputFloat == inputFloat);
		// ReSharper restore CompareOfFloatsByEqualityOperator

		// ReSharper disable CompareOfFloatsByEqualityOperator
		double inputDouble = 128.0;
		Utils.Assert(TestClass.FuncDouble(inputDouble, out var outputDouble) == inputDouble);
		Utils.Assert(outputDouble == inputDouble);
		// ReSharper restore CompareOfFloatsByEqualityOperator

		bool inputBool = true;
		Utils.Assert(TestClass.FuncBool(inputBool, out var outputBool) == inputBool);
		Utils.Assert(outputBool == inputBool);

		string inputString = "123";
		Utils.Assert(TestClass.FuncString(inputString, out var outputString) == inputString);
		Utils.Assert(outputString == inputString);

		Name inputName = new("123");
		Utils.Assert(TestClass.FuncName(inputName, out var outputName) == inputName);
		Utils.Assert(outputName == inputName);

		Text inputText = new("123");
		Utils.Assert(TestClass.FuncText(inputText, out var outputText) == inputText);
		Utils.Assert(outputText == inputText);

		ESsBindingTestEnum inputEnum = ESsBindingTestEnum.Two;
		Utils.Assert(TestClass.FuncEnum(inputEnum, out var outputEnum) == inputEnum);
		Utils.Assert(outputEnum == inputEnum);

		ESsBindingTestLongEnum inputLongEnum = ESsBindingTestLongEnum.Two;
		Utils.Assert(TestClass.FuncLongEnum(inputLongEnum, out var outputLongEnum) == inputLongEnum);
		Utils.Assert(outputLongEnum == inputLongEnum);

		FieldPath inputFieldPath = new("/Script/SharpScriptUnitTest.SsTestStruct:Int");
		Utils.Assert(TestClass.FuncFieldPath(inputFieldPath, out var outputFieldPath).Path == inputFieldPath.Path);
		Utils.Assert(outputFieldPath.Path == inputFieldPath.Path);

		List<string> inputStringArray = ["123", "456"];
		Utils.Assert(TestClass.FuncStringArray(inputStringArray, out var outputStringArray).SequenceEqual(inputStringArray));
		Utils.Assert(outputStringArray.SequenceEqual(inputStringArray));

		HashSet<string> inputStringSet = ["123", "456"];
		Utils.Assert(TestClass.FuncStringSet(inputStringSet, out var outputStringSet).SetEquals(inputStringSet));
		Utils.Assert(outputStringSet.SetEquals(inputStringSet));

		Dictionary<string, int> inputStringIntMap = new()
		{
			["123"] = 123,
			["456"] = 456
		};
		Utils.Assert(TestClass.FuncStringIntMap(inputStringIntMap, out var outputStringIntMap).DictEquals(inputStringIntMap));
		Utils.Assert(outputStringIntMap.DictEquals(inputStringIntMap));

		SsBindingTestStruct inputStruct = new()
		{
			Int = 123,
			String = "123",
		};
		Utils.Assert(TestClass.FuncStruct(inputStruct, out var outputStruct).Int == inputStruct.Int);
		Utils.Assert(outputStruct.String == inputStruct.String);

		SsBindingTestBlittableStruct inputBlittableStruct = new()
		{
			X = 10,
			Y = 20,
		};
		Utils.Assert(TestClass.FuncBlittableStruct(inputBlittableStruct, out var outputBlittableStruct).X == inputBlittableStruct.X);
		Utils.Assert(outputBlittableStruct.Y == inputBlittableStruct.Y);

		SsBindingTestObject inputObject = NewObject<SsBindingTestObject>();
		Utils.Assert(TestClass.FuncObject(inputObject, out var outputObject) == inputObject);
		Utils.Assert(outputObject == inputObject);

		Utils.Assert(TestClass.FuncSoftObjectPtr(inputObject, out var outputSoftObjectPtr) == inputObject);
		Utils.Assert(outputSoftObjectPtr == inputObject);

		Class inputClass = SsBindingTestObject.StaticClass!;
		Utils.Assert(TestClass.FuncClass<SsBindingTestObject, SsBindingTestObject>(inputClass, out var outputClass) == inputClass);
		Utils.Assert(outputClass == inputClass);

		Utils.Assert(TestClass.FuncSoftClassPtr(inputClass, out var outputSoftClassPtr) == inputClass);
		Utils.Assert(outputSoftClassPtr == inputClass);

		ISsBindingTestChildInterface inputInterface = NewObject<SsBindingTestObject>();
		Utils.Assert(TestClass.FuncInterface(inputInterface, out var outputInterface) == inputInterface);
		Utils.Assert(outputInterface == inputInterface);

		FSsBindingTestDelegate inputDelegate = inputObject.CallFuncBlueprintNative;
		Utils.Assert(TestClass.FuncDelegate(inputDelegate) == inputDelegate);

		var testGenericClass1 = InputComponent.StaticClass;
		var testGenericClass2 = CameraComponent.StaticClass;

		Utils.Assert(TestClass.FuncGenericRet(testGenericClass1, testGenericClass2, out var outputGeneric11, out var outputGeneric12, out var outputGeneric13) == testGenericClass1.GetDefaultObject());
		Utils.Assert(outputGeneric11[0] == testGenericClass1.GetDefaultObject());
		Utils.Assert(outputGeneric12.Contains(testGenericClass1.GetDefaultObject()));
		Utils.Assert(outputGeneric13[testGenericClass1.GetDefaultObject()!] == testGenericClass1.GetDefaultObject());

		Utils.Assert(TestClass.FuncGenericOut(testGenericClass1, testGenericClass2, out var outputGeneric21, out var outputGeneric22, out var outputGeneric23) == testGenericClass2.GetDefaultObject());
		Utils.Assert(outputGeneric21[0] == testGenericClass2.GetDefaultObject());
		Utils.Assert(outputGeneric22.Contains(testGenericClass2.GetDefaultObject()));
		Utils.Assert(outputGeneric23[testGenericClass2.GetDefaultObject()!] == testGenericClass2.GetDefaultObject());

		return true;
	}
}

public static class BindingFunctionTestUtils
{
	public static bool DictEquals<TKey, TValue>(this Dictionary<TKey, TValue> a, Dictionary<TKey, TValue> b)
		where TKey : notnull
	{
		// ReSharper disable once UsageOfDefaultStructEquality
		return a.OrderBy(x => x.Key).SequenceEqual(b.OrderBy(x => x.Key));
	}
}
