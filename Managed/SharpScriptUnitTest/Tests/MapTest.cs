using SharpScript;
using SharpScript.Interop;
using SharpScriptUnitTest.Types;
using UnrealEngine.CoreUObject;
using UnrealEngine.Engine;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test various functionalities of the TMap wrapper class.
/// </summary>
[RecordFilePath]
public class MapTest : IUnitTestInterface
{
	public bool RunTest()
	{
		IntPtr testStructOneNativePtr = 0;
		IntPtr testStructTwoNativePtr = 0;
		try
		{
			testStructOneNativePtr = TypeInterop.CreateStructInstance(FSsMapTestStructNativeRef.NativeType);
			FSsMapTestStructNativeRef testStructOneNativeRef = new FSsMapTestStructNativeRef(testStructOneNativePtr);
			testStructTwoNativePtr = TypeInterop.CreateStructInstance(FSsMapTestStructNativeRef.NativeType);
			FSsMapTestStructNativeRef testStructTwoNativeRef = new FSsMapTestStructNativeRef(testStructTwoNativePtr);

			StringTextMapTest(testStructOneNativeRef.StringTextMap, testStructTwoNativeRef.StringTextMap);
			IntBoolMapTest(testStructOneNativeRef.IntBoolMap, testStructTwoNativeRef.IntBoolMap);
			EnumMapTest(testStructOneNativeRef.EnumMap, testStructTwoNativeRef.EnumMap);
			LongEnumMapTest(testStructOneNativeRef.LongEnumMap, testStructTwoNativeRef.LongEnumMap);
			IntStructMapTest(testStructOneNativeRef.IntStructMap, testStructTwoNativeRef.IntStructMap);
			ObjectMapTest(testStructOneNativeRef.ObjectMap, testStructTwoNativeRef.ObjectMap);
			SoftObjectPtrMapTest(testStructOneNativeRef.SoftObjectPtrMap, testStructTwoNativeRef.SoftObjectPtrMap);
			ClassMapTest(testStructOneNativeRef.ClassMap, testStructTwoNativeRef.ClassMap);
			SoftClassPtrMapTest(testStructOneNativeRef.SoftClassPtrMap, testStructTwoNativeRef.SoftClassPtrMap);
			IntInterfaceMapTest(testStructOneNativeRef.IntInterfaceMap, testStructTwoNativeRef.IntInterfaceMap);
			IntDelegateMapTest(testStructOneNativeRef.IntDelegateMap, testStructTwoNativeRef.IntDelegateMap);
		}
		finally
		{
			if (testStructOneNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(FSsMapTestStructNativeRef.NativeType, ref testStructOneNativePtr);
			}

			if (testStructTwoNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(FSsMapTestStructNativeRef.NativeType, ref testStructTwoNativePtr);
			}
		}

		return true;
	}

	public static void StringTextMapTest(TMap<string, FText> StringTextMapOne, TMap<string, FText> StringTestMapTwo)
	{
		// ------------------------------------------
		// Test FString-FText dictionary - Add
		StringTextMapOne.Add("one", new FText("one"));
		StringTextMapOne.Add("two", new FText("two"));
		Utils.Assert(DictEquals(StringTextMapOne, new() { { "one", new FText("one") }, { "two", new FText("two") } }));

		// Test FString-FText dictionary - Query
		Utils.Assert(StringTextMapOne.ContainsKey("one"));
		Utils.Assert(!StringTextMapOne.ContainsKey("three"));
		Utils.Assert(StringTextMapOne.ContainsValue(new FText("two")));
		Utils.Assert(!StringTextMapOne.ContainsValue(new FText("three")));

		// Test FString-FText dictionary - Assign
		StringTextMapOne["one"] = new FText("eno");
		Utils.Assert(StringTextMapOne["one"] == "eno");
		StringTextMapOne["three"] = new FText("three");
		Utils.Assert(StringTextMapOne["three"] == "three");
		StringTextMapOne["one"] = new FText("one");
		Utils.Assert(DictEquals(StringTextMapOne, new() { { "one", new FText("one") }, { "two", new FText("two") }, { "three", new FText("three") } }));

		// Test FString-FText dictionary - Binary equality
		StringTestMapTwo.CopyFrom(new Dictionary<string, FText> { { "one", new FText("one") }, { "two", new FText("two") } });
		Utils.Assert(DictEquals(StringTestMapTwo, new() { { "one", new FText("one") }, { "two", new FText("two") } }));
		StringTestMapTwo.Clear();
		Utils.Assert(DictEquals(StringTestMapTwo, []));
		StringTestMapTwo.CopyFrom(StringTextMapOne);
		Utils.Assert(DictEquals(StringTestMapTwo, new() { { "one", new FText("one") }, { "two", new FText("two") }, { "three", new FText("three") } }));

		// Test FString-FText dictionary - Remove
		StringTextMapOne.Remove("two");
		Utils.Assert(DictEquals(StringTextMapOne, new() { { "one", new FText("one") }, { "three", new FText("three") } }));
		StringTextMapOne.Clear();
		Utils.Assert(DictEquals(StringTextMapOne, new()));
	}

