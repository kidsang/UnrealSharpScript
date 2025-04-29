using SharpScript;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class SsArrayTestInnerStructNativeRef(IntPtr nativePtr)
	: IStructNativeRef<SsArrayTestInnerStruct>
{
	// ReSharper disable InconsistentNaming
	public static readonly IntPtr NativeType;
	public static readonly int NativeDataSize;
	internal static readonly IntPtr IntArray_NativeProp;
	internal static readonly int IntArray_Offset;
	// ReSharper restore InconsistentNaming

	static SsArrayTestInnerStructNativeRef()
	{
		NativeType = TypeInterop.FindStruct("SsArrayTestInnerStruct");
		NativeDataSize = TypeInterop.GetStructureSize(NativeType);
		PropertyIterator propIter = new PropertyIterator(NativeType);

		IntArray_NativeProp = propIter.FindNext("IntArray");
		IntArray_Offset = TypeInterop.GetPropertyOffset(IntArray_NativeProp);
	}

	private Array<int>? _intArray;
	public Array<int> IntArray => _intArray ??= new(nativePtr + IntArray_Offset, IntArray_NativeProp, BlittableMarshaller<int>.Instance);

	public SsArrayTestInnerStruct ToManaged()
	{
		return new SsArrayTestInnerStruct()
		{
			IntArray = IntArray,
		};
	}

	public void FromManaged(in SsArrayTestInnerStruct value)
	{
		IntArray.CopyFrom(value.IntArray);
	}

	public static IStructNativeRef<SsArrayTestInnerStruct> CreateInstance(IntPtr valuePtr)
	{
		return new SsArrayTestInnerStructNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator SsArrayTestInnerStruct(SsArrayTestInnerStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

public struct SsArrayTestInnerStruct : IEquatable<SsArrayTestInnerStruct>
{
	public List<int> IntArray;

	public bool Equals(SsArrayTestInnerStruct other)
	{
		return IntArray.SequenceEqual(other.IntArray);
	}

	public override bool Equals(object? obj)
	{
		return obj is SsArrayTestInnerStruct other && Equals(other);
	}

	public override int GetHashCode()
	{
		return IntArray.GetHashCode();
	}
}
