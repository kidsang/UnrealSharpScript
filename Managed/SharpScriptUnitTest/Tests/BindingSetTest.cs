using SharpScript.Interop;
using UnrealEngine.Intrinsic;
using UnrealEngine.SharpScriptUnitTest;

// ReSharper disable UsageOfDefaultStructEquality

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test various functionalities of the TSet wrapper class.
/// </summary>
[RecordFilePath]
public class BindingSetTest : IUnitTestInterface
{
	public bool RunTest()
	{
		IntPtr testStructOneNativePtr = 0;
		IntPtr testStructTwoNativePtr = 0;
		try
		{
			testStructOneNativePtr = TypeInterop.CreateStructInstance(SsBindingSetTestStructNativeRef.NativeType);
			SsBindingSetTestStructNativeRef testStructOneNativeRef = new SsBindingSetTestStructNativeRef(testStructOneNativePtr);
			testStructTwoNativePtr = TypeInterop.CreateStructInstance(SsBindingSetTestStructNativeRef.NativeType);
			SsBindingSetTestStructNativeRef testStructTwoNativeRef = new SsBindingSetTestStructNativeRef(testStructTwoNativePtr);

			SetTests.IntSetTest(testStructOneNativeRef.IntSet, testStructTwoNativeRef.IntSet);
			StructSetTest(testStructOneNativeRef.StructSet, testStructTwoNativeRef.StructSet);
		}
		finally
		{
			if (testStructOneNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsBindingSetTestStructNativeRef.NativeType, ref testStructOneNativePtr);
			}

			if (testStructTwoNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsBindingSetTestStructNativeRef.NativeType, ref testStructTwoNativePtr);
			}
		}

		return true;
	}

	public static void StructSetTest(Set<SsBindingTestBlittableStruct> StructSetOne, Set<SsBindingTestBlittableStruct> StructSetTwo)
	{
		SsBindingTestBlittableStruct one = new SsBindingTestBlittableStruct { X = 1, Y = 1 };
		SsBindingTestBlittableStruct two = new SsBindingTestBlittableStruct { X = 2, Y = 2 };
		SsBindingTestBlittableStruct three = new SsBindingTestBlittableStruct { X = 3, Y = 3 };
		SsBindingTestBlittableStruct four = new SsBindingTestBlittableStruct { X = 4, Y = 4 };
		SsBindingTestBlittableStruct five = new SsBindingTestBlittableStruct { X = 5, Y = 5 };
		SsBindingTestBlittableStruct six = new SsBindingTestBlittableStruct { X = 6, Y = 6 };

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

		// Test read-only set methods
		StructSetOne.CopyFrom(new HashSet<SsBindingTestBlittableStruct> { one, two, three });
		Utils.Assert(StructSetOne.SetEquals([one, two, three]));
		Utils.Assert(StructSetOne.IsSubsetOf([one, two, three]));
		Utils.Assert(!StructSetOne.IsProperSubsetOf([one, two, three]));
		Utils.Assert(StructSetOne.IsProperSubsetOf([one, two, three, four]));
		Utils.Assert(StructSetOne.IsSupersetOf([one, two, three]));
		Utils.Assert(!StructSetOne.IsProperSupersetOf([one, two, three]));
		Utils.Assert(StructSetOne.IsProperSupersetOf([two, three]));
		Utils.Assert(StructSetOne.Overlaps([three, four]));
		Utils.Assert(!StructSetOne.Overlaps([four, five]));

		// Test read-write set methods
		StructSetOne.CopyFrom(new HashSet<SsBindingTestBlittableStruct> { one, two, three });
		StructSetOne.ExceptWith([two, three, four]);
		Utils.Assert(StructSetOne.SetEquals([one]));
		StructSetOne.CopyFrom(new HashSet<SsBindingTestBlittableStruct> { one, two, three });
		StructSetOne.IntersectWith([two, three, four]);
		Utils.Assert(StructSetOne.SetEquals([two, three]));
		StructSetOne.CopyFrom(new HashSet<SsBindingTestBlittableStruct> { one, two, three });
		StructSetOne.SymmetricExceptWith([two, three, four]);
		Utils.Assert(StructSetOne.SetEquals([one, four]));
		StructSetOne.CopyFrom(new HashSet<SsBindingTestBlittableStruct> { one, two, three });
		StructSetOne.UnionWith([two, three, four]);
		Utils.Assert(StructSetOne.SetEquals([one, two, three, four]));

		// Test set assignment and equality
		StructSetOne.CopyFrom(new HashSet<SsBindingTestBlittableStruct> { four, five, six });
		Utils.Assert(StructSetOne.SetEquals([four, five, six]));
		StructSetTwo.CopyFrom(StructSetOne);
		Utils.Assert(StructSetTwo.SetEquals(StructSetOne));
	}
}