	public static void IntBoolMapTest(TMap<int, bool> IntBoolMapOne, TMap<int, bool> IntBoolMapTwo)
	{
		// ------------------------------------------
		// Test int-bool dictionary - Add
		IntBoolMapOne.Add(1, true);
		IntBoolMapOne.Add(2, true);
		Utils.Assert(DictEquals(IntBoolMapOne, new() { { 1, true }, { 2, true } }));

		// Test int-bool dictionary - Query
		Utils.Assert(IntBoolMapOne.ContainsKey(1));
		Utils.Assert(!IntBoolMapOne.ContainsKey(3));
		Utils.Assert(IntBoolMapOne.ContainsValue(true));
		Utils.Assert(!IntBoolMapOne.ContainsValue(false));

		// Test int-bool dictionary - Assign
		IntBoolMapOne[1] = false;
		Utils.Assert(IntBoolMapOne[1] == false);
		IntBoolMapOne[3] = false;
		Utils.Assert(IntBoolMapOne[3] == false);
		IntBoolMapOne[1] = true;
		Utils.Assert(DictEquals(IntBoolMapOne, new() { { 1, true }, { 2, true }, { 3, false } }));

		// Test int-bool dictionary - Binary equality
		IntBoolMapTwo.CopyFrom(new Dictionary<int, bool> { { 1, true }, { 2, true } });
		Utils.Assert(DictEquals(IntBoolMapTwo, new() { { 1, true }, { 2, true } }));
		IntBoolMapTwo.Clear();
		Utils.Assert(DictEquals(IntBoolMapTwo, []));
		IntBoolMapTwo.CopyFrom(IntBoolMapOne);
		Utils.Assert(DictEquals(IntBoolMapOne, new() { { 1, true }, { 2, true }, { 3, false } }));

		// Test int-bool dictionary - Remove
		IntBoolMapOne.Remove(2);
		Utils.Assert(DictEquals(IntBoolMapOne, new() { { 1, true }, { 3, false } }));
		IntBoolMapOne.Clear();
		Utils.Assert(DictEquals(IntBoolMapOne, new()));
	}

	public static void EnumMapTest(TMap<ESsTestEnum, ESsTestEnum> EnumMapOne, TMap<ESsTestEnum, ESsTestEnum> EnumMapTwo)
	{
		// ------------------------------------------
		// Test enum dictionary - Add
		EnumMapOne.Add(ESsTestEnum.One, ESsTestEnum.One);
		EnumMapOne.Add(ESsTestEnum.Two, ESsTestEnum.Two);
		Utils.Assert(DictEquals(EnumMapOne, new() { { ESsTestEnum.One, ESsTestEnum.One }, { ESsTestEnum.Two, ESsTestEnum.Two } }));

		// Test enum dictionary - Query
		Utils.Assert(EnumMapOne.ContainsKey(ESsTestEnum.One));
		Utils.Assert(!EnumMapOne.ContainsKey(ESsTestEnum.Three));
		Utils.Assert(EnumMapOne.ContainsValue(ESsTestEnum.One));
		Utils.Assert(!EnumMapOne.ContainsValue(ESsTestEnum.Three));

		// Test enum dictionary - Assign
		EnumMapOne[ESsTestEnum.One] = ESsTestEnum.Four;
		Utils.Assert(EnumMapOne[ESsTestEnum.One] == ESsTestEnum.Four);
		EnumMapOne[ESsTestEnum.Three] = ESsTestEnum.Three;
		Utils.Assert(EnumMapOne[ESsTestEnum.Three] == ESsTestEnum.Three);
		EnumMapOne[ESsTestEnum.One] = ESsTestEnum.One;
		Utils.Assert(DictEquals(EnumMapOne, new() { { ESsTestEnum.One, ESsTestEnum.One }, { ESsTestEnum.Two, ESsTestEnum.Two }, { ESsTestEnum.Three, ESsTestEnum.Three } }));

		// Test enum dictionary - Binary equality
		EnumMapTwo.CopyFrom(new Dictionary<ESsTestEnum, ESsTestEnum> { { ESsTestEnum.One, ESsTestEnum.One }, { ESsTestEnum.Two, ESsTestEnum.Two } });
		Utils.Assert(DictEquals(EnumMapTwo, new() { { ESsTestEnum.One, ESsTestEnum.One }, { ESsTestEnum.Two, ESsTestEnum.Two } }));
		EnumMapTwo.Clear();
		Utils.Assert(DictEquals(EnumMapTwo, []));
		EnumMapTwo.CopyFrom(EnumMapOne);
		Utils.Assert(DictEquals(EnumMapOne, new() { { ESsTestEnum.One, ESsTestEnum.One }, { ESsTestEnum.Two, ESsTestEnum.Two }, { ESsTestEnum.Three, ESsTestEnum.Three } }));

		// Test enum dictionary - Remove
		EnumMapOne.Remove(ESsTestEnum.Two);
		Utils.Assert(DictEquals(EnumMapOne, new() { { ESsTestEnum.One, ESsTestEnum.One }, { ESsTestEnum.Three, ESsTestEnum.Three } }));
		EnumMapOne.Clear();
		Utils.Assert(DictEquals(EnumMapOne, new()));
	}

