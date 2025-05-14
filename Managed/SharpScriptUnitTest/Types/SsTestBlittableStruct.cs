using System.Runtime.InteropServices;
using SharpScript;
using SharpScript.Interop;

namespace SharpScriptUnitTest.Types;

public class FSsTestBlittableStructNativeRef(IntPtr nativePtr) : IStructNativeRef<FSsTestBlittableStruct>
{
	public unsafe int X
	{
		get => (*(FSsTestBlittableStruct*)nativePtr).X;
		set => (*(FSsTestBlittableStruct*)nativePtr).X = value;
	}

	public unsafe int Y
	{
		get => (*(FSsTestBlittableStruct*)nativePtr).Y;
		set => (*(FSsTestBlittableStruct*)nativePtr).Y = value;
	}

	public FSsTestBlittableStruct ToManaged()
	{
		return BlittableMarshaller<FSsTestBlittableStruct>.FromNative(nativePtr);
	}

	public void FromManaged(in FSsTestBlittableStruct value)
	{
		BlittableMarshaller<FSsTestBlittableStruct>.ToNative(nativePtr, value);
	}

	public static IStructNativeRef<FSsTestBlittableStruct> CreateInstance(IntPtr valuePtr)
	{
		return new FSsTestBlittableStructNativeRef(valuePtr);
	}

	public static unsafe int GetNativeDataSize()
	{
		return sizeof(FSsTestBlittableStruct);
	}

	public static implicit operator FSsTestBlittableStruct(FSsTestBlittableStructNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

[StructLayout(LayoutKind.Sequential)]
public struct FSsTestBlittableStruct : IStructMarshallerHelper<FSsTestBlittableStruct>, IEquatable<FSsTestBlittableStruct>
{
	public int X;

	public int Y;

	public static int GetNativeDataSize()
	{
		return FSsTestBlittableStructNativeRef.GetNativeDataSize();
	}

	public static IStructNativeRef<FSsTestBlittableStruct> CreateStructNativeRef(IntPtr valuePtr)
	{
		return new FSsTestBlittableStructNativeRef(valuePtr);
	}

	public bool Equals(FSsTestBlittableStruct other)
	{
		return X == other.X && Y == other.Y;
	}

	public override bool Equals(object? obj)
	{
		return obj is FSsTestBlittableStruct other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}
}
