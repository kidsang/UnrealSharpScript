using static UnrealEngine.Globals;
using static SharpScriptUnitTest.Tests.MapTest;
using SharpScript;
using SharpScript.Interop;
using SharpScriptUnitTest.Types;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test the properties and methods of statically exported UStruct.
/// </summary>
[RecordFilePath]
[NativeCallbacks]
public unsafe class WrapperStructTest : IUnitTestInterface
{
	public bool RunTest()
	{
		IntPtr testStructDefaultNativePtr = 0;
		IntPtr testStructCustomNativePtr = 0;
		try
		{
			// Create a default struct, test if the struct reference and struct copy have equal values.
			testStructDefaultNativePtr = CreateNativeTestStructDefault();
			SsTestStructNativeRef testStructDefaultNativeRef = new SsTestStructNativeRef(testStructDefaultNativePtr);
			SsTestStruct testStructDefault = testStructDefaultNativeRef;
			TestFieldEquality(testStructDefaultNativeRef, testStructDefault);

			// Test if the values of the default struct meet expectations.
			Utils.Assert(!testStructDefault.Bool);
			Utils.Assert(!testStructDefault.BitfieldBoolA);
			Utils.Assert(!testStructDefault.BitfieldBoolB);
			Utils.Assert(testStructDefault.Int == 0);
			Utils.Assert(testStructDefault.Float == 0);
			Utils.Assert(testStructDefault.Enum == 0);
			Utils.Assert(testStructDefault.LongEnum == 0);
			Utils.Assert(testStructDefault.String == string.Empty);
			Utils.Assert(testStructDefault.Name == Name.None);
			Utils.Assert(testStructDefault.Text == string.Empty);
			Utils.Assert(testStructDefault.FieldPath.Path == string.Empty);
			Utils.Assert(testStructDefault.StructFieldPath.Path == string.Empty);
			Utils.Assert(testStructDefault.StringArray.SequenceEqual([]));
			Utils.Assert(testStructDefault.StringSet.SequenceEqual([]));
			Utils.Assert(DictEquals(testStructDefault.StringIntMap, []));
			Utils.Assert(testStructDefault.Struct.IntArray.SequenceEqual([]));
			Utils.Assert(testStructDefault.Object == null);
			Utils.Assert(testStructDefault.ObjectPtr == null);
			Utils.Assert(testStructDefault.SoftObjectPtr == null);
			Utils.Assert(testStructDefault.WeakObjectPtr == null);
			Utils.Assert(testStructDefault.LazyObjectPtr == null);
			Utils.Assert(testStructDefault.Class == null);
			Utils.Assert(testStructDefault.ClassPtr == null);
			Utils.Assert(testStructDefault.SoftClassPtr == null);
			Utils.Assert(testStructDefault.Interface == null);

			// Create a struct filled with custom data, test if the struct reference and struct copy have equal values.
			testStructCustomNativePtr = CreateNativeTestStructCustom();
			SsTestStructNativeRef testStructCustomNativeRef = new SsTestStructNativeRef(testStructCustomNativePtr);
			SsTestObject objValue = NewObject<SsTestObject>();
			testStructCustomNativeRef.Object = objValue;
			testStructCustomNativeRef.ObjectPtr = objValue;
			testStructCustomNativeRef.SoftObjectPtr = objValue;
			testStructCustomNativeRef.WeakObjectPtr = objValue;
			testStructCustomNativeRef.LazyObjectPtr = objValue;
			Class clsValue = SsTestObject.StaticClass!;
			testStructCustomNativeRef.Class = clsValue;
			testStructCustomNativeRef.ClassPtr = clsValue;
			testStructCustomNativeRef.SoftClassPtr = clsValue;
			testStructCustomNativeRef.Interface = objValue;
			SsTestStruct testStructCustom = testStructCustomNativeRef;
			TestFieldEquality(testStructCustomNativeRef, testStructCustom);

			// Test if the values of the struct filled with custom data meet expectations.
			Utils.Assert(testStructCustom.Bool);
			Utils.Assert(testStructCustom.BitfieldBoolA);
			Utils.Assert(!testStructCustom.BitfieldBoolB);
			Utils.Assert(testStructCustom.Int == 123);
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			Utils.Assert(testStructCustom.Float == 2);
			Utils.Assert(testStructCustom.Enum == ESsTestEnum.Two);
			Utils.Assert(testStructCustom.LongEnum == ESsTestLongEnum.Two);
			Utils.Assert(testStructCustom.String == "String");
			Utils.Assert(testStructCustom.Name == "Name");
			Utils.Assert(testStructCustom.Text == "Text");
			Utils.Assert(testStructCustom.FieldPath.Path == "/Script/SharpScriptUnitTest.SsTestStruct:StringArray");
			Utils.Assert(testStructCustom.StructFieldPath.Path == "/Script/SharpScriptUnitTest.SsTestObject:Struct");
			Utils.Assert(testStructCustom.StringArray.SequenceEqual(["String", "Array"]));
			Utils.Assert(testStructCustom.StringSet.SetEquals(["String", "Set"]));
			Dictionary<string, int> testDict = new() { { "A", 1 }, { "B", 2 } };
			Utils.Assert(DictEquals(testStructCustom.StringIntMap, testDict));
			Utils.Assert(testStructCustom.Struct.IntArray.SequenceEqual([1, 2, 3]));
			Utils.Assert(testStructCustom.Object == objValue);
			Utils.Assert(testStructCustom.ObjectPtr == objValue);
			Utils.Assert(testStructCustom.SoftObjectPtr == objValue);
			Utils.Assert(testStructCustom.WeakObjectPtr == objValue);
			Utils.Assert(testStructCustom.LazyObjectPtr == objValue);
			Utils.Assert(testStructCustom.Class == clsValue);
			Utils.Assert(testStructCustom.ClassPtr == clsValue);
			Utils.Assert(testStructCustom.SoftClassPtr == clsValue);
			Utils.Assert(testStructCustom.Interface == objValue);

			// Fill the values of the C# struct back into the C++ struct, and verify equality.
			testStructDefaultNativeRef.FromManaged(testStructCustom);
			TestFieldEquality(testStructDefaultNativeRef, testStructCustom);
		}
		finally
		{
			if (testStructDefaultNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsTestStructNativeRef.NativeType, ref testStructDefaultNativePtr);
			}

			if (testStructCustomNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(SsTestStructNativeRef.NativeType, ref testStructCustomNativePtr);
			}
		}

