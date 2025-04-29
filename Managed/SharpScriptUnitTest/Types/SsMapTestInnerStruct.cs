using SharpScript;
using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class SsMapTestInnerStructNativeRef(IntPtr nativePtr)
	: IStructNativeRef<SsMapTestInnerStruct>
{
	// ReSharper disable InconsistentNaming
	public static readonly IntPtr NativeType;
	public static readonly int NativeDataSize;
	internal static readonly IntPtr IntIntMap_NativeProp;
	internal static readonly int IntIntMap_Offset;
	// ReSharper restore InconsistentNaming

	static SsMapTestInnerStructNativeRef()
	{
		NativeType = TypeInterop.FindStruct("SsMapTestInnerStruct");
		NativeDataSize = TypeInterop.GetStructureSize(NativeType);
		PropertyIterator propIter = new PropertyIterator(NativeType);

		IntIntMap_NativeProp = propIter.FindNext("IntIntMap");
		IntIntMap_Offset = TypeInterop.GetPropertyOffset(IntIntMap_NativeProp);
	}

	private Map<int, int>? _intIntMap;
	public Map<int, int> IntIntMap => _intIntMap ??= new(nativePtr + IntIntMap_Offset, IntIntMap_NativeProp, BlittableMarshaller<int>.Instance, BlittableMarshaller<int>.Instance);

	public SsMapTestInnerStruct ToManaged()
	{
		return new SsMapTestInnerStruct()
		{
			IntIntMap = IntIntMap,
		};
	}

	public void FromManaged(in SsMapTestInnerStruct value)
	{
		IntIntMap.CopyFrom(value.IntIntMap);
	}

	public static IStructNativeRef<SsMapTestInnerStruct> CreateInstance(IntPtr valuePtr)
	{
		return new SsMapTestInnerStructNativeRef(valuePtr);
	}

	public static int GetNativeDataSize()
	{
		return NativeDataSize;
	}

	public static implicit operator SsMapTestInnerStruct(SsMapTestInnerStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

public struct SsMapTestInnerStruct
{
	public bool Equals(SsMapTestInnerStruct other)
	{
		// ReSharper disable once UsageOfDefaultStructEquality
		return IntIntMap.OrderBy(x => x.Key)
			.SequenceEqual(other.IntIntMap.OrderBy(x => x.Key));
	}

	public override bool Equals(object? obj)
	{
		return obj is SsMapTestInnerStruct other && Equals(other);
	}

	public override int GetHashCode()
	{
		return IntIntMap.GetHashCode();
	}

	public Dictionary<int, int> IntIntMap;
}
