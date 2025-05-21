#nullable enable
using SharpScript;
using SharpScript.Interop;
using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public partial class USsTestGenClassManual : IStaticClass<USsTestGenClassManual>
{
	public new static TSubclassOf<USsTestGenClassManual> StaticClass { get; }

	private new static readonly IntPtr NativeType;
	private static readonly IntPtr Bool_NativeProp;
	private static readonly int Bool_Offset;
	private static readonly IntPtr Int_NativeProp;
	private static readonly int Int_Offset;
	private static readonly IntPtr Float_NativeProp;
	private static readonly int Float_Offset;
	// private static readonly IntPtr Enum_NativeProp;
	// private static readonly int Enum_Offset;
	// private static readonly IntPtr LongEnum_NativeProp;
	// private static readonly int LongEnum_Offset;
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

	static USsTestGenClassManual()
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
				PropName = "StringArray",
				PropType = UArrayProperty.StaticClass.NativeClass,
				InnerPropType = UStrProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "StringSet",
				PropType = UArrayProperty.StaticClass.NativeClass,
				InnerPropType = UStrProperty.StaticClass.NativeClass,
			},
			new()
			{
				PropName = "StringIntMap",
				PropType = UArrayProperty.StaticClass.NativeClass,
				InnerPropType = UIntProperty.StaticClass.NativeClass,
				KeyPropType = UStrProperty.StaticClass.NativeClass,
			},
		];

		unsafe
		{
			fixed (PropertyDef* _propertyDefsPtr = _propertyDefs)
			{
				NativeType = SubclassingUtils.GenerateClass(
					RuntimeTypeHandle.ToIntPtr(typeof(USsTestGenClassManual).TypeHandle),
					"SsTestGenClassManual",
					UObject.StaticClass.NativeClass,
					(IntPtr)_propertyDefsPtr, _propertyDefs.Length);
			}
		}

		StaticClass = new TSubclassOf<USsTestGenClassManual>(NativeType);
		HouseKeeper.AddBindedUnrealClass(StaticClass.Class!, typeof(USsTestGenClassManual));

		PropertyIterator propIter = new PropertyIterator(NativeType);
		Bool_NativeProp = propIter.FindNext("Bool");
		Bool_Offset = TypeInterop.GetPropertyOffset(Bool_NativeProp);
		Int_NativeProp = propIter.FindNext("Int");
		Int_Offset = TypeInterop.GetPropertyOffset(Int_NativeProp);
		Float_NativeProp = propIter.FindNext("Float");
		Float_Offset = TypeInterop.GetPropertyOffset(Float_NativeProp);
		// Enum_NativeProp = propIter.FindNext("Enum");
		// Enum_Offset = TypeInterop.GetPropertyOffset(Enum_NativeProp);
		// LongEnum_NativeProp = propIter.FindNext("LongEnum");
		// LongEnum_Offset = TypeInterop.GetPropertyOffset(LongEnum_NativeProp);
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
	}

	public partial bool Bool
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

	public partial int Int
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

	public partial double Float
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

	public partial string String
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

	public partial FName Name
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

	public partial FText Text
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

	private TArray<string>? _stringArray;

	public partial TArray<string> StringArray
	{
		get
		{
			ThrowIfNotValid();
			return _stringArray ??= new(NativeObject + StringArray_Offset, StringArray_NativeProp, StringMarshaller.Instance);
		}
	}

	private TSet<string>? _stringSet;

	public partial TSet<string> StringSet
	{
		get
		{
			ThrowIfNotValid();
			return _stringSet ??= new(NativeObject + StringSet_Offset, StringSet_NativeProp, StringMarshaller.Instance);
		}
	}

	private TMap<string, int>? _stringIntMap;

	public partial TMap<string, int> StringIntMap
	{
		get
		{
			ThrowIfNotValid();
			return _stringIntMap ??= new(NativeObject + StringIntMap_Offset, StringIntMap_NativeProp, StringMarshaller.Instance, BlittableMarshaller<int>.Instance);
		}
	}

	public partial UObject? Object
	{
		get
		{
			ThrowIfNotValid();
			return ObjectMarshaller<UObject>.FromNative(NativeObject + Object_Offset);
		}
		set
		{
			ThrowIfNotValid();
			ObjectMarshaller<UObject>.ToNative(NativeObject + Object_Offset, value);
		}
	}

	public partial TSoftObjectPtr<UObject> SoftObjectPtr
	{
		get
		{
			ThrowIfNotValid();
			return SoftObjectPtrMarshaller<UObject>.FromNative(NativeObject + SoftObjectPtr_Offset);
		}
		set
		{
			ThrowIfNotValid();
			SoftObjectPtrMarshaller<UObject>.ToNative(NativeObject + SoftObjectPtr_Offset, value);
		}
	}

	public partial TLazyObjectPtr<UObject> LazyObjectPtr
	{
		get
		{
			ThrowIfNotValid();
			return LazyObjectPtrMarshaller<UObject>.FromNative(NativeObject + LazyObjectPtr_Offset);
		}
		set
		{
			ThrowIfNotValid();
			LazyObjectPtrMarshaller<UObject>.ToNative(NativeObject + LazyObjectPtr_Offset, value);
		}
	}

	public partial TSubclassOf<UObject> Class
	{
		get
		{
			ThrowIfNotValid();
			return SubclassOfMarshaller<UObject>.FromNative(NativeObject + Class_Offset);
		}
		set
		{
			ThrowIfNotValid();
			SubclassOfMarshaller<UObject>.ToNative(NativeObject + Class_Offset, value);
		}
	}

	public partial TSoftClassPtr<UObject> SoftClassPtr
	{
		get
		{
			ThrowIfNotValid();
			return SoftClassPtrMarshaller<UObject>.FromNative(NativeObject + SoftClassPtr_Offset);
		}
		set
		{
			ThrowIfNotValid();
			SoftClassPtrMarshaller<UObject>.ToNative(NativeObject + SoftClassPtr_Offset, value);
		}
	}
}