	public static void LongEnumMapTest(TMap<ESsTestLongEnum, ESsTestLongEnum> LongEnumMapOne, TMap<ESsTestLongEnum, ESsTestLongEnum> LongEnumMapTwo)
	{
		// ------------------------------------------
		// Test enum dictionary - Add
		LongEnumMapOne.Add(ESsTestLongEnum.One, ESsTestLongEnum.One);
		LongEnumMapOne.Add(ESsTestLongEnum.Two, ESsTestLongEnum.Two);
		Utils.Assert(DictEquals(LongEnumMapOne, new() { { ESsTestLongEnum.One, ESsTestLongEnum.One }, { ESsTestLongEnum.Two, ESsTestLongEnum.Two } }));

		// Test enum dictionary - Query
		Utils.Assert(LongEnumMapOne.ContainsKey(ESsTestLongEnum.One));
		Utils.Assert(!LongEnumMapOne.ContainsKey(ESsTestLongEnum.Three));
		Utils.Assert(LongEnumMapOne.ContainsValue(ESsTestLongEnum.One));
		Utils.Assert(!LongEnumMapOne.ContainsValue(ESsTestLongEnum.Three));

		// Test enum dictionary - Assign
		LongEnumMapOne[ESsTestLongEnum.One] = ESsTestLongEnum.Four;
		Utils.Assert(LongEnumMapOne[ESsTestLongEnum.One] == ESsTestLongEnum.Four);
		LongEnumMapOne[ESsTestLongEnum.Three] = ESsTestLongEnum.Three;
		Utils.Assert(LongEnumMapOne[ESsTestLongEnum.Three] == ESsTestLongEnum.Three);
		LongEnumMapOne[ESsTestLongEnum.One] = ESsTestLongEnum.One;
		Utils.Assert(DictEquals(LongEnumMapOne, new() { { ESsTestLongEnum.One, ESsTestLongEnum.One }, { ESsTestLongEnum.Two, ESsTestLongEnum.Two }, { ESsTestLongEnum.Three, ESsTestLongEnum.Three } }));

		// Test enum dictionary - Binary equality
		LongEnumMapTwo.CopyFrom(new Dictionary<ESsTestLongEnum, ESsTestLongEnum> { { ESsTestLongEnum.One, ESsTestLongEnum.One }, { ESsTestLongEnum.Two, ESsTestLongEnum.Two } });
		Utils.Assert(DictEquals(LongEnumMapTwo, new() { { ESsTestLongEnum.One, ESsTestLongEnum.One }, { ESsTestLongEnum.Two, ESsTestLongEnum.Two } }));
		LongEnumMapTwo.Clear();
		Utils.Assert(DictEquals(LongEnumMapTwo, []));
		LongEnumMapTwo.CopyFrom(LongEnumMapOne);
		Utils.Assert(DictEquals(LongEnumMapOne, new() { { ESsTestLongEnum.One, ESsTestLongEnum.One }, { ESsTestLongEnum.Two, ESsTestLongEnum.Two }, { ESsTestLongEnum.Three, ESsTestLongEnum.Three } }));

		// Test enum dictionary - Remove
		LongEnumMapOne.Remove(ESsTestLongEnum.Two);
		Utils.Assert(DictEquals(LongEnumMapOne, new() { { ESsTestLongEnum.One, ESsTestLongEnum.One }, { ESsTestLongEnum.Three, ESsTestLongEnum.Three } }));
		LongEnumMapOne.Clear();
		Utils.Assert(DictEquals(LongEnumMapOne, new()));
	}

