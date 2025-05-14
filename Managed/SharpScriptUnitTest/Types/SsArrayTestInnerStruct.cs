using SharpScript;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class FSsArrayTestInnerStructNativeRef(IntPtr nativePtr)
	: IStructNativeRef<FSsArrayTestInnerStruct>
{
	// ReSharper disable InconsistentNaming
	public static readonly IntPtr NativeType;
	public static readonly int NativeDataSize;
	internal static readonly IntPtr IntArray_NativeProp;
	internal static readonly int IntArray_Offset;
	// ReSharper restore InconsistentNaming

	static FSsArrayTestInnerStructNativeRef()
	{
		NativeType = TypeInterop.FindStruct("SsArrayTestInnerStruct");
		NativeDataSize = TypeInterop.GetStructureSize(NativeType);
		PropertyIterator propIter = new PropertyIterator(NativeType);

		IntArray_NativeProp = propIter.FindNext("IntArray");
		IntArray_Offset = TypeInterop.GetPropertyOffset(IntArray_NativeProp);
	}

	private TArray<int>? _intArray;
	public TArray<int> IntArray => _intArray ??= new(nativePtr + IntArray_Offset, IntArray_NativeProp, BlittableMarshaller<int>.Instance);

	public FSsArrayTestInnerStruct ToManaged()
	{
		return new FSsArrayTestInnerStruct()
		{
			IntArray = IntArray,
		};
	}

	public void FromManaged(in FSsArrayTestInnerStruct value)
	{
		IntArray.CopyFrom(value.IntArray);
	}

	public static IStructNativeRef<FSsArrayTestInnerStruct> CreateInstance(IntPtr valuePtr)
	{
		return new FSsArrayTestInnerStructNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator FSsArrayTestInnerStruct(FSsArrayTestInnerStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

public struct FSsArrayTestInnerStruct : IEquatable<FSsArrayTestInnerStruct>
{
	public List<int> IntArray;

	public bool Equals(FSsArrayTestInnerStruct other)
	{
		return IntArray.SequenceEqual(other.IntArray);
	}

	public override bool Equals(object? obj)
	{
		return obj is FSsArrayTestInnerStruct other && Equals(other);
	}

	public override int GetHashCode()
	{
		return IntArray.GetHashCode();
	}
}
