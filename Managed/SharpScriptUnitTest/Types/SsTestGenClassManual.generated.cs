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

	static USsTestGenClassManual()
	{
		PropertyDef[] _propertyDefs =
		[
			new()
			{
				PropName = "Bool",
				PropType = UBoolProperty.StaticClass.NativeClass
			},
			new()
			{
				PropName = "Int",
				PropType = UIntProperty.StaticClass.NativeClass
			},
			new()
			{
				PropName = "Float",
				PropType = UDoubleProperty.StaticClass.NativeClass
			},
			new()
			{
				PropName = "String",
				PropType = UStrProperty.StaticClass.NativeClass
			},
			new()
			{
				PropName = "Name",
				PropType = UNameProperty.StaticClass.NativeClass
			},
			new()
			{
				PropName = "Text",
				PropType = UTextProperty.StaticClass.NativeClass
			}
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
		HouseKeeper.AddBindedUnrealClass(StaticClass.Class, typeof(USsTestGenClassManual));

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
}
