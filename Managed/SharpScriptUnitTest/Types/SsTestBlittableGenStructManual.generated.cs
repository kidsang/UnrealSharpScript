using System.Runtime.InteropServices;
using SharpScript;
using SharpScript.Interop;
using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;

namespace SharpScriptUnitTest.Types;

public class FSsTestBlittableGenStructManualNativeRef(IntPtr nativePtr) : IStructNativeRef<FSsTestBlittableGenStructManual>
{
	public static readonly IntPtr NativeType;

	static FSsTestBlittableGenStructManualNativeRef()
	{
		PropertyDef[] _propertyDefs =
		[
			new()
			{
				PropName = "X",
				PropType = UIntProperty.StaticClass.NativeClass
			},
			new()
			{
				PropName = "Y",
				PropType = UIntProperty.StaticClass.NativeClass
			},
		];

		unsafe
		{
			fixed (PropertyDef* _propertyDefsPtr = _propertyDefs)
			{
				StructDef _structDef = new()
				{
					StructName = "SsTestBlittableGenStructManual",
					PropertyDefines = (IntPtr)_propertyDefsPtr,
					PropertyCount = _propertyDefs.Length,
				};
				NativeType = SubclassingUtils.GenerateStruct((IntPtr)(&_structDef));
			}
		}
	}

	public unsafe int X
	{
		get => (*(FSsTestBlittableGenStructManual*)nativePtr).X;
		set => (*(FSsTestBlittableGenStructManual*)nativePtr).X = value;
	}

	public unsafe int Y
	{
		get => (*(FSsTestBlittableGenStructManual*)nativePtr).Y;
		set => (*(FSsTestBlittableGenStructManual*)nativePtr).Y = value;
	}

	public FSsTestBlittableGenStructManual ToManaged()
	{
		return BlittableMarshaller<FSsTestBlittableGenStructManual>.FromNative(nativePtr);
	}

	public void FromManaged(in FSsTestBlittableGenStructManual value)
	{
		BlittableMarshaller<FSsTestBlittableGenStructManual>.ToNative(nativePtr, value);
	}

	public static IStructNativeRef<FSsTestBlittableGenStructManual> CreateInstance(IntPtr valuePtr)
	{
		return new FSsTestBlittableGenStructManualNativeRef(valuePtr);
	}

	public static unsafe int GetNativeDataSize()
	{
		return sizeof(FSsTestBlittableGenStructManual);
	}

	public static implicit operator FSsTestBlittableGenStructManual(FSsTestBlittableGenStructManualNativeRef nativeRef)
	{
		return nativeRef.ToManaged();
	}
}

[StructLayout(LayoutKind.Sequential)]
public partial struct FSsTestBlittableGenStructManual : IStructMarshallerHelper<FSsTestBlittableGenStructManual>
{
	public static int GetNativeDataSize()
	{
		return FSsTestBlittableGenStructManualNativeRef.GetNativeDataSize();
	}

	public static IStructNativeRef<FSsTestBlittableGenStructManual> CreateStructNativeRef(IntPtr valuePtr)
	{
		return new FSsTestBlittableGenStructManualNativeRef(valuePtr);
	}
}
