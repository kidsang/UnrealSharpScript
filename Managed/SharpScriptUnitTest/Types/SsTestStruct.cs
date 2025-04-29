using SharpScript;
using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using Object = UnrealEngine.CoreUObject.Object;

namespace SharpScriptUnitTest.Types;

public class SsTestStructNativeRef(IntPtr nativePtr)
	: IStructNativeRef<SsTestStruct>
{
	// ReSharper disable InconsistentNaming
	public static readonly IntPtr NativeType;
	public static readonly int NativeDataSize;
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
	internal static readonly IntPtr Struct_NativeProp;
	internal static readonly int Struct_Offset;
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
	// ReSharper restore InconsistentNaming

	static SsTestStructNativeRef()
	{
		NativeType = TypeInterop.FindStruct("SsTestStruct");
		NativeDataSize = TypeInterop.GetStructureSize(NativeType);
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

		Struct_NativeProp = propIter.FindNext("Struct");
		Struct_Offset = TypeInterop.GetPropertyOffset(Struct_NativeProp);

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
	}

	public bool Bool
	{
		get => BoolMarshaller.FromNative(nativePtr + Bool_Offset);
		set => BoolMarshaller.ToNative(nativePtr + Bool_Offset, value);
	}

	public bool BitfieldBoolA
	{
		get => BitfieldBoolMarshaller.FromNative(nativePtr + BitfieldBoolA_Offset, BitfieldBoolA_FieldMask);
		set => BitfieldBoolMarshaller.ToNative(nativePtr + BitfieldBoolA_Offset, BitfieldBoolA_FieldMask, value);
	}

	public bool BitfieldBoolB
	{
		get => BitfieldBoolMarshaller.FromNative(nativePtr + BitfieldBoolB_Offset, BitfieldBoolB_FieldMask);
		set => BitfieldBoolMarshaller.ToNative(nativePtr + BitfieldBoolB_Offset, BitfieldBoolB_FieldMask, value);
	}

	public int Int
	{
		get => BlittableMarshaller<int>.FromNative(nativePtr + Int_Offset);
		set => BlittableMarshaller<int>.ToNative(nativePtr + Int_Offset, value);
	}

	public double Float
	{
		get => BlittableMarshaller<double>.FromNative(nativePtr + Float_Offset);
		set => BlittableMarshaller<double>.ToNative(nativePtr + Float_Offset, value);
	}

	public ESsTestEnum Enum
	{
		get => EnumMarshaller<ESsTestEnum>.FromNative(nativePtr + Enum_Offset);
		set => EnumMarshaller<ESsTestEnum>.ToNative(nativePtr + Enum_Offset, value);
	}

	public ESsTestLongEnum LongEnum
	{
		get => EnumMarshaller<ESsTestLongEnum>.FromNative(nativePtr + LongEnum_Offset);
		set => EnumMarshaller<ESsTestLongEnum>.ToNative(nativePtr + LongEnum_Offset, value);
	}

	public string String
	{
		get => StringMarshaller.FromNative(nativePtr + String_Offset);
		set => StringMarshaller.ToNative(nativePtr + String_Offset, value);
	}

	public Name Name
	{
		get => NameMarshaller.FromNative(nativePtr + Name_Offset);
		set => NameMarshaller.ToNative(nativePtr + Name_Offset, value);
	}

	public Text Text
	{
		get => TextMarshaller.FromNative(nativePtr + Text_Offset);
		set => TextMarshaller.ToNative(nativePtr + Text_Offset, value);
	}

	public FieldPath FieldPath
	{
		get => FieldPathMarshaller.FromNative(nativePtr + FieldPath_Offset);
		set => FieldPathMarshaller.ToNative(nativePtr + FieldPath_Offset, value);
	}

	public FieldPath StructFieldPath
	{
		get => FieldPathMarshaller.FromNative(nativePtr + StructFieldPath_Offset);
		set => FieldPathMarshaller.ToNative(nativePtr + StructFieldPath_Offset, value);
	}

	private Array<string>? _stringArray;
	public Array<string> StringArray => _stringArray ??= new(nativePtr + StringArray_Offset, StringArray_NativeProp, StringMarshaller.Instance);

	private Set<string>? _stringSet;
	public Set<string> StringSet => _stringSet ??= new(nativePtr + StringSet_Offset, StringSet_NativeProp, StringMarshaller.Instance);

	private Map<string, int>? _stringIntMap;
	public Map<string, int> StringIntMap => _stringIntMap ??= new(nativePtr + StringIntMap_Offset, StringIntMap_NativeProp, StringMarshaller.Instance, BlittableMarshaller<int>.Instance);

	private SsArrayTestInnerStructNativeRef? _struct;
	public SsArrayTestInnerStructNativeRef Struct => _struct ??= new(nativePtr + Struct_Offset);

	public Object? Object
	{
		get => ObjectMarshaller<Object>.FromNative(nativePtr + Object_Offset);
		set => ObjectMarshaller<Object>.ToNative(nativePtr + Object_Offset, value);
	}

	public Object? ObjectPtr
	{
		get => ObjectMarshaller<Object>.FromNative(nativePtr + ObjectPtr_Offset);
		set => ObjectMarshaller<Object>.ToNative(nativePtr + ObjectPtr_Offset, value);
	}

	public SoftObjectPtr<Object> SoftObjectPtr
	{
		get => SoftObjectPtrMarshaller<Object>.FromNative(nativePtr + SoftObjectPtr_Offset);
		set => SoftObjectPtrMarshaller<Object>.ToNative(nativePtr + SoftObjectPtr_Offset, value);
	}

	public Object? WeakObjectPtr
	{
		get => ObjectMarshaller<Object>.FromNative(nativePtr + WeakObjectPtr_Offset);
		set => ObjectMarshaller<Object>.ToNative(nativePtr + WeakObjectPtr_Offset, value);
	}

	public LazyObjectPtr<Object> LazyObjectPtr
	{
		get => LazyObjectPtrMarshaller<Object>.FromNative(nativePtr + LazyObjectPtr_Offset);
		set => LazyObjectPtrMarshaller<Object>.ToNative(nativePtr + LazyObjectPtr_Offset, value);
	}

	public SubclassOf<Object> Class
	{
		get => SubclassOfMarshaller<Object>.FromNative(nativePtr + Class_Offset);
		set => SubclassOfMarshaller<Object>.ToNative(nativePtr + Class_Offset, value);
	}

	public Class? ClassPtr
	{
		get => ObjectMarshaller<Class>.FromNative(nativePtr + ClassPtr_Offset);
		set => ObjectMarshaller<Class>.ToNative(nativePtr + ClassPtr_Offset, value);
	}

	public SoftClassPtr<Object> SoftClassPtr
	{
		get => SoftClassPtrMarshaller<Object>.FromNative(nativePtr + SoftClassPtr_Offset);
		set => SoftClassPtrMarshaller<Object>.ToNative(nativePtr + SoftClassPtr_Offset, value);
	}

	public ISsTestChildInterface? Interface
	{
		get => InterfaceMarshaller<ISsTestChildInterface>.FromNative(nativePtr + Interface_Offset);
		set => InterfaceMarshaller<ISsTestChildInterface>.ToNative(nativePtr + Interface_Offset, value);
	}

	public SsTestStruct ToManaged()
	{
		return new SsTestStruct()
		{
			Bool = Bool,
			BitfieldBoolA = BitfieldBoolA,
			BitfieldBoolB = BitfieldBoolB,
			Int = Int,
			Float = Float,
			Enum = Enum,
			LongEnum = LongEnum,
			String = String,
			Name = Name,
			Text = Text,
			FieldPath = FieldPath,
			StructFieldPath = StructFieldPath,
			StringArray = StringArray,
			StringSet = StringSet,
			StringIntMap = StringIntMap,
			Struct = Struct,
			Object = Object,
			ObjectPtr = ObjectPtr,
			SoftObjectPtr = SoftObjectPtr,
			WeakObjectPtr = WeakObjectPtr,
			LazyObjectPtr = LazyObjectPtr,
			Class = Class,
			ClassPtr = ClassPtr,
			SoftClassPtr = SoftClassPtr,
			Interface = Interface,
		};
	}

	public void FromManaged(in SsTestStruct value)
	{
		Bool = value.Bool;
		BitfieldBoolA = value.BitfieldBoolA;
		BitfieldBoolB = value.BitfieldBoolB;
		Int = value.Int;
		Float = value.Float;
		Enum = value.Enum;
		LongEnum = value.LongEnum;
		String = value.String;
		Name = value.Name;
		Text = value.Text;
		FieldPath = value.FieldPath;
		StructFieldPath = value.StructFieldPath;
		StringArray.CopyFrom(value.StringArray);
		StringSet.CopyFrom(value.StringSet);
		StringIntMap.CopyFrom(value.StringIntMap);
		Struct.FromManaged(value.Struct);
		Object = value.Object;
		ObjectPtr = value.ObjectPtr;
		SoftObjectPtr = value.SoftObjectPtr;
		WeakObjectPtr = value.WeakObjectPtr;
		LazyObjectPtr = value.LazyObjectPtr;
		Class = value.Class;
		ClassPtr = value.ClassPtr;
		SoftClassPtr = value.SoftClassPtr;
		Interface = value.Interface;
	}

	public static IStructNativeRef<SsTestStruct> CreateInstance(IntPtr valuePtr)
	{
		return new SsTestStructNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator SsTestStruct(SsTestStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

public struct SsTestStruct : IStructMarshallerHelper<SsTestStruct>
{
	public bool Bool;

	public bool BitfieldBoolA;

	public bool BitfieldBoolB;

	public int Int;

	public double Float;

	public ESsTestEnum Enum;

	public ESsTestLongEnum LongEnum;

	public string String;

	public Name Name;

	public Text Text;

	public FieldPath FieldPath;

	public FieldPath StructFieldPath;

	public List<string> StringArray;

	public HashSet<string> StringSet;

	public Dictionary<string, int> StringIntMap;

	public SsArrayTestInnerStruct Struct;

	public Object? Object;

	public Object? ObjectPtr;

	public SoftObjectPtr<Object> SoftObjectPtr;

	public Object? WeakObjectPtr;

	public LazyObjectPtr<Object> LazyObjectPtr;

	public SubclassOf<Object> Class;

	public Class? ClassPtr;

	public SoftClassPtr<Object> SoftClassPtr;

	public ISsTestChildInterface? Interface;

	public static int GetNativeDataSize()
	{
		return SsTestStructNativeRef.GetNativeDataSize();
	}

	public static IStructNativeRef<SsTestStruct> CreateStructNativeRef(IntPtr valuePtr)
	{
		return new SsTestStructNativeRef(valuePtr);
	}
}