	public static void IntStructMapTest(TMap<int, FSsMapTestInnerStruct, FSsMapTestInnerStructNativeRef> IntStructMapOne, TMap<int, FSsMapTestInnerStruct, FSsMapTestInnerStructNativeRef> IntStructMapTwo)
	{
		// ------------------------------------------
		// Test int-struct dictionary
		FSsMapTestInnerStruct innerStructA = new FSsMapTestInnerStruct()
		{
			IntIntMap = new() { { 1, 1 } }
		};
		FSsMapTestInnerStruct innerStructB = new FSsMapTestInnerStruct()
		{
			IntIntMap = new() { { 2, 2 } }
		};
		FSsMapTestInnerStruct innerStructC = new FSsMapTestInnerStruct()
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
		Utils.Assert(IntStructMapOne.ContainsValue(innerStructA));
		Utils.Assert(!IntStructMapOne.ContainsValue(innerStructC));

		// Test int-struct dictionary - Assign
		IntStructMapOne[1].FromManaged(innerStructC);
		Utils.Assert(IntStructMapOne[1].ToManaged().Equals(innerStructC));
		IntStructMapOne.Add(3, innerStructC);
		Utils.Assert(IntStructMapOne[3].ToManaged().Equals(innerStructC));
		IntStructMapOne[1].FromManaged(innerStructA);
		Utils.Assert(DictEquals(IntStructMapOne, new() { { 1, innerStructA }, { 2, innerStructB }, { 3, innerStructC } }));

		// Test int-struct dictionary - Binary equality
		IntStructMapTwo.CopyFrom(new Dictionary<int, FSsMapTestInnerStruct> { { 1, innerStructA }, { 2, innerStructB } });
		Utils.Assert(DictEquals(IntStructMapTwo, new() { { 1, innerStructA }, { 2, innerStructB } }));
		IntStructMapTwo.Clear();
		Utils.Assert(DictEquals(IntStructMapTwo, []));
		IntStructMapTwo.CopyFrom(IntStructMapOne);
		Utils.Assert(DictEquals(IntStructMapOne, new() { { 1, innerStructA }, { 2, innerStructB }, { 3, innerStructC } }));

		// Test int-struct dictionary - Reference penetration
		IntStructMapOne[2].IntIntMap[1] = 2;
		Utils.Assert(DictEquals(IntStructMapOne[2].IntIntMap, new() { { 1, 2 }, { 2, 2 } }));
		IntStructMapOne[2].IntIntMap[2] = 4;
		Utils.Assert(DictEquals(IntStructMapOne[2].IntIntMap, new() { { 1, 2 }, { 2, 4 } }));
		IntStructMapOne[2].IntIntMap.Add(3, 6);
		Utils.Assert(DictEquals(IntStructMapOne[2].IntIntMap, new() { { 1, 2 }, { 2, 4 }, { 3, 6 } }));
		IntStructMapOne[2].IntIntMap.CopyFrom(innerStructB.IntIntMap);
		Utils.Assert(IntStructMapOne[2].ToManaged().Equals(innerStructB));

		// Test int-struct dictionary - Remove
		IntStructMapOne.Remove(2);
		Utils.Assert(DictEquals(IntStructMapOne, new() { { 1, innerStructA }, { 3, innerStructC } }));
		IntStructMapOne.Clear();
		Utils.Assert(DictEquals(IntStructMapOne, new()));
	}

