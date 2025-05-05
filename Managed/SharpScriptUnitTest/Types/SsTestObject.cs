using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using Object = UnrealEngine.CoreUObject.Object;

namespace SharpScriptUnitTest.Types;

public class SsTestObject : Object, ISsTestChildInterface, ISsTestOtherInterface
{
	public static Class InterfaceClass => null!;

	// ReSharper disable InconsistentNaming
	public new static readonly IntPtr NativeType;
	public new static readonly SubclassOf<SsTestObject> StaticClass;
	internal static readonly IntPtr Bool_NativeProp;
	internal static readonly int Bool_Offset;
	internal static readonly IntPtr BitfieldBoolA_NativeProp;
	internal static readonly int BitfieldBoolA_Offset;
	internal static readonly byte BitfieldBoolA_FieldMask;
	internal static readonly IntPtr BitfieldBoolB_NativeProp;
	internal static readonly int BitfieldBoolB_Offset;
	internal static readonly byte BitfieldBoolB_FieldMask;
	internal static readonly IntPtr Int_NativeProp;
	internal static readonly int Int_Offset;
	internal static readonly IntPtr Float_NativeProp;
	internal static readonly int Float_Offset;
	internal static readonly IntPtr Enum_NativeProp;
	internal static readonly int Enum_Offset;
	internal static readonly IntPtr LongEnum_NativeProp;
	internal static readonly int LongEnum_Offset;
	internal static readonly IntPtr String_NativeProp;
	internal static readonly int String_Offset;
	internal static readonly IntPtr Name_NativeProp;
	internal static readonly int Name_Offset;
	internal static readonly IntPtr Text_NativeProp;
	internal static readonly int Text_Offset;
	internal static readonly IntPtr FieldPath_NativeProp;
	internal static readonly int FieldPath_Offset;
	internal static readonly IntPtr StructFieldPath_NativeProp;
	internal static readonly int StructFieldPath_Offset;
	internal static readonly IntPtr StringArray_NativeProp;
	internal static readonly int StringArray_Offset;
	internal static readonly IntPtr StringSet_NativeProp;
	internal static readonly int StringSet_Offset;
	internal static readonly IntPtr StringIntMap_NativeProp;
	internal static readonly int StringIntMap_Offset;
	internal static readonly IntPtr Delegate_NativeProp;
	internal static readonly int Delegate_Offset;
	internal static readonly IntPtr MulticastDelegate_NativeProp;
	internal static readonly int MulticastDelegate_Offset;
	internal static readonly IntPtr Struct_NativeProp;
	internal static readonly int Struct_Offset;
	internal static readonly IntPtr StructArray_NativeProp;
	internal static readonly int StructArray_Offset;
	internal static readonly IntPtr BlittableStruct_NativeProp;
	internal static readonly int BlittableStruct_Offset;
	internal static readonly IntPtr Object_NativeProp;
	internal static readonly int Object_Offset;
	internal static readonly IntPtr ObjectPtr_NativeProp;
	internal static readonly int ObjectPtr_Offset;
	internal static readonly IntPtr SoftObjectPtr_NativeProp;
	internal static readonly int SoftObjectPtr_Offset;
	internal static readonly IntPtr WeakObjectPtr_NativeProp;
	internal static readonly int WeakObjectPtr_Offset;
	internal static readonly IntPtr LazyObjectPtr_NativeProp;
	internal static readonly int LazyObjectPtr_Offset;
	internal static readonly IntPtr Class_NativeProp;
	internal static readonly int Class_Offset;
	internal static readonly IntPtr ClassPtr_NativeProp;
	internal static readonly int ClassPtr_Offset;
	internal static readonly IntPtr SoftClassPtr_NativeProp;
	internal static readonly int SoftClassPtr_Offset;
	internal static readonly IntPtr Interface_NativeProp;
	internal static readonly int Interface_Offset;
	internal static readonly IntPtr FuncBlueprintImplementable_NativeFunc;
	internal static readonly int FuncBlueprintImplementable_ParamsSize;
	internal static readonly int FuncBlueprintImplementable_InValue_Offset;
	internal static readonly int FuncBlueprintImplementable_ReturnValue_Offset;
	internal static readonly IntPtr FuncBlueprintNative_NativeFunc;
	internal static readonly int FuncBlueprintNative_ParamsSize;
	internal static readonly int FuncBlueprintNative_InValue_Offset;
	internal static readonly int FuncBlueprintNative_ReturnValue_Offset;
	internal static readonly IntPtr FuncBlueprintNativeRef_NativeFunc;
	internal static readonly int FuncBlueprintNativeRef_ParamsSize;
	internal static readonly int FuncBlueprintNativeRef_InOutStruct_Offset;
	internal static readonly IntPtr CallFuncBlueprintImplementable_NativeFunc;
	internal static readonly int CallFuncBlueprintImplementable_ParamsSize;
	internal static readonly int CallFuncBlueprintImplementable_InValue_Offset;
	internal static readonly int CallFuncBlueprintImplementable_ReturnValue_Offset;
	internal static readonly IntPtr CallFuncBlueprintNative_NativeFunc;
	internal static readonly int CallFuncBlueprintNative_ParamsSize;
	internal static readonly int CallFuncBlueprintNative_InValue_Offset;
	internal static readonly int CallFuncBlueprintNative_ReturnValue_Offset;
	internal static readonly IntPtr CallFuncBlueprintNativeRef_NativeFunc;
	internal static readonly int CallFuncBlueprintNativeRef_ParamsSize;
	internal static readonly int CallFuncBlueprintNativeRef_InOutStruct_Offset;
	internal static readonly IntPtr FuncTakingSsTestDelegate_NativeFunc;
	internal static readonly int FuncTakingSsTestDelegate_ParamsSize;
	internal static readonly int FuncTakingSsTestDelegate_InDelegate_Offset;
	internal static readonly int FuncTakingSsTestDelegate_InValue_Offset;
	internal static readonly int FuncTakingSsTestDelegate_ReturnValue_Offset;
	internal static readonly IntPtr FuncTakingFieldPath_NativeFunc;
	internal static readonly int FuncTakingFieldPath_ParamsSize;
	internal static readonly int FuncTakingFieldPath_InFieldPath_Offset;
	internal static readonly IntPtr MulticastDelegatePropertyCallback_NativeFunc;
	internal static readonly int MulticastDelegatePropertyCallback_ParamsSize;
	internal static readonly int MulticastDelegatePropertyCallback_InStr_Offset;
	internal static readonly IntPtr ReturnArray_NativeFunc;
	internal static readonly int ReturnArray_ParamsSize;
	internal static readonly IntPtr ReturnArray_ReturnValue_NativeProp;
	internal static readonly int ReturnArray_ReturnValue_Offset;
	internal static readonly IntPtr ReturnSet_NativeFunc;
	internal static readonly int ReturnSet_ParamsSize;
	internal static readonly IntPtr ReturnSet_ReturnValue_NativeProp;
	internal static readonly int ReturnSet_ReturnValue_Offset;
	internal static readonly IntPtr ReturnMap_NativeFunc;
	internal static readonly int ReturnMap_ParamsSize;
	internal static readonly IntPtr ReturnMap_ReturnValue_NativeProp;
	internal static readonly int ReturnMap_ReturnValue_Offset;
	internal static readonly IntPtr ReturnFieldPath_NativeFunc;
	internal static readonly int ReturnFieldPath_ParamsSize;
	internal static readonly int ReturnFieldPath_ReturnValue_Offset;
	internal static readonly IntPtr SetInt_NativeFunc;
	internal static readonly int SetInt_ParamsSize;
	internal static readonly int SetInt_InValue_Offset;
	internal static readonly IntPtr GetInt_NativeFunc;
	internal static readonly int GetInt_ParamsSize;
	internal static readonly int GetInt_ReturnValue_Offset;
	internal static readonly IntPtr FuncInterface_NativeFunc;
	internal static readonly int FuncInterface_ParamsSize;
	internal static readonly int FuncInterface_InValue_Offset;
	internal static readonly int FuncInterface_ReturnValue_Offset;
	internal static readonly IntPtr FuncInterfaceChild_NativeFunc;
	internal static readonly int FuncInterfaceChild_ParamsSize;
	internal static readonly int FuncInterfaceChild_InValue_Offset;
	internal static readonly int FuncInterfaceChild_ReturnValue_Offset;
	internal static readonly IntPtr FuncInterfaceOther_NativeFunc;
	internal static readonly int FuncInterfaceOther_ParamsSize;
	internal static readonly int FuncInterfaceOther_InValue_Offset;
	internal static readonly int FuncInterfaceOther_ReturnValue_Offset;
	// ReSharper restore InconsistentNaming

