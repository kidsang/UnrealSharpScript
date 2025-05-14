using SharpScript;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class FSsMapTestInnerStructNativeRef(IntPtr nativePtr)
	: IStructNativeRef<FSsMapTestInnerStruct>
{
	// ReSharper disable InconsistentNaming
	public static readonly IntPtr NativeType;
	public static readonly int NativeDataSize;
	internal static readonly IntPtr IntIntMap_NativeProp;
	internal static readonly int IntIntMap_Offset;
	// ReSharper restore InconsistentNaming

	static FSsMapTestInnerStructNativeRef()
	{
		NativeType = TypeInterop.FindStruct("SsMapTestInnerStruct");
		NativeDataSize = TypeInterop.GetStructureSize(NativeType);
		PropertyIterator propIter = new PropertyIterator(NativeType);

		IntIntMap_NativeProp = propIter.FindNext("IntIntMap");
		IntIntMap_Offset = TypeInterop.GetPropertyOffset(IntIntMap_NativeProp);
	}

	private TMap<int, int>? _intIntMap;
	public TMap<int, int> IntIntMap => _intIntMap ??= new(nativePtr + IntIntMap_Offset, IntIntMap_NativeProp, BlittableMarshaller<int>.Instance, BlittableMarshaller<int>.Instance);

	public FSsMapTestInnerStruct ToManaged()
	{
		return new FSsMapTestInnerStruct()
		{
			IntIntMap = IntIntMap,
		};
	}

	public void FromManaged(in FSsMapTestInnerStruct value)
	{
		IntIntMap.CopyFrom(value.IntIntMap);
	}

	public static IStructNativeRef<FSsMapTestInnerStruct> CreateInstance(IntPtr valuePtr)
	{
		return new FSsMapTestInnerStructNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator FSsMapTestInnerStruct(FSsMapTestInnerStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

public struct FSsMapTestInnerStruct
{
	public bool Equals(FSsMapTestInnerStruct other)
	{
		// ReSharper disable once UsageOfDefaultStructEquality
		return IntIntMap.OrderBy(x => x.Key)
			.SequenceEqual(other.IntIntMap.OrderBy(x => x.Key));
	}

	public override bool Equals(object? obj)
	{
		return obj is FSsMapTestInnerStruct other && Equals(other);
	}

	public override int GetHashCode()
	{
		return IntIntMap.GetHashCode();
	}

	public Dictionary<int, int> IntIntMap;
}
