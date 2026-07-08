#nullable enable
using SharpScript;
using SharpScript.Interop;
using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class FSsArrayTestInnerGenStructManualNativeRef(IntPtr nativePtr)
	: IStructNativeRef<FSsArrayTestInnerGenStructManual>
{
	public static readonly IntPtr NativeType;
	public static readonly int NativeDataSize;
	internal static readonly IntPtr IntArray_NativeProp;
	internal static readonly int IntArray_Offset;

	static FSsArrayTestInnerGenStructManualNativeRef()
	{
		PropertyDef[] _propertyDefs =
		[
			new()
			{
				PropName = "IntArray",
				PropType = UArrayProperty.StaticClass.NativeClass,
				InnerPropType = UIntProperty.StaticClass.NativeClass
			},
		];

		unsafe
		{
			fixed (PropertyDef* _propertyDefsPtr = _propertyDefs)
			{
				StructDef _structDef = new()
				{
					StructName = "SsArrayTestInnerGenStructManual",
					PropertyDefines = (IntPtr)_propertyDefsPtr,
					PropertyCount = _propertyDefs.Length,
				};
				NativeType = SubclassingUtils.GenerateStruct((IntPtr)(&_structDef));
			}
		}

		NativeDataSize = TypeInterop.GetStructureSize(NativeType);
		PropertyIterator propIter = new PropertyIterator(NativeType);

		IntArray_NativeProp = propIter.FindNext("IntArray");
		IntArray_Offset = TypeInterop.GetPropertyOffset(IntArray_NativeProp);
	}

	private TArray<int>? _intArray;

	public TArray<int> IntArray => _intArray ??=
		new(nativePtr + IntArray_Offset, IntArray_NativeProp, BlittableMarshaller<int>.Instance);

	public FSsArrayTestInnerGenStructManual ToManaged()
	{
		return new FSsArrayTestInnerGenStructManual()
		{
			IntArray = IntArray
		};
	}

	public void FromManaged(in FSsArrayTestInnerGenStructManual value)
	{
		IntArray.CopyFrom(value.IntArray);
	}

	public static IStructNativeRef<FSsArrayTestInnerGenStructManual> CreateInstance(IntPtr valuePtr)
	{
		return new FSsArrayTestInnerGenStructManualNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator FSsArrayTestInnerGenStructManual(
		FSsArrayTestInnerGenStructManualNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}
