#nullable enable
using SharpScript;
using SharpScript.Interop;
using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class FSsTestGenStructManualNativeRef(IntPtr nativePtr)
	: IStructNativeRef<FSsTestGenStructManual>
{
	public static readonly IntPtr NativeType;
	public static readonly int NativeDataSize;
	private static readonly IntPtr Bool_NativeProp;
	private static readonly int Bool_Offset;
	private static readonly IntPtr Int_NativeProp;
	private static readonly int Int_Offset;
	private static readonly IntPtr Float_NativeProp;
	private static readonly int Float_Offset;
	private static readonly IntPtr Enum_NativeProp;
	private static readonly int Enum_Offset;
	private static readonly IntPtr String_NativeProp;
	private static readonly int String_Offset;
	private static readonly IntPtr Name_NativeProp;
	private static readonly int Name_Offset;
	private static readonly IntPtr Text_NativeProp;
	private static readonly int Text_Offset;
	private static readonly IntPtr StringArray_NativeProp;
	private static readonly int StringArray_Offset;
	private static readonly IntPtr StringSet_NativeProp;
	private static readonly int StringSet_Offset;
	private static readonly IntPtr StringIntMap_NativeProp;
	private static readonly int StringIntMap_Offset;
	private static readonly IntPtr Struct_NativeProp;
	private static readonly int Struct_Offset;
	private static readonly IntPtr Object_NativeProp;
	private static readonly int Object_Offset;
	private static readonly IntPtr SoftObjectPtr_NativeProp;
	private static readonly int SoftObjectPtr_Offset;
	private static readonly IntPtr LazyObjectPtr_NativeProp;
	private static readonly int LazyObjectPtr_Offset;
	private static readonly IntPtr Class_NativeProp;
	private static readonly int Class_Offset;
	private static readonly IntPtr SoftClassPtr_NativeProp;
	private static readonly int SoftClassPtr_Offset;
	private static readonly IntPtr StructArray_NativeProp;
	private static readonly int StructArray_Offset;
	private static readonly IntPtr IntStructMap_NativeProp;
	private static readonly int IntStructMap_Offset;
	private static readonly IntPtr BlittableStructArray_NativeProp;
	private static readonly int BlittableStructArray_Offset;
	private static readonly IntPtr IntBlittableStructMap_NativeProp;
	private static readonly int IntBlittableStructMap_Offset;

	static FSsTestGenStructManualNativeRef()
	{
		PropertyDef[] _propertyDefs =
		[
			new()
			{
				PropName = "Bool",
				PropType = UBoolProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "Int",
				PropType = UIntProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "Float",
				PropType = UDoubleProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "Enum",
				PropType = UByteProperty.StaticClass.NativeClass,
				UnderlyingType = ESsTestGenEnumManualNativeRef.NativeType,
			},
			new()
			{
				PropName = "String",
				PropType = UStrProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "Name",
				PropType = UNameProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "Text",
				PropType = UTextProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "StringArray",
				PropType = UArrayProperty.StaticClass.NativeClass,
				InnerPropType = UStrProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "StringSet",
				PropType = USetProperty.StaticClass.NativeClass,
				InnerPropType = UStrProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "StringIntMap",
				PropType = UMapProperty.StaticClass.NativeClass,
				InnerPropType = UIntProperty.StaticClass.NativeClass,
				KeyPropType = UStrProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "Struct",
				PropType = UStructProperty.StaticClass.NativeClass,
				UnderlyingType = FSsArrayTestInnerGenStructManualNativeRef.NativeType,
			},
			new()
			{
				PropName = "Object",
				PropType = UObjectProperty.StaticClass.NativeClass,
				UnderlyingType = UObject.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "SoftObjectPtr",
				PropType = USoftObjectProperty.StaticClass.NativeClass,
				UnderlyingType = UObject.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "LazyObjectPtr",
				PropType = ULazyObjectProperty.StaticClass.NativeClass,
				UnderlyingType = UObject.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "Class",
				PropType = UClassProperty.StaticClass.NativeClass,
				UnderlyingType = UObject.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "SoftClassPtr",
				PropType = USoftClassProperty.StaticClass.NativeClass,
				UnderlyingType = UObject.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "StructArray",
				PropType = UArrayProperty.StaticClass.NativeClass,
				InnerPropType = UStructProperty.StaticClass.NativeClass,
				InnerUnderlyingType = FSsArrayTestInnerGenStructManualNativeRef.NativeType,
			},
			new()
			{
				PropName = "IntStructMap",
				PropType = UMapProperty.StaticClass.NativeClass,
				InnerPropType = UStructProperty.StaticClass.NativeClass,
				InnerUnderlyingType = FSsArrayTestInnerGenStructManualNativeRef.NativeType,
				KeyPropType = UIntProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "BlittableStructArray",
				PropType = UArrayProperty.StaticClass.NativeClass,
				InnerPropType = UStructProperty.StaticClass.NativeClass,
				InnerUnderlyingType = FSsTestBlittableGenStructManualNativeRef.NativeType,
			},
			new()
			{
				PropName = "IntBlittableStructMap",
				PropType = UMapProperty.StaticClass.NativeClass,
				InnerPropType = UStructProperty.StaticClass.NativeClass,
				InnerUnderlyingType = FSsTestBlittableGenStructManualNativeRef.NativeType,
				KeyPropType = UIntProperty.StaticClass.NativeClass,
			},
		];

		unsafe
		{
			fixed (PropertyDef* _propertyDefsPtr = _propertyDefs)
			{
				StructDef _structDef = new()
				{
					StructName = "SsTestGenStructManual",
					PropertyDefines = (IntPtr)_propertyDefsPtr,
					PropertyCount = _propertyDefs.Length,
				};
				NativeType = SubclassingUtils.GenerateStruct((IntPtr)(&_structDef));
			}
		}

		NativeDataSize = TypeInterop.GetStructureSize(NativeType);
		PropertyIterator propIter = new PropertyIterator(NativeType);
		Bool_NativeProp = propIter.FindNext("Bool");
		Bool_Offset = TypeInterop.GetPropertyOffset(Bool_NativeProp);
		Int_NativeProp = propIter.FindNext("Int");
		Int_Offset = TypeInterop.GetPropertyOffset(Int_NativeProp);
		Float_NativeProp = propIter.FindNext("Float");
		Float_Offset = TypeInterop.GetPropertyOffset(Float_NativeProp);
		Enum_NativeProp = propIter.FindNext("Enum");
		Enum_Offset = TypeInterop.GetPropertyOffset(Enum_NativeProp);
		String_NativeProp = propIter.FindNext("String");
		String_Offset = TypeInterop.GetPropertyOffset(String_NativeProp);
		Name_NativeProp = propIter.FindNext("Name");
		Name_Offset = TypeInterop.GetPropertyOffset(Name_NativeProp);
		Text_NativeProp = propIter.FindNext("Text");
		Text_Offset = TypeInterop.GetPropertyOffset(Text_NativeProp);
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
		SoftObjectPtr_NativeProp = propIter.FindNext("SoftObjectPtr");
		SoftObjectPtr_Offset = TypeInterop.GetPropertyOffset(SoftObjectPtr_NativeProp);
		LazyObjectPtr_NativeProp = propIter.FindNext("LazyObjectPtr");
		LazyObjectPtr_Offset = TypeInterop.GetPropertyOffset(LazyObjectPtr_NativeProp);
		Class_NativeProp = propIter.FindNext("Class");
		Class_Offset = TypeInterop.GetPropertyOffset(Class_NativeProp);
		SoftClassPtr_NativeProp = propIter.FindNext("SoftClassPtr");
		SoftClassPtr_Offset = TypeInterop.GetPropertyOffset(SoftClassPtr_NativeProp);
		StructArray_NativeProp = propIter.FindNext("StructArray");
		StructArray_Offset = TypeInterop.GetPropertyOffset(StructArray_NativeProp);
		IntStructMap_NativeProp = propIter.FindNext("IntStructMap");
		IntStructMap_Offset = TypeInterop.GetPropertyOffset(IntStructMap_NativeProp);
		BlittableStructArray_NativeProp = propIter.FindNext("BlittableStructArray");
		BlittableStructArray_Offset = TypeInterop.GetPropertyOffset(BlittableStructArray_NativeProp);
		IntBlittableStructMap_NativeProp = propIter.FindNext("IntBlittableStructMap");
		IntBlittableStructMap_Offset = TypeInterop.GetPropertyOffset(IntBlittableStructMap_NativeProp);
	}

	public bool Bool
	{
		get => BoolMarshaller.FromNative(nativePtr + Bool_Offset);
		set => BoolMarshaller.ToNative(nativePtr + Bool_Offset, value);
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

	public ESsTestGenEnumManual Enum
	{
		get => EnumMarshaller<ESsTestGenEnumManual>.FromNative(nativePtr + Enum_Offset);
		set => EnumMarshaller<ESsTestGenEnumManual>.ToNative(nativePtr + Enum_Offset, value);
	}

	public string String
	{
		get => StringMarshaller.FromNative(nativePtr + String_Offset);
		set => StringMarshaller.ToNative(nativePtr + String_Offset, value);
	}

	public FName Name
	{
		get => NameMarshaller.FromNative(nativePtr + Name_Offset);
		set => NameMarshaller.ToNative(nativePtr + Name_Offset, value);
	}

	public FText Text
	{
		get => TextMarshaller.FromNative(nativePtr + Text_Offset);
		set => TextMarshaller.ToNative(nativePtr + Text_Offset, value);
	}

	private TArray<string>? _stringArray;

	public TArray<string> StringArray => _stringArray ??=
		new(nativePtr + StringArray_Offset, StringArray_NativeProp, StringMarshaller.Instance);

	private TSet<string>? _stringSet;

	public TSet<string> StringSet => _stringSet ??=
		new(nativePtr + StringSet_Offset, StringSet_NativeProp, StringMarshaller.Instance);

	private TMap<string, int>? _stringIntMap;

	public TMap<string, int> StringIntMap => _stringIntMap ??=
		new(nativePtr + StringIntMap_Offset, StringIntMap_NativeProp, StringMarshaller.Instance, BlittableMarshaller<int>.Instance);

	private FSsArrayTestInnerGenStructManualNativeRef? _struct;

	public FSsArrayTestInnerGenStructManualNativeRef Struct => _struct ??= new(nativePtr + Struct_Offset);

	public UObject? Object
	{
		get => ObjectMarshaller<UObject>.FromNative(nativePtr + Object_Offset);
		set => ObjectMarshaller<UObject>.ToNative(nativePtr + Object_Offset, value);
	}

	public TSoftObjectPtr<UObject> SoftObjectPtr
	{
		get => SoftObjectPtrMarshaller<UObject>.FromNative(nativePtr + SoftObjectPtr_Offset);
		set => SoftObjectPtrMarshaller<UObject>.ToNative(nativePtr + SoftObjectPtr_Offset, value);
	}

	public TLazyObjectPtr<UObject> LazyObjectPtr
	{
		get => LazyObjectPtrMarshaller<UObject>.FromNative(nativePtr + LazyObjectPtr_Offset);
		set => LazyObjectPtrMarshaller<UObject>.ToNative(nativePtr + LazyObjectPtr_Offset, value);
	}

	public TSubclassOf<UObject> Class
	{
		get => SubclassOfMarshaller<UObject>.FromNative(nativePtr + Class_Offset);
		set => SubclassOfMarshaller<UObject>.ToNative(nativePtr + Class_Offset, value);
	}

	public TSoftClassPtr<UObject> SoftClassPtr
	{
		get => SoftClassPtrMarshaller<UObject>.FromNative(nativePtr + SoftClassPtr_Offset);
		set => SoftClassPtrMarshaller<UObject>.ToNative(nativePtr + SoftClassPtr_Offset, value);
	}

	private TArray<FSsArrayTestInnerGenStructManual, FSsArrayTestInnerGenStructManualNativeRef>? _structArray;

	public TArray<FSsArrayTestInnerGenStructManual, FSsArrayTestInnerGenStructManualNativeRef> StructArray => _structArray ??=
		new(nativePtr + StructArray_Offset, StructArray_NativeProp);

	private TMap<int, FSsArrayTestInnerGenStructManual, FSsArrayTestInnerGenStructManualNativeRef>? _intStructMap;

	public TMap<int, FSsArrayTestInnerGenStructManual, FSsArrayTestInnerGenStructManualNativeRef> IntStructMap => _intStructMap ??=
		new(nativePtr + IntStructMap_Offset, IntStructMap_NativeProp, BlittableMarshaller<int>.Instance);

	private TArray<FSsTestBlittableGenStructManual, FSsTestBlittableGenStructManualNativeRef>? _blittableStructArray;

	public TArray<FSsTestBlittableGenStructManual, FSsTestBlittableGenStructManualNativeRef> BlittableStructArray => _blittableStructArray ??=
		new(nativePtr + BlittableStructArray_Offset, BlittableStructArray_NativeProp);

	private TMap<int, FSsTestBlittableGenStructManual, FSsTestBlittableGenStructManualNativeRef>? _intBlittableStructMap;

	public TMap<int, FSsTestBlittableGenStructManual, FSsTestBlittableGenStructManualNativeRef> IntBlittableStructMap => _intBlittableStructMap ??=
		new(nativePtr + IntBlittableStructMap_Offset, IntBlittableStructMap_NativeProp, BlittableMarshaller<int>.Instance);

	public FSsTestGenStructManual ToManaged()
	{
		return new FSsTestGenStructManual()
		{
			Bool = Bool,
			Int = Int,
			Float = Float,
			Enum = Enum,
			String = String,
			Name = Name,
			Text = Text,
			StringArray = StringArray,
			StringSet = StringSet,
			StringIntMap = StringIntMap,
			Struct = Struct,
			Object = Object,
			SoftObjectPtr = SoftObjectPtr,
			LazyObjectPtr = LazyObjectPtr,
			Class = Class,
			SoftClassPtr = SoftClassPtr,
			StructArray = StructArray,
			IntStructMap = IntStructMap,
			BlittableStructArray = BlittableStructArray,
			IntBlittableStructMap = IntBlittableStructMap
		};
	}

	public void FromManaged(in FSsTestGenStructManual value)
	{
		Bool = value.Bool;
		Int = value.Int;
		Float = value.Float;
		Enum = value.Enum;
		String = value.String;
		Name = value.Name;
		Text = value.Text;
		StringArray.CopyFrom(value.StringArray);
		StringSet.CopyFrom(value.StringSet);
		StringIntMap.CopyFrom(value.StringIntMap);
		Struct.FromManaged(value.Struct);
		Object = value.Object;
		SoftObjectPtr = value.SoftObjectPtr;
		LazyObjectPtr = value.LazyObjectPtr;
		Class = value.Class;
		SoftClassPtr = value.SoftClassPtr;
		StructArray.CopyFrom(value.StructArray);
		IntStructMap.CopyFrom(value.IntStructMap);
		BlittableStructArray.CopyFrom(value.BlittableStructArray);
		IntBlittableStructMap.CopyFrom(value.IntBlittableStructMap);
	}

	public static IStructNativeRef<FSsTestGenStructManual> CreateInstance(IntPtr valuePtr)
	{
		return new FSsTestGenStructManualNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator FSsTestGenStructManual(FSsTestGenStructManualNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

public partial struct FSsTestGenStructManual : IStructMarshallerHelper<FSsTestGenStructManual>
{
	public static int GetNativeDataSize()
	{
		return FSsTestGenStructManualNativeRef.GetNativeDataSize();
	}

	public static IStructNativeRef<FSsTestGenStructManual> CreateStructNativeRef(IntPtr valuePtr)
	{
		return new FSsTestGenStructManualNativeRef(valuePtr);
	}
}
