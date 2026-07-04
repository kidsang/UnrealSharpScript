#nullable enable
using SharpScript;
using SharpScript.Interop;
using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public partial class USsTestGenEnumContainerClassManual : IStaticClass<USsTestGenEnumContainerClassManual>
{
	public new static TSubclassOf<USsTestGenEnumContainerClassManual> StaticClass { get; }

	private new static readonly IntPtr NativeType;
	private static readonly IntPtr EnumArray_NativeProp;
	private static readonly int EnumArray_Offset;
	private static readonly IntPtr EnumSet_NativeProp;
	private static readonly int EnumSet_Offset;
	private static readonly IntPtr EnumKeyMap_NativeProp;
	private static readonly int EnumKeyMap_Offset;
	private static readonly IntPtr EnumValueMap_NativeProp;
	private static readonly int EnumValueMap_Offset;

	static USsTestGenEnumContainerClassManual()
	{
		PropertyDef[] _propertyDefs =
		[
			new()
			{
				PropName = "EnumArray",
				PropType = UArrayProperty.StaticClass.NativeClass,
				InnerPropType = UByteProperty.StaticClass.NativeClass,
				InnerUnderlyingType = ESsTestGenEnumManualNativeRef.NativeType,
			},
			new()
			{
				PropName = "EnumSet",
				PropType = USetProperty.StaticClass.NativeClass,
				InnerPropType = UByteProperty.StaticClass.NativeClass,
				InnerUnderlyingType = ESsTestGenEnumManualNativeRef.NativeType,
			},
			new()
			{
				PropName = "EnumKeyMap",
				PropType = UMapProperty.StaticClass.NativeClass,
				InnerPropType = UIntProperty.StaticClass.NativeClass,
				KeyPropType = UByteProperty.StaticClass.NativeClass,
				KeyUnderlyingType = ESsTestGenEnumManualNativeRef.NativeType,
			},
			new()
			{
				PropName = "EnumValueMap",
				PropType = UMapProperty.StaticClass.NativeClass,
				InnerPropType = UByteProperty.StaticClass.NativeClass,
				InnerUnderlyingType = ESsTestGenEnumManualNativeRef.NativeType,
				KeyPropType = UIntProperty.StaticClass.NativeClass,
			},
		];

		unsafe
		{
			fixed (PropertyDef* _propertyDefsPtr = _propertyDefs)
			{
				NativeType = SubclassingUtils.GenerateClass(
					RuntimeTypeHandle.ToIntPtr(typeof(USsTestGenEnumContainerClassManual).TypeHandle),
					"SsTestGenEnumContainerClassManual",
					UObject.StaticClass.NativeClass,
					(IntPtr)_propertyDefsPtr, _propertyDefs.Length,
					IntPtr.Zero, 0);
			}
		}

		StaticClass = new TSubclassOf<USsTestGenEnumContainerClassManual>(NativeType);
		HouseKeeper.AddBindedUnrealClass(StaticClass.Class!, typeof(USsTestGenEnumContainerClassManual));

		PropertyIterator propIter = new PropertyIterator(NativeType);
		EnumArray_NativeProp = propIter.FindNext("EnumArray");
		EnumArray_Offset = TypeInterop.GetPropertyOffset(EnumArray_NativeProp);
		EnumSet_NativeProp = propIter.FindNext("EnumSet");
		EnumSet_Offset = TypeInterop.GetPropertyOffset(EnumSet_NativeProp);
		EnumKeyMap_NativeProp = propIter.FindNext("EnumKeyMap");
		EnumKeyMap_Offset = TypeInterop.GetPropertyOffset(EnumKeyMap_NativeProp);
		EnumValueMap_NativeProp = propIter.FindNext("EnumValueMap");
		EnumValueMap_Offset = TypeInterop.GetPropertyOffset(EnumValueMap_NativeProp);
	}

	private TArray<ESsTestGenEnumManual>? _enumArray;

	public partial TArray<ESsTestGenEnumManual> EnumArray
	{
		get
		{
			ThrowIfNotValid();
			return _enumArray ??= new(NativeObject + EnumArray_Offset, EnumArray_NativeProp, EnumMarshaller<ESsTestGenEnumManual>.Instance);
		}
	}

	private TSet<ESsTestGenEnumManual>? _enumSet;

	public partial TSet<ESsTestGenEnumManual> EnumSet
	{
		get
		{
			ThrowIfNotValid();
			return _enumSet ??= new(NativeObject + EnumSet_Offset, EnumSet_NativeProp, EnumMarshaller<ESsTestGenEnumManual>.Instance);
		}
	}

	private TMap<ESsTestGenEnumManual, int>? _enumKeyMap;

	public partial TMap<ESsTestGenEnumManual, int> EnumKeyMap
	{
		get
		{
			ThrowIfNotValid();
			return _enumKeyMap ??= new(NativeObject + EnumKeyMap_Offset, EnumKeyMap_NativeProp, EnumMarshaller<ESsTestGenEnumManual>.Instance, BlittableMarshaller<int>.Instance);
		}
	}

	private TMap<int, ESsTestGenEnumManual>? _enumValueMap;

	public partial TMap<int, ESsTestGenEnumManual> EnumValueMap
	{
		get
		{
			ThrowIfNotValid();
			return _enumValueMap ??= new(NativeObject + EnumValueMap_Offset, EnumValueMap_NativeProp, BlittableMarshaller<int>.Instance, EnumMarshaller<ESsTestGenEnumManual>.Instance);
		}
	}
}
