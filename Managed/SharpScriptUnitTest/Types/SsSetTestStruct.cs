using SharpScript;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class SsSetTestStructNativeRef(IntPtr nativePtr)
	: IStructNativeRef<SsSetTestStruct>
{
	// ReSharper disable InconsistentNaming
	public static readonly IntPtr NativeType;
	public static readonly int NativeDataSize;
	internal static readonly IntPtr IntSet_NativeProp;
	internal static readonly int IntSet_Offset;
	internal static readonly IntPtr StructSet_NativeProp;
	internal static readonly int StructSet_Offset;
	// ReSharper restore InconsistentNaming

	static SsSetTestStructNativeRef()
	{
		NativeType = TypeInterop.FindStruct("SsSetTestStruct");
		NativeDataSize = TypeInterop.GetStructureSize(NativeType);
		PropertyIterator propIter = new PropertyIterator(NativeType);

		IntSet_NativeProp = propIter.FindNext("IntSet");
		IntSet_Offset = TypeInterop.GetPropertyOffset(IntSet_NativeProp);

		StructSet_NativeProp = propIter.FindNext("StructSet");
		StructSet_Offset = TypeInterop.GetPropertyOffset(StructSet_NativeProp);
	}

	private Set<int>? _intSet;
	public Set<int> IntSet => _intSet ??= new(nativePtr + IntSet_Offset, IntSet_NativeProp, BlittableMarshaller<int>.Instance);

	private Set<SsTestBlittableStruct>? _structSet;
	public Set<SsTestBlittableStruct> StructSet => _structSet ??= new(nativePtr + StructSet_Offset, StructSet_NativeProp, BlittableMarshaller<SsTestBlittableStruct>.Instance);

	public SsSetTestStruct ToManaged()
	{
		return new SsSetTestStruct()
		{
			IntSet = IntSet,
		};
	}

	public void FromManaged(in SsSetTestStruct value)
	{
		IntSet.CopyFrom(value.IntSet);
	}

	public static IStructNativeRef<SsSetTestStruct> CreateInstance(IntPtr valuePtr)
	{
		return new SsSetTestStructNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator SsSetTestStruct(SsSetTestStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

public struct SsSetTestStruct
{
	public HashSet<int> IntSet;
}
