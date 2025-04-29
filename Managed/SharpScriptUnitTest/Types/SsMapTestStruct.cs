using SharpScript;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;
using Object = UnrealEngine.CoreUObject.Object;

namespace SharpScriptUnitTest.Types;

public class SsMapTestStructNativeRef(IntPtr nativePtr)
	: IStructNativeRef<SsMapTestStruct>
{
	// ReSharper disable InconsistentNaming
	public static readonly IntPtr NativeType;
	public static readonly int NativeDataSize;
	internal static readonly IntPtr StringTextMap_NativeProp;
	internal static readonly int StringTextMap_Offset;
	internal static readonly IntPtr IntBoolMap_NativeProp;
	internal static readonly int IntBoolMap_Offset;
	internal static readonly IntPtr EnumMap_NativeProp;
	internal static readonly int EnumMap_Offset;
	internal static readonly IntPtr LongEnumMap_NativeProp;
	internal static readonly int LongEnumMap_Offset;
	internal static readonly IntPtr IntStructMap_NativeProp;
	internal static readonly int IntStructMap_Offset;
	internal static readonly IntPtr BlittableStructMap_NativeProp;
	internal static readonly int BlittableStructMap_Offset;
	internal static readonly IntPtr ObjectMap_NativeProp;
	internal static readonly int ObjectMap_Offset;
	internal static readonly IntPtr SoftObjectPtrMap_NativeProp;
	internal static readonly int SoftObjectPtrMap_Offset;
	internal static readonly IntPtr ClassMap_NativeProp;
	internal static readonly int ClassMap_Offset;
	internal static readonly IntPtr SoftClassPtrMap_NativeProp;
	internal static readonly int SoftClassPtrMap_Offset;
	internal static readonly IntPtr IntInterfaceMap_NativeProp;
	internal static readonly int IntInterfaceMap_Offset;
	internal static readonly IntPtr IntDelegateMap_NativeProp;
	internal static readonly int IntDelegateMap_Offset;
	// ReSharper restore InconsistentNaming

	static SsMapTestStructNativeRef()
	{
		NativeType = TypeInterop.FindStruct("SsMapTestStruct");
		NativeDataSize = TypeInterop.GetStructureSize(NativeType);
		PropertyIterator propIter = new PropertyIterator(NativeType);

		StringTextMap_NativeProp = propIter.FindNext("StringTextMap");
		StringTextMap_Offset = TypeInterop.GetPropertyOffset(StringTextMap_NativeProp);

		IntBoolMap_NativeProp = propIter.FindNext("IntBoolMap");
		IntBoolMap_Offset = TypeInterop.GetPropertyOffset(IntBoolMap_NativeProp);

		EnumMap_NativeProp = propIter.FindNext("EnumMap");
		EnumMap_Offset = TypeInterop.GetPropertyOffset(EnumMap_NativeProp);

		LongEnumMap_NativeProp = propIter.FindNext("LongEnumMap");
		LongEnumMap_Offset = TypeInterop.GetPropertyOffset(LongEnumMap_NativeProp);

		IntStructMap_NativeProp = propIter.FindNext("IntStructMap");
		IntStructMap_Offset = TypeInterop.GetPropertyOffset(IntStructMap_NativeProp);

		BlittableStructMap_NativeProp = propIter.FindNext("BlittableStructMap");
		BlittableStructMap_Offset = TypeInterop.GetPropertyOffset(BlittableStructMap_NativeProp);

		ObjectMap_NativeProp = propIter.FindNext("ObjectMap");
		ObjectMap_Offset = TypeInterop.GetPropertyOffset(ObjectMap_NativeProp);

		SoftObjectPtrMap_NativeProp = propIter.FindNext("SoftObjectPtrMap");
		SoftObjectPtrMap_Offset = TypeInterop.GetPropertyOffset(SoftObjectPtrMap_NativeProp);

		ClassMap_NativeProp = propIter.FindNext("ClassMap");
		ClassMap_Offset = TypeInterop.GetPropertyOffset(ClassMap_NativeProp);

		SoftClassPtrMap_NativeProp = propIter.FindNext("SoftClassPtrMap");
		SoftClassPtrMap_Offset = TypeInterop.GetPropertyOffset(SoftClassPtrMap_NativeProp);

		IntInterfaceMap_NativeProp = propIter.FindNext("IntInterfaceMap");
		IntInterfaceMap_Offset = TypeInterop.GetPropertyOffset(IntInterfaceMap_NativeProp);

		IntDelegateMap_NativeProp = propIter.FindNext("IntDelegateMap");
		IntDelegateMap_Offset = TypeInterop.GetPropertyOffset(IntDelegateMap_NativeProp);
	}

	private Map<string, Text>? _stringTextMap;
	public Map<string, Text> StringTextMap => _stringTextMap ??= new(nativePtr + StringTextMap_Offset, StringTextMap_NativeProp, StringMarshaller.Instance, TextMarshaller.Instance);

	private Map<int, bool>? _intBoolMap;
	public Map<int, bool> IntBoolMap => _intBoolMap ??= new(nativePtr + IntBoolMap_Offset, IntBoolMap_NativeProp, BlittableMarshaller<int>.Instance, BoolMarshaller.Instance);

	private Map<ESsTestEnum, ESsTestEnum>? _enumMap;
	public Map<ESsTestEnum, ESsTestEnum> EnumMap => _enumMap ??= new(nativePtr + EnumMap_Offset, EnumMap_NativeProp, EnumMarshaller<ESsTestEnum>.Instance, EnumMarshaller<ESsTestEnum>.Instance);

	private Map<ESsTestLongEnum, ESsTestLongEnum>? _longEnumMap;
	public Map<ESsTestLongEnum, ESsTestLongEnum> LongEnumMap => _longEnumMap ??= new(nativePtr + LongEnumMap_Offset, LongEnumMap_NativeProp, EnumMarshaller<ESsTestLongEnum>.Instance, EnumMarshaller<ESsTestLongEnum>.Instance);

	private Map<int, SsMapTestInnerStruct, SsMapTestInnerStructNativeRef>? _intStructMap;

	public Map<int, SsMapTestInnerStruct, SsMapTestInnerStructNativeRef> IntStructMap
		=> _intStructMap ??= new(nativePtr + IntStructMap_Offset, IntStructMap_NativeProp, BlittableMarshaller<int>.Instance);

	private Map<SsTestBlittableStruct, SsTestBlittableStruct, SsTestBlittableStructNativeRef>? _blittableStructMap;

	public Map<SsTestBlittableStruct, SsTestBlittableStruct, SsTestBlittableStructNativeRef> BlittableStructMap
		=> _blittableStructMap ??= new(nativePtr + BlittableStructMap_Offset, BlittableStructMap_NativeProp, BlittableMarshaller<SsTestBlittableStruct>.Instance);

	private Map<Object, Object?>? _objectMap;
	public Map<Object, Object?> ObjectMap => _objectMap ??= new(nativePtr + ObjectMap_Offset, ObjectMap_NativeProp, ObjectMarshaller<Object>.Instance!, ObjectMarshaller<Object>.Instance);

	private Map<SoftObjectPtr<Object>, SoftObjectPtr<Object>>? _softObjectPtrMap;
	public Map<SoftObjectPtr<Object>, SoftObjectPtr<Object>> SoftObjectPtrMap => _softObjectPtrMap ??= new(nativePtr + SoftObjectPtrMap_Offset, SoftObjectPtrMap_NativeProp, SoftObjectPtrMarshaller<Object>.Instance, SoftObjectPtrMarshaller<Object>.Instance);

	private Map<SubclassOf<Object>, SubclassOf<Object>>? _classMap;
	public Map<SubclassOf<Object>, SubclassOf<Object>> ClassMap => _classMap ??= new(nativePtr + ClassMap_Offset, ClassMap_NativeProp, SubclassOfMarshaller<Object>.Instance, SubclassOfMarshaller<Object>.Instance);

	private Map<SoftClassPtr<Object>, SoftClassPtr<Object>>? _softClassPtrMap;
	public Map<SoftClassPtr<Object>, SoftClassPtr<Object>> SoftClassPtrMap => _softClassPtrMap ??= new(nativePtr + SoftClassPtrMap_Offset, SoftClassPtrMap_NativeProp, SoftClassPtrMarshaller<Object>.Instance, SoftClassPtrMarshaller<Object>.Instance);

	private Map<int, ISsTestChildInterface?>? _intInterfaceMap;
	public Map<int, ISsTestChildInterface?> IntInterfaceMap => _intInterfaceMap ??= new(nativePtr + IntInterfaceMap_Offset, IntInterfaceMap_NativeProp, BlittableMarshaller<int>.Instance, InterfaceMarshaller<ISsTestChildInterface>.Instance);

	private DelegateMap<int, SsTestDelegate, Delegate<SsTestDelegate>>? _intDelegateMap;

	public DelegateMap<int, SsTestDelegate, Delegate<SsTestDelegate>> IntDelegateMap
		=> _intDelegateMap ??= new(nativePtr + IntDelegateMap_Offset, IntDelegateMap_NativeProp, BlittableMarshaller<int>.Instance, DelegateMarshaller<SsTestDelegate>.Instance);

	public SsMapTestStruct ToManaged()
	{
		return new SsMapTestStruct()
		{
			StringTextMap = StringTextMap,
			IntBoolMap = IntBoolMap,
			EnumMap = EnumMap,
			LongEnumMap = LongEnumMap,
			IntStructMap = IntStructMap,
			BlittableStructMap = BlittableStructMap,
			ObjectMap = ObjectMap,
			ClassMap = ClassMap,
			SoftClassPtrMap = SoftClassPtrMap,
			IntInterfaceMap = IntInterfaceMap,
			IntDelegateMap = IntDelegateMap,
		};
	}

	public void FromManaged(in SsMapTestStruct value)
	{
		StringTextMap.CopyFrom(value.StringTextMap);
		IntBoolMap.CopyFrom(value.IntBoolMap);
		EnumMap.CopyFrom(value.EnumMap);
		LongEnumMap.CopyFrom(value.LongEnumMap);
		IntStructMap.CopyFrom(value.IntStructMap);
		BlittableStructMap.CopyFrom(value.BlittableStructMap);
		ObjectMap.CopyFrom(value.ObjectMap);
		ClassMap.CopyFrom(value.ClassMap);
		SoftClassPtrMap.CopyFrom(value.SoftClassPtrMap);
		IntInterfaceMap.CopyFrom(value.IntInterfaceMap);
		IntDelegateMap.CopyFrom(value.IntDelegateMap);
	}

	public static IStructNativeRef<SsMapTestStruct> CreateInstance(IntPtr valuePtr)
	{
		return new SsMapTestStructNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator SsMapTestStruct(SsMapTestStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

public struct SsMapTestStruct
{
	public Dictionary<string, Text> StringTextMap;

	public Dictionary<int, bool> IntBoolMap;

	public Dictionary<ESsTestEnum, ESsTestEnum> EnumMap;

	public Dictionary<ESsTestLongEnum, ESsTestLongEnum> LongEnumMap;

	public Dictionary<int, SsMapTestInnerStruct> IntStructMap;

	public Dictionary<SsTestBlittableStruct, SsTestBlittableStruct> BlittableStructMap;

	public Dictionary<Object, Object?> ObjectMap;

	public Dictionary<SubclassOf<Object>, SubclassOf<Object>> ClassMap;

	public Dictionary<SoftClassPtr<Object>, SoftClassPtr<Object>> SoftClassPtrMap;

	public Dictionary<int, ISsTestChildInterface?> IntInterfaceMap;

	public Dictionary<int, SsTestDelegate> IntDelegateMap;
}
