using SharpScript.Interop;
using SharpScriptUnitTest.Types;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types
{
	// ReSharper disable InconsistentNaming
	public partial class USsTestGenFunctionManual
	{
		public unsafe int CallFuncInt32(int InValue, out int OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncInt32_ParamsSize];
			using ScopedFuncParams _params = new(FuncInt32_NativeFunc, _paramsBuffer);
			BlittableMarshaller<int>.ToNative(_params.Buffer + FuncInt32_InValue_Offset, InValue);
			InvokeFunctionCall(FuncInt32_NativeFunc, _params.Buffer);
			OutValue = BlittableMarshaller<int>.FromNative(_params.Buffer + FuncInt32_OutValue_Offset);
			return BlittableMarshaller<int>.FromNative(_params.Buffer + FuncInt32_ReturnValue_Offset);
		}

		public unsafe long CallFuncInt64(long InValue, out long OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncInt64_ParamsSize];
			using ScopedFuncParams _params = new(FuncInt64_NativeFunc, _paramsBuffer);
			BlittableMarshaller<long>.ToNative(_params.Buffer + FuncInt64_InValue_Offset, InValue);
			InvokeFunctionCall(FuncInt64_NativeFunc, _params.Buffer);
			OutValue = BlittableMarshaller<long>.FromNative(_params.Buffer + FuncInt64_OutValue_Offset);
			return BlittableMarshaller<long>.FromNative(_params.Buffer + FuncInt64_ReturnValue_Offset);
		}

		public unsafe float CallFuncFloat(float InValue, out float OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncFloat_ParamsSize];
			using ScopedFuncParams _params = new(FuncFloat_NativeFunc, _paramsBuffer);
			BlittableMarshaller<float>.ToNative(_params.Buffer + FuncFloat_InValue_Offset, InValue);
			InvokeFunctionCall(FuncFloat_NativeFunc, _params.Buffer);
			OutValue = BlittableMarshaller<float>.FromNative(_params.Buffer + FuncFloat_OutValue_Offset);
			return BlittableMarshaller<float>.FromNative(_params.Buffer + FuncFloat_ReturnValue_Offset);
		}

		public unsafe double CallFuncDouble(double InValue, out double OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncDouble_ParamsSize];
			using ScopedFuncParams _params = new(FuncDouble_NativeFunc, _paramsBuffer);
			BlittableMarshaller<double>.ToNative(_params.Buffer + FuncDouble_InValue_Offset, InValue);
			InvokeFunctionCall(FuncDouble_NativeFunc, _params.Buffer);
			OutValue = BlittableMarshaller<double>.FromNative(_params.Buffer + FuncDouble_OutValue_Offset);
			return BlittableMarshaller<double>.FromNative(_params.Buffer + FuncDouble_ReturnValue_Offset);
		}

		public unsafe bool CallFuncBool(bool InValue, out bool OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncBool_ParamsSize];
			using ScopedFuncParams _params = new(FuncBool_NativeFunc, _paramsBuffer);
			BoolMarshaller.ToNative(_params.Buffer + FuncBool_InValue_Offset, InValue);
			InvokeFunctionCall(FuncBool_NativeFunc, _params.Buffer);
			OutValue = BoolMarshaller.FromNative(_params.Buffer + FuncBool_OutValue_Offset);
			return BoolMarshaller.FromNative(_params.Buffer + FuncBool_ReturnValue_Offset);
		}

		public unsafe string CallFuncString(string InValue, out string OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncString_ParamsSize];
			using ScopedFuncParams _params = new(FuncString_NativeFunc, _paramsBuffer);
			StringMarshaller.ToNative(_params.Buffer + FuncString_InValue_Offset, InValue);
			InvokeFunctionCall(FuncString_NativeFunc, _params.Buffer);
			OutValue = StringMarshaller.FromNative(_params.Buffer + FuncString_OutValue_Offset);
			return StringMarshaller.FromNative(_params.Buffer + FuncString_ReturnValue_Offset);
		}

		public unsafe FName CallFuncName(FName InValue, out FName OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncName_ParamsSize];
			using ScopedFuncParams _params = new(FuncName_NativeFunc, _paramsBuffer);
			NameMarshaller.ToNative(_params.Buffer + FuncName_InValue_Offset, InValue);
			InvokeFunctionCall(FuncName_NativeFunc, _params.Buffer);
			OutValue = NameMarshaller.FromNative(_params.Buffer + FuncName_OutValue_Offset);
			return NameMarshaller.FromNative(_params.Buffer + FuncName_ReturnValue_Offset);
		}

		public unsafe FText CallFuncText(FText InValue, out FText OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncText_ParamsSize];
			using ScopedFuncParams _params = new(FuncText_NativeFunc, _paramsBuffer);
			TextMarshaller.ToNative(_params.Buffer + FuncText_InValue_Offset, InValue);
			InvokeFunctionCall(FuncText_NativeFunc, _params.Buffer);
			OutValue = TextMarshaller.FromNative(_params.Buffer + FuncText_OutValue_Offset);
			return TextMarshaller.FromNative(_params.Buffer + FuncText_ReturnValue_Offset);
		}

		public unsafe ESsTestGenEnumManual CallFuncEnum(ESsTestGenEnumManual InValue, out ESsTestGenEnumManual OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncEnum_ParamsSize];
			using ScopedFuncParams _params = new(FuncEnum_NativeFunc, _paramsBuffer);
			EnumMarshaller<ESsTestGenEnumManual>.ToNative(_params.Buffer + FuncEnum_InValue_Offset, InValue);
			InvokeFunctionCall(FuncEnum_NativeFunc, _params.Buffer);
			OutValue = EnumMarshaller<ESsTestGenEnumManual>.FromNative(_params.Buffer + FuncEnum_OutValue_Offset);
			return EnumMarshaller<ESsTestGenEnumManual>.FromNative(_params.Buffer + FuncEnum_ReturnValue_Offset);
		}

		public unsafe List<string> CallFuncStringArray(List<string> InValue, out List<string> OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncStringArray_ParamsSize];
			using ScopedFuncParams _params = new(FuncStringArray_NativeFunc, _paramsBuffer);
			new TArray<string>(_params.Buffer + FuncStringArray_InValue_Offset, FuncStringArray_InValue_NativeProp, StringMarshaller.Instance).CopyFrom(InValue);
			InvokeFunctionCall(FuncStringArray_NativeFunc, _params.Buffer);
			OutValue = new TArray<string>(_params.Buffer + FuncStringArray_OutValue_Offset, FuncStringArray_OutValue_NativeProp, StringMarshaller.Instance);
			return new TArray<string>(_params.Buffer + FuncStringArray_ReturnValue_Offset, FuncStringArray_ReturnValue_NativeProp, StringMarshaller.Instance);
		}

		public unsafe HashSet<string> CallFuncStringSet(HashSet<string> InValue, out HashSet<string> OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncStringSet_ParamsSize];
			using ScopedFuncParams _params = new(FuncStringSet_NativeFunc, _paramsBuffer);
			new TSet<string>(_params.Buffer + FuncStringSet_InValue_Offset, FuncStringSet_InValue_NativeProp, StringMarshaller.Instance).CopyFrom(InValue);
			InvokeFunctionCall(FuncStringSet_NativeFunc, _params.Buffer);
			OutValue = new TSet<string>(_params.Buffer + FuncStringSet_OutValue_Offset, FuncStringSet_OutValue_NativeProp, StringMarshaller.Instance);
			return new TSet<string>(_params.Buffer + FuncStringSet_ReturnValue_Offset, FuncStringSet_ReturnValue_NativeProp, StringMarshaller.Instance);
		}

		public unsafe Dictionary<string, int> CallFuncStringIntMap(Dictionary<string, int> InValue, out Dictionary<string, int> OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncStringIntMap_ParamsSize];
			using ScopedFuncParams _params = new(FuncStringIntMap_NativeFunc, _paramsBuffer);
			new TMap<string, int>(_params.Buffer + FuncStringIntMap_InValue_Offset, FuncStringIntMap_InValue_NativeProp, StringMarshaller.Instance, BlittableMarshaller<int>.Instance).CopyFrom(InValue);
			InvokeFunctionCall(FuncStringIntMap_NativeFunc, _params.Buffer);
			OutValue = new TMap<string, int>(_params.Buffer + FuncStringIntMap_OutValue_Offset, FuncStringIntMap_OutValue_NativeProp, StringMarshaller.Instance, BlittableMarshaller<int>.Instance);
			return new TMap<string, int>(_params.Buffer + FuncStringIntMap_ReturnValue_Offset, FuncStringIntMap_ReturnValue_NativeProp, StringMarshaller.Instance, BlittableMarshaller<int>.Instance);
		}

		public unsafe FSsTestGenStructManual CallFuncStruct(FSsTestGenStructManual InValue, out FSsTestGenStructManual OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncStruct_ParamsSize];
			using ScopedFuncParams _params = new(FuncStruct_NativeFunc, _paramsBuffer);
			new FSsTestGenStructManualNativeRef(_params.Buffer + FuncStruct_InValue_Offset).FromManaged(InValue);
			InvokeFunctionCall(FuncStruct_NativeFunc, _params.Buffer);
			OutValue = new FSsTestGenStructManualNativeRef(_params.Buffer + FuncStruct_OutValue_Offset).ToManaged();
			return new FSsTestGenStructManualNativeRef(_params.Buffer + FuncStruct_ReturnValue_Offset).ToManaged();
		}

		public unsafe FSsTestBlittableGenStructManual CallFuncBlittableStruct(FSsTestBlittableGenStructManual InValue, out FSsTestBlittableGenStructManual OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncBlittableStruct_ParamsSize];
			using ScopedFuncParams _params = new(FuncBlittableStruct_NativeFunc, _paramsBuffer);
			BlittableMarshaller<FSsTestBlittableGenStructManual>.ToNative(_params.Buffer + FuncBlittableStruct_InValue_Offset, InValue);
			InvokeFunctionCall(FuncBlittableStruct_NativeFunc, _params.Buffer);
			OutValue = BlittableMarshaller<FSsTestBlittableGenStructManual>.FromNative(_params.Buffer + FuncBlittableStruct_OutValue_Offset);
			return BlittableMarshaller<FSsTestBlittableGenStructManual>.FromNative(_params.Buffer + FuncBlittableStruct_ReturnValue_Offset);
		}

		public unsafe UObject? CallFuncObject(UObject? InValue, out UObject? OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncObject_ParamsSize];
			using ScopedFuncParams _params = new(FuncObject_NativeFunc, _paramsBuffer);
			ObjectMarshaller<UObject>.ToNative(_params.Buffer + FuncObject_InValue_Offset, InValue);
			InvokeFunctionCall(FuncObject_NativeFunc, _params.Buffer);
			OutValue = ObjectMarshaller<UObject>.FromNative(_params.Buffer + FuncObject_OutValue_Offset);
			return ObjectMarshaller<UObject>.FromNative(_params.Buffer + FuncObject_ReturnValue_Offset);
		}

		public unsafe TSoftObjectPtr<UObject> CallFuncSoftObjectPtr(TSoftObjectPtr<UObject> InValue, out TSoftObjectPtr<UObject> OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncSoftObjectPtr_ParamsSize];
			using ScopedFuncParams _params = new(FuncSoftObjectPtr_NativeFunc, _paramsBuffer);
			SoftObjectPtrMarshaller<UObject>.ToNative(_params.Buffer + FuncSoftObjectPtr_InValue_Offset, InValue);
			InvokeFunctionCall(FuncSoftObjectPtr_NativeFunc, _params.Buffer);
			OutValue = SoftObjectPtrMarshaller<UObject>.FromNative(_params.Buffer + FuncSoftObjectPtr_OutValue_Offset);
			return SoftObjectPtrMarshaller<UObject>.FromNative(_params.Buffer + FuncSoftObjectPtr_ReturnValue_Offset);
		}

		public unsafe TSubclassOf<UObject> CallFuncClass(TSubclassOf<UObject> InValue, out TSubclassOf<UObject> OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncClass_ParamsSize];
			using ScopedFuncParams _params = new(FuncClass_NativeFunc, _paramsBuffer);
			SubclassOfMarshaller<UObject>.ToNative(_params.Buffer + FuncClass_InValue_Offset, InValue);
			InvokeFunctionCall(FuncClass_NativeFunc, _params.Buffer);
			OutValue = SubclassOfMarshaller<UObject>.FromNative(_params.Buffer + FuncClass_OutValue_Offset);
			return SubclassOfMarshaller<UObject>.FromNative(_params.Buffer + FuncClass_ReturnValue_Offset);
		}

		public unsafe TSoftClassPtr<UObject> CallFuncSoftClassPtr(TSoftClassPtr<UObject> InValue, out TSoftClassPtr<UObject> OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncSoftClassPtr_ParamsSize];
			using ScopedFuncParams _params = new(FuncSoftClassPtr_NativeFunc, _paramsBuffer);
			SoftClassPtrMarshaller<UObject>.ToNative(_params.Buffer + FuncSoftClassPtr_InValue_Offset, InValue);
			InvokeFunctionCall(FuncSoftClassPtr_NativeFunc, _params.Buffer);
			OutValue = SoftClassPtrMarshaller<UObject>.FromNative(_params.Buffer + FuncSoftClassPtr_OutValue_Offset);
			return SoftClassPtrMarshaller<UObject>.FromNative(_params.Buffer + FuncSoftClassPtr_ReturnValue_Offset);
		}

		public static unsafe int CallFuncStaticInt32(int InValue, out int OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncStaticInt32_ParamsSize];
			using ScopedFuncParams _params = new(FuncStaticInt32_NativeFunc, _paramsBuffer);
			BlittableMarshaller<int>.ToNative(_params.Buffer + FuncStaticInt32_InValue_Offset, InValue);
			InvokeStaticFunctionCall(NativeType, FuncStaticInt32_NativeFunc, _params.Buffer);
			OutValue = BlittableMarshaller<int>.FromNative(_params.Buffer + FuncStaticInt32_OutValue_Offset);
			return BlittableMarshaller<int>.FromNative(_params.Buffer + FuncStaticInt32_ReturnValue_Offset);
		}

		public static unsafe string CallFuncStaticString(string InValue, out string OutValue)
		{
			byte* _paramsBuffer = stackalloc byte[FuncStaticString_ParamsSize];
			using ScopedFuncParams _params = new(FuncStaticString_NativeFunc, _paramsBuffer);
			StringMarshaller.ToNative(_params.Buffer + FuncStaticString_InValue_Offset, InValue);
			InvokeStaticFunctionCall(NativeType, FuncStaticString_NativeFunc, _params.Buffer);
			OutValue = StringMarshaller.FromNative(_params.Buffer + FuncStaticString_OutValue_Offset);
			return StringMarshaller.FromNative(_params.Buffer + FuncStaticString_ReturnValue_Offset);
		}
	}
	// ReSharper restore InconsistentNaming
}

