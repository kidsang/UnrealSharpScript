using SharpScript;
using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class FSsMapTestStructNativeRef(IntPtr nativePtr)
	: IStructNativeRef<FSsMapTestStruct>
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

	static FSsMapTestStructNativeRef()
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

	private TMap<string, FText>? _stringTextMap;
	public TMap<string, FText> StringTextMap => _stringTextMap ??= new(nativePtr + StringTextMap_Offset, StringTextMap_NativeProp, StringMarshaller.Instance, TextMarshaller.Instance);

	private TMap<int, bool>? _intBoolMap;
	public TMap<int, bool> IntBoolMap => _intBoolMap ??= new(nativePtr + IntBoolMap_Offset, IntBoolMap_NativeProp, BlittableMarshaller<int>.Instance, BoolMarshaller.Instance);

	private TMap<ESsTestEnum, ESsTestEnum>? _enumMap;
	public TMap<ESsTestEnum, ESsTestEnum> EnumMap => _enumMap ??= new(nativePtr + EnumMap_Offset, EnumMap_NativeProp, EnumMarshaller<ESsTestEnum>.Instance, EnumMarshaller<ESsTestEnum>.Instance);

	private TMap<ESsTestLongEnum, ESsTestLongEnum>? _longEnumMap;
	public TMap<ESsTestLongEnum, ESsTestLongEnum> LongEnumMap => _longEnumMap ??= new(nativePtr + LongEnumMap_Offset, LongEnumMap_NativeProp, EnumMarshaller<ESsTestLongEnum>.Instance, EnumMarshaller<ESsTestLongEnum>.Instance);

	private TMap<int, FSsMapTestInnerStruct, FSsMapTestInnerStructNativeRef>? _intStructMap;

	public TMap<int, FSsMapTestInnerStruct, FSsMapTestInnerStructNativeRef> IntStructMap
		=> _intStructMap ??= new(nativePtr + IntStructMap_Offset, IntStructMap_NativeProp, BlittableMarshaller<int>.Instance);

	private TMap<FSsTestBlittableStruct, FSsTestBlittableStruct, FSsTestBlittableStructNativeRef>? _blittableStructMap;

	public TMap<FSsTestBlittableStruct, FSsTestBlittableStruct, FSsTestBlittableStructNativeRef> BlittableStructMap
		=> _blittableStructMap ??= new(nativePtr + BlittableStructMap_Offset, BlittableStructMap_NativeProp, BlittableMarshaller<FSsTestBlittableStruct>.Instance);

	private TMap<UObject, UObject?>? _objectMap;
	public TMap<UObject, UObject?> ObjectMap => _objectMap ??= new(nativePtr + ObjectMap_Offset, ObjectMap_NativeProp, ObjectMarshaller<UObject>.Instance!, ObjectMarshaller<UObject>.Instance);

	private TMap<TSoftObjectPtr<UObject>, TSoftObjectPtr<UObject>>? _softObjectPtrMap;
	public TMap<TSoftObjectPtr<UObject>, TSoftObjectPtr<UObject>> SoftObjectPtrMap => _softObjectPtrMap ??= new(nativePtr + SoftObjectPtrMap_Offset, SoftObjectPtrMap_NativeProp, SoftObjectPtrMarshaller<UObject>.Instance, SoftObjectPtrMarshaller<UObject>.Instance);

	private TMap<TSubclassOf<UObject>, TSubclassOf<UObject>>? _classMap;
	public TMap<TSubclassOf<UObject>, TSubclassOf<UObject>> ClassMap => _classMap ??= new(nativePtr + ClassMap_Offset, ClassMap_NativeProp, SubclassOfMarshaller<UObject>.Instance, SubclassOfMarshaller<UObject>.Instance);

	private TMap<TSoftClassPtr<UObject>, TSoftClassPtr<UObject>>? _softClassPtrMap;
	public TMap<TSoftClassPtr<UObject>, TSoftClassPtr<UObject>> SoftClassPtrMap => _softClassPtrMap ??= new(nativePtr + SoftClassPtrMap_Offset, SoftClassPtrMap_NativeProp, SoftClassPtrMarshaller<UObject>.Instance, SoftClassPtrMarshaller<UObject>.Instance);

	private TMap<int, ISsTestChildInterface?>? _intInterfaceMap;
	public TMap<int, ISsTestChildInterface?> IntInterfaceMap => _intInterfaceMap ??= new(nativePtr + IntInterfaceMap_Offset, IntInterfaceMap_NativeProp, BlittableMarshaller<int>.Instance, InterfaceMarshaller<ISsTestChildInterface>.Instance);

	private TDelegateMap<int, FSsTestDelegate, Delegate<FSsTestDelegate>>? _intDelegateMap;

	public TDelegateMap<int, FSsTestDelegate, Delegate<FSsTestDelegate>> IntDelegateMap
		=> _intDelegateMap ??= new(nativePtr + IntDelegateMap_Offset, IntDelegateMap_NativeProp, BlittableMarshaller<int>.Instance, DelegateMarshaller<FSsTestDelegate>.Instance);

	public FSsMapTestStruct ToManaged()
	{
		return new FSsMapTestStruct()
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

	public void FromManaged(in FSsMapTestStruct value)
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

	public static IStructNativeRef<FSsMapTestStruct> CreateInstance(IntPtr valuePtr)
	{
		return new FSsMapTestStructNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator FSsMapTestStruct(FSsMapTestStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

public struct FSsMapTestStruct
{
	public Dictionary<string, FText> StringTextMap;

	public Dictionary<int, bool> IntBoolMap;

	public Dictionary<ESsTestEnum, ESsTestEnum> EnumMap;

	public Dictionary<ESsTestLongEnum, ESsTestLongEnum> LongEnumMap;

	public Dictionary<int, FSsMapTestInnerStruct> IntStructMap;

	public Dictionary<FSsTestBlittableStruct, FSsTestBlittableStruct> BlittableStructMap;

	public Dictionary<UObject, UObject?> ObjectMap;

	public Dictionary<TSubclassOf<UObject>, TSubclassOf<UObject>> ClassMap;

	public Dictionary<TSoftClassPtr<UObject>, TSoftClassPtr<UObject>> SoftClassPtrMap;

	public Dictionary<int, ISsTestChildInterface?> IntInterfaceMap;

	public Dictionary<int, FSsTestDelegate> IntDelegateMap;
}