		return true;
	}

	/// <summary>
	/// Test if the struct reference and its copy have equal values.
	/// </summary>
	private void TestFieldEquality(in SsTestStructNativeRef structRef, in SsTestStruct structVal)
	{
		Utils.Assert(structRef.Bool == structVal.Bool);
		Utils.Assert(structRef.BitfieldBoolA == structVal.BitfieldBoolA);
		Utils.Assert(structRef.BitfieldBoolB == structVal.BitfieldBoolB);
		Utils.Assert(structRef.Int == structVal.Int);
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		Utils.Assert(structRef.Float == structVal.Float);
		Utils.Assert(structRef.Enum == structVal.Enum);
		Utils.Assert(structRef.LongEnum == structVal.LongEnum);
		Utils.Assert(structRef.String == structVal.String);
		Utils.Assert(structRef.Name == structVal.Name);
		Utils.Assert(structRef.Text == structVal.Text);
		Utils.Assert(structRef.FieldPath.Path == structVal.FieldPath.Path);
		Utils.Assert(structRef.StructFieldPath.Path == structVal.StructFieldPath.Path);
		Utils.Assert(structRef.StringArray.SequenceEqual(structVal.StringArray));
		Utils.Assert(structRef.StringSet.SetEquals(structVal.StringSet));
		Utils.Assert(DictEquals(structRef.StringIntMap, structVal.StringIntMap));
		Utils.Assert(structRef.Struct.IntArray.SequenceEqual(structVal.Struct.IntArray));
		Utils.Assert(structRef.Object == structVal.Object);
		Utils.Assert(structRef.ObjectPtr == structVal.ObjectPtr);
		Utils.Assert(structRef.SoftObjectPtr == structVal.SoftObjectPtr);
		Utils.Assert(structRef.WeakObjectPtr == structVal.WeakObjectPtr);
		Utils.Assert(structRef.LazyObjectPtr == structVal.LazyObjectPtr);
		Utils.Assert(structRef.Class == structVal.Class);
		Utils.Assert(structRef.ClassPtr == structVal.ClassPtr);
		Utils.Assert(structRef.SoftClassPtr == structVal.SoftClassPtr);
		Utils.Assert(structRef.Interface == structVal.Interface);
	}

#pragma warning disable CS0649
	internal static delegate* unmanaged[Cdecl]<IntPtr> CreateNativeTestStructDefault;
	internal static delegate* unmanaged[Cdecl]<IntPtr> CreateNativeTestStructCustom;
#pragma warning restore CS0649
}