namespace SharpScriptUnitTest.Tests
{

	/// <summary>
	/// Mirror of <see cref="BindingFunctionTest"/> for the subclassing UFunction path: instead of C#
	/// calling UE-implemented UFunctions, UE invokes C#-implemented UFunctions on the hand-written
	/// subclassing class <see cref="USsTestGenFunctionManual"/>. Each Call* helper drives the UE UFunction
	/// call path (ProcessEvent), which routes into the generated native thunk and back into the C# body.
	///
	/// Functions whose parameter types the subclassing path cannot express are skipped with a comment,
	/// matching the same set skipped by <see cref="BindingFunctionTest"/> plus the subclassing-specific
	/// unsupported types (see USsTestGenFunctionManual for the full list).
	/// </summary>
	[RecordFilePath]
	public class SubclassingFunctionManualTest : IUnitTestInterface
	{
		public bool RunTest()
		{
			USsTestGenFunctionManual TestObj = NewObject<USsTestGenFunctionManual>();

			int inputInt32 = 123;
			Utils.Assert(TestObj.CallFuncInt32(inputInt32, out var outputInt32) == inputInt32);
			Utils.Assert(outputInt32 == inputInt32);

			long inputInt64 = 123;
			Utils.Assert(TestObj.CallFuncInt64(inputInt64, out var outputInt64) == inputInt64);
			Utils.Assert(outputInt64 == inputInt64);

			// ReSharper disable CompareOfFloatsByEqualityOperator
			float inputFloat = 128.0f;
			Utils.Assert(TestObj.CallFuncFloat(inputFloat, out var outputFloat) == inputFloat);
			Utils.Assert(outputFloat == inputFloat);
			// ReSharper restore CompareOfFloatsByEqualityOperator

			// ReSharper disable CompareOfFloatsByEqualityOperator
			double inputDouble = 128.0;
			Utils.Assert(TestObj.CallFuncDouble(inputDouble, out var outputDouble) == inputDouble);
			Utils.Assert(outputDouble == inputDouble);
			// ReSharper restore CompareOfFloatsByEqualityOperator

			bool inputBool = true;
			Utils.Assert(TestObj.CallFuncBool(inputBool, out var outputBool) == inputBool);
			Utils.Assert(outputBool == inputBool);

			string inputString = "123";
			Utils.Assert(TestObj.CallFuncString(inputString, out var outputString) == inputString);
			Utils.Assert(outputString == inputString);

			FName inputName = new("123");
			Utils.Assert(TestObj.CallFuncName(inputName, out var outputName) == inputName);
			Utils.Assert(outputName == inputName);

			FText inputText = new("123");
			Utils.Assert(TestObj.CallFuncText(inputText, out var outputText) == inputText);
			Utils.Assert(outputText == inputText);

			ESsTestGenEnumManual inputEnum = ESsTestGenEnumManual.Two;
			Utils.Assert(TestObj.CallFuncEnum(inputEnum, out var outputEnum) == inputEnum);
			Utils.Assert(outputEnum == inputEnum);

			List<string> inputStringArray = ["123", "456"];
			Utils.Assert(TestObj.CallFuncStringArray(inputStringArray, out var outputStringArray).SequenceEqual(inputStringArray));
			Utils.Assert(outputStringArray.SequenceEqual(inputStringArray));

			HashSet<string> inputStringSet = ["123", "456"];
			Utils.Assert(TestObj.CallFuncStringSet(inputStringSet, out var outputStringSet).SetEquals(inputStringSet));
			Utils.Assert(outputStringSet.SetEquals(inputStringSet));

			Dictionary<string, int> inputStringIntMap = new()
			{
				["123"] = 123,
				["456"] = 456
			};
			Utils.Assert(TestObj.CallFuncStringIntMap(inputStringIntMap, out var outputStringIntMap).DictEquals(inputStringIntMap));
			Utils.Assert(outputStringIntMap.DictEquals(inputStringIntMap));

			FSsTestGenStructManual inputStruct = new()
			{
				Int = 123,
				String = "123",
			};
			Utils.Assert(TestObj.CallFuncStruct(inputStruct, out var outputStruct).Int == inputStruct.Int);
			Utils.Assert(outputStruct.String == inputStruct.String);

			FSsTestBlittableGenStructManual inputBlittableStruct = new()
			{
				X = 10,
				Y = 20,
			};
			Utils.Assert(TestObj.CallFuncBlittableStruct(inputBlittableStruct, out var outputBlittableStruct).X == inputBlittableStruct.X);
			Utils.Assert(outputBlittableStruct.Y == inputBlittableStruct.Y);

			UObject inputObject = NewObject<USsTestGenFunctionManual>();
			Utils.Assert(TestObj.CallFuncObject(inputObject, out var outputObject) == inputObject);
			Utils.Assert(outputObject == inputObject);

			Utils.Assert(TestObj.CallFuncSoftObjectPtr(inputObject, out var outputSoftObjectPtr) == inputObject);
			Utils.Assert(outputSoftObjectPtr == inputObject);

			UClass inputClass = USsTestGenFunctionManual.StaticClass!;
			Utils.Assert(TestObj.CallFuncClass(inputClass, out var outputClass) == inputClass);
			Utils.Assert(outputClass == inputClass);

			Utils.Assert(TestObj.CallFuncSoftClassPtr(inputClass, out var outputSoftClassPtr) == inputClass);
			Utils.Assert(outputSoftClassPtr == inputClass);

			// FuncInterface is skipped: interface properties are not supported by subclassing CreateProperty.

			// FuncDelegate is skipped: delegate properties are not supported by subclassing CreateProperty.

			// FuncGenericRet / FuncGenericOut / FuncGenericRet(Array|Set|Map) are skipped: they rely on
			// DeterminesOutputType metadata and generic templates, unsupported by subclassing.

			int inputStaticInt32 = 789;
			Utils.Assert(USsTestGenFunctionManual.CallFuncStaticInt32(inputStaticInt32, out var outputStaticInt32) == inputStaticInt32);
			Utils.Assert(outputStaticInt32 == inputStaticInt32);

			string inputStaticString = "static-789";
			Utils.Assert(USsTestGenFunctionManual.CallFuncStaticString(inputStaticString, out var outputStaticString) == inputStaticString);
			Utils.Assert(outputStaticString == inputStaticString);

			return true;
		}
	}
}
