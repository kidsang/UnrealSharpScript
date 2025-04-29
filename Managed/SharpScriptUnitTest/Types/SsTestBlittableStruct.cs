using System.Runtime.InteropServices;
using SharpScript;
using SharpScript.Interop;

namespace SharpScriptUnitTest.Types;

public class SsTestBlittableStructNativeRef(IntPtr nativePtr) : IStructNativeRef<SsTestBlittableStruct>
{
	public unsafe int X
	{
		get => (*(SsTestBlittableStruct*)nativePtr).X;
		set => (*(SsTestBlittableStruct*)nativePtr).X = value;
	}

	public unsafe int Y
	{
		get => (*(SsTestBlittableStruct*)nativePtr).Y;
		set => (*(SsTestBlittableStruct*)nativePtr).Y = value;
	}

	public SsTestBlittableStruct ToManaged()
	{
		return BlittableMarshaller<SsTestBlittableStruct>.FromNative(nativePtr);
	}

	public void FromManaged(in SsTestBlittableStruct value)
	{
		BlittableMarshaller<SsTestBlittableStruct>.ToNative(nativePtr, value);
	}

	public static IStructNativeRef<SsTestBlittableStruct> CreateInstance(IntPtr valuePtr)
	{
		return new SsTestBlittableStructNativeRef(valuePtr);
	}

	public static unsafe int GetNativeDataSize()
	{
		return sizeof(SsTestBlittableStruct);
	}

	public static implicit operator SsTestBlittableStruct(SsTestBlittableStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

[StructLayout(LayoutKind.Sequential)]
public struct SsTestBlittableStruct : IStructMarshallerHelper<SsTestBlittableStruct>, IEquatable<SsTestBlittableStruct>
{
	public int X;

	public int Y;

	public static int GetNativeDataSize()
	{
		return SsTestBlittableStructNativeRef.GetNativeDataSize();
	}

	public static IStructNativeRef<SsTestBlittableStruct> CreateStructNativeRef(IntPtr valuePtr)
	{
		return new SsTestBlittableStructNativeRef(valuePtr);
	}

	public bool Equals(SsTestBlittableStruct other)
	{
		return X == other.X && Y == other.Y;
	}

	public override bool Equals(object? obj)
	{
		return obj is SsTestBlittableStruct other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}
}
