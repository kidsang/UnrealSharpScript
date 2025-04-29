using static UnrealEngine.Globals;
using SharpScript.Interop;
using SharpScriptUnitTest.Types;
using UnrealEngine.CoreUObject;
using UnrealEngine.Engine;
using UnrealEngine.Intrinsic;
using Object = UnrealEngine.CoreUObject.Object;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test various functionalities of the TArray wrapper class.
/// </summary>
[RecordFilePath]
public class ArrayTest : IUnitTestInterface
{
	public bool RunTest()
	{
		IntPtr testStructOneNativePtr = 0;
		IntPtr testStructTwoNativePtr = 0;
		try
		{
			testStructOneNativePtr = TypeInterop.CreateStructInstance(SsArrayTestStructNativeRef.NativeType);
			SsArrayTestStructNativeRef testStructOneNativeRef = new SsArrayTestStructNativeRef(testStructOneNativePtr);
			testStructTwoNativePtr = TypeInterop.CreateStructInstance(SsArrayTestStructNativeRef.NativeType);
			SsArrayTestStructNativeRef testStructTwoNativeRef = new SsArrayTestStructNativeRef(testStructTwoNativePtr);

			IntArrayTest(testStructOneNativeRef.IntArray, testStructTwoNativeRef.IntArray);
			BoolArrayTest(testStructOneNativeRef.BoolArray, testStructTwoNativeRef.BoolArray);
			StringArrayTest(testStructOneNativeRef.StringArray, testStructTwoNativeRef.StringArray);
			TextArrayTest(testStructOneNativeRef.TextArray, testStructTwoNativeRef.TextArray);
			EnumArrayTest(testStructOneNativeRef.EnumArray, testStructTwoNativeRef.EnumArray);
			LongEnumArrayTest(testStructOneNativeRef.LongEnumArray, testStructTwoNativeRef.LongEnumArray);
			StructArrayTest(testStructOneNativeRef.StructArray, testStructTwoNativeRef.StructArray);
			BlittableStructArrayTest(testStructOneNativeRef.BlittableStructArray, testStructTwoNativeRef.BlittableStructArray);
			ObjectArrayTest(testStructOneNativeRef.ObjectArray, testStructTwoNativeRef.ObjectArray);
			SoftObjectPtrArrayTest(testStructOneNativeRef.SoftObjectPtrArray, testStructTwoNativeRef.SoftObjectPtrArray);
			ObjectArrayTest(testStructOneNativeRef.WeakObjectPtrArray, testStructTwoNativeRef.WeakObjectPtrArray);
			LazyObjectPtrArrayTest(testStructOneNativeRef.LazyObjectPtrArray, testStructTwoNativeRef.LazyObjectPtrArray);
			ClassArrayTest(testStructOneNativeRef.ClassArray, testStructTwoNativeRef.ClassArray);
			SoftClassPtrArrayTest(testStructOneNativeRef.SoftClassPtrArray, testStructTwoNativeRef.SoftClassPtrArray);
			InterfaceArrayTest(testStructOneNativeRef.InterfaceArray, testStructTwoNativeRef.InterfaceArray);
			DelegateArrayTest(testStructOneNativeRef.DelegateArray, testStructTwoNativeRef.DelegateArray);
		}
		finally
		{
			if (testStructOneNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsArrayTestStructNativeRef.NativeType, ref testStructOneNativePtr);
			}

			if (testStructTwoNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsArrayTestStructNativeRef.NativeType, ref testStructTwoNativePtr);
			}
		}

		return true;
	}

	public static void IntArrayTest(Array<int> IntArrayOne, Array<int> IntArrayTwo)
	{
		// ------------------------------------------
		// Test integer array - Add
		IntArrayOne.Add(1);
		IntArrayOne.Add(3);
		Utils.Assert(IntArrayOne.ToList().SequenceEqual([1, 3]));

		// Test integer array - Insert
		IntArrayOne.Insert(1, 2);
		Utils.Assert(IntArrayOne.ToList().SequenceEqual([1, 2, 3]));

		// Test integer array - Query
		Utils.Assert(IntArrayOne.Contains(1));
		Utils.Assert(IntArrayOne.IndexOf(2) == 1);
		Utils.Assert(IntArrayOne[2] == 3);
		Utils.Assert(!IntArrayOne.Contains(4));

		// Test integer array - Assignment
		IntArrayOne[2] = 4;
		Utils.Assert(IntArrayOne[2] == 4);
		Utils.Assert(IntArrayOne.ToList().SequenceEqual([1, 2, 4]));

		// Test integer array - Binary equality
		IntArrayTwo.CopyFrom(new List<int> { 1, 2, 4 });
		Utils.Assert(IntArrayTwo.SequenceEqual(IntArrayOne));
		IntArrayTwo.Clear();
		Utils.Assert(IntArrayTwo.ToList().SequenceEqual([]));
		IntArrayTwo.CopyFrom(IntArrayOne);
		Utils.Assert(IntArrayTwo.SequenceEqual(IntArrayOne));

		// Test integer array - Remove
		IntArrayOne.Remove(2);
		Utils.Assert(IntArrayOne.ToList().SequenceEqual([1, 4]));
		IntArrayOne.RemoveAt(0);
		Utils.Assert(IntArrayOne.ToList().SequenceEqual([4]));
		IntArrayOne.Clear();
		Utils.Assert(IntArrayOne.ToList().SequenceEqual([]));
	}

	public static void BoolArrayTest(Array<bool> BoolArrayOne, Array<bool> BoolArrayTwo)
	{
		// ------------------------------------------
		// Test boolean array - Add
		BoolArrayOne.Add(true);
		BoolArrayOne.Add(false);
		Utils.Assert(BoolArrayOne.ToList().SequenceEqual([true, false]));

		// Test boolean array - Insert
		BoolArrayOne.Insert(0, false);
		Utils.Assert(BoolArrayOne.ToList().SequenceEqual([false, true, false]));

		// Test boolean array - Query
		Utils.Assert(BoolArrayOne.Contains(true));
		Utils.Assert(BoolArrayOne.IndexOf(true) == 1);
		Utils.Assert(BoolArrayOne[2] == false);

		// Test boolean array - Assignment
		BoolArrayOne[2] = true;
		Utils.Assert(BoolArrayOne[2]);
		Utils.Assert(BoolArrayOne.ToList().SequenceEqual([false, true, true]));

		// Test boolean array - Binary equality
		BoolArrayTwo.CopyFrom(new List<bool> { false, true, true });
		Utils.Assert(BoolArrayTwo.SequenceEqual(BoolArrayOne));
		BoolArrayTwo.Clear();
		Utils.Assert(BoolArrayTwo.ToList().SequenceEqual([]));
		BoolArrayTwo.CopyFrom(BoolArrayOne);
		Utils.Assert(BoolArrayTwo.SequenceEqual(BoolArrayOne));

		// Test boolean array - Remove
		BoolArrayOne.Remove(true);
		Utils.Assert(BoolArrayOne.ToList().SequenceEqual([false, true]));
		BoolArrayOne.RemoveAt(0);
		Utils.Assert(BoolArrayOne.ToList().SequenceEqual([true]));
		BoolArrayOne.Clear();
		Utils.Assert(BoolArrayOne.ToList().SequenceEqual([]));
	}

	public static void StringArrayTest(Array<string> StringArrayOne, Array<string> StringArrayTwo)
	{
		// ------------------------------------------
		// Test FString array - Add
		StringArrayOne.Add("one");
		StringArrayOne.Add("three");
		Utils.Assert(StringArrayOne.ToList().SequenceEqual(["one", "three"]));

		// Test FString array - Insert
		StringArrayOne.Insert(1, "two");
		Utils.Assert(StringArrayOne.ToList().SequenceEqual(["one", "two", "three"]));

		// Test FString array - Query
		Utils.Assert(StringArrayOne.Contains("one"));
		Utils.Assert(StringArrayOne.IndexOf("two") == 1);
		Utils.Assert(StringArrayOne[2] == "three");
		Utils.Assert(!StringArrayOne.Contains("four"));

		// Test FString array - Assignment
		StringArrayOne[2] = "four";
		Utils.Assert(StringArrayOne[2] == "four");
		Utils.Assert(StringArrayOne.ToList().SequenceEqual(["one", "two", "four"]));

		// Test FString array - Binary equality
		StringArrayTwo.CopyFrom(new List<string> { "one", "two", "four" });
		Utils.Assert(StringArrayTwo.SequenceEqual(StringArrayOne));
		StringArrayTwo.Clear();
		Utils.Assert(StringArrayTwo.ToList().SequenceEqual([]));
		StringArrayTwo.CopyFrom(StringArrayOne);
		Utils.Assert(StringArrayTwo.SequenceEqual(StringArrayOne));

		// Test FString array - Remove
		StringArrayOne.Remove("two");
		Utils.Assert(StringArrayOne.ToList().SequenceEqual(["one", "four"]));
		StringArrayOne.RemoveAt(0);
		Utils.Assert(StringArrayOne.ToList().SequenceEqual(["four"]));
		StringArrayOne.Clear();
		Utils.Assert(StringArrayOne.ToList().SequenceEqual([]));
	}

	public static void TextArrayTest(Array<Text> TextArrayOne, Array<Text> TextArrayTwo)
	{
		// ------------------------------------------
		// Test FText array - Add
		TextArrayOne.Add(new Text("one"));
		TextArrayOne.Add(new Text("three"));
		Utils.Assert(TextArrayOne.ToList().SequenceEqual([new Text("one"), new Text("three")]));

		// Test FText array - Insert
		TextArrayOne.Insert(1, new Text("two"));
		Utils.Assert(TextArrayOne.ToList().SequenceEqual([new Text("one"), new Text("two"), new Text("three")]));

		// Test FText array - Query
		Utils.Assert(TextArrayOne.Contains(new Text("one")));
		Utils.Assert(TextArrayOne.IndexOf(new Text("two")) == 1);
		Utils.Assert(TextArrayOne[2] == new Text("three"));
		Utils.Assert(!TextArrayOne.Contains(new Text("four")));

		// Test FText array - Assignment
		TextArrayOne[2] = new Text("four");
		Utils.Assert(TextArrayOne[2] == new Text("four"));
		Utils.Assert(TextArrayOne.ToList().SequenceEqual([new Text("one"), new Text("two"), new Text("four")]));

		// Test FText array - Binary equality
		TextArrayTwo.CopyFrom(new List<Text> { new Text("one"), new Text("two"), new Text("four") });
		Utils.Assert(TextArrayTwo.SequenceEqual(TextArrayOne));
		TextArrayTwo.Clear();
		Utils.Assert(TextArrayTwo.ToList().SequenceEqual([]));
		TextArrayTwo.CopyFrom(TextArrayOne);
		Utils.Assert(TextArrayTwo.SequenceEqual(TextArrayOne));

		// Test FText array - Remove
		TextArrayOne.Remove(new Text("two"));
		Utils.Assert(TextArrayOne.ToList().SequenceEqual([new Text("one"), new Text("four")]));
		TextArrayOne.RemoveAt(0);
		Utils.Assert(TextArrayOne.ToList().SequenceEqual([new Text("four")]));
		TextArrayOne.Clear();
		Utils.Assert(TextArrayOne.ToList().SequenceEqual([]));
	}

	public static void EnumArrayTest(Array<ESsTestEnum> EnumArrayOne, Array<ESsTestEnum> EnumArrayTwo)
	{
		// ------------------------------------------
		// Test FEnum array - Add
		EnumArrayOne.Add(ESsTestEnum.One);
		EnumArrayOne.Add(ESsTestEnum.Three);
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([ESsTestEnum.One, ESsTestEnum.Three]));

		// Test FEnum array - Insert
		EnumArrayOne.Insert(1, ESsTestEnum.Two);
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([ESsTestEnum.One, ESsTestEnum.Two, ESsTestEnum.Three]));

		// Test FEnum array - Query
		Utils.Assert(EnumArrayOne.Contains(ESsTestEnum.One));
		Utils.Assert(EnumArrayOne.IndexOf(ESsTestEnum.Two) == 1);
		Utils.Assert(EnumArrayOne[2] == ESsTestEnum.Three);
		Utils.Assert(!EnumArrayOne.Contains(ESsTestEnum.Four));

		// Test FEnum array - Assignment
		EnumArrayOne[2] = ESsTestEnum.Four;
		Utils.Assert(EnumArrayOne[2] == ESsTestEnum.Four);
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([ESsTestEnum.One, ESsTestEnum.Two, ESsTestEnum.Four]));

		// Test FEnum array - Binary equality
		EnumArrayTwo.CopyFrom(new List<ESsTestEnum> { ESsTestEnum.One, ESsTestEnum.Two, ESsTestEnum.Four });
		Utils.Assert(EnumArrayTwo.SequenceEqual(EnumArrayOne));
		EnumArrayTwo.Clear();
		Utils.Assert(EnumArrayTwo.ToList().SequenceEqual([]));
		EnumArrayTwo.CopyFrom(EnumArrayOne);
		Utils.Assert(EnumArrayTwo.SequenceEqual(EnumArrayOne));

		// Test FEnum array - Remove
		EnumArrayOne.Remove(ESsTestEnum.Two);
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([ESsTestEnum.One, ESsTestEnum.Four]));
		EnumArrayOne.RemoveAt(0);
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([ESsTestEnum.Four]));
		EnumArrayOne.Clear();
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([]));
	}

	public static void LongEnumArrayTest(Array<ESsTestLongEnum> LongEnumArrayOne, Array<ESsTestLongEnum> LongEnumArrayTwo)
	{
		// ------------------------------------------
		// Test FLongEnum array - Add
		LongEnumArrayOne.Add(ESsTestLongEnum.One);
		LongEnumArrayOne.Add(ESsTestLongEnum.Three);
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([ESsTestLongEnum.One, ESsTestLongEnum.Three]));

		// Test FLongEnum array - Insert
		LongEnumArrayOne.Insert(1, ESsTestLongEnum.Two);
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([ESsTestLongEnum.One, ESsTestLongEnum.Two, ESsTestLongEnum.Three]));

		// Test FLongEnum array - Query
		Utils.Assert(LongEnumArrayOne.Contains(ESsTestLongEnum.One));
		Utils.Assert(LongEnumArrayOne.IndexOf(ESsTestLongEnum.Two) == 1);
		Utils.Assert(LongEnumArrayOne[2] == ESsTestLongEnum.Three);
		Utils.Assert(!LongEnumArrayOne.Contains(ESsTestLongEnum.Four));

		// Test FLongEnum array - Assignment
		LongEnumArrayOne[2] = ESsTestLongEnum.Four;
		Utils.Assert(LongEnumArrayOne[2] == ESsTestLongEnum.Four);
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([ESsTestLongEnum.One, ESsTestLongEnum.Two, ESsTestLongEnum.Four]));

		// Test FLongEnum array - Binary equality
		LongEnumArrayTwo.CopyFrom(new List<ESsTestLongEnum> { ESsTestLongEnum.One, ESsTestLongEnum.Two, ESsTestLongEnum.Four });
		Utils.Assert(LongEnumArrayTwo.SequenceEqual(LongEnumArrayOne));
		LongEnumArrayTwo.Clear();
		Utils.Assert(LongEnumArrayTwo.ToList().SequenceEqual([]));
		LongEnumArrayTwo.CopyFrom(LongEnumArrayOne);
		Utils.Assert(LongEnumArrayTwo.SequenceEqual(LongEnumArrayOne));

		// Test FLongEnum array - Remove
		LongEnumArrayOne.Remove(ESsTestLongEnum.Two);
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([ESsTestLongEnum.One, ESsTestLongEnum.Four]));
		LongEnumArrayOne.RemoveAt(0);
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([ESsTestLongEnum.Four]));
		LongEnumArrayOne.Clear();
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([]));
	}

	public static void ObjectArrayTest(Array<Object?> ObjectArrayOne, Array<Object?> ObjectArrayTwo)
	{
		SsTestObject objOne = NewObject<SsTestObject>();
		SsTestObject objTwo = NewObject<SsTestObject>();
		SsTestObject objThree = NewObject<SsTestObject>();
		SsTestObject objFour = NewObject<SsTestObject>();

		// ------------------------------------------
		// Test UObject array - Add
		ObjectArrayOne.Add(objOne);
		ObjectArrayOne.Add(objThree);
		Utils.Assert(ObjectArrayOne.ToList().SequenceEqual([objOne, objThree]));

		// Test UObject array - Insert
		ObjectArrayOne.Insert(1, objTwo);
		Utils.Assert(ObjectArrayOne.ToList().SequenceEqual([objOne, objTwo, objThree]));

		// Test UObject array - Query
		Utils.Assert(ObjectArrayOne.Contains(objOne));
		Utils.Assert(ObjectArrayOne.IndexOf(objTwo) == 1);
		Utils.Assert(ObjectArrayOne[2] == objThree);
		Utils.Assert(!ObjectArrayOne.Contains(objFour));

		// Test UObject array - Assignment
		ObjectArrayOne[2] = objFour;
		Utils.Assert(ObjectArrayOne[2] == objFour);
		Utils.Assert(ObjectArrayOne.ToList().SequenceEqual([objOne, objTwo, objFour]));

		// Test UObject array - Binary equality
		ObjectArrayTwo.CopyFrom(new List<Object> { objOne, objTwo, objFour });
		Utils.Assert(ObjectArrayTwo.SequenceEqual(ObjectArrayOne));
		ObjectArrayTwo.Clear();
		Utils.Assert(ObjectArrayTwo.ToList().SequenceEqual([]));
		ObjectArrayTwo.CopyFrom(ObjectArrayOne);
		Utils.Assert(ObjectArrayTwo.SequenceEqual(ObjectArrayOne));

		// Test UObject array - Remove
		ObjectArrayOne.Remove(objTwo);
		Utils.Assert(ObjectArrayOne.ToList().SequenceEqual([objOne, objFour]));
		ObjectArrayOne.RemoveAt(0);
		Utils.Assert(ObjectArrayOne.ToList().SequenceEqual([objFour]));
		ObjectArrayOne.Clear();
		Utils.Assert(ObjectArrayOne.ToList().SequenceEqual([]));
	}

	public static void SoftObjectPtrArrayTest(Array<SoftObjectPtr<Object>> SoftObjectPtrArrayOne, Array<SoftObjectPtr<Object>> SoftObjectPtrArrayTwo)
	{
		SoftObjectPtr<Object> objOne = NewObject<SsTestObject>();
		SoftObjectPtr<Object> objTwo = NewObject<SsTestObject>();
		SoftObjectPtr<Object> objThree = NewObject<SsTestObject>();
		SoftObjectPtr<Object> objFour = NewObject<SsTestObject>();

		// ------------------------------------------
		// Test UObject array - Add
		SoftObjectPtrArrayOne.Add(objOne);
		SoftObjectPtrArrayOne.Add(objThree);
		Utils.Assert(SoftObjectPtrArrayOne.ToList().SequenceEqual([objOne, objThree]));

		// Test UObject array - Insert
		SoftObjectPtrArrayOne.Insert(1, objTwo);
		Utils.Assert(SoftObjectPtrArrayOne.ToList().SequenceEqual([objOne, objTwo, objThree]));

		// Test UObject array - Query
		Utils.Assert(SoftObjectPtrArrayOne.Contains(objOne));
		Utils.Assert(SoftObjectPtrArrayOne.IndexOf(objTwo) == 1);
		Utils.Assert(SoftObjectPtrArrayOne[2] == objThree);
		Utils.Assert(!SoftObjectPtrArrayOne.Contains(objFour));

		// Test UObject array - Assignment
		SoftObjectPtrArrayOne[2] = objFour;
		Utils.Assert(SoftObjectPtrArrayOne[2] == objFour);
		Utils.Assert(SoftObjectPtrArrayOne.ToList().SequenceEqual([objOne, objTwo, objFour]));

		// Test UObject array - Binary equality
		SoftObjectPtrArrayTwo.CopyFrom(new List<SoftObjectPtr<Object>> { objOne, objTwo, objFour });
		Utils.Assert(SoftObjectPtrArrayTwo.SequenceEqual(SoftObjectPtrArrayOne));
		SoftObjectPtrArrayTwo.Clear();
		Utils.Assert(SoftObjectPtrArrayTwo.ToList().SequenceEqual([]));
		SoftObjectPtrArrayTwo.CopyFrom(SoftObjectPtrArrayOne);
		Utils.Assert(SoftObjectPtrArrayTwo.SequenceEqual(SoftObjectPtrArrayOne));

		// Test UObject array - Remove
		SoftObjectPtrArrayOne.Remove(objTwo);
		Utils.Assert(SoftObjectPtrArrayOne.ToList().SequenceEqual([objOne, objFour]));
		SoftObjectPtrArrayOne.RemoveAt(0);
		Utils.Assert(SoftObjectPtrArrayOne.ToList().SequenceEqual([objFour]));
		SoftObjectPtrArrayOne.Clear();
		Utils.Assert(SoftObjectPtrArrayOne.ToList().SequenceEqual([]));
	}

	public static void LazyObjectPtrArrayTest(Array<LazyObjectPtr<Object>> LazyObjectPtrArrayOne, Array<LazyObjectPtr<Object>> LazyObjectPtrArrayTwo)
	{
		LazyObjectPtr<Object> objOne = NewObject<SsTestObject>();
		LazyObjectPtr<Object> objTwo = NewObject<SsTestObject>();
		LazyObjectPtr<Object> objThree = NewObject<SsTestObject>();
		LazyObjectPtr<Object> objFour = NewObject<SsTestObject>();

		// ------------------------------------------
		// Test UObject array - Add
		LazyObjectPtrArrayOne.Add(objOne);
		LazyObjectPtrArrayOne.Add(objThree);
		Utils.Assert(LazyObjectPtrArrayOne.ToList().SequenceEqual([objOne, objThree]));

		// Test UObject array - Insert
		LazyObjectPtrArrayOne.Insert(1, objTwo);
		Utils.Assert(LazyObjectPtrArrayOne.ToList().SequenceEqual([objOne, objTwo, objThree]));

		// Test UObject array - Query
		Utils.Assert(LazyObjectPtrArrayOne.Contains(objOne));
		Utils.Assert(LazyObjectPtrArrayOne.IndexOf(objTwo) == 1);
		Utils.Assert(LazyObjectPtrArrayOne[2] == objThree);
		Utils.Assert(!LazyObjectPtrArrayOne.Contains(objFour));

		// Test UObject array - Assignment
		LazyObjectPtrArrayOne[2] = objFour;
		Utils.Assert(LazyObjectPtrArrayOne[2] == objFour);
		Utils.Assert(LazyObjectPtrArrayOne.ToList().SequenceEqual([objOne, objTwo, objFour]));

		// Test UObject array - Binary equality
		LazyObjectPtrArrayTwo.CopyFrom(new List<LazyObjectPtr<Object>> { objOne, objTwo, objFour });
		Utils.Assert(LazyObjectPtrArrayTwo.SequenceEqual(LazyObjectPtrArrayOne));
		LazyObjectPtrArrayTwo.Clear();
		Utils.Assert(LazyObjectPtrArrayTwo.ToList().SequenceEqual([]));
		LazyObjectPtrArrayTwo.CopyFrom(LazyObjectPtrArrayOne);
		Utils.Assert(LazyObjectPtrArrayTwo.SequenceEqual(LazyObjectPtrArrayOne));

		// Test UObject array - Remove
		LazyObjectPtrArrayOne.Remove(objTwo);
		Utils.Assert(LazyObjectPtrArrayOne.ToList().SequenceEqual([objOne, objFour]));
		LazyObjectPtrArrayOne.RemoveAt(0);
		Utils.Assert(LazyObjectPtrArrayOne.ToList().SequenceEqual([objFour]));
		LazyObjectPtrArrayOne.Clear();
		Utils.Assert(LazyObjectPtrArrayOne.ToList().SequenceEqual([]));
	}

	public static void ClassArrayTest(Array<SubclassOf<Object>> ClassArrayOne, Array<SubclassOf<Object>> ClassArrayTwo)
	{
		Class clsOne = Object.StaticClass!;
		Class clsTwo = Actor.StaticClass!;
		Class clsThree = Pawn.StaticClass!;
		Class clsFour = Character.StaticClass!;

		// ------------------------------------------
		// Test UObject array - Add
		ClassArrayOne.Add(clsOne);
		ClassArrayOne.Add(clsThree);
		Utils.Assert(ClassArrayOne.ToList().SequenceEqual([clsOne, clsThree]));

		// Test UObject array - Insert
		ClassArrayOne.Insert(1, clsTwo);
		Utils.Assert(ClassArrayOne.ToList().SequenceEqual([clsOne, clsTwo, clsThree]));

		// Test UObject array - Query
		Utils.Assert(ClassArrayOne.Contains(clsOne));
		Utils.Assert(ClassArrayOne.IndexOf(clsTwo) == 1);
		Utils.Assert(ClassArrayOne[2] == clsThree);
		Utils.Assert(!ClassArrayOne.Contains(clsFour));

		// Test UObject array - Assignment
		ClassArrayOne[2] = clsFour;
		Utils.Assert(ClassArrayOne[2] == clsFour);
		Utils.Assert(ClassArrayOne.ToList().SequenceEqual([clsOne, clsTwo, clsFour]));

		// Test UObject array - Binary equality
		ClassArrayTwo.CopyFrom(new List<SubclassOf<Object>> { clsOne, clsTwo, clsFour });
		Utils.Assert(ClassArrayTwo.SequenceEqual(ClassArrayOne));
		ClassArrayTwo.Clear();
		Utils.Assert(ClassArrayTwo.ToList().SequenceEqual([]));
		ClassArrayTwo.CopyFrom(ClassArrayOne);
		Utils.Assert(ClassArrayTwo.SequenceEqual(ClassArrayOne));

		// Test UObject array - Remove
		ClassArrayOne.Remove(clsTwo);
		Utils.Assert(ClassArrayOne.ToList().SequenceEqual([clsOne, clsFour]));
		ClassArrayOne.RemoveAt(0);
		Utils.Assert(ClassArrayOne.ToList().SequenceEqual([clsFour]));
		ClassArrayOne.Clear();
		Utils.Assert(ClassArrayOne.ToList().SequenceEqual([]));
	}

	public static void SoftClassPtrArrayTest(Array<SoftClassPtr<Object>> SoftClassPtrArrayOne, Array<SoftClassPtr<Object>> SoftClassPtrArrayTwo)
	{
		Class clsOne = Object.StaticClass!;
		Class clsTwo = Actor.StaticClass!;
		Class clsThree = Pawn.StaticClass!;
		Class clsFour = Character.StaticClass!;

		// ------------------------------------------
		// Test UObject array - Add
		SoftClassPtrArrayOne.Add(clsOne);
		SoftClassPtrArrayOne.Add(clsThree);
		Utils.Assert(SoftClassPtrArrayOne.ToList().SequenceEqual([clsOne, clsThree]));

		// Test UObject array - Insert
		SoftClassPtrArrayOne.Insert(1, clsTwo);
		Utils.Assert(SoftClassPtrArrayOne.ToList().SequenceEqual([clsOne, clsTwo, clsThree]));

		// Test UObject array - Query
		Utils.Assert(SoftClassPtrArrayOne.Contains(clsOne));
		Utils.Assert(SoftClassPtrArrayOne.IndexOf(clsTwo) == 1);
		Utils.Assert(SoftClassPtrArrayOne[2] == clsThree);
		Utils.Assert(!SoftClassPtrArrayOne.Contains(clsFour));

		// Test UObject array - Assignment
		SoftClassPtrArrayOne[2] = clsFour;
		Utils.Assert(SoftClassPtrArrayOne[2] == clsFour);
		Utils.Assert(SoftClassPtrArrayOne.ToList().SequenceEqual([clsOne, clsTwo, clsFour]));

		// Test UObject array - Binary equality
		SoftClassPtrArrayTwo.CopyFrom(new List<SoftClassPtr<Object>> { clsOne, clsTwo, clsFour });
		Utils.Assert(SoftClassPtrArrayTwo.SequenceEqual(SoftClassPtrArrayOne));
		SoftClassPtrArrayTwo.Clear();
		Utils.Assert(SoftClassPtrArrayTwo.ToList().SequenceEqual([]));
		SoftClassPtrArrayTwo.CopyFrom(SoftClassPtrArrayOne);
		Utils.Assert(SoftClassPtrArrayTwo.SequenceEqual(SoftClassPtrArrayOne));

		// Test UObject array - Remove
		SoftClassPtrArrayOne.Remove(clsTwo);
		Utils.Assert(SoftClassPtrArrayOne.ToList().SequenceEqual([clsOne, clsFour]));
		SoftClassPtrArrayOne.RemoveAt(0);
		Utils.Assert(SoftClassPtrArrayOne.ToList().SequenceEqual([clsFour]));
		SoftClassPtrArrayOne.Clear();
		Utils.Assert(SoftClassPtrArrayOne.ToList().SequenceEqual([]));
	}

	public static void StructArrayTest(Array<SsArrayTestInnerStruct, SsArrayTestInnerStructNativeRef> StructArrayOne, Array<SsArrayTestInnerStruct, SsArrayTestInnerStructNativeRef> StructArrayTwo)
	{
		// ------------------------------------------
		// Test struct array
		SsArrayTestInnerStruct innerStructA = new SsArrayTestInnerStruct
		{
			IntArray = [1, 2, 3]
		};
		SsArrayTestInnerStruct innerStructB = new SsArrayTestInnerStruct
		{
			IntArray = [4, 5, 6]
		};
		SsArrayTestInnerStruct innerStructC = new SsArrayTestInnerStruct
		{
			IntArray = [7, 8, 9]
		};
		SsArrayTestInnerStruct innerStructD = new SsArrayTestInnerStruct
		{
			IntArray = [10, 11, 12]
		};

		// Test struct array - Add
		StructArrayOne.Add(innerStructA);
		StructArrayOne.Add(innerStructC);
		Utils.Assert(StructArrayOne.ToList().SequenceEqual([
			innerStructA, innerStructC
		]));

		// Test struct array - Insert
		StructArrayOne.Insert(1, innerStructB);
		Utils.Assert(StructArrayOne.ToList().SequenceEqual([
			innerStructA, innerStructB, innerStructC
		]));

		// Test struct array - Query
		Utils.Assert(StructArrayOne.Contains(innerStructA));
		Utils.Assert(StructArrayOne.IndexOf(innerStructB) == 1);
		Utils.Assert(StructArrayOne[2].ToManaged().Equals(innerStructC));
		Utils.Assert(!StructArrayOne.Contains(innerStructD));

		// Test struct array - Assignment
		StructArrayOne[2].FromManaged(innerStructD);
		Utils.Assert(StructArrayOne[2].ToManaged().Equals(innerStructD));
		Utils.Assert(StructArrayOne.ToList().SequenceEqual([
			innerStructA, innerStructB, innerStructD
		]));

		// Test struct array - Binary equality
		StructArrayTwo.CopyFrom(new List<SsArrayTestInnerStruct> { innerStructA, innerStructB, innerStructD });
		Utils.Assert(StructArrayTwo.SequenceEqual(StructArrayOne));
		StructArrayTwo.Clear();
		Utils.Assert(StructArrayTwo.ToList().SequenceEqual([]));
		StructArrayTwo.CopyFrom(StructArrayOne);
		Utils.Assert(StructArrayTwo.SequenceEqual(StructArrayOne));

		// Test struct array - Reference penetration
		StructArrayOne[1].IntArray[0] = 6;
		Utils.Assert(StructArrayOne[1].IntArray.ToList().SequenceEqual([6, 5, 6]));
		StructArrayOne[1].IntArray[2] = 4;
		Utils.Assert(StructArrayOne[1].IntArray.ToList().SequenceEqual([6, 5, 4]));
		StructArrayOne[1].IntArray.Add(3);
		Utils.Assert(StructArrayOne[1].IntArray.ToList().SequenceEqual([6, 5, 4, 3]));
		StructArrayOne[1].IntArray.CopyFrom(new List<int> { 4, 5, 6 });
		Utils.Assert(StructArrayOne[1].ToManaged().Equals(innerStructB));

		// Test struct array - Remove
		StructArrayOne.Remove(innerStructB);
		Utils.Assert(StructArrayOne.ToList().SequenceEqual([innerStructA, innerStructD]));
		StructArrayOne.RemoveAt(0);
		Utils.Assert(StructArrayOne.ToList().SequenceEqual([innerStructD]));
		StructArrayOne.Clear();
		Utils.Assert(StructArrayOne.ToList().SequenceEqual([]));
	}

	public static void BlittableStructArrayTest(Array<SsTestBlittableStruct, SsTestBlittableStructNativeRef> BlittableStructArrayOne, Array<SsTestBlittableStruct, SsTestBlittableStructNativeRef> BlittableStructArrayTwo)
	{
		// ------------------------------------------
		// Test struct array
		SsTestBlittableStruct innerBlittableStructA = new SsTestBlittableStruct
		{
			X = 10,
			Y = 10,
		};
		SsTestBlittableStruct innerBlittableStructB = new SsTestBlittableStruct
		{
			X = 20,
			Y = 20,
		};
		SsTestBlittableStruct innerBlittableStructC = new SsTestBlittableStruct
		{
			X = 30,
			Y = 30,
		};
		SsTestBlittableStruct innerBlittableStructD = new SsTestBlittableStruct
		{
			X = 40,
			Y = 40,
		};

		// Test struct array - Add
		BlittableStructArrayOne.Add(innerBlittableStructA);
		BlittableStructArrayOne.Add(innerBlittableStructC);
		Utils.Assert(BlittableStructArrayOne.ToList().SequenceEqual([
			innerBlittableStructA, innerBlittableStructC
		]));

		// Test struct array - Insert
		BlittableStructArrayOne.Insert(1, innerBlittableStructB);
		Utils.Assert(BlittableStructArrayOne.ToList().SequenceEqual([
			innerBlittableStructA, innerBlittableStructB, innerBlittableStructC
		]));

		// Test struct array - Query
		Utils.Assert(BlittableStructArrayOne.Contains(innerBlittableStructA));
		Utils.Assert(BlittableStructArrayOne.IndexOf(innerBlittableStructB) == 1);
		Utils.Assert(BlittableStructArrayOne[2].ToManaged().Equals(innerBlittableStructC));
		Utils.Assert(!BlittableStructArrayOne.Contains(innerBlittableStructD));

		// Test struct array - Assignment
		BlittableStructArrayOne[2].FromManaged(innerBlittableStructD);
		Utils.Assert(BlittableStructArrayOne[2].ToManaged().Equals(innerBlittableStructD));
		Utils.Assert(BlittableStructArrayOne.ToList().SequenceEqual([
			innerBlittableStructA, innerBlittableStructB, innerBlittableStructD
		]));

		// Test struct array - Binary equality
		BlittableStructArrayTwo.CopyFrom(new List<SsTestBlittableStruct> { innerBlittableStructA, innerBlittableStructB, innerBlittableStructD });
		Utils.Assert(BlittableStructArrayTwo.SequenceEqual(BlittableStructArrayOne));
		BlittableStructArrayTwo.Clear();
		Utils.Assert(BlittableStructArrayTwo.ToList().SequenceEqual([]));
		BlittableStructArrayTwo.CopyFrom(BlittableStructArrayOne);
		Utils.Assert(BlittableStructArrayTwo.SequenceEqual(BlittableStructArrayOne));

		// Test struct array - Reference penetration
		BlittableStructArrayOne[1].X = 111;
		Utils.Assert(BlittableStructArrayOne[1].X == 111);
		BlittableStructArrayOne[1].Y = 222;
		Utils.Assert(BlittableStructArrayOne[1].Y == 222);
		BlittableStructArrayOne[1].FromManaged(innerBlittableStructB);
		Utils.Assert(BlittableStructArrayOne[1].ToManaged().Equals(innerBlittableStructB));

		// Test struct array - Remove
		BlittableStructArrayOne.Remove(innerBlittableStructB);
		Utils.Assert(BlittableStructArrayOne.ToList().SequenceEqual([innerBlittableStructA, innerBlittableStructD]));
		BlittableStructArrayOne.RemoveAt(0);
		Utils.Assert(BlittableStructArrayOne.ToList().SequenceEqual([innerBlittableStructD]));
		BlittableStructArrayOne.Clear();
		Utils.Assert(BlittableStructArrayOne.ToList().SequenceEqual([]));
	}

	public static void InterfaceArrayTest(Array<ISsTestChildInterface?> InterfaceArrayOne, Array<ISsTestChildInterface?> InterfaceArrayTwo)
	{
		// ------------------------------------------
		// Test interface array
		ISsTestChildInterface objA = NewObject<SsTestObject>();
		ISsTestChildInterface objB = NewObject<SsTestObject>();
		ISsTestChildInterface objC = NewObject<SsTestObject>();
		ISsTestChildInterface objD = NewObject<SsTestObject>();

		// Test interface array - Add
		InterfaceArrayOne.Add(objA);
		InterfaceArrayOne.Add(objC);
		Utils.Assert(InterfaceArrayOne.ToList().SequenceEqual([
			objA, objC
		]));

		// Test interface array - Insert
		InterfaceArrayOne.Insert(1, objB);
		Utils.Assert(InterfaceArrayOne.ToList().SequenceEqual([
			objA, objB, objC
		]));

		// Test interface array - Query
		Utils.Assert(InterfaceArrayOne.Contains(objA));
		Utils.Assert(InterfaceArrayOne.IndexOf(objB) == 1);
		Utils.Assert(InterfaceArrayOne[2] == objC);
		Utils.Assert(!InterfaceArrayOne.Contains(objD));

		// Test interface array - Assignment
		InterfaceArrayOne[2] = objD;
		Utils.Assert(InterfaceArrayOne[2] == objD);
		Utils.Assert(InterfaceArrayOne.ToList().SequenceEqual([
			objA, objB, objD
		]));

		// Test interface array - Binary equality
		InterfaceArrayTwo.CopyFrom(new List<ISsTestChildInterface> { objA, objB, objD });
		Utils.Assert(InterfaceArrayTwo.SequenceEqual(InterfaceArrayOne));
		InterfaceArrayTwo.Clear();
		Utils.Assert(InterfaceArrayTwo.ToList().SequenceEqual([]));
		InterfaceArrayTwo.CopyFrom(InterfaceArrayOne);
		Utils.Assert(InterfaceArrayTwo.SequenceEqual(InterfaceArrayOne));

		// Test interface array - Reference penetration
		Utils.Assert(InterfaceArrayOne[0]!.FuncInterface(123) == 123);
		Utils.Assert(InterfaceArrayOne[1]!.FuncInterface(123) == 123);
		Utils.Assert(InterfaceArrayOne[2]!.FuncInterfaceChild(123) == 123);

		// Test interface array - Remove
		InterfaceArrayOne.Remove(objB);
		Utils.Assert(InterfaceArrayOne.ToList().SequenceEqual([objA, objD]));
		InterfaceArrayOne.RemoveAt(0);
		Utils.Assert(InterfaceArrayOne.ToList().SequenceEqual([objD]));
		InterfaceArrayOne.Clear();
		Utils.Assert(InterfaceArrayOne.ToList().SequenceEqual([]));
	}

	public static void DelegateArrayTest(DelegateArray<SsTestDelegate, Delegate<SsTestDelegate>> DelegateArrayOne, DelegateArray<SsTestDelegate, Delegate<SsTestDelegate>> DelegateArrayTwo)
	{
		// ------------------------------------------
		// Test delegate array
		SsTestObject obj = NewObject<SsTestObject>();
		SsTestDelegate delegateA = obj.FuncBlueprintImplementable;
		SsTestDelegate delegateB = obj.FuncBlueprintNative;
		SsTestDelegate delegateC = obj.CallFuncBlueprintImplementable;
		SsTestDelegate delegateD = obj.CallFuncBlueprintNative;

		// Test delegate array - Add
		DelegateArrayOne.Add(delegateA);
		DelegateArrayOne.Add(delegateC);
		Utils.Assert(DelegateArrayOne.ToList().SequenceEqual([
			delegateA, delegateC
		]));

		// Test delegate array - Insert
		DelegateArrayOne.Insert(1, delegateB);
		Utils.Assert(DelegateArrayOne.ToList().SequenceEqual([
			delegateA, delegateB, delegateC
		]));

		// Test delegate array - Query
		Utils.Assert(DelegateArrayOne.Contains(delegateA));
		Utils.Assert(DelegateArrayOne.IndexOf(delegateB) == 1);
		Utils.Assert(DelegateArrayOne[2].ToManaged().Equals(delegateC));
		Utils.Assert(!DelegateArrayOne.Contains(delegateD));

		// Test delegate array - Assignment
		DelegateArrayOne[2].FromManaged(delegateD);
		Utils.Assert(DelegateArrayOne[2].ToManaged().Equals(delegateD));
		Utils.Assert(DelegateArrayOne.ToList().SequenceEqual([
			delegateA, delegateB, delegateD
		]));

		// Test delegate array - Binary equality
		DelegateArrayTwo.CopyFrom(new List<SsTestDelegate> { delegateA, delegateB, delegateD });
		Utils.Assert(DelegateArrayTwo.SequenceEqual(DelegateArrayOne));
		DelegateArrayTwo.Clear();
		Utils.Assert(DelegateArrayTwo.ToList().SequenceEqual([]));
		DelegateArrayTwo.CopyFrom(DelegateArrayOne);
		Utils.Assert(DelegateArrayTwo.SequenceEqual(DelegateArrayOne));

		// Test delegate array - Reference penetration
		Utils.Assert(DelegateArrayOne[0].Execute(2) == 0);
		Utils.Assert(DelegateArrayOne[1].Execute(2) == 2);
		Utils.Assert(DelegateArrayOne[2].Execute(2) == 2);

		// Test delegate array - Remove
		DelegateArrayOne.Remove(delegateB);
		Utils.Assert(DelegateArrayOne.ToList().SequenceEqual([delegateA, delegateD]));
		DelegateArrayOne.RemoveAt(0);
		Utils.Assert(DelegateArrayOne.ToList().SequenceEqual([delegateD]));
		DelegateArrayOne.Clear();
		Utils.Assert(DelegateArrayOne.ToList().SequenceEqual([]));
	}
}
