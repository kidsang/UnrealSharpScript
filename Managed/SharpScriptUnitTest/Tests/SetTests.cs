using SharpScript.Interop;
using SharpScriptUnitTest.Types;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test various functionalities of the TSet wrapper class.
/// </summary>
[RecordFilePath]
public class SetTests : IUnitTestInterface
{
	public bool RunTest()
	{
		IntPtr testStructOneNativePtr = 0;
		IntPtr testStructTwoNativePtr = 0;
		try
		{
			testStructOneNativePtr = TypeInterop.CreateStructInstance(SsSetTestStructNativeRef.NativeType);
			SsSetTestStructNativeRef testStructOneNativeRef = new SsSetTestStructNativeRef(testStructOneNativePtr);
			testStructTwoNativePtr = TypeInterop.CreateStructInstance(SsSetTestStructNativeRef.NativeType);
			SsSetTestStructNativeRef testStructTwoNativeRef = new SsSetTestStructNativeRef(testStructTwoNativePtr);

			IntSetTest(testStructOneNativeRef.IntSet, testStructTwoNativeRef.IntSet);
			StructSetTest(testStructOneNativeRef.StructSet, testStructTwoNativeRef.StructSet);
		}
		finally
		{
			if (testStructOneNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsSetTestStructNativeRef.NativeType, ref testStructOneNativePtr);
			}

			if (testStructTwoNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsSetTestStructNativeRef.NativeType, ref testStructTwoNativePtr);
			}
		}

		return true;
	}

	public static void IntSetTest(Set<int> IntSetOne, Set<int> IntSetTwo)
	{
		// Test basic operations
		Utils.Assert(IntSetOne.Add(1));
		Utils.Assert(IntSetOne.SetEquals([1]));
		Utils.Assert(IntSetOne.Add(2));
		Utils.Assert(IntSetOne.SetEquals([1, 2]));
		Utils.Assert(!IntSetOne.Add(2));
		Utils.Assert(IntSetOne.SetEquals([1, 2]));
		Utils.Assert(!IntSetOne.Remove(3));
		Utils.Assert(IntSetOne.SetEquals([1, 2]));
		Utils.Assert(IntSetOne.Remove(1));
		Utils.Assert(IntSetOne.SetEquals([2]));
		IntSetOne.Clear();
		Utils.Assert(IntSetOne.SetEquals([]));

		// Test set read-only methods
		IntSetOne.CopyFrom(new HashSet<int> { 1, 2, 3 });
		Utils.Assert(IntSetOne.SetEquals([1, 2, 3]));
		Utils.Assert(IntSetOne.IsSubsetOf([1, 2, 3]));
		Utils.Assert(!IntSetOne.IsProperSubsetOf([1, 2, 3]));
		Utils.Assert(IntSetOne.IsProperSubsetOf([1, 2, 3, 4]));
		Utils.Assert(IntSetOne.IsSupersetOf([1, 2, 3]));
		Utils.Assert(!IntSetOne.IsProperSupersetOf([1, 2, 3]));
		Utils.Assert(IntSetOne.IsProperSupersetOf([2, 3]));
		Utils.Assert(IntSetOne.Overlaps([3, 4]));
		Utils.Assert(!IntSetOne.Overlaps([4, 5]));

		// Test set read-write methods
		IntSetOne.CopyFrom(new HashSet<int> { 1, 2, 3 });
		IntSetOne.ExceptWith([2, 3, 4]);
		Utils.Assert(IntSetOne.SetEquals([1]));
		IntSetOne.CopyFrom(new HashSet<int> { 1, 2, 3 });
		IntSetOne.IntersectWith([2, 3, 4]);
		Utils.Assert(IntSetOne.SetEquals([2, 3]));
		IntSetOne.CopyFrom(new HashSet<int> { 1, 2, 3 });
		IntSetOne.SymmetricExceptWith([2, 3, 4]);
		Utils.Assert(IntSetOne.SetEquals([1, 4]));
		IntSetOne.CopyFrom(new HashSet<int> { 1, 2, 3 });
		IntSetOne.UnionWith([2, 3, 4]);
		Utils.Assert(IntSetOne.SetEquals([1, 2, 3, 4]));

		// Test set assignment and equality
		IntSetOne.CopyFrom(new HashSet<int> { 4, 5, 6 });
		Utils.Assert(IntSetOne.SetEquals([4, 5, 6]));
		IntSetTwo.CopyFrom(IntSetOne);
		Utils.Assert(IntSetTwo.SetEquals(IntSetOne));
	}

	public static void StructSetTest(Set<SsTestBlittableStruct> StructSetOne, Set<SsTestBlittableStruct> StructSetTwo)
	{
		SsTestBlittableStruct one = new SsTestBlittableStruct { X = 1, Y = 1 };
		SsTestBlittableStruct two = new SsTestBlittableStruct { X = 2, Y = 2 };
		SsTestBlittableStruct three = new SsTestBlittableStruct { X = 3, Y = 3 };
		SsTestBlittableStruct four = new SsTestBlittableStruct { X = 4, Y = 4 };
		SsTestBlittableStruct five = new SsTestBlittableStruct { X = 5, Y = 5 };
		SsTestBlittableStruct six = new SsTestBlittableStruct { X = 6, Y = 6 };

		// Test basic operations
		Utils.Assert(StructSetOne.Add(one));
		Utils.Assert(StructSetOne.SetEquals([one]));
		Utils.Assert(StructSetOne.Add(two));
		Utils.Assert(StructSetOne.SetEquals([one, two]));
		Utils.Assert(!StructSetOne.Add(two));
		Utils.Assert(StructSetOne.SetEquals([one, two]));
		Utils.Assert(!StructSetOne.Remove(three));
		Utils.Assert(StructSetOne.SetEquals([one, two]));
		Utils.Assert(StructSetOne.Remove(one));
		Utils.Assert(StructSetOne.SetEquals([two]));
		StructSetOne.Clear();
		Utils.Assert(StructSetOne.SetEquals([]));

		// Test set read-only methods
		StructSetOne.CopyFrom(new HashSet<SsTestBlittableStruct> { one, two, three });
		Utils.Assert(StructSetOne.SetEquals([one, two, three]));
		Utils.Assert(StructSetOne.IsSubsetOf([one, two, three]));
		Utils.Assert(!StructSetOne.IsProperSubsetOf([one, two, three]));
		Utils.Assert(StructSetOne.IsProperSubsetOf([one, two, three, four]));
		Utils.Assert(StructSetOne.IsSupersetOf([one, two, three]));
		Utils.Assert(!StructSetOne.IsProperSupersetOf([one, two, three]));
		Utils.Assert(StructSetOne.IsProperSupersetOf([two, three]));
		Utils.Assert(StructSetOne.Overlaps([three, four]));
		Utils.Assert(!StructSetOne.Overlaps([four, five]));

		// Test set read-write methods
		StructSetOne.CopyFrom(new HashSet<SsTestBlittableStruct> { one, two, three });
		StructSetOne.ExceptWith([two, three, four]);
		Utils.Assert(StructSetOne.SetEquals([one]));
		StructSetOne.CopyFrom(new HashSet<SsTestBlittableStruct> { one, two, three });
		StructSetOne.IntersectWith([two, three, four]);
		Utils.Assert(StructSetOne.SetEquals([two, three]));
		StructSetOne.CopyFrom(new HashSet<SsTestBlittableStruct> { one, two, three });
		StructSetOne.SymmetricExceptWith([two, three, four]);
		Utils.Assert(StructSetOne.SetEquals([one, four]));
		StructSetOne.CopyFrom(new HashSet<SsTestBlittableStruct> { one, two, three });
		StructSetOne.UnionWith([two, three, four]);
		Utils.Assert(StructSetOne.SetEquals([one, two, three, four]));

		// Test set assignment and equality
		StructSetOne.CopyFrom(new HashSet<SsTestBlittableStruct> { four, five, six });
		Utils.Assert(StructSetOne.SetEquals([four, five, six]));
		StructSetTwo.CopyFrom(StructSetOne);
		Utils.Assert(StructSetTwo.SetEquals(StructSetOne));
	}
}
