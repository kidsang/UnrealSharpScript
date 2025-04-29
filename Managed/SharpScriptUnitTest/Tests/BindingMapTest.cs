using static UnrealEngine.Globals;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;
using UnrealEngine.SharpScriptUnitTest;

// ReSharper disable UsageOfDefaultStructEquality

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test various functionalities of the TMap wrapper class.
/// </summary>
[RecordFilePath]
public class BindingMapTest : IUnitTestInterface
{
	public bool RunTest()
	{
		IntPtr testStructOneNativePtr = 0;
		IntPtr testStructTwoNativePtr = 0;
		try
		{
			testStructOneNativePtr = TypeInterop.CreateStructInstance(SsBindingMapTestStructNativeRef.NativeType);
			SsBindingMapTestStructNativeRef testStructOneNativeRef = new SsBindingMapTestStructNativeRef(testStructOneNativePtr);
			testStructTwoNativePtr = TypeInterop.CreateStructInstance(SsBindingMapTestStructNativeRef.NativeType);
			SsBindingMapTestStructNativeRef testStructTwoNativeRef = new SsBindingMapTestStructNativeRef(testStructTwoNativePtr);

			MapTest.StringTextMapTest(testStructOneNativeRef.StringTextMap, testStructTwoNativeRef.StringTextMap);
			MapTest.IntBoolMapTest(testStructOneNativeRef.IntBoolMap, testStructTwoNativeRef.IntBoolMap);
			EnumMapTest(testStructOneNativeRef.EnumMap, testStructTwoNativeRef.EnumMap);
			LongEnumMapTest(testStructOneNativeRef.LongEnumMap, testStructTwoNativeRef.LongEnumMap);
			IntStructMapTest(testStructOneNativeRef.IntStructMap, testStructTwoNativeRef.IntStructMap);
			BlittableStructMapTest(testStructOneNativeRef.BlittableStructMap, testStructTwoNativeRef.BlittableStructMap);
			MapTest.ObjectMapTest(testStructOneNativeRef.ObjectMap, testStructTwoNativeRef.ObjectMap);
			MapTest.SoftObjectPtrMapTest(testStructOneNativeRef.SoftObjectPtrMap, testStructTwoNativeRef.SoftObjectPtrMap);
			MapTest.ClassMapTest(testStructOneNativeRef.ClassMap, testStructTwoNativeRef.ClassMap);
			MapTest.SoftClassPtrMapTest(testStructOneNativeRef.SoftClassPtrMap, testStructTwoNativeRef.SoftClassPtrMap);
			IntInterfaceMapTest(testStructOneNativeRef.IntInterfaceMap, testStructTwoNativeRef.IntInterfaceMap);
			IntDelegateMapTest(testStructOneNativeRef.IntDelegateMap, testStructTwoNativeRef.IntDelegateMap);
		}
		finally
		{
			if (testStructOneNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsBindingMapTestStructNativeRef.NativeType, ref testStructOneNativePtr);
			}

			if (testStructTwoNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsBindingMapTestStructNativeRef.NativeType, ref testStructTwoNativePtr);
			}
		}

		return true;
	}

	public static void EnumMapTest(Map<ESsBindingTestEnum, ESsBindingTestEnum> EnumMapOne, Map<ESsBindingTestEnum, ESsBindingTestEnum> EnumMapTwo)
	{
		// ------------------------------------------
		// Test enum dictionary - Add
		EnumMapOne.Add(ESsBindingTestEnum.One, ESsBindingTestEnum.One);
		EnumMapOne.Add(ESsBindingTestEnum.Two, ESsBindingTestEnum.Two);
		Utils.Assert(DictEquals(EnumMapOne, new() { { ESsBindingTestEnum.One, ESsBindingTestEnum.One }, { ESsBindingTestEnum.Two, ESsBindingTestEnum.Two } }));

		// Test enum dictionary - Query
		Utils.Assert(EnumMapOne.ContainsKey(ESsBindingTestEnum.One));
		Utils.Assert(!EnumMapOne.ContainsKey(ESsBindingTestEnum.Three));
		Utils.Assert(EnumMapOne.ContainsValue(ESsBindingTestEnum.One));
		Utils.Assert(!EnumMapOne.ContainsValue(ESsBindingTestEnum.Three));

		// Test enum dictionary - Assignment
		EnumMapOne[ESsBindingTestEnum.One] = ESsBindingTestEnum.Four;
		Utils.Assert(EnumMapOne[ESsBindingTestEnum.One] == ESsBindingTestEnum.Four);
		EnumMapOne[ESsBindingTestEnum.Three] = ESsBindingTestEnum.Three;
		Utils.Assert(EnumMapOne[ESsBindingTestEnum.Three] == ESsBindingTestEnum.Three);
		EnumMapOne[ESsBindingTestEnum.One] = ESsBindingTestEnum.One;
		Utils.Assert(DictEquals(EnumMapOne, new() { { ESsBindingTestEnum.One, ESsBindingTestEnum.One }, { ESsBindingTestEnum.Two, ESsBindingTestEnum.Two }, { ESsBindingTestEnum.Three, ESsBindingTestEnum.Three } }));

		// Test enum dictionary - Binary equality
		EnumMapTwo.CopyFrom(new Dictionary<ESsBindingTestEnum, ESsBindingTestEnum> { { ESsBindingTestEnum.One, ESsBindingTestEnum.One }, { ESsBindingTestEnum.Two, ESsBindingTestEnum.Two } });
		Utils.Assert(DictEquals(EnumMapTwo, new() { { ESsBindingTestEnum.One, ESsBindingTestEnum.One }, { ESsBindingTestEnum.Two, ESsBindingTestEnum.Two } }));
		EnumMapTwo.Clear();
		Utils.Assert(DictEquals(EnumMapTwo, []));
		EnumMapTwo.CopyFrom(EnumMapOne);
		Utils.Assert(DictEquals(EnumMapOne, new() { { ESsBindingTestEnum.One, ESsBindingTestEnum.One }, { ESsBindingTestEnum.Two, ESsBindingTestEnum.Two }, { ESsBindingTestEnum.Three, ESsBindingTestEnum.Three } }));

		// Test enum dictionary - Remove
		EnumMapOne.Remove(ESsBindingTestEnum.Two);
		Utils.Assert(DictEquals(EnumMapOne, new() { { ESsBindingTestEnum.One, ESsBindingTestEnum.One }, { ESsBindingTestEnum.Three, ESsBindingTestEnum.Three } }));
		EnumMapOne.Clear();
		Utils.Assert(DictEquals(EnumMapOne, new()));
	}

	public static void LongEnumMapTest(Map<ESsBindingTestLongEnum, ESsBindingTestLongEnum> LongEnumMapOne, Map<ESsBindingTestLongEnum, ESsBindingTestLongEnum> LongEnumMapTwo)
	{
		// ------------------------------------------
		// Test enum dictionary - Add
		LongEnumMapOne.Add(ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.One);
		LongEnumMapOne.Add(ESsBindingTestLongEnum.Two, ESsBindingTestLongEnum.Two);
		Utils.Assert(DictEquals(LongEnumMapOne, new() { { ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.One }, { ESsBindingTestLongEnum.Two, ESsBindingTestLongEnum.Two } }));

		// Test enum dictionary - Query
		Utils.Assert(LongEnumMapOne.ContainsKey(ESsBindingTestLongEnum.One));
		Utils.Assert(!LongEnumMapOne.ContainsKey(ESsBindingTestLongEnum.Three));
		Utils.Assert(LongEnumMapOne.ContainsValue(ESsBindingTestLongEnum.One));
		Utils.Assert(!LongEnumMapOne.ContainsValue(ESsBindingTestLongEnum.Three));

		// Test enum dictionary - Assignment
		LongEnumMapOne[ESsBindingTestLongEnum.One] = ESsBindingTestLongEnum.Four;
		Utils.Assert(LongEnumMapOne[ESsBindingTestLongEnum.One] == ESsBindingTestLongEnum.Four);
		LongEnumMapOne[ESsBindingTestLongEnum.Three] = ESsBindingTestLongEnum.Three;
		Utils.Assert(LongEnumMapOne[ESsBindingTestLongEnum.Three] == ESsBindingTestLongEnum.Three);
		LongEnumMapOne[ESsBindingTestLongEnum.One] = ESsBindingTestLongEnum.One;
		Utils.Assert(DictEquals(LongEnumMapOne, new() { { ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.One }, { ESsBindingTestLongEnum.Two, ESsBindingTestLongEnum.Two }, { ESsBindingTestLongEnum.Three, ESsBindingTestLongEnum.Three } }));

		// Test enum dictionary - Binary equality
		LongEnumMapTwo.CopyFrom(new Dictionary<ESsBindingTestLongEnum, ESsBindingTestLongEnum> { { ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.One }, { ESsBindingTestLongEnum.Two, ESsBindingTestLongEnum.Two } });
		Utils.Assert(DictEquals(LongEnumMapTwo, new() { { ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.One }, { ESsBindingTestLongEnum.Two, ESsBindingTestLongEnum.Two } }));
		LongEnumMapTwo.Clear();
		Utils.Assert(DictEquals(LongEnumMapTwo, []));
		LongEnumMapTwo.CopyFrom(LongEnumMapOne);
		Utils.Assert(DictEquals(LongEnumMapOne, new() { { ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.One }, { ESsBindingTestLongEnum.Two, ESsBindingTestLongEnum.Two }, { ESsBindingTestLongEnum.Three, ESsBindingTestLongEnum.Three } }));

		// Test enum dictionary - Remove
		LongEnumMapOne.Remove(ESsBindingTestLongEnum.Two);
		Utils.Assert(DictEquals(LongEnumMapOne, new() { { ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.One }, { ESsBindingTestLongEnum.Three, ESsBindingTestLongEnum.Three } }));
		LongEnumMapOne.Clear();
		Utils.Assert(DictEquals(LongEnumMapOne, new()));
	}

	private static void IntStructMapTest(Map<int, SsBindingMapTestInnerStruct, SsBindingMapTestInnerStructNativeRef> IntStructMapOne, Map<int, SsBindingMapTestInnerStruct, SsBindingMapTestInnerStructNativeRef> IntStructMapTwo)
	{
		// ------------------------------------------
		// Test int-struct dictionary
		SsBindingMapTestInnerStruct innerStructA = new SsBindingMapTestInnerStruct()
		{
			IntIntMap = new() { { 1, 1 } }
		};
		SsBindingMapTestInnerStruct innerStructB = new SsBindingMapTestInnerStruct()
		{
			IntIntMap = new() { { 2, 2 } }
		};
		SsBindingMapTestInnerStruct innerStructC = new SsBindingMapTestInnerStruct()
		{
			IntIntMap = new() { { 3, 3 } }
		};

		// Test int-struct dictionary - Add
		IntStructMapOne.Add(1, innerStructA);
		IntStructMapOne.Add(2, innerStructB);
		Utils.Assert(DictEquals(IntStructMapOne, new() { { 1, innerStructA }, { 2, innerStructB } }));

		// Test int-struct dictionary - Query
		Utils.Assert(IntStructMapOne.ContainsKey(1));
		Utils.Assert(!IntStructMapOne.ContainsKey(3));
		// Utils.Assert(IntStructMapOne.ContainsValue(innerStructA));
		// Utils.Assert(!IntStructMapOne.ContainsValue(innerStructC));

		// Test int-struct dictionary - Assignment
		IntStructMapOne[1].FromManaged(innerStructC);
		// Utils.Assert(IntStructMapOne[1].ToManaged().Equals(innerStructC));
		IntStructMapOne.Add(3, innerStructC);
		// Utils.Assert(IntStructMapOne[3].ToManaged().Equals(innerStructC));
		IntStructMapOne[1].FromManaged(innerStructA);
		Utils.Assert(DictEquals(IntStructMapOne, new() { { 1, innerStructA }, { 2, innerStructB }, { 3, innerStructC } }));

		// Test int-struct dictionary - Binary equality
		IntStructMapTwo.CopyFrom(new Dictionary<int, SsBindingMapTestInnerStruct> { { 1, innerStructA }, { 2, innerStructB } });
		Utils.Assert(DictEquals(IntStructMapTwo, new() { { 1, innerStructA }, { 2, innerStructB } }));
		IntStructMapTwo.Clear();
		Utils.Assert(DictEquals(IntStructMapTwo, []));
		IntStructMapTwo.CopyFrom(IntStructMapOne);
		Utils.Assert(DictEquals(IntStructMapOne, new() { { 1, innerStructA }, { 2, innerStructB }, { 3, innerStructC } }));

		// Test int-struct dictionary - Reference penetration
		IntStructMapOne[2].IntIntMap[1] = 2;
		Utils.Assert(MapTest.DictEquals(IntStructMapOne[2].IntIntMap, new() { { 1, 2 }, { 2, 2 } }));
		IntStructMapOne[2].IntIntMap[2] = 4;
		Utils.Assert(MapTest.DictEquals(IntStructMapOne[2].IntIntMap, new() { { 1, 2 }, { 2, 4 } }));
		IntStructMapOne[2].IntIntMap.Add(3, 6);
		Utils.Assert(MapTest.DictEquals(IntStructMapOne[2].IntIntMap, new() { { 1, 2 }, { 2, 4 }, { 3, 6 } }));
		IntStructMapOne[2].IntIntMap.CopyFrom(innerStructB.IntIntMap);
		// Utils.Assert(IntStructMapOne[2].ToManaged().Equals(innerStructB));

		// Test int-struct dictionary - Remove
		// IntStructMapOne.Remove(2);
		// Utils.Assert(DictEquals(IntStructMapOne, new() { { 1, innerStructA }, { 3, innerStructC } }));
		IntStructMapOne.Clear();
		Utils.Assert(DictEquals(IntStructMapOne, new()));
	}

	public static void BlittableStructMapTest(Map<SsBindingTestBlittableStruct, SsBindingTestBlittableStruct, SsBindingTestBlittableStructNativeRef> BlittableStructMapOne, Map<SsBindingTestBlittableStruct, SsBindingTestBlittableStruct, SsBindingTestBlittableStructNativeRef> BlittableStructMapTwo)
	{
		// ------------------------------------------
		// Test blittable struct dictionary
		SsBindingTestBlittableStruct one = new SsBindingTestBlittableStruct { X = 1, Y = 1 };
		SsBindingTestBlittableStruct two = new SsBindingTestBlittableStruct { X = 2, Y = 2 };
		SsBindingTestBlittableStruct three = new SsBindingTestBlittableStruct { X = 3, Y = 3 };

		// Test blittable struct dictionary - Add
		BlittableStructMapOne.Add(one, one);
		BlittableStructMapOne.Add(two, two);
		Utils.Assert(DictEquals(BlittableStructMapOne, new() { { one, one }, { two, two } }));

		// Test blittable struct dictionary - Query
		Utils.Assert(BlittableStructMapOne.ContainsKey(one));
		Utils.Assert(!BlittableStructMapOne.ContainsKey(three));
		Utils.Assert(BlittableStructMapOne.ContainsValue(one));
		Utils.Assert(!BlittableStructMapOne.ContainsValue(three));

		// Test blittable struct dictionary - Assignment
		BlittableStructMapOne[one].FromManaged(three);
		Utils.Assert(BlittableStructMapOne[one].ToManaged().Equals(three));
		BlittableStructMapOne.Add(three, three);
		Utils.Assert(BlittableStructMapOne[three].ToManaged().Equals(three));
		BlittableStructMapOne[one].FromManaged(one);
		Utils.Assert(DictEquals(BlittableStructMapOne, new() { { one, one }, { two, two }, { three, three } }));

		// Test blittable struct dictionary - Binary equality
		BlittableStructMapTwo.CopyFrom(new Dictionary<SsBindingTestBlittableStruct, SsBindingTestBlittableStruct> { { one, one }, { two, two } });
		Utils.Assert(DictEquals(BlittableStructMapTwo, new() { { one, one }, { two, two } }));
		BlittableStructMapTwo.Clear();
		Utils.Assert(DictEquals(BlittableStructMapTwo, []));
		BlittableStructMapTwo.CopyFrom(BlittableStructMapOne);
		Utils.Assert(DictEquals(BlittableStructMapOne, new() { { one, one }, { two, two }, { three, three } }));

		// Test blittable struct dictionary - Remove
		BlittableStructMapOne.Remove(two);
		Utils.Assert(DictEquals(BlittableStructMapOne, new() { { one, one }, { three, three } }));
		BlittableStructMapOne.Clear();
		Utils.Assert(DictEquals(BlittableStructMapOne, new()));
	}

	public static void IntInterfaceMapTest(Map<int, ISsBindingTestChildInterface?> IntInterfaceMapOne, Map<int, ISsBindingTestChildInterface?> IntInterfaceMapTwo)
	{
		// ------------------------------------------
		// Test int-interface dictionary
		ISsBindingTestChildInterface objA = NewObject<SsBindingTestObject>();
		ISsBindingTestChildInterface objB = NewObject<SsBindingTestObject>();
		ISsBindingTestChildInterface objC = NewObject<SsBindingTestObject>();

		// Test int-interface dictionary - Add
		IntInterfaceMapOne.Add(1, objA);
		IntInterfaceMapOne.Add(2, objB);
		Utils.Assert(DictEquals(IntInterfaceMapOne, new() { { 1, objA }, { 2, objB } }));

		// Test int-interface dictionary - Query
		Utils.Assert(IntInterfaceMapOne.ContainsKey(1));
		Utils.Assert(!IntInterfaceMapOne.ContainsKey(3));
		Utils.Assert(IntInterfaceMapOne.ContainsValue(objA));
		Utils.Assert(!IntInterfaceMapOne.ContainsValue(objC));

		// Test int-interface dictionary - Assignment
		IntInterfaceMapOne[1] = objC;
		Utils.Assert(IntInterfaceMapOne[1] == objC);
		IntInterfaceMapOne.Add(3, objC);
		Utils.Assert(IntInterfaceMapOne[3] == objC);
		IntInterfaceMapOne[1] = objA;
		Utils.Assert(DictEquals(IntInterfaceMapOne, new() { { 1, objA }, { 2, objB }, { 3, objC } }));

		// Test int-interface dictionary - Binary equality
		IntInterfaceMapTwo.CopyFrom(new Dictionary<int, ISsBindingTestChildInterface?> { { 1, objA }, { 2, objB } });
		Utils.Assert(DictEquals(IntInterfaceMapTwo, new() { { 1, objA }, { 2, objB } }));
		IntInterfaceMapTwo.Clear();
		Utils.Assert(DictEquals(IntInterfaceMapTwo, []));
		IntInterfaceMapTwo.CopyFrom(IntInterfaceMapOne);
		Utils.Assert(DictEquals(IntInterfaceMapOne, new() { { 1, objA }, { 2, objB }, { 3, objC } }));

		// Test int-interface dictionary - Reference penetration
		Utils.Assert(IntInterfaceMapOne[1]!.FuncInterface(123) == 123);
		Utils.Assert(IntInterfaceMapOne[2]!.FuncInterface(123) == 123);
		Utils.Assert(IntInterfaceMapOne[3]!.FuncInterfaceChild(123) == 123);

		// Test int-interface dictionary - Remove
		IntInterfaceMapOne.Remove(2);
		Utils.Assert(DictEquals(IntInterfaceMapOne, new() { { 1, objA }, { 3, objC } }));
		IntInterfaceMapOne.Clear();
		Utils.Assert(DictEquals(IntInterfaceMapOne, new()));
	}

	public static void IntDelegateMapTest(DelegateMap<int, FSsBindingTestDelegate, Delegate<FSsBindingTestDelegate>> IntDelegateMapOne, DelegateMap<int, FSsBindingTestDelegate, Delegate<FSsBindingTestDelegate>> IntDelegateMapTwo)
	{
		// ------------------------------------------
		// Test int-delegate dictionary
		SsBindingTestObject obj = NewObject<SsBindingTestObject>();
		// FSsBindingTestDelegate  delegateA = obj.FuncBlueprintImplementable;
		// FSsBindingTestDelegate  delegateB = obj.FuncBlueprintNative;
		FSsBindingTestDelegate delegateA = obj.CallFuncBlueprintImplementable;
		FSsBindingTestDelegate delegateB = obj.CallFuncBlueprintNative;
		FSsBindingTestDelegate delegateC = obj.CallFuncBlueprintImplementable;

		IntDelegateMapOne.Add(1, delegateA);
		IntDelegateMapOne.Add(2, delegateB);
		Utils.Assert(MapTest.DictEquals(IntDelegateMapOne, new() { { 1, delegateA }, { 2, delegateB } }));

		// Test int-delegate dictionary - Query
		Utils.Assert(IntDelegateMapOne.ContainsKey(1));
		Utils.Assert(!IntDelegateMapOne.ContainsKey(3));
		Utils.Assert(IntDelegateMapOne.ContainsValue(delegateA));
		//Utils.Assert(!IntDelegateMapOne.ContainsValue(delegateC));

		// Test int-delegate dictionary - Assignment
		IntDelegateMapOne[1].FromManaged(delegateC);
		Utils.Assert(IntDelegateMapOne[1].ToManaged().Equals(delegateC));
		IntDelegateMapOne.Add(3, delegateC);
		Utils.Assert(IntDelegateMapOne[3].ToManaged().Equals(delegateC));
		IntDelegateMapOne[1].FromManaged(delegateA);
		Utils.Assert(MapTest.DictEquals(IntDelegateMapOne, new() { { 1, delegateA }, { 2, delegateB }, { 3, delegateC } }));

		// Test int-delegate dictionary - Binary equality
		IntDelegateMapTwo.CopyFrom(new Dictionary<int, FSsBindingTestDelegate> { { 1, delegateA }, { 2, delegateB } });
		Utils.Assert(MapTest.DictEquals(IntDelegateMapTwo, new() { { 1, delegateA }, { 2, delegateB } }));
		IntDelegateMapTwo.Clear();
		Utils.Assert(MapTest.DictEquals(IntDelegateMapTwo, []));
		IntDelegateMapTwo.CopyFrom(IntDelegateMapOne);
		Utils.Assert(MapTest.DictEquals(IntDelegateMapOne, new() { { 1, delegateA }, { 2, delegateB }, { 3, delegateC } }));

		// Test int-delegate dictionary - Reference penetration
		Utils.Assert(IntDelegateMapOne[1].Execute(2) == 0);
		Utils.Assert(IntDelegateMapOne[2].Execute(2) == 2);
		Utils.Assert(IntDelegateMapOne[3].Execute(2) == 0);

		// Test int-delegate dictionary - Remove
		IntDelegateMapOne.Remove(2);
		Utils.Assert(MapTest.DictEquals(IntDelegateMapOne, new() { { 1, delegateA }, { 3, delegateC } }));
		IntDelegateMapOne.Clear();
		Utils.Assert(MapTest.DictEquals(IntDelegateMapOne, new()));
	}

	private static bool DictEquals<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> a, Dictionary<TKey, TValue> b)
		where TKey : notnull
	{
		// ReSharper disable once UsageOfDefaultStructEquality
		return a.OrderBy(x => x.Key).SequenceEqual(b.OrderBy(x => x.Key));
	}

	private static bool DictEquals(Map<int, SsBindingMapTestInnerStruct, SsBindingMapTestInnerStructNativeRef> mapA,
		Dictionary<int, SsBindingMapTestInnerStruct> mapB)
	{
		if (mapA.Count != mapB.Count)
		{
			return false;
		}

		List<KeyValuePair<int, SsBindingMapTestInnerStruct>> listA = [..mapA.OrderBy(x => x.Key)];
		List<KeyValuePair<int, SsBindingMapTestInnerStruct>> listB = [..mapB.OrderBy(x => x.Key)];
		for (int i = 0; i < listA.Count; ++i)
		{
			var pairA = listA[i];
			var pairB = listB[i];
			if (pairA.Key != pairB.Key)
			{
				return false;
			}

			// ReSharper disable once UsageOfDefaultStructEquality
			if (!pairA.Value.IntIntMap.OrderBy(x => x.Key).SequenceEqual(pairB.Value.IntIntMap.OrderBy(x => x.Key)))
			{
				return false;
			}
		}

		return true;
	}

	private static bool DictEquals(Map<SsBindingTestBlittableStruct, SsBindingTestBlittableStruct, SsBindingTestBlittableStructNativeRef> mapA,
		Dictionary<SsBindingTestBlittableStruct, SsBindingTestBlittableStruct> mapB)
	{
		if (mapA.Count != mapB.Count)
		{
			return false;
		}

		foreach (var pairA in mapA)
		{
			if (!mapB.TryGetValue(pairA.Key, out SsBindingTestBlittableStruct valueB))
			{
				return false;
			}

			if (!pairA.Value.Equals(valueB))
			{
				return false;
			}
		}

		return true;
	}
}
