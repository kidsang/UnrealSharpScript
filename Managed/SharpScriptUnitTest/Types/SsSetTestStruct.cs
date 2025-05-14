using SharpScript;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class FSsSetTestStructNativeRef(IntPtr nativePtr)
	: IStructNativeRef<FSsSetTestStruct>
{
	// ReSharper disable InconsistentNaming
	public static readonly IntPtr NativeType;
	public static readonly int NativeDataSize;
	internal static readonly IntPtr IntSet_NativeProp;
	internal static readonly int IntSet_Offset;
	internal static readonly IntPtr StructSet_NativeProp;
	internal static readonly int StructSet_Offset;
	// ReSharper restore InconsistentNaming

	static FSsSetTestStructNativeRef()
	{
		NativeType = TypeInterop.FindStruct("SsSetTestStruct");
		NativeDataSize = TypeInterop.GetStructureSize(NativeType);
		PropertyIterator propIter = new PropertyIterator(NativeType);

		IntSet_NativeProp = propIter.FindNext("IntSet");
		IntSet_Offset = TypeInterop.GetPropertyOffset(IntSet_NativeProp);

		StructSet_NativeProp = propIter.FindNext("StructSet");
		StructSet_Offset = TypeInterop.GetPropertyOffset(StructSet_NativeProp);
	}

	private TSet<int>? _intSet;
	public TSet<int> IntSet => _intSet ??= new(nativePtr + IntSet_Offset, IntSet_NativeProp, BlittableMarshaller<int>.Instance);

	private TSet<FSsTestBlittableStruct>? _structSet;
	public TSet<FSsTestBlittableStruct> StructSet => _structSet ??= new(nativePtr + StructSet_Offset, StructSet_NativeProp, BlittableMarshaller<FSsTestBlittableStruct>.Instance);

	public FSsSetTestStruct ToManaged()
	{
		return new FSsSetTestStruct()
		{
			IntSet = IntSet,
		};
	}

	public void FromManaged(in FSsSetTestStruct value)
	{
		IntSet.CopyFrom(value.IntSet);
	}

	public static IStructNativeRef<FSsSetTestStruct> CreateInstance(IntPtr valuePtr)
	{
		return new FSsSetTestStructNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator FSsSetTestStruct(FSsSetTestStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

public struct FSsSetTestStruct
{
	public HashSet<int> IntSet;
}
