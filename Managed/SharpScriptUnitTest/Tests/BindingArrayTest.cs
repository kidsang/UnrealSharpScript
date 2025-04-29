using static UnrealEngine.Globals;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;
using UnrealEngine.SharpScriptUnitTest;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test various functionalities of the TArray wrapper class.
/// </summary>
[RecordFilePath]
public class BindingArrayTest : IUnitTestInterface
{
	public bool RunTest()
	{
		IntPtr testStructOneNativePtr = 0;
		IntPtr testStructTwoNativePtr = 0;
		try
		{
			testStructOneNativePtr = TypeInterop.CreateStructInstance(SsBindingArrayTestStructNativeRef.NativeType);
			SsBindingArrayTestStructNativeRef testStructOneNativeRef = new SsBindingArrayTestStructNativeRef(testStructOneNativePtr);
			testStructTwoNativePtr = TypeInterop.CreateStructInstance(SsBindingArrayTestStructNativeRef.NativeType);
			SsBindingArrayTestStructNativeRef testStructTwoNativeRef = new SsBindingArrayTestStructNativeRef(testStructTwoNativePtr);

			ArrayTest.IntArrayTest(testStructOneNativeRef.IntArray, testStructTwoNativeRef.IntArray);
			ArrayTest.BoolArrayTest(testStructOneNativeRef.BoolArray, testStructTwoNativeRef.BoolArray);
			ArrayTest.StringArrayTest(testStructOneNativeRef.StringArray, testStructTwoNativeRef.StringArray);
			ArrayTest.TextArrayTest(testStructOneNativeRef.TextArray, testStructTwoNativeRef.TextArray);
			EnumArrayTest(testStructOneNativeRef.EnumArray, testStructTwoNativeRef.EnumArray);
			LongEnumArrayTest(testStructOneNativeRef.LongEnumArray, testStructTwoNativeRef.LongEnumArray);
			StructArrayTest(testStructOneNativeRef.StructArray, testStructTwoNativeRef.StructArray);
			BlittableStructArrayTest(testStructOneNativeRef.BlittableStructArray, testStructTwoNativeRef.BlittableStructArray);
			ArrayTest.ObjectArrayTest(testStructOneNativeRef.ObjectArray, testStructTwoNativeRef.ObjectArray);
			ArrayTest.SoftObjectPtrArrayTest(testStructOneNativeRef.SoftObjectPtrArray, testStructTwoNativeRef.SoftObjectPtrArray);
			ArrayTest.ObjectArrayTest(testStructOneNativeRef.WeakObjectPtrArray, testStructTwoNativeRef.WeakObjectPtrArray);
			ArrayTest.LazyObjectPtrArrayTest(testStructOneNativeRef.LazyObjectPtrArray, testStructTwoNativeRef.LazyObjectPtrArray);
			ArrayTest.ClassArrayTest(testStructOneNativeRef.ClassArray, testStructTwoNativeRef.ClassArray);
			ArrayTest.SoftClassPtrArrayTest(testStructOneNativeRef.SoftClassPtrArray, testStructTwoNativeRef.SoftClassPtrArray);
			InterfaceArrayTest(testStructOneNativeRef.InterfaceArray, testStructTwoNativeRef.InterfaceArray);
			DelegateArrayTest(testStructOneNativeRef.DelegateArray, testStructTwoNativeRef.DelegateArray);
		}
		finally
		{
			if (testStructOneNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsBindingArrayTestStructNativeRef.NativeType, ref testStructOneNativePtr);
			}

			if (testStructTwoNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsBindingArrayTestStructNativeRef.NativeType, ref testStructTwoNativePtr);
			}
		}

		return true;
	}

	private static void StructArrayTest(Array<SsBindingArrayTestInnerStruct, SsBindingArrayTestInnerStructNativeRef> StructArrayOne, Array<SsBindingArrayTestInnerStruct, SsBindingArrayTestInnerStructNativeRef> StructArrayTwo)
	{
		// ------------------------------------------
		// Test struct array
		SsBindingArrayTestInnerStruct innerStructA = new SsBindingArrayTestInnerStruct
		{
			IntArray = [1, 2, 3]
		};
		SsBindingArrayTestInnerStruct innerStructB = new SsBindingArrayTestInnerStruct
		{
			IntArray = [4, 5, 6]
		};
		SsBindingArrayTestInnerStruct innerStructC = new SsBindingArrayTestInnerStruct
		{
			IntArray = [7, 8, 9]
		};
		SsBindingArrayTestInnerStruct innerStructD = new SsBindingArrayTestInnerStruct
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

		//// Test struct array - Query
		//Utils.Assert(StructArrayOne.Contains(innerStructA));
		//Utils.Assert(StructArrayOne.IndexOf(innerStructB) == 1);
		//Utils.Assert(StructArrayOne[2].ToManaged().Equals(innerStructC));
		//Utils.Assert(!StructArrayOne.Contains(innerStructD));

		// Test struct array - Assignment
		StructArrayOne[2].FromManaged(innerStructD);
		//Utils.Assert(StructArrayOne[2].ToManaged().Equals(innerStructD));
		Utils.Assert(StructArrayOne.ToList().SequenceEqual([
			innerStructA, innerStructB, innerStructD
		]));

		// Test struct array - Binary equality
		StructArrayTwo.CopyFrom(new List<SsBindingArrayTestInnerStruct> { innerStructA, innerStructB, innerStructD });
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
		//Utils.Assert(StructArrayOne[1].ToManaged().Equals(innerStructB));

		// Test struct array - Remove
		//StructArrayOne.Remove(innerStructB);
		//Utils.Assert(StructArrayOne.ToList().SequenceEqual([innerStructA, innerStructD]));
		//StructArrayOne.RemoveAt(0);
		//Utils.Assert(StructArrayOne.ToList().SequenceEqual([innerStructD]));
		StructArrayOne.Clear();
		Utils.Assert(StructArrayOne.ToList().SequenceEqual([]));
	}

	public static void EnumArrayTest(Array<ESsBindingTestEnum> EnumArrayOne, Array<ESsBindingTestEnum> EnumArrayTwo)
	{
		// ------------------------------------------
		// Test FEnum array - Add
		EnumArrayOne.Add(ESsBindingTestEnum.One);
		EnumArrayOne.Add(ESsBindingTestEnum.Three);
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([ESsBindingTestEnum.One, ESsBindingTestEnum.Three]));

		// Test FEnum array - Insert
		EnumArrayOne.Insert(1, ESsBindingTestEnum.Two);
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([ESsBindingTestEnum.One, ESsBindingTestEnum.Two, ESsBindingTestEnum.Three]));

		// Test FEnum array - Query
		Utils.Assert(EnumArrayOne.Contains(ESsBindingTestEnum.One));
		Utils.Assert(EnumArrayOne.IndexOf(ESsBindingTestEnum.Two) == 1);
		Utils.Assert(EnumArrayOne[2] == ESsBindingTestEnum.Three);
		Utils.Assert(!EnumArrayOne.Contains(ESsBindingTestEnum.Four));

		// Test FEnum array - Assignment
		EnumArrayOne[2] = ESsBindingTestEnum.Four;
		Utils.Assert(EnumArrayOne[2] == ESsBindingTestEnum.Four);
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([ESsBindingTestEnum.One, ESsBindingTestEnum.Two, ESsBindingTestEnum.Four]));

		// Test FEnum array - Binary equality
		EnumArrayTwo.CopyFrom(new List<ESsBindingTestEnum> { ESsBindingTestEnum.One, ESsBindingTestEnum.Two, ESsBindingTestEnum.Four });
		Utils.Assert(EnumArrayTwo.SequenceEqual(EnumArrayOne));
		EnumArrayTwo.Clear();
		Utils.Assert(EnumArrayTwo.ToList().SequenceEqual([]));
		EnumArrayTwo.CopyFrom(EnumArrayOne);
		Utils.Assert(EnumArrayTwo.SequenceEqual(EnumArrayOne));

		// Test FEnum array - Remove
		EnumArrayOne.Remove(ESsBindingTestEnum.Two);
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([ESsBindingTestEnum.One, ESsBindingTestEnum.Four]));
		EnumArrayOne.RemoveAt(0);
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([ESsBindingTestEnum.Four]));
		EnumArrayOne.Clear();
		Utils.Assert(EnumArrayOne.ToList().SequenceEqual([]));
	}

	public static void LongEnumArrayTest(Array<ESsBindingTestLongEnum> LongEnumArrayOne, Array<ESsBindingTestLongEnum> LongEnumArrayTwo)
	{
		// ------------------------------------------
		// Test FLongEnum array - Add
		LongEnumArrayOne.Add(ESsBindingTestLongEnum.One);
		LongEnumArrayOne.Add(ESsBindingTestLongEnum.Three);
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.Three]));

		// Test FLongEnum array - Insert
		LongEnumArrayOne.Insert(1, ESsBindingTestLongEnum.Two);
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.Two, ESsBindingTestLongEnum.Three]));

		// Test FLongEnum array - Query
		Utils.Assert(LongEnumArrayOne.Contains(ESsBindingTestLongEnum.One));
		Utils.Assert(LongEnumArrayOne.IndexOf(ESsBindingTestLongEnum.Two) == 1);
		Utils.Assert(LongEnumArrayOne[2] == ESsBindingTestLongEnum.Three);
		Utils.Assert(!LongEnumArrayOne.Contains(ESsBindingTestLongEnum.Four));

		// Test FLongEnum array - Assignment
		LongEnumArrayOne[2] = ESsBindingTestLongEnum.Four;
		Utils.Assert(LongEnumArrayOne[2] == ESsBindingTestLongEnum.Four);
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.Two, ESsBindingTestLongEnum.Four]));

		// Test FLongEnum array - Binary equality
		LongEnumArrayTwo.CopyFrom(new List<ESsBindingTestLongEnum> { ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.Two, ESsBindingTestLongEnum.Four });
		Utils.Assert(LongEnumArrayTwo.SequenceEqual(LongEnumArrayOne));
		LongEnumArrayTwo.Clear();
		Utils.Assert(LongEnumArrayTwo.ToList().SequenceEqual([]));
		LongEnumArrayTwo.CopyFrom(LongEnumArrayOne);
		Utils.Assert(LongEnumArrayTwo.SequenceEqual(LongEnumArrayOne));

		// Test FLongEnum array - Remove
		LongEnumArrayOne.Remove(ESsBindingTestLongEnum.Two);
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([ESsBindingTestLongEnum.One, ESsBindingTestLongEnum.Four]));
		LongEnumArrayOne.RemoveAt(0);
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([ESsBindingTestLongEnum.Four]));
		LongEnumArrayOne.Clear();
		Utils.Assert(LongEnumArrayOne.ToList().SequenceEqual([]));
	}

	public static void BlittableStructArrayTest(Array<SsBindingTestBlittableStruct, SsBindingTestBlittableStructNativeRef> BlittableStructArrayOne, Array<SsBindingTestBlittableStruct, SsBindingTestBlittableStructNativeRef> BlittableStructArrayTwo)
	{
		// ReSharper disable UsageOfDefaultStructEquality
		// ------------------------------------------
		// Test struct array
		SsBindingTestBlittableStruct innerBlittableStructA = new SsBindingTestBlittableStruct
		{
			X = 10,
			Y = 10,
		};
		SsBindingTestBlittableStruct innerBlittableStructB = new SsBindingTestBlittableStruct
		{
			X = 20,
			Y = 20,
		};
		SsBindingTestBlittableStruct innerBlittableStructC = new SsBindingTestBlittableStruct
		{
			X = 30,
			Y = 30,
		};
		SsBindingTestBlittableStruct innerBlittableStructD = new SsBindingTestBlittableStruct
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
		BlittableStructArrayTwo.CopyFrom(new List<SsBindingTestBlittableStruct> { innerBlittableStructA, innerBlittableStructB, innerBlittableStructD });
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
		// ReSharper restore UsageOfDefaultStructEquality
	}

	public static void InterfaceArrayTest(Array<ISsBindingTestChildInterface?> InterfaceArrayOne, Array<ISsBindingTestChildInterface?> InterfaceArrayTwo)
	{
		// ------------------------------------------
		// Test interface array
		ISsBindingTestChildInterface objA = NewObject<SsBindingTestObject>();
		ISsBindingTestChildInterface objB = NewObject<SsBindingTestObject>();
		ISsBindingTestChildInterface objC = NewObject<SsBindingTestObject>();
		ISsBindingTestChildInterface objD = NewObject<SsBindingTestObject>();

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
		InterfaceArrayTwo.CopyFrom(new List<ISsBindingTestChildInterface> { objA, objB, objD });
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

	public static void DelegateArrayTest(DelegateArray<FSsBindingTestDelegate, Delegate<FSsBindingTestDelegate>> DelegateArrayOne, DelegateArray<FSsBindingTestDelegate, Delegate<FSsBindingTestDelegate>> DelegateArrayTwo)
	{
		// ------------------------------------------
		// Test delegate array
		SsBindingTestObject obj = NewObject<SsBindingTestObject>();
		// FSsBindingTestDelegate  delegateA = obj.FuncBlueprintImplementable;
		// FSsBindingTestDelegate  delegateB = obj.FuncBlueprintNative;
		FSsBindingTestDelegate delegateA = obj.CallFuncBlueprintImplementable;
		FSsBindingTestDelegate delegateB = obj.CallFuncBlueprintNative;
		FSsBindingTestDelegate delegateC = obj.CallFuncBlueprintImplementable;
		FSsBindingTestDelegate delegateD = obj.CallFuncBlueprintNative;

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
		//Utils.Assert(!DelegateArrayOne.Contains(delegateD));

		// Test delegate array - Assignment
		DelegateArrayOne[2].FromManaged(delegateD);
		Utils.Assert(DelegateArrayOne[2].ToManaged().Equals(delegateD));
		Utils.Assert(DelegateArrayOne.ToList().SequenceEqual([
			delegateA, delegateB, delegateD
		]));

		// Test delegate array - Binary equality
		DelegateArrayTwo.CopyFrom(new List<FSsBindingTestDelegate> { delegateA, delegateB, delegateD });
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

public static class BindingArrayTestMethods
{
	public static bool SequenceEqual(this List<SsBindingArrayTestInnerStruct> listA, IEnumerable<SsBindingArrayTestInnerStruct> b)
	{
		List<SsBindingArrayTestInnerStruct> listB = [..b];
		if (listA.Count != listB.Count)
		{
			return false;
		}

		for (int i = 0; i < listA.Count; ++i)
		{
			var itemA = listA[i];
			var itemB = listB[i];
			if (!itemA.IntArray.SequenceEqual(itemB.IntArray))
			{
				return false;
			}
		}

		return true;
	}
}