	public static void BlittableStructMapTest(TMap<FSsTestBlittableStruct, FSsTestBlittableStruct, FSsTestBlittableStructNativeRef> BlittableStructMapOne, TMap<FSsTestBlittableStruct, FSsTestBlittableStruct, FSsTestBlittableStructNativeRef> BlittableStructMapTwo)
	{
		// ------------------------------------------
		// Test blittable struct dictionary
		FSsTestBlittableStruct one = new FSsTestBlittableStruct { X = 1, Y = 1 };
		FSsTestBlittableStruct two = new FSsTestBlittableStruct { X = 2, Y = 2 };
		FSsTestBlittableStruct three = new FSsTestBlittableStruct { X = 3, Y = 3 };

		// Test blittable struct dictionary - Add
		BlittableStructMapOne.Add(one, one);
		BlittableStructMapOne.Add(two, two);
		Utils.Assert(DictEquals(BlittableStructMapOne, new() { { one, one }, { two, two } }));

		// Test blittable struct dictionary - Query
		Utils.Assert(BlittableStructMapOne.ContainsKey(one));
		Utils.Assert(!BlittableStructMapOne.ContainsKey(three));
		Utils.Assert(BlittableStructMapOne.ContainsValue(one));
		Utils.Assert(!BlittableStructMapOne.ContainsValue(three));

		// Test blittable struct dictionary - Assign
		BlittableStructMapOne[one].FromManaged(three);
		Utils.Assert(BlittableStructMapOne[one].ToManaged().Equals(three));
		BlittableStructMapOne.Add(three, three);
		Utils.Assert(BlittableStructMapOne[three].ToManaged().Equals(three));
		BlittableStructMapOne[one].FromManaged(one);
		Utils.Assert(DictEquals(BlittableStructMapOne, new() { { one, one }, { two, two }, { three, three } }));

		// Test blittable struct dictionary - Binary equality
		BlittableStructMapTwo.CopyFrom(new Dictionary<FSsTestBlittableStruct, FSsTestBlittableStruct> { { one, one }, { two, two } });
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

	public static void ObjectMapTest(TMap<UObject, UObject?> ObjectMapOne, TMap<UObject, UObject?> ObjectMapTwo)
	{
		USsTestObject objOne = NewObject<USsTestObject>();
		USsTestObject objTwo = NewObject<USsTestObject>();
		USsTestObject objThree = NewObject<USsTestObject>();

		// ------------------------------------------
		// Test object-object dictionary - Add
		ObjectMapOne.Add(objOne, objOne);
		ObjectMapOne.Add(objTwo, objTwo);
		Utils.Assert(DictEquals(ObjectMapOne, new() { { objOne, objOne }, { objTwo, objTwo } }));

		// Test object-object dictionary - Query
		Utils.Assert(ObjectMapOne.ContainsKey(objOne));
		Utils.Assert(!ObjectMapOne.ContainsKey(objThree));
		Utils.Assert(ObjectMapOne.ContainsValue(objOne));
		Utils.Assert(!ObjectMapOne.ContainsValue(objThree));

		// Test object-object dictionary - Assign
		ObjectMapOne[objOne] = objThree;
		Utils.Assert(ObjectMapOne[objOne] == objThree);
		ObjectMapOne[objThree] = objThree;
		Utils.Assert(ObjectMapOne[objThree] == objThree);
		ObjectMapOne[objOne] = objOne;
		Utils.Assert(DictEquals(ObjectMapOne, new() { { objOne, objOne }, { objTwo, objTwo }, { objThree, objThree } }));

		// Test object-object dictionary - Binary equality
		ObjectMapTwo.CopyFrom(new Dictionary<UObject, UObject?> { { objOne, objOne }, { objTwo, objTwo } });
		Utils.Assert(DictEquals(ObjectMapTwo, new() { { objOne, objOne }, { objTwo, objTwo } }));
		ObjectMapTwo.Clear();
		Utils.Assert(DictEquals(ObjectMapTwo, []));
		ObjectMapTwo.CopyFrom(ObjectMapOne);
		Utils.Assert(DictEquals(ObjectMapOne, new() { { objOne, objOne }, { objTwo, objTwo }, { objThree, objThree } }));

		// Test object-object dictionary - Remove
		ObjectMapOne.Remove(objTwo);
		Utils.Assert(DictEquals(ObjectMapOne, new() { { objOne, objOne }, { objThree, objThree } }));
		ObjectMapOne.Clear();
		Utils.Assert(DictEquals(ObjectMapOne, new()));
	}

	public static void SoftObjectPtrMapTest(TMap<TSoftObjectPtr<UObject>, TSoftObjectPtr<UObject>> ObjectMapOne, TMap<TSoftObjectPtr<UObject>, TSoftObjectPtr<UObject>> ObjectMapTwo)
	{
		TSoftObjectPtr<UObject> objOne = NewObject<USsTestObject>();
		TSoftObjectPtr<UObject> objTwo = NewObject<USsTestObject>();
		TSoftObjectPtr<UObject> objThree = NewObject<USsTestObject>();

		// ------------------------------------------
		// Test object-object dictionary - Add
		ObjectMapOne.Add(objOne, objOne);
		ObjectMapOne.Add(objTwo, objTwo);
		Utils.Assert(DictEquals(ObjectMapOne, new() { { objOne, objOne }, { objTwo, objTwo } }));

		// Test object-object dictionary - Query
		Utils.Assert(ObjectMapOne.ContainsKey(objOne));
		Utils.Assert(!ObjectMapOne.ContainsKey(objThree));
		Utils.Assert(ObjectMapOne.ContainsValue(objOne));
		Utils.Assert(!ObjectMapOne.ContainsValue(objThree));

		// Test object-object dictionary - Assign
		ObjectMapOne[objOne] = objThree;
		Utils.Assert(ObjectMapOne[objOne] == objThree);
		ObjectMapOne[objThree] = objThree;
		Utils.Assert(ObjectMapOne[objThree] == objThree);
		ObjectMapOne[objOne] = objOne;
		Utils.Assert(DictEquals(ObjectMapOne, new() { { objOne, objOne }, { objTwo, objTwo }, { objThree, objThree } }));

		// Test object-object dictionary - Binary equality
		ObjectMapTwo.CopyFrom(new Dictionary<TSoftObjectPtr<UObject>, TSoftObjectPtr<UObject>> { { objOne, objOne }, { objTwo, objTwo } });
		Utils.Assert(DictEquals(ObjectMapTwo, new() { { objOne, objOne }, { objTwo, objTwo } }));
		ObjectMapTwo.Clear();
		Utils.Assert(DictEquals(ObjectMapTwo, []));
		ObjectMapTwo.CopyFrom(ObjectMapOne);
		Utils.Assert(DictEquals(ObjectMapOne, new() { { objOne, objOne }, { objTwo, objTwo }, { objThree, objThree } }));

		// Test object-object dictionary - Remove
		ObjectMapOne.Remove(objTwo);
		Utils.Assert(DictEquals(ObjectMapOne, new() { { objOne, objOne }, { objThree, objThree } }));
		ObjectMapOne.Clear();
		Utils.Assert(DictEquals(ObjectMapOne, new()));
	}

	public static void ClassMapTest(TMap<TSubclassOf<UObject>, TSubclassOf<UObject>> ClassMapOne, TMap<TSubclassOf<UObject>, TSubclassOf<UObject>> ClassMapTwo)
	{
		UClass clsOne = UObject.StaticClass!;
		UClass clsTwo = AActor.StaticClass!;
		UClass clsThree = APawn.StaticClass!;

		// ------------------------------------------
		// Test class-class dictionary - Add
		ClassMapOne.Add(clsOne, clsOne);
		ClassMapOne.Add(clsTwo, clsTwo);
		Utils.Assert(DictEquals(ClassMapOne, new() { { clsOne, clsOne }, { clsTwo, clsTwo } }));

		// Test class-class dictionary - Query
		Utils.Assert(ClassMapOne.ContainsKey(clsOne));
		Utils.Assert(!ClassMapOne.ContainsKey(clsThree));
		Utils.Assert(ClassMapOne.ContainsValue(clsOne));
		Utils.Assert(!ClassMapOne.ContainsValue(clsThree));

		// Test class-class dictionary - Assign
		ClassMapOne[clsOne] = clsThree;
		Utils.Assert(ClassMapOne[clsOne] == clsThree);
		ClassMapOne[clsThree] = clsThree;
		Utils.Assert(ClassMapOne[clsThree] == clsThree);
		ClassMapOne[clsOne] = clsOne;
		Utils.Assert(DictEquals(ClassMapOne, new() { { clsOne, clsOne }, { clsTwo, clsTwo }, { clsThree, clsThree } }));

		// Test class-class dictionary - Binary equality
		ClassMapTwo.CopyFrom(new Dictionary<TSubclassOf<UObject>, TSubclassOf<UObject>> { { clsOne, clsOne }, { clsTwo, clsTwo } });
		Utils.Assert(DictEquals(ClassMapTwo, new() { { clsOne, clsOne }, { clsTwo, clsTwo } }));
		ClassMapTwo.Clear();
		Utils.Assert(DictEquals(ClassMapTwo, []));
		ClassMapTwo.CopyFrom(ClassMapOne);
		Utils.Assert(DictEquals(ClassMapOne, new() { { clsOne, clsOne }, { clsTwo, clsTwo }, { clsThree, clsThree } }));

		// Test class-class dictionary - Remove
		ClassMapOne.Remove(clsTwo);
		Utils.Assert(DictEquals(ClassMapOne, new() { { clsOne, clsOne }, { clsThree, clsThree } }));
		ClassMapOne.Clear();
		Utils.Assert(DictEquals(ClassMapOne, new()));
	}

	public static void SoftClassPtrMapTest(TMap<TSoftClassPtr<UObject>, TSoftClassPtr<UObject>> ObjectMapOne, TMap<TSoftClassPtr<UObject>, TSoftClassPtr<UObject>> ObjectMapTwo)
	{
		UClass clsOne = UObject.StaticClass!;
		UClass clsTwo = AActor.StaticClass!;
		UClass clsThree = APawn.StaticClass!;

		// ------------------------------------------
		// Test class-class dictionary - Add
		ObjectMapOne.Add(clsOne, clsOne);
		ObjectMapOne.Add(clsTwo, clsTwo);
		Utils.Assert(DictEquals(ObjectMapOne, new() { { clsOne, clsOne }, { clsTwo, clsTwo } }));

		// Test class-class dictionary - Query
		Utils.Assert(ObjectMapOne.ContainsKey(clsOne));
		Utils.Assert(!ObjectMapOne.ContainsKey(clsThree));
		Utils.Assert(ObjectMapOne.ContainsValue(clsOne));
		Utils.Assert(!ObjectMapOne.ContainsValue(clsThree));

		// Test class-class dictionary - Assign
		ObjectMapOne[clsOne] = clsThree;
		Utils.Assert(ObjectMapOne[clsOne] == clsThree);
		ObjectMapOne[clsThree] = clsThree;
		Utils.Assert(ObjectMapOne[clsThree] == clsThree);
		ObjectMapOne[clsOne] = clsOne;
		Utils.Assert(DictEquals(ObjectMapOne, new() { { clsOne, clsOne }, { clsTwo, clsTwo }, { clsThree, clsThree } }));

		// Test class-class dictionary - Binary equality
		ObjectMapTwo.CopyFrom(new Dictionary<TSoftClassPtr<UObject>, TSoftClassPtr<UObject>> { { clsOne, clsOne }, { clsTwo, clsTwo } });
		Utils.Assert(DictEquals(ObjectMapTwo, new() { { clsOne, clsOne }, { clsTwo, clsTwo } }));
		ObjectMapTwo.Clear();
		Utils.Assert(DictEquals(ObjectMapTwo, []));
		ObjectMapTwo.CopyFrom(ObjectMapOne);
		Utils.Assert(DictEquals(ObjectMapOne, new() { { clsOne, clsOne }, { clsTwo, clsTwo }, { clsThree, clsThree } }));

		// Test class-class dictionary - Remove
		ObjectMapOne.Remove(clsTwo);
		Utils.Assert(DictEquals(ObjectMapOne, new() { { clsOne, clsOne }, { clsThree, clsThree } }));
		ObjectMapOne.Clear();
		Utils.Assert(DictEquals(ObjectMapOne, new()));
	}

	public static void IntInterfaceMapTest(TMap<int, ISsTestChildInterface?> IntInterfaceMapOne, TMap<int, ISsTestChildInterface?> IntInterfaceMapTwo)
	{
		// ------------------------------------------
		// Test int-interface dictionary
		ISsTestChildInterface objA = NewObject<USsTestObject>();
		ISsTestChildInterface objB = NewObject<USsTestObject>();
		ISsTestChildInterface objC = NewObject<USsTestObject>();

		// Test int-interface dictionary - Add
		IntInterfaceMapOne.Add(1, objA);
		IntInterfaceMapOne.Add(2, objB);
		Utils.Assert(DictEquals(IntInterfaceMapOne, new() { { 1, objA }, { 2, objB } }));

		// Test int-interface dictionary - Query
		Utils.Assert(IntInterfaceMapOne.ContainsKey(1));
		Utils.Assert(!IntInterfaceMapOne.ContainsKey(3));
		Utils.Assert(IntInterfaceMapOne.ContainsValue(objA));
		Utils.Assert(!IntInterfaceMapOne.ContainsValue(objC));

		// Test int-interface dictionary - Assign
		IntInterfaceMapOne[1] = objC;
		Utils.Assert(IntInterfaceMapOne[1] == objC);
		IntInterfaceMapOne.Add(3, objC);
		Utils.Assert(IntInterfaceMapOne[3] == objC);
		IntInterfaceMapOne[1] = objA;
		Utils.Assert(DictEquals(IntInterfaceMapOne, new() { { 1, objA }, { 2, objB }, { 3, objC } }));

		// Test int-interface dictionary - Binary equality
		IntInterfaceMapTwo.CopyFrom(new Dictionary<int, ISsTestChildInterface?> { { 1, objA }, { 2, objB } });
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

	public static void IntDelegateMapTest(TDelegateMap<int, FSsTestDelegate, Delegate<FSsTestDelegate>> IntDelegateMapOne, TDelegateMap<int, FSsTestDelegate, Delegate<FSsTestDelegate>> IntDelegateMapTwo)
	{
		// ------------------------------------------
		// Test int-delegate dictionary
		USsTestObject obj = NewObject<USsTestObject>();
		FSsTestDelegate delegateA = obj.FuncBlueprintImplementable;
		FSsTestDelegate delegateB = obj.FuncBlueprintNative;
		FSsTestDelegate delegateC = obj.CallFuncBlueprintImplementable;

		// Test int-delegate dictionary - Add
		IntDelegateMapOne.Add(1, delegateA);
		IntDelegateMapOne.Add(2, delegateB);
		Utils.Assert(DictEquals(IntDelegateMapOne, new() { { 1, delegateA }, { 2, delegateB } }));

		// Test int-delegate dictionary - Query
		Utils.Assert(IntDelegateMapOne.ContainsKey(1));
		Utils.Assert(!IntDelegateMapOne.ContainsKey(3));
		Utils.Assert(IntDelegateMapOne.ContainsValue(delegateA));
		Utils.Assert(!IntDelegateMapOne.ContainsValue(delegateC));

		// Test int-delegate dictionary - Assign
		IntDelegateMapOne[1].FromManaged(delegateC);
		Utils.Assert(IntDelegateMapOne[1].ToManaged().Equals(delegateC));
		IntDelegateMapOne.Add(3, delegateC);
		Utils.Assert(IntDelegateMapOne[3].ToManaged().Equals(delegateC));
		IntDelegateMapOne[1].FromManaged(delegateA);
		Utils.Assert(DictEquals(IntDelegateMapOne, new() { { 1, delegateA }, { 2, delegateB }, { 3, delegateC } }));

		// Test int-delegate dictionary - Binary equality
		IntDelegateMapTwo.CopyFrom(new Dictionary<int, FSsTestDelegate> { { 1, delegateA }, { 2, delegateB } });
		Utils.Assert(DictEquals(IntDelegateMapTwo, new() { { 1, delegateA }, { 2, delegateB } }));
		IntDelegateMapTwo.Clear();
		Utils.Assert(DictEquals(IntDelegateMapTwo, []));
		IntDelegateMapTwo.CopyFrom(IntDelegateMapOne);
		Utils.Assert(DictEquals(IntDelegateMapOne, new() { { 1, delegateA }, { 2, delegateB }, { 3, delegateC } }));
		// Test int-delegate dictionary - Reference penetration
		Utils.Assert(IntDelegateMapOne[1].Execute(2) == 0);
		Utils.Assert(IntDelegateMapOne[2].Execute(2) == 2);
		Utils.Assert(IntDelegateMapOne[3].Execute(2) == 0);

		// Test int-delegate dictionary - Remove
		IntDelegateMapOne.Remove(2);
		Utils.Assert(DictEquals(IntDelegateMapOne, new() { { 1, delegateA }, { 3, delegateC } }));
		IntDelegateMapOne.Clear();
		Utils.Assert(DictEquals(IntDelegateMapOne, new()));
	}

	public static bool DictEquals<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> a, Dictionary<TKey, TValue> b)
		where TKey : notnull
	{
		// ReSharper disable once UsageOfDefaultStructEquality
		return a.OrderBy(x => x.Key).SequenceEqual(b.OrderBy(x => x.Key));
	}

	private static bool DictEquals<TKey, TValue, TValueRef>(TMapBase<TKey, TValue, TValueRef> a, Dictionary<TKey, TValue> b)
		where TKey : notnull where TValueRef : IStructNativeRef<TValue> where TValue : struct
	{
		// ReSharper disable once UsageOfDefaultStructEquality
		return a.OrderBy(x => x.Key).SequenceEqual(b.OrderBy(x => x.Key));
	}

	private static bool DictEquals<TKey, TValue, TValueRef>(TDelegateMapBase<TKey, TValue, TValueRef> a, Dictionary<TKey, TValue> b)
		where TKey : notnull where TValueRef : Delegate<TValue> where TValue : Delegate
	{
		// ReSharper disable once UsageOfDefaultStructEquality
		return a.OrderBy(x => x.Key).SequenceEqual(b.OrderBy(x => x.Key));
	}
}