	static SsTestObject()
	{
		NativeType = TypeInterop.FindClass("SsTestObject");
		StaticClass = new SubclassOf<SsTestObject>(NativeType);
		PropertyIterator propIter = new PropertyIterator(NativeType);

		Bool_NativeProp = propIter.FindNext("Bool");
		Bool_Offset = TypeInterop.GetPropertyOffset(Bool_NativeProp);

		BitfieldBoolA_NativeProp = propIter.FindNext("BitfieldBoolA");
		BitfieldBoolA_Offset = TypeInterop.GetPropertyOffset(BitfieldBoolA_NativeProp);
		BitfieldBoolA_FieldMask = TypeInterop.GetBoolPropertyFieldMask(BitfieldBoolA_NativeProp);

		BitfieldBoolB_NativeProp = propIter.FindNext("BitfieldBoolB");
		BitfieldBoolB_Offset = TypeInterop.GetPropertyOffset(BitfieldBoolB_NativeProp);
		BitfieldBoolB_FieldMask = TypeInterop.GetBoolPropertyFieldMask(BitfieldBoolB_NativeProp);

		Int_NativeProp = propIter.FindNext("Int");
		Int_Offset = TypeInterop.GetPropertyOffset(Int_NativeProp);

		Float_NativeProp = propIter.FindNext("Float");
		Float_Offset = TypeInterop.GetPropertyOffset(Float_NativeProp);

		Enum_NativeProp = propIter.FindNext("Enum");
		Enum_Offset = TypeInterop.GetPropertyOffset(Enum_NativeProp);

		LongEnum_NativeProp = propIter.FindNext("LongEnum");
		LongEnum_Offset = TypeInterop.GetPropertyOffset(LongEnum_NativeProp);

		String_NativeProp = propIter.FindNext("String");
		String_Offset = TypeInterop.GetPropertyOffset(String_NativeProp);

		Name_NativeProp = propIter.FindNext("Name");
		Name_Offset = TypeInterop.GetPropertyOffset(Name_NativeProp);

		Text_NativeProp = propIter.FindNext("Text");
		Text_Offset = TypeInterop.GetPropertyOffset(Text_NativeProp);

		FieldPath_NativeProp = propIter.FindNext("FieldPath");
		FieldPath_Offset = TypeInterop.GetPropertyOffset(FieldPath_NativeProp);

		StructFieldPath_NativeProp = propIter.FindNext("StructFieldPath");
		StructFieldPath_Offset = TypeInterop.GetPropertyOffset(StructFieldPath_NativeProp);

		StringArray_NativeProp = propIter.FindNext("StringArray");
		StringArray_Offset = TypeInterop.GetPropertyOffset(StringArray_NativeProp);

		StringSet_NativeProp = propIter.FindNext("StringSet");
		StringSet_Offset = TypeInterop.GetPropertyOffset(StringSet_NativeProp);

		StringIntMap_NativeProp = propIter.FindNext("StringIntMap");
		StringIntMap_Offset = TypeInterop.GetPropertyOffset(StringIntMap_NativeProp);

		Delegate_NativeProp = propIter.FindNext("Delegate");
		Delegate_Offset = TypeInterop.GetPropertyOffset(Delegate_NativeProp);
		SsTestDelegateInvoker.Initialize(Delegate_NativeProp);

		MulticastDelegate_NativeProp = propIter.FindNext("MulticastDelegate");
		MulticastDelegate_Offset = TypeInterop.GetPropertyOffset(MulticastDelegate_NativeProp);
		SsTestMulticastDelegateInvoker.Initialize(MulticastDelegate_NativeProp);

		Struct_NativeProp = propIter.FindNext("Struct");
		Struct_Offset = TypeInterop.GetPropertyOffset(Struct_NativeProp);

		StructArray_NativeProp = propIter.FindNext("StructArray");
		StructArray_Offset = TypeInterop.GetPropertyOffset(StructArray_NativeProp);

		BlittableStruct_NativeProp = propIter.FindNext("BlittableStruct");
		BlittableStruct_Offset = TypeInterop.GetPropertyOffset(BlittableStruct_NativeProp);

		Object_NativeProp = propIter.FindNext("Object");
		Object_Offset = TypeInterop.GetPropertyOffset(Object_NativeProp);

		ObjectPtr_NativeProp = propIter.FindNext("ObjectPtr");
		ObjectPtr_Offset = TypeInterop.GetPropertyOffset(ObjectPtr_NativeProp);

		SoftObjectPtr_NativeProp = propIter.FindNext("SoftObjectPtr");
		SoftObjectPtr_Offset = TypeInterop.GetPropertyOffset(SoftObjectPtr_NativeProp);

		WeakObjectPtr_NativeProp = propIter.FindNext("WeakObjectPtr");
		WeakObjectPtr_Offset = TypeInterop.GetPropertyOffset(WeakObjectPtr_NativeProp);

		LazyObjectPtr_NativeProp = propIter.FindNext("LazyObjectPtr");
		LazyObjectPtr_Offset = TypeInterop.GetPropertyOffset(LazyObjectPtr_NativeProp);

		Class_NativeProp = propIter.FindNext("Class");
		Class_Offset = TypeInterop.GetPropertyOffset(Class_NativeProp);

		ClassPtr_NativeProp = propIter.FindNext("ClassPtr");
		ClassPtr_Offset = TypeInterop.GetPropertyOffset(ClassPtr_NativeProp);

		SoftClassPtr_NativeProp = propIter.FindNext("SoftClassPtr");
		SoftClassPtr_Offset = TypeInterop.GetPropertyOffset(SoftClassPtr_NativeProp);

		Interface_NativeProp = propIter.FindNext("Interface");
		Interface_Offset = TypeInterop.GetPropertyOffset(Interface_NativeProp);

		FuncBlueprintImplementable_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncBlueprintImplementable");
		FuncBlueprintImplementable_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncBlueprintImplementable_NativeFunc);
		FuncBlueprintImplementable_InValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncBlueprintImplementable_NativeFunc, "InValue");
		FuncBlueprintImplementable_ReturnValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncBlueprintImplementable_NativeFunc, "ReturnValue");

		FuncBlueprintNative_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncBlueprintNative");
		FuncBlueprintNative_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncBlueprintNative_NativeFunc);
		FuncBlueprintNative_InValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncBlueprintNative_NativeFunc, "InValue");
		FuncBlueprintNative_ReturnValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncBlueprintNative_NativeFunc, "ReturnValue");

		FuncBlueprintNativeRef_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncBlueprintNativeRef");
		FuncBlueprintNativeRef_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncBlueprintNativeRef_NativeFunc);
		FuncBlueprintNativeRef_InOutStruct_Offset = TypeInterop.GetPropertyOffsetFromName(FuncBlueprintNativeRef_NativeFunc, "InOutStruct");

		CallFuncBlueprintImplementable_NativeFunc = TypeInterop.FindFunction(NativeType, "CallFuncBlueprintImplementable");
		CallFuncBlueprintImplementable_ParamsSize = TypeInterop.GetFunctionParamsSize(CallFuncBlueprintImplementable_NativeFunc);
		CallFuncBlueprintImplementable_InValue_Offset = TypeInterop.GetPropertyOffsetFromName(CallFuncBlueprintImplementable_NativeFunc, "InValue");
		CallFuncBlueprintImplementable_ReturnValue_Offset = TypeInterop.GetPropertyOffsetFromName(CallFuncBlueprintImplementable_NativeFunc, "ReturnValue");

		CallFuncBlueprintNative_NativeFunc = TypeInterop.FindFunction(NativeType, "CallFuncBlueprintNative");
		CallFuncBlueprintNative_ParamsSize = TypeInterop.GetFunctionParamsSize(CallFuncBlueprintNative_NativeFunc);
		CallFuncBlueprintNative_InValue_Offset = TypeInterop.GetPropertyOffsetFromName(CallFuncBlueprintNative_NativeFunc, "InValue");
		CallFuncBlueprintNative_ReturnValue_Offset = TypeInterop.GetPropertyOffsetFromName(CallFuncBlueprintNative_NativeFunc, "ReturnValue");

		CallFuncBlueprintNativeRef_NativeFunc = TypeInterop.FindFunction(NativeType, "CallFuncBlueprintNativeRef");
		CallFuncBlueprintNativeRef_ParamsSize = TypeInterop.GetFunctionParamsSize(CallFuncBlueprintNativeRef_NativeFunc);
		CallFuncBlueprintNativeRef_InOutStruct_Offset = TypeInterop.GetPropertyOffsetFromName(CallFuncBlueprintNativeRef_NativeFunc, "InOutStruct");

		FuncTakingSsTestDelegate_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncTakingSsTestDelegate");
		FuncTakingSsTestDelegate_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncTakingSsTestDelegate_NativeFunc);
		FuncTakingSsTestDelegate_InDelegate_Offset = TypeInterop.GetPropertyOffsetFromName(FuncTakingSsTestDelegate_NativeFunc, "InDelegate");
		FuncTakingSsTestDelegate_InValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncTakingSsTestDelegate_NativeFunc, "InValue");
		FuncTakingSsTestDelegate_ReturnValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncTakingSsTestDelegate_NativeFunc, "ReturnValue");

		FuncTakingFieldPath_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncTakingFieldPath");
		FuncTakingFieldPath_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncTakingFieldPath_NativeFunc);
		FuncTakingFieldPath_InFieldPath_Offset = TypeInterop.GetPropertyOffsetFromName(FuncTakingFieldPath_NativeFunc, "InFieldPath");

		MulticastDelegatePropertyCallback_NativeFunc = TypeInterop.FindFunction(NativeType, "MulticastDelegatePropertyCallback");
		MulticastDelegatePropertyCallback_ParamsSize = TypeInterop.GetFunctionParamsSize(MulticastDelegatePropertyCallback_NativeFunc);
		MulticastDelegatePropertyCallback_InStr_Offset = TypeInterop.GetPropertyOffsetFromName(MulticastDelegatePropertyCallback_NativeFunc, "InStr");

		ReturnArray_NativeFunc = TypeInterop.FindFunction(NativeType, "ReturnArray");
		ReturnArray_ParamsSize = TypeInterop.GetFunctionParamsSize(ReturnArray_NativeFunc);
		ReturnArray_ReturnValue_NativeProp = TypeInterop.FindProperty(ReturnArray_NativeFunc, "ReturnValue");
		ReturnArray_ReturnValue_Offset = TypeInterop.GetPropertyOffset(ReturnArray_ReturnValue_NativeProp);

		ReturnSet_NativeFunc = TypeInterop.FindFunction(NativeType, "ReturnSet");
		ReturnSet_ParamsSize = TypeInterop.GetFunctionParamsSize(ReturnSet_NativeFunc);
		ReturnSet_ReturnValue_NativeProp = TypeInterop.FindProperty(ReturnSet_NativeFunc, "ReturnValue");
		ReturnSet_ReturnValue_Offset = TypeInterop.GetPropertyOffset(ReturnSet_ReturnValue_NativeProp);

		ReturnMap_NativeFunc = TypeInterop.FindFunction(NativeType, "ReturnMap");
		ReturnMap_ParamsSize = TypeInterop.GetFunctionParamsSize(ReturnMap_NativeFunc);
		ReturnMap_ReturnValue_NativeProp = TypeInterop.FindProperty(ReturnMap_NativeFunc, "ReturnValue");
		ReturnMap_ReturnValue_Offset = TypeInterop.GetPropertyOffset(ReturnMap_ReturnValue_NativeProp);

		ReturnFieldPath_NativeFunc = TypeInterop.FindFunction(NativeType, "ReturnFieldPath");
		ReturnFieldPath_ParamsSize = TypeInterop.GetFunctionParamsSize(ReturnFieldPath_NativeFunc);
		ReturnFieldPath_ReturnValue_Offset = TypeInterop.GetPropertyOffsetFromName(ReturnFieldPath_NativeFunc, "ReturnValue");

		SetInt_NativeFunc = TypeInterop.FindFunction(NativeType, "SetInt");
		SetInt_ParamsSize = TypeInterop.GetFunctionParamsSize(SetInt_NativeFunc);
		SetInt_InValue_Offset = TypeInterop.GetPropertyOffsetFromName(SetInt_NativeFunc, "InValue");

		GetInt_NativeFunc = TypeInterop.FindFunction(NativeType, "GetInt");
		GetInt_ParamsSize = TypeInterop.GetFunctionParamsSize(GetInt_NativeFunc);
		GetInt_ReturnValue_Offset = TypeInterop.GetPropertyOffsetFromName(GetInt_NativeFunc, "ReturnValue");

		FuncInterface_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncInterface");
		FuncInterface_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncInterface_NativeFunc);
		FuncInterface_InValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncInterface_NativeFunc, "InValue");
		FuncInterface_ReturnValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncInterface_NativeFunc, "ReturnValue");

		FuncInterfaceChild_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncInterfaceChild");
		FuncInterfaceChild_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncInterfaceChild_NativeFunc);
		FuncInterfaceChild_InValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncInterfaceChild_NativeFunc, "InValue");
		FuncInterfaceChild_ReturnValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncInterfaceChild_NativeFunc, "ReturnValue");

		FuncInterfaceOther_NativeFunc = TypeInterop.FindFunction(NativeType, "FuncInterfaceOther");
		FuncInterfaceOther_ParamsSize = TypeInterop.GetFunctionParamsSize(FuncInterfaceOther_NativeFunc);
		FuncInterfaceOther_InValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncInterfaceOther_NativeFunc, "InValue");
		FuncInterfaceOther_ReturnValue_Offset = TypeInterop.GetPropertyOffsetFromName(FuncInterfaceOther_NativeFunc, "ReturnValue");
	}

	public bool Bool
	{
		get
		{
			ThrowIfNotValid();
			return BoolMarshaller.FromNative(NativeObject + Bool_Offset);
		}
		set
		{
			ThrowIfNotValid();
			BoolMarshaller.ToNative(NativeObject + Bool_Offset, value);
		}
	}

	public bool BitfieldBoolA
	{
		get
		{
			ThrowIfNotValid();
			return BitfieldBoolMarshaller.FromNative(NativeObject + BitfieldBoolA_Offset, BitfieldBoolA_FieldMask);
		}
		set
		{
			ThrowIfNotValid();
			BitfieldBoolMarshaller.ToNative(NativeObject + BitfieldBoolA_Offset, BitfieldBoolA_FieldMask, value);
		}
	}

	public bool BitfieldBoolB
	{
		get
		{
			ThrowIfNotValid();
			return BitfieldBoolMarshaller.FromNative(NativeObject + BitfieldBoolB_Offset, BitfieldBoolB_FieldMask);
		}
		set
		{
			ThrowIfNotValid();
			BitfieldBoolMarshaller.ToNative(NativeObject + BitfieldBoolB_Offset, BitfieldBoolB_FieldMask, value);
		}
	}

	public int Int
	{
		get
		{
			ThrowIfNotValid();
			return BlittableMarshaller<int>.FromNative(NativeObject + Int_Offset);
		}
		set
		{
			ThrowIfNotValid();
			BlittableMarshaller<int>.ToNative(NativeObject + Int_Offset, value);
		}
	}

	public double Float
	{
		get
		{
			ThrowIfNotValid();
			return BlittableMarshaller<double>.FromNative(NativeObject + Float_Offset);
		}
		set
		{
			ThrowIfNotValid();
			BlittableMarshaller<double>.ToNative(NativeObject + Float_Offset, value);
		}
	}

	public ESsTestEnum Enum
	{
		get
		{
			ThrowIfNotValid();
			return EnumMarshaller<ESsTestEnum>.FromNative(NativeObject + Enum_Offset);
		}
		set
		{
			ThrowIfNotValid();
			EnumMarshaller<ESsTestEnum>.ToNative(NativeObject + Enum_Offset, value);
		}
	}

	public ESsTestLongEnum LongEnum
	{
		get
		{
			ThrowIfNotValid();
			return EnumMarshaller<ESsTestLongEnum>.FromNative(NativeObject + LongEnum_Offset);
		}
		set
		{
			ThrowIfNotValid();
			EnumMarshaller<ESsTestLongEnum>.ToNative(NativeObject + LongEnum_Offset, value);
		}
	}

	public string String
	{
		get
		{
			ThrowIfNotValid();
			return StringMarshaller.FromNative(NativeObject + String_Offset);
		}
		set
		{
			ThrowIfNotValid();
			StringMarshaller.ToNative(NativeObject + String_Offset, value);
		}
	}

	public Name Name
	{
		get
		{
			ThrowIfNotValid();
			return NameMarshaller.FromNative(NativeObject + Name_Offset);
		}
		set
		{
			ThrowIfNotValid();
			NameMarshaller.ToNative(NativeObject + Name_Offset, value);
		}
	}

	public Text Text
	{
		get
		{
			ThrowIfNotValid();
			return TextMarshaller.FromNative(NativeObject + Text_Offset);
		}
		set
		{
			ThrowIfNotValid();
			TextMarshaller.ToNative(NativeObject + Text_Offset, value);
		}
	}

	public FieldPath FieldPath
	{
		get
		{
			ThrowIfNotValid();
			return FieldPathMarshaller.FromNative(NativeObject + FieldPath_Offset);
		}
		set
		{
			ThrowIfNotValid();
			FieldPathMarshaller.ToNative(NativeObject + FieldPath_Offset, value);
		}
	}

	public FieldPath StructFieldPath
	{
		get
		{
			ThrowIfNotValid();
			return FieldPathMarshaller.FromNative(NativeObject + StructFieldPath_Offset);
		}
		set
		{
			ThrowIfNotValid();
			FieldPathMarshaller.ToNative(NativeObject + StructFieldPath_Offset, value);
		}
	}

	private Array<string>? _stringArray;

	public Array<string> StringArray
	{
		get
		{
			ThrowIfNotValid();
			return _stringArray ??= new(NativeObject + StringArray_Offset, StringArray_NativeProp, StringMarshaller.Instance);
		}
	}

	private Set<string>? _stringSet;

	public Set<string> StringSet
	{
		get
		{
			ThrowIfNotValid();
			return _stringSet ??= new(NativeObject + StringSet_Offset, StringSet_NativeProp, StringMarshaller.Instance);
		}
	}

	private Map<string, int>? _stringIntMap;

	public Map<string, int> StringIntMap
	{
		get
		{
			ThrowIfNotValid();
			return _stringIntMap ??= new(NativeObject + StringIntMap_Offset, StringIntMap_NativeProp, StringMarshaller.Instance, BlittableMarshaller<int>.Instance);
		}
	}

	private Delegate<SsTestDelegate>? _delegate;

	public Delegate<SsTestDelegate> Delegate
	{
		get
		{
			ThrowIfNotValid();
			return _delegate ??= new(NativeObject + Delegate_Offset);
		}
	}

	private MulticastDelegate<SsTestMulticastDelegate>? _multicastDelegate;

	public MulticastDelegate<SsTestMulticastDelegate> MulticastDelegate
	{
		get
		{
			ThrowIfNotValid();
			return _multicastDelegate ??= new(NativeObject + MulticastDelegate_Offset);
		}
	}

	private SsTestStructNativeRef? _struct;

	public SsTestStructNativeRef Struct
	{
		get
		{
			ThrowIfNotValid();
			return _struct ??= new(NativeObject + Struct_Offset);
		}
	}

	private Array<SsTestStruct, SsTestStructNativeRef>? _structArray;

	public Array<SsTestStruct, SsTestStructNativeRef> StructArray
	{
		get
		{
			ThrowIfNotValid();
			return _structArray ??= new(NativeObject + StructArray_Offset, StructArray_NativeProp);
		}
	}

	public ref SsTestBlittableStruct BlittableStruct
	{
		get
		{
			unsafe
			{
				ThrowIfNotValid();
				return ref *(SsTestBlittableStruct*)(NativeObject + BlittableStruct_Offset);
			}
		}
	}

	public Object? Object
	{
		get
		{
			ThrowIfNotValid();
			return ObjectMarshaller<Object>.FromNative(NativeObject + Object_Offset);
		}
		set
		{
			ThrowIfNotValid();
			ObjectMarshaller<Object>.ToNative(NativeObject + Object_Offset, value);
		}
	}

	public Object? ObjectPtr
	{
		get
		{
			ThrowIfNotValid();
			return ObjectMarshaller<Object>.FromNative(NativeObject + ObjectPtr_Offset);
		}
		set
		{
			ThrowIfNotValid();
			ObjectMarshaller<Object>.ToNative(NativeObject + ObjectPtr_Offset, value);
		}
	}

	public SoftObjectPtr<Object> SoftObjectPtr
	{
		get
		{
			ThrowIfNotValid();
			return SoftObjectPtrMarshaller<Object>.FromNative(NativeObject + SoftObjectPtr_Offset);
		}
		set
		{
			ThrowIfNotValid();
			SoftObjectPtrMarshaller<Object>.ToNative(NativeObject + SoftObjectPtr_Offset, value);
		}
	}

	public Object? WeakObjectPtr
	{
		get
		{
			ThrowIfNotValid();
			return ObjectMarshaller<Object>.FromNative(NativeObject + WeakObjectPtr_Offset);
		}
		set
		{
			ThrowIfNotValid();
			ObjectMarshaller<Object>.ToNative(NativeObject + WeakObjectPtr_Offset, value);
		}
	}

	public LazyObjectPtr<Object> LazyObjectPtr
	{
		get
		{
			ThrowIfNotValid();
			return LazyObjectPtrMarshaller<Object>.FromNative(NativeObject + LazyObjectPtr_Offset);
		}
		set
		{
			ThrowIfNotValid();
			LazyObjectPtrMarshaller<Object>.ToNative(NativeObject + LazyObjectPtr_Offset, value);
		}
	}

	public SubclassOf<Object> Class
	{
		get
		{
			ThrowIfNotValid();
			return SubclassOfMarshaller<Object>.FromNative(NativeObject + Class_Offset);
		}
		set
		{
			ThrowIfNotValid();
			SubclassOfMarshaller<Object>.ToNative(NativeObject + Class_Offset, value);
		}
	}

	public Class? ClassPtr
	{
		get
		{
			ThrowIfNotValid();
			return ObjectMarshaller<Class>.FromNative(NativeObject + ClassPtr_Offset);
		}
		set
		{
			ThrowIfNotValid();
			ObjectMarshaller<Class>.ToNative(NativeObject + ClassPtr_Offset, value);
		}
	}

	public SoftClassPtr<Object> SoftClassPtr
	{
		get
		{
			ThrowIfNotValid();
			return SoftClassPtrMarshaller<Object>.FromNative(NativeObject + SoftClassPtr_Offset);
		}
		set
		{
			ThrowIfNotValid();
			SoftClassPtrMarshaller<Object>.ToNative(NativeObject + SoftClassPtr_Offset, value);
		}
	}

	public ISsTestChildInterface? Interface
	{
		get
		{
			ThrowIfNotValid();
			return InterfaceMarshaller<ISsTestChildInterface>.FromNative(NativeObject + Interface_Offset);
		}
		set
		{
			ThrowIfNotValid();
			InterfaceMarshaller<ISsTestChildInterface>.ToNative(NativeObject + Interface_Offset, value);
		}
	}

	public unsafe int FuncBlueprintImplementable(int InValue)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[FuncBlueprintImplementable_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(FuncBlueprintImplementable_NativeFunc, _paramsBuffer);

		BlittableMarshaller<int>.ToNative(_params.Buffer + FuncBlueprintNative_InValue_Offset, InValue);

		InvokeFunctionCall(FuncBlueprintImplementable_NativeFunc, _params.Buffer);

		int returnValue = BlittableMarshaller<int>.FromNative(_params.Buffer + FuncBlueprintNative_ReturnValue_Offset);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe int FuncBlueprintNative(int InValue)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[FuncBlueprintNative_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(FuncBlueprintNative_NativeFunc, _paramsBuffer);

		BlittableMarshaller<int>.ToNative(_params.Buffer + FuncBlueprintNative_InValue_Offset, InValue);

		InvokeFunctionCall(FuncBlueprintNative_NativeFunc, _params.Buffer);

		int returnValue = BlittableMarshaller<int>.FromNative(_params.Buffer + FuncBlueprintNative_ReturnValue_Offset);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe void FuncBlueprintNativeRef(ref SsTestStruct InOutStruct)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[FuncBlueprintNativeRef_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(FuncBlueprintNativeRef_NativeFunc, _paramsBuffer);

		StructMarshaller<SsTestStruct>.ToNative(_params.Buffer + FuncBlueprintNativeRef_InOutStruct_Offset, InOutStruct);

		InvokeFunctionCall(FuncBlueprintNativeRef_NativeFunc, _params.Buffer);

		InOutStruct = StructMarshaller<SsTestStruct>.FromNative(_params.Buffer + FuncBlueprintNativeRef_InOutStruct_Offset);
		// ReSharper restore InconsistentNaming
	}

	public unsafe int CallFuncBlueprintImplementable(int InValue)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[CallFuncBlueprintImplementable_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(CallFuncBlueprintImplementable_NativeFunc, _paramsBuffer);

		BlittableMarshaller<int>.ToNative(_params.Buffer + CallFuncBlueprintNative_InValue_Offset, InValue);

		InvokeFunctionCall(CallFuncBlueprintImplementable_NativeFunc, _params.Buffer);

		int returnValue = BlittableMarshaller<int>.FromNative(_params.Buffer + CallFuncBlueprintNative_ReturnValue_Offset);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe int CallFuncBlueprintNative(int InValue)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[CallFuncBlueprintNative_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(CallFuncBlueprintNative_NativeFunc, _paramsBuffer);

		BlittableMarshaller<int>.ToNative(_params.Buffer + CallFuncBlueprintNative_InValue_Offset, InValue);

		InvokeFunctionCall(CallFuncBlueprintNative_NativeFunc, _params.Buffer);

		int returnValue = BlittableMarshaller<int>.FromNative(_params.Buffer + CallFuncBlueprintNative_ReturnValue_Offset);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe void CallFuncBlueprintNativeRef(ref SsTestStruct InOutStruct)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[CallFuncBlueprintNativeRef_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(CallFuncBlueprintNativeRef_NativeFunc, _paramsBuffer);

		StructMarshaller<SsTestStruct>.ToNative(_params.Buffer + CallFuncBlueprintNativeRef_InOutStruct_Offset, InOutStruct);

		InvokeFunctionCall(CallFuncBlueprintNativeRef_NativeFunc, _params.Buffer);

		InOutStruct = StructMarshaller<SsTestStruct>.FromNative(_params.Buffer + CallFuncBlueprintNativeRef_InOutStruct_Offset);
		// ReSharper restore InconsistentNaming
	}

	public unsafe int FuncTakingSsTestDelegate(SsTestDelegate InDelegate, int InValue)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[FuncTakingSsTestDelegate_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(FuncTakingSsTestDelegate_NativeFunc, _paramsBuffer);

		DelegateMarshaller<SsTestDelegate>.ToNative(_params.Buffer + FuncTakingSsTestDelegate_InDelegate_Offset, InDelegate);
		BlittableMarshaller<int>.ToNative(_params.Buffer + FuncTakingSsTestDelegate_InValue_Offset, InValue);

		InvokeFunctionCall(FuncTakingSsTestDelegate_NativeFunc, _params.Buffer);

		int returnValue = BlittableMarshaller<int>.FromNative(_params.Buffer + FuncTakingSsTestDelegate_ReturnValue_Offset);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe void FuncTakingFieldPath(in FieldPath InFieldPath)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[FuncTakingFieldPath_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(FuncTakingFieldPath_NativeFunc, _paramsBuffer);

		FieldPathMarshaller.ToNative(_params.Buffer + FuncTakingFieldPath_InFieldPath_Offset, InFieldPath);

		InvokeFunctionCall(FuncTakingFieldPath_NativeFunc, _params.Buffer);
		// ReSharper restore InconsistentNaming
	}

	public unsafe void MulticastDelegatePropertyCallback(string InStr)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[MulticastDelegatePropertyCallback_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(MulticastDelegatePropertyCallback_NativeFunc, _paramsBuffer);

		StringMarshaller.ToNative(_params.Buffer + MulticastDelegatePropertyCallback_InStr_Offset, InStr);

		InvokeFunctionCall(MulticastDelegatePropertyCallback_NativeFunc, _params.Buffer);
		// ReSharper restore InconsistentNaming
	}

	public unsafe List<int> ReturnArray()
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[ReturnArray_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(ReturnArray_NativeFunc, _paramsBuffer);

		InvokeFunctionCall(ReturnArray_NativeFunc, _params.Buffer);

		List<int> returnValue = new Array<int>(_params.Buffer + ReturnArray_ReturnValue_Offset,
			ReturnArray_ReturnValue_NativeProp, BlittableMarshaller<int>.Instance);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe HashSet<int> ReturnSet()
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[ReturnSet_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(ReturnSet_NativeFunc, _paramsBuffer);

		InvokeFunctionCall(ReturnSet_NativeFunc, _params.Buffer);

		HashSet<int> returnValue = new Set<int>(_params.Buffer + ReturnSet_ReturnValue_Offset,
			ReturnSet_ReturnValue_NativeProp, BlittableMarshaller<int>.Instance);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe Dictionary<int, bool> ReturnMap()
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[ReturnMap_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(ReturnMap_NativeFunc, _paramsBuffer);

		InvokeFunctionCall(ReturnMap_NativeFunc, _params.Buffer);

		Dictionary<int, bool> returnValue = new Map<int, bool>(_params.Buffer + ReturnMap_ReturnValue_Offset,
			ReturnMap_ReturnValue_NativeProp, BlittableMarshaller<int>.Instance, BoolMarshaller.Instance);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe FieldPath ReturnFieldPath()
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[ReturnFieldPath_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(ReturnFieldPath_NativeFunc, _paramsBuffer);

		InvokeFunctionCall(ReturnFieldPath_NativeFunc, _params.Buffer);

		FieldPath returnValue = FieldPathMarshaller.FromNative(_params.Buffer + ReturnFieldPath_ReturnValue_Offset);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe void SetInt(int InValue)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = (byte*)&InValue;
		InvokeFunctionCall(SetInt_NativeFunc, (IntPtr)_paramsBuffer);
		// ReSharper restore InconsistentNaming
	}

	public unsafe int GetInt()
	{
		// ReSharper disable InconsistentNaming
		int returnValue;
		byte* _paramsBuffer = (byte*)&returnValue;
		InvokeFunctionCall(GetInt_NativeFunc, (IntPtr)_paramsBuffer);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe int FuncInterface(int InValue)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[FuncInterface_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(FuncInterface_NativeFunc, _paramsBuffer);
		BlittableMarshaller<int>.ToNative(_params.Buffer + FuncInterface_InValue_Offset, InValue);
		InvokeFunctionCall(FuncInterface_NativeFunc, _params.Buffer);
		int returnValue = BlittableMarshaller<int>.FromNative(_params.Buffer + FuncInterface_ReturnValue_Offset);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe int FuncInterfaceChild(int InValue)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[FuncInterfaceChild_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(FuncInterfaceChild_NativeFunc, _paramsBuffer);
		BlittableMarshaller<int>.ToNative(_params.Buffer + FuncInterfaceChild_InValue_Offset, InValue);
		InvokeFunctionCall(FuncInterfaceChild_NativeFunc, _params.Buffer);
		int returnValue = BlittableMarshaller<int>.FromNative(_params.Buffer + FuncInterfaceChild_ReturnValue_Offset);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}

	public unsafe int FuncInterfaceOther(int InValue)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[FuncInterfaceOther_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(FuncInterfaceOther_NativeFunc, _paramsBuffer);
		BlittableMarshaller<int>.ToNative(_params.Buffer + FuncInterfaceOther_InValue_Offset, InValue);
		InvokeFunctionCall(FuncInterfaceOther_NativeFunc, _params.Buffer);
		int returnValue = BlittableMarshaller<int>.FromNative(_params.Buffer + FuncInterfaceOther_ReturnValue_Offset);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}
}
