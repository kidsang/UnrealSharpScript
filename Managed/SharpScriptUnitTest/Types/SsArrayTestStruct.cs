using SharpScript;
using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class FSsArrayTestStructNativeRef(IntPtr nativePtr)
	: IStructNativeRef<FSsArrayTestStruct>
{
	// ReSharper disable InconsistentNaming
	public static readonly IntPtr NativeType;
	public static readonly int NativeDataSize;
	internal static readonly IntPtr IntArray_NativeProp;
	internal static readonly int IntArray_Offset;
	internal static readonly IntPtr BoolArray_NativeProp;
	internal static readonly int BoolArray_Offset;
	internal static readonly IntPtr StringArray_NativeProp;
	internal static readonly int StringArray_Offset;
	internal static readonly IntPtr TextArray_NativeProp;
	internal static readonly int TextArray_Offset;
	internal static readonly IntPtr EnumArray_NativeProp;
	internal static readonly int EnumArray_Offset;
	internal static readonly IntPtr LongEnumArray_NativeProp;
	internal static readonly int LongEnumArray_Offset;
	internal static readonly IntPtr StructArray_NativeProp;
	internal static readonly int StructArray_Offset;
	internal static readonly IntPtr BlittableStructArray_NativeProp;
	internal static readonly int BlittableStructArray_Offset;
	internal static readonly IntPtr ObjectArray_NativeProp;
	internal static readonly int ObjectArray_Offset;
	internal static readonly IntPtr SoftObjectPtrArray_NativeProp;
	internal static readonly int SoftObjectPtrArray_Offset;
	internal static readonly IntPtr WeakObjectPtrArray_NativeProp;
	internal static readonly int WeakObjectPtrArray_Offset;
	internal static readonly IntPtr LazyObjectPtrArray_NativeProp;
	internal static readonly int LazyObjectPtrArray_Offset;
	internal static readonly IntPtr ClassArray_NativeProp;
	internal static readonly int ClassArray_Offset;
	internal static readonly IntPtr SoftClassPtrArray_NativeProp;
	internal static readonly int SoftClassPtrArray_Offset;
	internal static readonly IntPtr InterfaceArray_NativeProp;
	internal static readonly int InterfaceArray_Offset;
	internal static readonly IntPtr DelegateArray_NativeProp;
	internal static readonly int DelegateArray_Offset;
	// ReSharper restore InconsistentNaming

	static FSsArrayTestStructNativeRef()
	{
		NativeType = TypeInterop.FindStruct("SsArrayTestStruct");
		NativeDataSize = TypeInterop.GetStructureSize(NativeType);
		PropertyIterator propIter = new PropertyIterator(NativeType);

		IntArray_NativeProp = propIter.FindNext("IntArray");
		IntArray_Offset = TypeInterop.GetPropertyOffset(IntArray_NativeProp);

		BoolArray_NativeProp = propIter.FindNext("BoolArray");
		BoolArray_Offset = TypeInterop.GetPropertyOffset(BoolArray_NativeProp);

		StringArray_NativeProp = propIter.FindNext("StringArray");
		StringArray_Offset = TypeInterop.GetPropertyOffset(StringArray_NativeProp);

		TextArray_NativeProp = propIter.FindNext("TextArray");
		TextArray_Offset = TypeInterop.GetPropertyOffset(TextArray_NativeProp);

		EnumArray_NativeProp = propIter.FindNext("EnumArray");
		EnumArray_Offset = TypeInterop.GetPropertyOffset(EnumArray_NativeProp);

		LongEnumArray_NativeProp = propIter.FindNext("LongEnumArray");
		LongEnumArray_Offset = TypeInterop.GetPropertyOffset(LongEnumArray_NativeProp);

		StructArray_NativeProp = propIter.FindNext("StructArray");
		StructArray_Offset = TypeInterop.GetPropertyOffset(StructArray_NativeProp);

		BlittableStructArray_NativeProp = propIter.FindNext("BlittableStructArray");
		BlittableStructArray_Offset = TypeInterop.GetPropertyOffset(BlittableStructArray_NativeProp);

		ObjectArray_NativeProp = propIter.FindNext("ObjectArray");
		ObjectArray_Offset = TypeInterop.GetPropertyOffset(ObjectArray_NativeProp);

		SoftObjectPtrArray_NativeProp = propIter.FindNext("SoftObjectPtrArray");
		SoftObjectPtrArray_Offset = TypeInterop.GetPropertyOffset(SoftObjectPtrArray_NativeProp);

		WeakObjectPtrArray_NativeProp = propIter.FindNext("WeakObjectPtrArray");
		WeakObjectPtrArray_Offset = TypeInterop.GetPropertyOffset(WeakObjectPtrArray_NativeProp);

		LazyObjectPtrArray_NativeProp = propIter.FindNext("LazyObjectPtrArray");
		LazyObjectPtrArray_Offset = TypeInterop.GetPropertyOffset(LazyObjectPtrArray_NativeProp);

		ClassArray_NativeProp = propIter.FindNext("ClassArray");
		ClassArray_Offset = TypeInterop.GetPropertyOffset(ClassArray_NativeProp);

		SoftClassPtrArray_NativeProp = propIter.FindNext("SoftClassPtrArray");
		SoftClassPtrArray_Offset = TypeInterop.GetPropertyOffset(SoftClassPtrArray_NativeProp);

		InterfaceArray_NativeProp = propIter.FindNext("InterfaceArray");
		InterfaceArray_Offset = TypeInterop.GetPropertyOffset(InterfaceArray_NativeProp);

		DelegateArray_NativeProp = propIter.FindNext("DelegateArray");
		DelegateArray_Offset = TypeInterop.GetPropertyOffset(DelegateArray_NativeProp);
	}

	private TArray<int>? _intArray;
	public TArray<int> IntArray => _intArray ??= new(nativePtr + IntArray_Offset, IntArray_NativeProp, BlittableMarshaller<int>.Instance);

	private TArray<bool>? _boolArray;
	public TArray<bool> BoolArray => _boolArray ??= new(nativePtr + BoolArray_Offset, BoolArray_NativeProp, BoolMarshaller.Instance);

	private TArray<string>? _stringArray;
	public TArray<string> StringArray => _stringArray ??= new(nativePtr + StringArray_Offset, StringArray_NativeProp, StringMarshaller.Instance);

	private TArray<FText>? _textArray;
	public TArray<FText> TextArray => _textArray ??= new(nativePtr + TextArray_Offset, TextArray_NativeProp, TextMarshaller.Instance);

	private TArray<ESsTestEnum>? _enumArray;
	public TArray<ESsTestEnum> EnumArray => _enumArray ??= new(nativePtr + EnumArray_Offset, EnumArray_NativeProp, EnumMarshaller<ESsTestEnum>.Instance);

	private TArray<ESsTestLongEnum>? _longEnumArray;
	public TArray<ESsTestLongEnum> LongEnumArray => _longEnumArray ??= new(nativePtr + LongEnumArray_Offset, LongEnumArray_NativeProp, EnumMarshaller<ESsTestLongEnum>.Instance);

	private TArray<FSsArrayTestInnerStruct, FSsArrayTestInnerStructNativeRef>? _structArray;

	public TArray<FSsArrayTestInnerStruct, FSsArrayTestInnerStructNativeRef> StructArray
		=> _structArray ??= new(nativePtr + StructArray_Offset, StructArray_NativeProp);

	private TArray<FSsTestBlittableStruct, FSsTestBlittableStructNativeRef>? _blittableStructArray;

	public TArray<FSsTestBlittableStruct, FSsTestBlittableStructNativeRef> BlittableStructArray
		=> _blittableStructArray ??= new(nativePtr + BlittableStructArray_Offset, BlittableStructArray_NativeProp);

	private TArray<UObject?>? _objectArray;

	public TArray<UObject?> ObjectArray
		=> _objectArray ??= new(nativePtr + ObjectArray_Offset, ObjectArray_NativeProp, ObjectMarshaller<UObject>.Instance);

	private TArray<TSoftObjectPtr<UObject>>? _softObjectPtrArray;

	public TArray<TSoftObjectPtr<UObject>> SoftObjectPtrArray
		=> _softObjectPtrArray ??= new(nativePtr + SoftObjectPtrArray_Offset, SoftObjectPtrArray_NativeProp, SoftObjectPtrMarshaller<UObject>.Instance);

	private TArray<UObject?>? _weakObjectPtrArray;

	public TArray<UObject?> WeakObjectPtrArray
		=> _weakObjectPtrArray ??= new(nativePtr + WeakObjectPtrArray_Offset, WeakObjectPtrArray_NativeProp, ObjectMarshaller<UObject>.Instance);

	private TArray<TLazyObjectPtr<UObject>>? _lazyObjectPtrArray;

	public TArray<TLazyObjectPtr<UObject>> LazyObjectPtrArray
		=> _lazyObjectPtrArray ??= new(nativePtr + LazyObjectPtrArray_Offset, LazyObjectPtrArray_NativeProp, LazyObjectPtrMarshaller<UObject>.Instance);

	private TArray<TSubclassOf<UObject>>? _classArray;

	public TArray<TSubclassOf<UObject>> ClassArray
		=> _classArray ??= new(nativePtr + ClassArray_Offset, ClassArray_NativeProp, SubclassOfMarshaller<UObject>.Instance);

	private TArray<TSoftClassPtr<UObject>>? _softClassPtrArray;

	public TArray<TSoftClassPtr<UObject>> SoftClassPtrArray
		=> _softClassPtrArray ??= new(nativePtr + SoftClassPtrArray_Offset, SoftClassPtrArray_NativeProp, SoftClassPtrMarshaller<UObject>.Instance);

	private TArray<ISsTestChildInterface?>? _interfaceArray;

	public TArray<ISsTestChildInterface?> InterfaceArray
		=> _interfaceArray ??= new(nativePtr + InterfaceArray_Offset, InterfaceArray_NativeProp, InterfaceMarshaller<ISsTestChildInterface>.Instance);

	private TDelegateArray<FSsTestDelegate, Delegate<FSsTestDelegate>>? _delegateArray;

	public TDelegateArray<FSsTestDelegate, Delegate<FSsTestDelegate>> DelegateArray
		=> _delegateArray ??= new(nativePtr + DelegateArray_Offset, DelegateArray_NativeProp, DelegateMarshaller<FSsTestDelegate>.Instance);

	public FSsArrayTestStruct ToManaged()
	{
		return new FSsArrayTestStruct()
		{
			IntArray = IntArray,
			BoolArray = BoolArray,
			StringArray = StringArray,
			TextArray = TextArray,
			EnumArray = EnumArray,
			LongEnumArray = LongEnumArray,
			StructArray = StructArray,
			ObjectArray = ObjectArray,
			SoftObjectPtrArray = SoftObjectPtrArray,
			WeakObjectPtrArray = WeakObjectPtrArray,
			LazyObjectPtrArray = LazyObjectPtrArray,
			ClassArray = ClassArray,
			SoftClassPtrArray = SoftClassPtrArray,
			InterfaceArray = InterfaceArray,
			DelegateArray = DelegateArray,
		};
	}

	public void FromManaged(in FSsArrayTestStruct value)
	{
		IntArray.CopyFrom(value.IntArray);
		BoolArray.CopyFrom(value.BoolArray);
		StringArray.CopyFrom(value.StringArray);
		TextArray.CopyFrom(value.TextArray);
		EnumArray.CopyFrom(value.EnumArray);
		LongEnumArray.CopyFrom(value.LongEnumArray);
		StructArray.CopyFrom(value.StructArray);
		ObjectArray.CopyFrom(value.ObjectArray);
		SoftObjectPtrArray.CopyFrom(value.SoftObjectPtrArray);
		WeakObjectPtrArray.CopyFrom(value.WeakObjectPtrArray);
		LazyObjectPtrArray.CopyFrom(value.LazyObjectPtrArray);
		ClassArray.CopyFrom(value.ClassArray);
		SoftClassPtrArray.CopyFrom(value.SoftClassPtrArray);
		InterfaceArray.CopyFrom(value.InterfaceArray);
		DelegateArray.CopyFrom(value.DelegateArray);
	}

	public static IStructNativeRef<FSsArrayTestStruct> CreateInstance(IntPtr valuePtr)
	{
		return new FSsArrayTestStructNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator FSsArrayTestStruct(FSsArrayTestStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

public struct FSsArrayTestStruct
{
	public List<int> IntArray;

	public List<bool> BoolArray;

	public List<string> StringArray;

	public List<FText> TextArray;

	public List<ESsTestEnum> EnumArray;

	public List<ESsTestLongEnum> LongEnumArray;

	public List<FSsArrayTestInnerStruct> StructArray;

	public List<UObject?> ObjectArray;

	public List<TSoftObjectPtr<UObject>> SoftObjectPtrArray;

	public List<UObject?> WeakObjectPtrArray;

	public List<TLazyObjectPtr<UObject>> LazyObjectPtrArray;

	public List<TSubclassOf<UObject>> ClassArray;

	public List<TSoftClassPtr<UObject>> SoftClassPtrArray;

	public List<ISsTestChildInterface?> InterfaceArray;

	public List<FSsTestDelegate> DelegateArray;
}
