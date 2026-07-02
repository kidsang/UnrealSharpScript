using SharpScript.Interop;
using SharpScriptUnitTest.Types;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using static SharpScriptUnitTest.Tests.MapTest;

namespace SharpScriptUnitTest.Tests;

/// <summary>
/// Test binding properties of a C# generated struct whose binding code is produced by the
/// SharpScript source generator (SharpScriptSourceGenerator), as opposed to
/// <see cref="SubclassingStructManualTest"/> which exercises hand-written bindings.
/// The two tests assert the same behaviour to prove the generator output is equivalent.
/// </summary>
[RecordFilePath]
public class SubclassingStructSourceGeneratorTest : IUnitTestInterface
{
	public bool RunTest()
	{
		IntPtr testStructDefaultNativePtr = 0;
		IntPtr testStructCustomNativePtr = 0;
		try
		{
			// Create a default struct, test if the struct reference and struct copy have equal values.
			testStructDefaultNativePtr = TypeInterop.CreateStructInstance(FSsTestGenStructSourceGeneratorNativeRef.NativeType);
			var testStructDefaultNativeRef = new FSsTestGenStructSourceGeneratorNativeRef(testStructDefaultNativePtr);
			FSsTestGenStructSourceGenerator testStructDefault = testStructDefaultNativeRef;
			TestFieldEquality(testStructDefaultNativeRef, testStructDefault);

			// Test if the values of the default struct meet expectations.
			Utils.Assert(!testStructDefault.Bool);
			Utils.Assert(testStructDefault.Int == 0);
			Utils.Assert(testStructDefault.Float == 0);
			Utils.Assert(testStructDefault.Enum == 0);
			Utils.Assert(testStructDefault.String == string.Empty);
			Utils.Assert(testStructDefault.Name == FName.None);
			Utils.Assert(testStructDefault.Text == string.Empty);
			Utils.Assert(testStructDefault.StringArray.SequenceEqual([]));
			Utils.Assert(testStructDefault.StringSet.SequenceEqual([]));
			Utils.Assert(DictEquals(testStructDefault.StringIntMap, []));
			Utils.Assert(testStructDefault.Struct.IntArray.SequenceEqual([]));
			Utils.Assert(testStructDefault.Object == null);
			Utils.Assert(testStructDefault.SoftObjectPtr == null);
			Utils.Assert(testStructDefault.LazyObjectPtr == null);
			Utils.Assert(testStructDefault.Class == null);
			Utils.Assert(testStructDefault.SoftClassPtr == null);
			// Utils.Assert(testStructDefault.Interface == null);
			Utils.Assert(testStructDefault.StructArray.SequenceEqual([]));
			Utils.Assert(DictEquals(testStructDefault.IntStructMap, []));
			Utils.Assert(testStructDefault.BlittableStructArray.SequenceEqual([]));
			Utils.Assert(DictEquals(testStructDefault.IntBlittableStructMap, []));

			// Create a struct filled with custom data, test if the struct reference and struct copy have equal values.
			testStructCustomNativePtr = TypeInterop.CreateStructInstance(FSsTestGenStructSourceGeneratorNativeRef.NativeType);
			var testStructCustomNativeRef = new FSsTestGenStructSourceGeneratorNativeRef(testStructCustomNativePtr);
			var objValue = NewObject<USsTestGenClassManual>();
			testStructCustomNativeRef.Bool = true;
			testStructCustomNativeRef.Int = 123;
			testStructCustomNativeRef.Float = 2;
			testStructCustomNativeRef.Enum = ESsTestGenEnumSourceGenerator.Two;
			testStructCustomNativeRef.String = "String";
			testStructCustomNativeRef.Name = "Name";
			testStructCustomNativeRef.Text = new FText("Text");
			testStructCustomNativeRef.StringArray.CopyFrom(["String", "Array"]);
			testStructCustomNativeRef.StringSet.CopyFrom(["String", "Set"]);
			testStructCustomNativeRef.StringIntMap.CopyFrom(new Dictionary<string, int> { { "A", 1 }, { "B", 2 } });
			testStructCustomNativeRef.Struct.IntArray.CopyFrom([1, 2, 3]);
			testStructCustomNativeRef.Object = objValue;
			testStructCustomNativeRef.SoftObjectPtr = objValue;
			testStructCustomNativeRef.LazyObjectPtr = objValue;
			UClass clsValue = USsTestGenClassManual.StaticClass!;
			testStructCustomNativeRef.Class = clsValue;
			testStructCustomNativeRef.SoftClassPtr = clsValue;
			FSsArrayTestInnerGenStructManual innerA = new() { IntArray = [1, 2, 3] };
			FSsArrayTestInnerGenStructManual innerB = new() { IntArray = [4, 5, 6] };
			testStructCustomNativeRef.StructArray.CopyFrom([innerA, innerB]);
			testStructCustomNativeRef.IntStructMap.CopyFrom(new Dictionary<int, FSsArrayTestInnerGenStructManual> { { 1, innerA }, { 2, innerB } });
			FSsTestBlittableGenStructManual blittableA = new() { X = 1, Y = 2 };
			FSsTestBlittableGenStructManual blittableB = new() { X = 3, Y = 4 };
			testStructCustomNativeRef.BlittableStructArray.CopyFrom([blittableA, blittableB]);
			testStructCustomNativeRef.IntBlittableStructMap.CopyFrom(new Dictionary<int, FSsTestBlittableGenStructManual> { { 1, blittableA }, { 2, blittableB } });
			FSsTestGenStructSourceGenerator testStructCustom = testStructCustomNativeRef;
			TestFieldEquality(testStructCustomNativeRef, testStructCustom);

			// Test if the values of the struct filled with custom data meet expectations.
			Utils.Assert(testStructCustom.Bool);
			Utils.Assert(testStructCustom.Int == 123);
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			Utils.Assert(testStructCustom.Float == 2);
			Utils.Assert(testStructCustom.Enum == ESsTestGenEnumSourceGenerator.Two);
			Utils.Assert(testStructCustom.String == "String");
			Utils.Assert(testStructCustom.Name == "Name");
			Utils.Assert(testStructCustom.Text == "Text");
			Utils.Assert(testStructCustom.StringArray.SequenceEqual(["String", "Array"]));
			Utils.Assert(testStructCustom.StringSet.SetEquals(["String", "Set"]));
			Utils.Assert(DictEquals(testStructCustom.StringIntMap, new() { { "A", 1 }, { "B", 2 } }));
			Utils.Assert(testStructCustom.Struct.IntArray.SequenceEqual([1, 2, 3]));
			Utils.Assert(testStructCustom.Object == objValue);
			Utils.Assert(testStructCustom.SoftObjectPtr == objValue);
			Utils.Assert(testStructCustom.LazyObjectPtr == objValue);
			Utils.Assert(testStructCustom.Class == clsValue);
			Utils.Assert(testStructCustom.SoftClassPtr == clsValue);
			// Utils.Assert(testStructCustom.Interface == objValue);
			Utils.Assert(testStructCustom.StructArray.SequenceEqual([innerA, innerB]));
			Utils.Assert(DictEquals(testStructCustom.IntStructMap, new() { { 1, innerA }, { 2, innerB } }));
			Utils.Assert(testStructCustom.BlittableStructArray.SequenceEqual([blittableA, blittableB]));
			Utils.Assert(DictEquals(testStructCustom.IntBlittableStructMap, new() { { 1, blittableA }, { 2, blittableB } }));

			// Fill values of the C# struct back into the C++ struct, and verify equality.
			testStructDefaultNativeRef.FromManaged(testStructCustom);
			TestFieldEquality(testStructDefaultNativeRef, testStructCustom);
		}
		finally
		{
			if (testStructDefaultNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(FSsTestGenStructSourceGeneratorNativeRef.NativeType, ref testStructDefaultNativePtr);
			}

			if (testStructCustomNativePtr != 0)
			{
				TypeInterop.DestroyStructInstance(FSsTestGenStructSourceGeneratorNativeRef.NativeType, ref testStructCustomNativePtr);
			}
		}

		return true;
	}

