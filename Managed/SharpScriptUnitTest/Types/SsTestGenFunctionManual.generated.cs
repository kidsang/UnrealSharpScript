#nullable enable
using System.Runtime.InteropServices;
using SharpScript;
using SharpScript.Interop;
using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public partial class USsTestGenFunctionManual : IStaticClass<USsTestGenFunctionManual>
{
	public new static TSubclassOf<USsTestGenFunctionManual> StaticClass { get; }

	private new static readonly IntPtr NativeType;
	private static readonly IntPtr FuncInt32_NativeFunc;
	private static readonly int FuncInt32_ParamsSize;
	private static readonly int FuncInt32_InValue_Offset;
	private static readonly int FuncInt32_OutValue_Offset;
	private static readonly int FuncInt32_ReturnValue_Offset;
	private static readonly IntPtr FuncInt64_NativeFunc;
	private static readonly int FuncInt64_ParamsSize;
	private static readonly int FuncInt64_InValue_Offset;
	private static readonly int FuncInt64_OutValue_Offset;
	private static readonly int FuncInt64_ReturnValue_Offset;
	private static readonly IntPtr FuncFloat_NativeFunc;
	private static readonly int FuncFloat_ParamsSize;
	private static readonly int FuncFloat_InValue_Offset;
	private static readonly int FuncFloat_OutValue_Offset;
	private static readonly int FuncFloat_ReturnValue_Offset;
	private static readonly IntPtr FuncDouble_NativeFunc;
	private static readonly int FuncDouble_ParamsSize;
	private static readonly int FuncDouble_InValue_Offset;
	private static readonly int FuncDouble_OutValue_Offset;
	private static readonly int FuncDouble_ReturnValue_Offset;
	private static readonly IntPtr FuncBool_NativeFunc;
	private static readonly int FuncBool_ParamsSize;
	private static readonly int FuncBool_InValue_Offset;
	private static readonly int FuncBool_OutValue_Offset;
	private static readonly int FuncBool_ReturnValue_Offset;
	private static readonly IntPtr FuncString_NativeFunc;
	private static readonly int FuncString_ParamsSize;
	private static readonly int FuncString_InValue_Offset;
	private static readonly int FuncString_OutValue_Offset;
	private static readonly int FuncString_ReturnValue_Offset;
	private static readonly IntPtr FuncName_NativeFunc;
	private static readonly int FuncName_ParamsSize;
	private static readonly int FuncName_InValue_Offset;
	private static readonly int FuncName_OutValue_Offset;
	private static readonly int FuncName_ReturnValue_Offset;
	private static readonly IntPtr FuncText_NativeFunc;
	private static readonly int FuncText_ParamsSize;
	private static readonly int FuncText_InValue_Offset;
	private static readonly int FuncText_OutValue_Offset;
	private static readonly int FuncText_ReturnValue_Offset;
	private static readonly IntPtr FuncEnum_NativeFunc;
	private static readonly int FuncEnum_ParamsSize;
	private static readonly int FuncEnum_InValue_Offset;
	private static readonly int FuncEnum_OutValue_Offset;
	private static readonly int FuncEnum_ReturnValue_Offset;
	private static readonly IntPtr FuncStringArray_NativeFunc;
	private static readonly int FuncStringArray_ParamsSize;
	private static readonly int FuncStringArray_InValue_Offset;
	private static readonly IntPtr FuncStringArray_InValue_NativeProp;
	private static readonly int FuncStringArray_OutValue_Offset;
	private static readonly IntPtr FuncStringArray_OutValue_NativeProp;
	private static readonly int FuncStringArray_ReturnValue_Offset;
	private static readonly IntPtr FuncStringArray_ReturnValue_NativeProp;
	private static readonly IntPtr FuncStringSet_NativeFunc;
	private static readonly int FuncStringSet_ParamsSize;
	private static readonly int FuncStringSet_InValue_Offset;
	private static readonly IntPtr FuncStringSet_InValue_NativeProp;
	private static readonly int FuncStringSet_OutValue_Offset;
	private static readonly IntPtr FuncStringSet_OutValue_NativeProp;
	private static readonly int FuncStringSet_ReturnValue_Offset;
	private static readonly IntPtr FuncStringSet_ReturnValue_NativeProp;
	private static readonly IntPtr FuncStringIntMap_NativeFunc;
	private static readonly int FuncStringIntMap_ParamsSize;
	private static readonly int FuncStringIntMap_InValue_Offset;
	private static readonly IntPtr FuncStringIntMap_InValue_NativeProp;
	private static readonly int FuncStringIntMap_OutValue_Offset;
	private static readonly IntPtr FuncStringIntMap_OutValue_NativeProp;
	private static readonly int FuncStringIntMap_ReturnValue_Offset;
	private static readonly IntPtr FuncStringIntMap_ReturnValue_NativeProp;
	private static readonly IntPtr FuncStruct_NativeFunc;
	private static readonly int FuncStruct_ParamsSize;
	private static readonly int FuncStruct_InValue_Offset;
	private static readonly int FuncStruct_OutValue_Offset;
	private static readonly int FuncStruct_ReturnValue_Offset;
	private static readonly IntPtr FuncBlittableStruct_NativeFunc;
	private static readonly int FuncBlittableStruct_ParamsSize;
	private static readonly int FuncBlittableStruct_InValue_Offset;
	private static readonly int FuncBlittableStruct_OutValue_Offset;
	private static readonly int FuncBlittableStruct_ReturnValue_Offset;
	private static readonly IntPtr FuncObject_NativeFunc;
	private static readonly int FuncObject_ParamsSize;
	private static readonly int FuncObject_InValue_Offset;
	private static readonly int FuncObject_OutValue_Offset;
	private static readonly int FuncObject_ReturnValue_Offset;
	private static readonly IntPtr FuncSoftObjectPtr_NativeFunc;
	private static readonly int FuncSoftObjectPtr_ParamsSize;
	private static readonly int FuncSoftObjectPtr_InValue_Offset;
	private static readonly int FuncSoftObjectPtr_OutValue_Offset;
	private static readonly int FuncSoftObjectPtr_ReturnValue_Offset;
	private static readonly IntPtr FuncClass_NativeFunc;
	private static readonly int FuncClass_ParamsSize;
	private static readonly int FuncClass_InValue_Offset;
	private static readonly int FuncClass_OutValue_Offset;
	private static readonly int FuncClass_ReturnValue_Offset;
	private static readonly IntPtr FuncSoftClassPtr_NativeFunc;
	private static readonly int FuncSoftClassPtr_ParamsSize;
	private static readonly int FuncSoftClassPtr_InValue_Offset;
	private static readonly int FuncSoftClassPtr_OutValue_Offset;
	private static readonly int FuncSoftClassPtr_ReturnValue_Offset;
	private static readonly IntPtr FuncStaticInt32_NativeFunc;
	private static readonly int FuncStaticInt32_ParamsSize;
	private static readonly int FuncStaticInt32_InValue_Offset;
	private static readonly int FuncStaticInt32_OutValue_Offset;
	private static readonly int FuncStaticInt32_ReturnValue_Offset;
	private static readonly IntPtr FuncStaticString_NativeFunc;
	private static readonly int FuncStaticString_ParamsSize;
	private static readonly int FuncStaticString_InValue_Offset;
	private static readonly int FuncStaticString_OutValue_Offset;
	private static readonly int FuncStaticString_ReturnValue_Offset;

	static unsafe USsTestGenFunctionManual()
	{
		FunctionParamDef[] _funcInt32Params =
		[
			new() { ParamName = "ReturnValue", PropType = UIntProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UIntProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UIntProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcInt64Params =
		[
			new() { ParamName = "ReturnValue", PropType = UInt64Property.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UInt64Property.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UInt64Property.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcFloatParams =
		[
			new() { ParamName = "ReturnValue", PropType = UFloatProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UFloatProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UFloatProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcDoubleParams =
		[
			new() { ParamName = "ReturnValue", PropType = UDoubleProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UDoubleProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UDoubleProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcBoolParams =
		[
			new() { ParamName = "ReturnValue", PropType = UBoolProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UBoolProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UBoolProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcStringParams =
		[
			new() { ParamName = "ReturnValue", PropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcNameParams =
		[
			new() { ParamName = "ReturnValue", PropType = UNameProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UNameProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UNameProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcTextParams =
		[
			new() { ParamName = "ReturnValue", PropType = UTextProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UTextProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UTextProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcEnumParams =
		[
			new() { ParamName = "ReturnValue", PropType = UByteProperty.StaticClass.NativeClass, UnderlyingType = ESsTestGenEnumManualNativeRef.NativeType, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UByteProperty.StaticClass.NativeClass, UnderlyingType = ESsTestGenEnumManualNativeRef.NativeType, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UByteProperty.StaticClass.NativeClass, UnderlyingType = ESsTestGenEnumManualNativeRef.NativeType, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcStringArrayParams =
		[
			new() { ParamName = "ReturnValue", PropType = UArrayProperty.StaticClass.NativeClass, InnerPropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UArrayProperty.StaticClass.NativeClass, InnerPropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UArrayProperty.StaticClass.NativeClass, InnerPropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcStringSetParams =
		[
			new() { ParamName = "ReturnValue", PropType = USetProperty.StaticClass.NativeClass, InnerPropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = USetProperty.StaticClass.NativeClass, InnerPropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = USetProperty.StaticClass.NativeClass, InnerPropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcStringIntMapParams =
		[
			new() { ParamName = "ReturnValue", PropType = UMapProperty.StaticClass.NativeClass, InnerPropType = UIntProperty.StaticClass.NativeClass, KeyPropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UMapProperty.StaticClass.NativeClass, InnerPropType = UIntProperty.StaticClass.NativeClass, KeyPropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UMapProperty.StaticClass.NativeClass, InnerPropType = UIntProperty.StaticClass.NativeClass, KeyPropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcStructParams =
		[
			new() { ParamName = "ReturnValue", PropType = UStructProperty.StaticClass.NativeClass, UnderlyingType = FSsTestGenStructManualNativeRef.NativeType, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UStructProperty.StaticClass.NativeClass, UnderlyingType = FSsTestGenStructManualNativeRef.NativeType, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UStructProperty.StaticClass.NativeClass, UnderlyingType = FSsTestGenStructManualNativeRef.NativeType, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcBlittableStructParams =
		[
			new() { ParamName = "ReturnValue", PropType = UStructProperty.StaticClass.NativeClass, UnderlyingType = FSsTestBlittableGenStructManualNativeRef.NativeType, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UStructProperty.StaticClass.NativeClass, UnderlyingType = FSsTestBlittableGenStructManualNativeRef.NativeType, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UStructProperty.StaticClass.NativeClass, UnderlyingType = FSsTestBlittableGenStructManualNativeRef.NativeType, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcObjectParams =
		[
			new() { ParamName = "ReturnValue", PropType = UObjectProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UObjectProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UObjectProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcSoftObjectPtrParams =
		[
			new() { ParamName = "ReturnValue", PropType = USoftObjectProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = USoftObjectProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = USoftObjectProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcClassParams =
		[
			new() { ParamName = "ReturnValue", PropType = UClassProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UClassProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UClassProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcSoftClassPtrParams =
		[
			new() { ParamName = "ReturnValue", PropType = USoftClassProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = USoftClassProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = USoftClassProperty.StaticClass.NativeClass, UnderlyingType = UObject.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcStaticInt32Params =
		[
			new() { ParamName = "ReturnValue", PropType = UIntProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UIntProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UIntProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		FunctionParamDef[] _funcStaticStringParams =
		[
			new() { ParamName = "ReturnValue", PropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.ReturnParam },
			new() { ParamName = "OutValue", PropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.OutParam },
			new() { ParamName = "InValue", PropType = UStrProperty.StaticClass.NativeClass, ParamFlags = ParamFlags.InParam },
		];

		fixed (FunctionParamDef* _p0 = _funcInt32Params)
		fixed (FunctionParamDef* _p1 = _funcInt64Params)
		fixed (FunctionParamDef* _p2 = _funcFloatParams)
		fixed (FunctionParamDef* _p3 = _funcDoubleParams)
		fixed (FunctionParamDef* _p4 = _funcBoolParams)
		fixed (FunctionParamDef* _p5 = _funcStringParams)
		fixed (FunctionParamDef* _p6 = _funcNameParams)
		fixed (FunctionParamDef* _p7 = _funcTextParams)
		fixed (FunctionParamDef* _p8 = _funcEnumParams)
		fixed (FunctionParamDef* _p9 = _funcStringArrayParams)
		fixed (FunctionParamDef* _p10 = _funcStringSetParams)
		fixed (FunctionParamDef* _p11 = _funcStringIntMapParams)
		fixed (FunctionParamDef* _p12 = _funcStructParams)
		fixed (FunctionParamDef* _p13 = _funcBlittableStructParams)
		fixed (FunctionParamDef* _p14 = _funcObjectParams)
		fixed (FunctionParamDef* _p15 = _funcSoftObjectPtrParams)
		fixed (FunctionParamDef* _p16 = _funcClassParams)
		fixed (FunctionParamDef* _p17 = _funcSoftClassPtrParams)
		fixed (FunctionParamDef* _p18 = _funcStaticInt32Params)
		fixed (FunctionParamDef* _p19 = _funcStaticStringParams)
		{
			FunctionDef[] _functionDefs =
			[
				new() { FuncName = "FuncInt32", Params = (IntPtr)_p0, ParamCount = _funcInt32Params.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncInt32 },
				new() { FuncName = "FuncInt64", Params = (IntPtr)_p1, ParamCount = _funcInt64Params.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncInt64 },
				new() { FuncName = "FuncFloat", Params = (IntPtr)_p2, ParamCount = _funcFloatParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncFloat },
				new() { FuncName = "FuncDouble", Params = (IntPtr)_p3, ParamCount = _funcDoubleParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncDouble },
				new() { FuncName = "FuncBool", Params = (IntPtr)_p4, ParamCount = _funcBoolParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncBool },
				new() { FuncName = "FuncString", Params = (IntPtr)_p5, ParamCount = _funcStringParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncString },
				new() { FuncName = "FuncName", Params = (IntPtr)_p6, ParamCount = _funcNameParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncName },
				new() { FuncName = "FuncText", Params = (IntPtr)_p7, ParamCount = _funcTextParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncText },
				new() { FuncName = "FuncEnum", Params = (IntPtr)_p8, ParamCount = _funcEnumParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncEnum },
				new() { FuncName = "FuncStringArray", Params = (IntPtr)_p9, ParamCount = _funcStringArrayParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncStringArray },
				new() { FuncName = "FuncStringSet", Params = (IntPtr)_p10, ParamCount = _funcStringSetParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncStringSet },
				new() { FuncName = "FuncStringIntMap", Params = (IntPtr)_p11, ParamCount = _funcStringIntMapParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncStringIntMap },
				new() { FuncName = "FuncStruct", Params = (IntPtr)_p12, ParamCount = _funcStructParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncStruct },
				new() { FuncName = "FuncBlittableStruct", Params = (IntPtr)_p13, ParamCount = _funcBlittableStructParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncBlittableStruct },
				new() { FuncName = "FuncObject", Params = (IntPtr)_p14, ParamCount = _funcObjectParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncObject },
				new() { FuncName = "FuncSoftObjectPtr", Params = (IntPtr)_p15, ParamCount = _funcSoftObjectPtrParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncSoftObjectPtr },
				new() { FuncName = "FuncClass", Params = (IntPtr)_p16, ParamCount = _funcClassParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncClass },
				new() { FuncName = "FuncSoftClassPtr", Params = (IntPtr)_p17, ParamCount = _funcSoftClassPtrParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncSoftClassPtr },
				new() { FuncName = "FuncStaticInt32", Params = (IntPtr)_p18, ParamCount = _funcStaticInt32Params.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncStaticInt32, FunctionFlags = SsFunctionFlags.Static },
				new() { FuncName = "FuncStaticString", Params = (IntPtr)_p19, ParamCount = _funcStaticStringParams.Length, ManagedDispatch = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, void>)&Dispatch_FuncStaticString, FunctionFlags = SsFunctionFlags.Static },
			];

			fixed (FunctionDef* _functionDefsPtr = _functionDefs)
			{
				NativeType = SubclassingUtils.GenerateClass(
					RuntimeTypeHandle.ToIntPtr(typeof(USsTestGenFunctionManual).TypeHandle),
					"SsTestGenFunctionManual",
					UObject.StaticClass.NativeClass,
					IntPtr.Zero, 0,
					(IntPtr)_functionDefsPtr, _functionDefs.Length);
			}
		}

		StaticClass = new TSubclassOf<USsTestGenFunctionManual>(NativeType);
		HouseKeeper.AddBindedUnrealClass(StaticClass.Class!, typeof(USsTestGenFunctionManual));

		FuncInt32_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncInt32");
		FuncInt32_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncInt32_NativeFunc);
		PropertyIterator _funcInt32Iter = new(FuncInt32_NativeFunc);
		FuncInt32_InValue_Offset = _funcInt32Iter.FindNextAndGetOffset("InValue");
		FuncInt32_OutValue_Offset = _funcInt32Iter.FindNextAndGetOffset("OutValue");
		FuncInt32_ReturnValue_Offset = _funcInt32Iter.FindNextAndGetOffset("ReturnValue");

		FuncInt64_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncInt64");
		FuncInt64_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncInt64_NativeFunc);
		PropertyIterator _funcInt64Iter = new(FuncInt64_NativeFunc);
		FuncInt64_InValue_Offset = _funcInt64Iter.FindNextAndGetOffset("InValue");
		FuncInt64_OutValue_Offset = _funcInt64Iter.FindNextAndGetOffset("OutValue");
		FuncInt64_ReturnValue_Offset = _funcInt64Iter.FindNextAndGetOffset("ReturnValue");

		FuncFloat_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncFloat");
		FuncFloat_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncFloat_NativeFunc);
		PropertyIterator _funcFloatIter = new(FuncFloat_NativeFunc);
		FuncFloat_InValue_Offset = _funcFloatIter.FindNextAndGetOffset("InValue");
		FuncFloat_OutValue_Offset = _funcFloatIter.FindNextAndGetOffset("OutValue");
		FuncFloat_ReturnValue_Offset = _funcFloatIter.FindNextAndGetOffset("ReturnValue");

		FuncDouble_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncDouble");
		FuncDouble_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncDouble_NativeFunc);
		PropertyIterator _funcDoubleIter = new(FuncDouble_NativeFunc);
		FuncDouble_InValue_Offset = _funcDoubleIter.FindNextAndGetOffset("InValue");
		FuncDouble_OutValue_Offset = _funcDoubleIter.FindNextAndGetOffset("OutValue");
		FuncDouble_ReturnValue_Offset = _funcDoubleIter.FindNextAndGetOffset("ReturnValue");

		FuncBool_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncBool");
		FuncBool_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncBool_NativeFunc);
		PropertyIterator _funcBoolIter = new(FuncBool_NativeFunc);
		FuncBool_InValue_Offset = _funcBoolIter.FindNextAndGetOffset("InValue");
		FuncBool_OutValue_Offset = _funcBoolIter.FindNextAndGetOffset("OutValue");
		FuncBool_ReturnValue_Offset = _funcBoolIter.FindNextAndGetOffset("ReturnValue");

		FuncString_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncString");
		FuncString_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncString_NativeFunc);
		PropertyIterator _funcStringIter = new(FuncString_NativeFunc);
		FuncString_InValue_Offset = _funcStringIter.FindNextAndGetOffset("InValue");
		FuncString_OutValue_Offset = _funcStringIter.FindNextAndGetOffset("OutValue");
		FuncString_ReturnValue_Offset = _funcStringIter.FindNextAndGetOffset("ReturnValue");

		FuncName_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncName");
		FuncName_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncName_NativeFunc);
		PropertyIterator _funcNameIter = new(FuncName_NativeFunc);
		FuncName_InValue_Offset = _funcNameIter.FindNextAndGetOffset("InValue");
		FuncName_OutValue_Offset = _funcNameIter.FindNextAndGetOffset("OutValue");
		FuncName_ReturnValue_Offset = _funcNameIter.FindNextAndGetOffset("ReturnValue");

		FuncText_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncText");
		FuncText_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncText_NativeFunc);
		PropertyIterator _funcTextIter = new(FuncText_NativeFunc);
		FuncText_InValue_Offset = _funcTextIter.FindNextAndGetOffset("InValue");
		FuncText_OutValue_Offset = _funcTextIter.FindNextAndGetOffset("OutValue");
		FuncText_ReturnValue_Offset = _funcTextIter.FindNextAndGetOffset("ReturnValue");

		FuncEnum_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncEnum");
		FuncEnum_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncEnum_NativeFunc);
		PropertyIterator _funcEnumIter = new(FuncEnum_NativeFunc);
		FuncEnum_InValue_Offset = _funcEnumIter.FindNextAndGetOffset("InValue");
		FuncEnum_OutValue_Offset = _funcEnumIter.FindNextAndGetOffset("OutValue");
		FuncEnum_ReturnValue_Offset = _funcEnumIter.FindNextAndGetOffset("ReturnValue");

		FuncStringArray_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncStringArray");
		FuncStringArray_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncStringArray_NativeFunc);
		PropertyIterator _funcStringArrayIter = new(FuncStringArray_NativeFunc);
		FuncStringArray_InValue_NativeProp = _funcStringArrayIter.FindNext("InValue");
		FuncStringArray_InValue_Offset = TypeInterop.GetPropertyOffset(FuncStringArray_InValue_NativeProp);
		FuncStringArray_OutValue_NativeProp = _funcStringArrayIter.FindNext("OutValue");
		FuncStringArray_OutValue_Offset = TypeInterop.GetPropertyOffset(FuncStringArray_OutValue_NativeProp);
		FuncStringArray_ReturnValue_NativeProp = _funcStringArrayIter.FindNext("ReturnValue");
		FuncStringArray_ReturnValue_Offset = TypeInterop.GetPropertyOffset(FuncStringArray_ReturnValue_NativeProp);

		FuncStringSet_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncStringSet");
		FuncStringSet_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncStringSet_NativeFunc);
		PropertyIterator _funcStringSetIter = new(FuncStringSet_NativeFunc);
		FuncStringSet_InValue_NativeProp = _funcStringSetIter.FindNext("InValue");
		FuncStringSet_InValue_Offset = TypeInterop.GetPropertyOffset(FuncStringSet_InValue_NativeProp);
		FuncStringSet_OutValue_NativeProp = _funcStringSetIter.FindNext("OutValue");
		FuncStringSet_OutValue_Offset = TypeInterop.GetPropertyOffset(FuncStringSet_OutValue_NativeProp);
		FuncStringSet_ReturnValue_NativeProp = _funcStringSetIter.FindNext("ReturnValue");
		FuncStringSet_ReturnValue_Offset = TypeInterop.GetPropertyOffset(FuncStringSet_ReturnValue_NativeProp);

		FuncStringIntMap_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncStringIntMap");
		FuncStringIntMap_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncStringIntMap_NativeFunc);
		PropertyIterator _funcStringIntMapIter = new(FuncStringIntMap_NativeFunc);
		FuncStringIntMap_InValue_NativeProp = _funcStringIntMapIter.FindNext("InValue");
		FuncStringIntMap_InValue_Offset = TypeInterop.GetPropertyOffset(FuncStringIntMap_InValue_NativeProp);
		FuncStringIntMap_OutValue_NativeProp = _funcStringIntMapIter.FindNext("OutValue");
		FuncStringIntMap_OutValue_Offset = TypeInterop.GetPropertyOffset(FuncStringIntMap_OutValue_NativeProp);
		FuncStringIntMap_ReturnValue_NativeProp = _funcStringIntMapIter.FindNext("ReturnValue");
		FuncStringIntMap_ReturnValue_Offset = TypeInterop.GetPropertyOffset(FuncStringIntMap_ReturnValue_NativeProp);

		FuncStruct_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncStruct");
		FuncStruct_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncStruct_NativeFunc);
		PropertyIterator _funcStructIter = new(FuncStruct_NativeFunc);
		FuncStruct_InValue_Offset = _funcStructIter.FindNextAndGetOffset("InValue");
		FuncStruct_OutValue_Offset = _funcStructIter.FindNextAndGetOffset("OutValue");
		FuncStruct_ReturnValue_Offset = _funcStructIter.FindNextAndGetOffset("ReturnValue");

		FuncBlittableStruct_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncBlittableStruct");
		FuncBlittableStruct_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncBlittableStruct_NativeFunc);
		PropertyIterator _funcBlittableStructIter = new(FuncBlittableStruct_NativeFunc);
		FuncBlittableStruct_InValue_Offset = _funcBlittableStructIter.FindNextAndGetOffset("InValue");
		FuncBlittableStruct_OutValue_Offset = _funcBlittableStructIter.FindNextAndGetOffset("OutValue");
		FuncBlittableStruct_ReturnValue_Offset = _funcBlittableStructIter.FindNextAndGetOffset("ReturnValue");

		FuncObject_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncObject");
		FuncObject_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncObject_NativeFunc);
		PropertyIterator _funcObjectIter = new(FuncObject_NativeFunc);
		FuncObject_InValue_Offset = _funcObjectIter.FindNextAndGetOffset("InValue");
		FuncObject_OutValue_Offset = _funcObjectIter.FindNextAndGetOffset("OutValue");
		FuncObject_ReturnValue_Offset = _funcObjectIter.FindNextAndGetOffset("ReturnValue");

		FuncSoftObjectPtr_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncSoftObjectPtr");
		FuncSoftObjectPtr_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncSoftObjectPtr_NativeFunc);
		PropertyIterator _funcSoftObjectPtrIter = new(FuncSoftObjectPtr_NativeFunc);
		FuncSoftObjectPtr_InValue_Offset = _funcSoftObjectPtrIter.FindNextAndGetOffset("InValue");
		FuncSoftObjectPtr_OutValue_Offset = _funcSoftObjectPtrIter.FindNextAndGetOffset("OutValue");
		FuncSoftObjectPtr_ReturnValue_Offset = _funcSoftObjectPtrIter.FindNextAndGetOffset("ReturnValue");

		FuncClass_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncClass");
		FuncClass_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncClass_NativeFunc);
		PropertyIterator _funcClassIter = new(FuncClass_NativeFunc);
		FuncClass_InValue_Offset = _funcClassIter.FindNextAndGetOffset("InValue");
		FuncClass_OutValue_Offset = _funcClassIter.FindNextAndGetOffset("OutValue");
		FuncClass_ReturnValue_Offset = _funcClassIter.FindNextAndGetOffset("ReturnValue");

		FuncSoftClassPtr_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncSoftClassPtr");
		FuncSoftClassPtr_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncSoftClassPtr_NativeFunc);
		PropertyIterator _funcSoftClassPtrIter = new(FuncSoftClassPtr_NativeFunc);
		FuncSoftClassPtr_InValue_Offset = _funcSoftClassPtrIter.FindNextAndGetOffset("InValue");
		FuncSoftClassPtr_OutValue_Offset = _funcSoftClassPtrIter.FindNextAndGetOffset("OutValue");
		FuncSoftClassPtr_ReturnValue_Offset = _funcSoftClassPtrIter.FindNextAndGetOffset("ReturnValue");

		FuncStaticInt32_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncStaticInt32");
		FuncStaticInt32_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncStaticInt32_NativeFunc);
		PropertyIterator _funcStaticInt32Iter = new(FuncStaticInt32_NativeFunc);
		FuncStaticInt32_InValue_Offset = _funcStaticInt32Iter.FindNextAndGetOffset("InValue");
		FuncStaticInt32_OutValue_Offset = _funcStaticInt32Iter.FindNextAndGetOffset("OutValue");
		FuncStaticInt32_ReturnValue_Offset = _funcStaticInt32Iter.FindNextAndGetOffset("ReturnValue");

		FuncStaticString_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncStaticString");
		FuncStaticString_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncStaticString_NativeFunc);
		PropertyIterator _funcStaticStringIter = new(FuncStaticString_NativeFunc);
		FuncStaticString_InValue_Offset = _funcStaticStringIter.FindNextAndGetOffset("InValue");
		FuncStaticString_OutValue_Offset = _funcStaticStringIter.FindNextAndGetOffset("OutValue");
		FuncStaticString_ReturnValue_Offset = _funcStaticStringIter.FindNextAndGetOffset("ReturnValue");
	}

	// ------------------------------------------------------------------
	// Native dispatch stubs (UE -> C#).
	// ------------------------------------------------------------------

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncInt32(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		int inValue = BlittableMarshaller<int>.FromNative(paramsBuffer + FuncInt32_InValue_Offset);
		int returnValue = self.FuncInt32(inValue, out int outValue);
		BlittableMarshaller<int>.ToNative(paramsBuffer + FuncInt32_OutValue_Offset, outValue);
		BlittableMarshaller<int>.ToNative(paramsBuffer + FuncInt32_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncInt64(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		long inValue = BlittableMarshaller<long>.FromNative(paramsBuffer + FuncInt64_InValue_Offset);
		long returnValue = self.FuncInt64(inValue, out long outValue);
		BlittableMarshaller<long>.ToNative(paramsBuffer + FuncInt64_OutValue_Offset, outValue);
		BlittableMarshaller<long>.ToNative(paramsBuffer + FuncInt64_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncFloat(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		float inValue = BlittableMarshaller<float>.FromNative(paramsBuffer + FuncFloat_InValue_Offset);
		float returnValue = self.FuncFloat(inValue, out float outValue);
		BlittableMarshaller<float>.ToNative(paramsBuffer + FuncFloat_OutValue_Offset, outValue);
		BlittableMarshaller<float>.ToNative(paramsBuffer + FuncFloat_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncDouble(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		double inValue = BlittableMarshaller<double>.FromNative(paramsBuffer + FuncDouble_InValue_Offset);
		double returnValue = self.FuncDouble(inValue, out double outValue);
		BlittableMarshaller<double>.ToNative(paramsBuffer + FuncDouble_OutValue_Offset, outValue);
		BlittableMarshaller<double>.ToNative(paramsBuffer + FuncDouble_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncBool(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		bool inValue = BoolMarshaller.FromNative(paramsBuffer + FuncBool_InValue_Offset);
		bool returnValue = self.FuncBool(inValue, out bool outValue);
		BoolMarshaller.ToNative(paramsBuffer + FuncBool_OutValue_Offset, outValue);
		BoolMarshaller.ToNative(paramsBuffer + FuncBool_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncString(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		string inValue = StringMarshaller.FromNative(paramsBuffer + FuncString_InValue_Offset);
		string returnValue = self.FuncString(inValue, out string outValue);
		StringMarshaller.ToNative(paramsBuffer + FuncString_OutValue_Offset, outValue);
		StringMarshaller.ToNative(paramsBuffer + FuncString_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncName(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		FName inValue = NameMarshaller.FromNative(paramsBuffer + FuncName_InValue_Offset);
		FName returnValue = self.FuncName(inValue, out FName outValue);
		NameMarshaller.ToNative(paramsBuffer + FuncName_OutValue_Offset, outValue);
		NameMarshaller.ToNative(paramsBuffer + FuncName_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncText(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		FText inValue = TextMarshaller.FromNative(paramsBuffer + FuncText_InValue_Offset);
		FText returnValue = self.FuncText(inValue, out FText outValue);
		TextMarshaller.ToNative(paramsBuffer + FuncText_OutValue_Offset, outValue);
		TextMarshaller.ToNative(paramsBuffer + FuncText_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncEnum(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		ESsTestGenEnumManual inValue = EnumMarshaller<ESsTestGenEnumManual>.FromNative(paramsBuffer + FuncEnum_InValue_Offset);
		ESsTestGenEnumManual returnValue = self.FuncEnum(inValue, out ESsTestGenEnumManual outValue);
		EnumMarshaller<ESsTestGenEnumManual>.ToNative(paramsBuffer + FuncEnum_OutValue_Offset, outValue);
		EnumMarshaller<ESsTestGenEnumManual>.ToNative(paramsBuffer + FuncEnum_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncStringArray(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		List<string> inValue = new TArray<string>(paramsBuffer + FuncStringArray_InValue_Offset, FuncStringArray_InValue_NativeProp, StringMarshaller.Instance);
		List<string> returnValue = self.FuncStringArray(inValue, out List<string> outValue);
		new TArray<string>(paramsBuffer + FuncStringArray_OutValue_Offset, FuncStringArray_OutValue_NativeProp, StringMarshaller.Instance).CopyFrom(outValue);
		new TArray<string>(paramsBuffer + FuncStringArray_ReturnValue_Offset, FuncStringArray_ReturnValue_NativeProp, StringMarshaller.Instance).CopyFrom(returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncStringSet(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		HashSet<string> inValue = new TSet<string>(paramsBuffer + FuncStringSet_InValue_Offset, FuncStringSet_InValue_NativeProp, StringMarshaller.Instance);
		HashSet<string> returnValue = self.FuncStringSet(inValue, out HashSet<string> outValue);
		new TSet<string>(paramsBuffer + FuncStringSet_OutValue_Offset, FuncStringSet_OutValue_NativeProp, StringMarshaller.Instance).CopyFrom(outValue);
		new TSet<string>(paramsBuffer + FuncStringSet_ReturnValue_Offset, FuncStringSet_ReturnValue_NativeProp, StringMarshaller.Instance).CopyFrom(returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncStringIntMap(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		Dictionary<string, int> inValue = new TMap<string, int>(paramsBuffer + FuncStringIntMap_InValue_Offset, FuncStringIntMap_InValue_NativeProp, StringMarshaller.Instance, BlittableMarshaller<int>.Instance);
		Dictionary<string, int> returnValue = self.FuncStringIntMap(inValue, out Dictionary<string, int> outValue);
		new TMap<string, int>(paramsBuffer + FuncStringIntMap_OutValue_Offset, FuncStringIntMap_OutValue_NativeProp, StringMarshaller.Instance, BlittableMarshaller<int>.Instance).CopyFrom(outValue);
		new TMap<string, int>(paramsBuffer + FuncStringIntMap_ReturnValue_Offset, FuncStringIntMap_ReturnValue_NativeProp, StringMarshaller.Instance, BlittableMarshaller<int>.Instance).CopyFrom(returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncStruct(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		FSsTestGenStructManual inValue = new FSsTestGenStructManualNativeRef(paramsBuffer + FuncStruct_InValue_Offset).ToManaged();
		FSsTestGenStructManual returnValue = self.FuncStruct(inValue, out FSsTestGenStructManual outValue);
		new FSsTestGenStructManualNativeRef(paramsBuffer + FuncStruct_OutValue_Offset).FromManaged(outValue);
		new FSsTestGenStructManualNativeRef(paramsBuffer + FuncStruct_ReturnValue_Offset).FromManaged(returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncBlittableStruct(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		FSsTestBlittableGenStructManual inValue = BlittableMarshaller<FSsTestBlittableGenStructManual>.FromNative(paramsBuffer + FuncBlittableStruct_InValue_Offset);
		FSsTestBlittableGenStructManual returnValue = self.FuncBlittableStruct(inValue, out FSsTestBlittableGenStructManual outValue);
		BlittableMarshaller<FSsTestBlittableGenStructManual>.ToNative(paramsBuffer + FuncBlittableStruct_OutValue_Offset, outValue);
		BlittableMarshaller<FSsTestBlittableGenStructManual>.ToNative(paramsBuffer + FuncBlittableStruct_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncObject(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		UObject? inValue = ObjectMarshaller<UObject>.FromNative(paramsBuffer + FuncObject_InValue_Offset);
		UObject? returnValue = self.FuncObject(inValue, out UObject? outValue);
		ObjectMarshaller<UObject>.ToNative(paramsBuffer + FuncObject_OutValue_Offset, outValue);
		ObjectMarshaller<UObject>.ToNative(paramsBuffer + FuncObject_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncSoftObjectPtr(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		TSoftObjectPtr<UObject> inValue = SoftObjectPtrMarshaller<UObject>.FromNative(paramsBuffer + FuncSoftObjectPtr_InValue_Offset);
		TSoftObjectPtr<UObject> returnValue = self.FuncSoftObjectPtr(inValue, out TSoftObjectPtr<UObject> outValue);
		SoftObjectPtrMarshaller<UObject>.ToNative(paramsBuffer + FuncSoftObjectPtr_OutValue_Offset, outValue);
		SoftObjectPtrMarshaller<UObject>.ToNative(paramsBuffer + FuncSoftObjectPtr_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncClass(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		TSubclassOf<UObject> inValue = SubclassOfMarshaller<UObject>.FromNative(paramsBuffer + FuncClass_InValue_Offset);
		TSubclassOf<UObject> returnValue = self.FuncClass(inValue, out TSubclassOf<UObject> outValue);
		SubclassOfMarshaller<UObject>.ToNative(paramsBuffer + FuncClass_OutValue_Offset, outValue);
		SubclassOfMarshaller<UObject>.ToNative(paramsBuffer + FuncClass_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncSoftClassPtr(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		var self = SubclassingUtils.ResolveManagedObject<USsTestGenFunctionManual>(objectHandle);
		TSoftClassPtr<UObject> inValue = SoftClassPtrMarshaller<UObject>.FromNative(paramsBuffer + FuncSoftClassPtr_InValue_Offset);
		TSoftClassPtr<UObject> returnValue = self.FuncSoftClassPtr(inValue, out TSoftClassPtr<UObject> outValue);
		SoftClassPtrMarshaller<UObject>.ToNative(paramsBuffer + FuncSoftClassPtr_OutValue_Offset, outValue);
		SoftClassPtrMarshaller<UObject>.ToNative(paramsBuffer + FuncSoftClassPtr_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncStaticInt32(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		int inValue = BlittableMarshaller<int>.FromNative(paramsBuffer + FuncStaticInt32_InValue_Offset);
		int returnValue = FuncStaticInt32(inValue, out int outValue);
		BlittableMarshaller<int>.ToNative(paramsBuffer + FuncStaticInt32_OutValue_Offset, outValue);
		BlittableMarshaller<int>.ToNative(paramsBuffer + FuncStaticInt32_ReturnValue_Offset, returnValue);
	}

	[UnmanagedCallersOnly]
	private static void Dispatch_FuncStaticString(IntPtr objectHandle, IntPtr paramsBuffer)
	{
		string inValue = StringMarshaller.FromNative(paramsBuffer + FuncStaticString_InValue_Offset);
		string returnValue = FuncStaticString(inValue, out string outValue);
		StringMarshaller.ToNative(paramsBuffer + FuncStaticString_OutValue_Offset, outValue);
		StringMarshaller.ToNative(paramsBuffer + FuncStaticString_ReturnValue_Offset, returnValue);
	}
}