	/// <summary>
	///     Test if the struct reference and its copy have equal values.
	/// </summary>
	private void TestFieldEquality(in FSsTestGenStructSourceGeneratorNativeRef structRef, in FSsTestGenStructSourceGenerator structVal)
	{
		Utils.Assert(structRef.Bool == structVal.Bool);
		Utils.Assert(structRef.Int == structVal.Int);
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		Utils.Assert(structRef.Float == structVal.Float);
		Utils.Assert(structRef.Enum == structVal.Enum);
		Utils.Assert(structRef.String == structVal.String);
		Utils.Assert(structRef.Name == structVal.Name);
		Utils.Assert(structRef.Text == structVal.Text);
		Utils.Assert(structRef.StringArray.SequenceEqual(structVal.StringArray));
		Utils.Assert(structRef.StringSet.SetEquals(structVal.StringSet));
		Utils.Assert(DictEquals(structRef.StringIntMap, structVal.StringIntMap));
		Utils.Assert(structRef.Struct.IntArray.SequenceEqual(structVal.Struct.IntArray));
		Utils.Assert(structRef.Object == structVal.Object);
		Utils.Assert(structRef.SoftObjectPtr == structVal.SoftObjectPtr);
		Utils.Assert(structRef.LazyObjectPtr == structVal.LazyObjectPtr);
		Utils.Assert(structRef.Class == structVal.Class);
		Utils.Assert(structRef.SoftClassPtr == structVal.SoftClassPtr);
		// Utils.Assert(structRef.Interface == structVal.Interface);
		Utils.Assert(structRef.StructArray.SequenceEqual(structVal.StructArray));
		Utils.Assert(DictEquals(structRef.IntStructMap, structVal.IntStructMap));
		Utils.Assert(structRef.BlittableStructArray.SequenceEqual(structVal.BlittableStructArray));
		Utils.Assert(DictEquals(structRef.IntBlittableStructMap, structVal.IntBlittableStructMap));
	}
}
