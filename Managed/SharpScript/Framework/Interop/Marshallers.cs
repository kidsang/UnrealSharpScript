using System.Reflection;
using System.Runtime.InteropServices;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using Object = UnrealEngine.CoreUObject.Object;

namespace SharpScript.Interop;

/// <summary>
/// C++/C# object marshall interface
/// </summary>
public interface IMarshaller<T>
{
	/// <summary>
	/// Convert C# object to C++ object.
	/// </summary>
	/// <param name="valuePtr">C++ object pointer</param>
	/// <param name="value">C# object</param>
	static abstract void ToNative(IntPtr valuePtr, in T value);

	/// <summary>
	/// Convert C# object to C++ object, this interface is used for arrays.
	/// </summary>
	/// <param name="arrayPtr">C++ array base address pointer</param>
	/// <param name="arrayIndex">Array index of the object to be converted</param>
	/// <param name="value">C# object</param>
	static abstract void ToNative(IntPtr arrayPtr, int arrayIndex, in T value);

	/// <summary>
	/// Convert C++ object to C# object.
	/// </summary>
	/// <param name="valuePtr">C++ object pointer</param>
	/// <returns>C# object</returns>
	static abstract T FromNative(IntPtr valuePtr);

	/// <summary>
	/// Convert C++ object to C# object, this interface is used for arrays.
	/// </summary>
	/// <param name="arrayPtr">C++ array base address pointer</param>
	/// <param name="arrayIndex">Array index of the object to be converted</param>
	/// <returns>C# object</returns>
	static abstract T FromNative(IntPtr arrayPtr, int arrayIndex);

	/// <summary>
	/// Convert C# object to C++ object.
	/// </summary>
	/// <param name="valuePtr">C++ object pointer</param>
	/// <param name="value">C# object</param>
	void MarshallToNative(IntPtr valuePtr, in T value);

	/// <summary>
	/// Convert C# object to C++ object, this interface is used for arrays.
	/// </summary>
	/// <param name="arrayPtr">C++ array base address pointer</param>
	/// <param name="arrayIndex">Array index of the object to be converted</param>
	/// <param name="value">C# object</param>
	void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in T value);

	/// <summary>
	/// Convert C++ object to C# object.
	/// </summary>
	/// <param name="valuePtr">C++ object pointer</param>
	/// <returns>C# object</returns>
	T MarshallFromNative(IntPtr valuePtr);

	/// <summary>
	/// Convert C++ object to C# object, this interface is used for arrays.
	/// </summary>
	/// <param name="arrayPtr">C++ array base address pointer</param>
	/// <param name="arrayIndex">Array index of the object to be converted</param>
	/// <returns>C# object</returns>
	T MarshallFromNative(IntPtr arrayPtr, int arrayIndex);
}

/// <summary>
/// Handles object marshalling for blittable types. C# blittable types are binary compatible with C++ types and can be directly cast.<br/>
/// See: https://learn.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types
/// </summary>
public unsafe class BlittableMarshaller<T> : IMarshaller<T> where T : unmanaged
{
	public static readonly BlittableMarshaller<T> Instance;

	static BlittableMarshaller()
	{
		Instance = new BlittableMarshaller<T>();
	}

	public static void ToNative(IntPtr valuePtr, in T value)
	{
		*(T*)valuePtr = value;
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in T value)
	{
		ToNative(arrayPtr + arrayIndex * sizeof(T), value);
	}

	public static T FromNative(IntPtr valuePtr)
	{
		return *(T*)valuePtr;
	}

	public static T FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr + arrayIndex * sizeof(T));
	}

	public void MarshallToNative(IntPtr valuePtr, in T value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in T value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public T MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public T MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}
}

// Boolean type is not a blittable type, we need to convert it to byte.
public enum NativeBool : byte
{
	True = 1,
	False = 0
}

/// <summary>
/// Handles marshalling of boolean objects.
/// </summary>
public class BoolMarshaller : IMarshaller<bool>
{
	public static readonly BoolMarshaller Instance;

	static BoolMarshaller()
	{
		Instance = new BoolMarshaller();
	}

	public static void ToNative(IntPtr valuePtr, in bool value)
	{
		BlittableMarshaller<NativeBool>.ToNative(valuePtr, value ? NativeBool.True : NativeBool.False);
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in bool value)
	{
		BlittableMarshaller<NativeBool>.ToNative(arrayPtr, arrayIndex, value ? NativeBool.True : NativeBool.False);
	}

	public static bool FromNative(IntPtr valuePtr)
	{
		return BlittableMarshaller<NativeBool>.FromNative(valuePtr) != 0;
	}

	public static bool FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return BlittableMarshaller<NativeBool>.FromNative(arrayPtr, arrayIndex) != 0;
	}

	public void MarshallToNative(IntPtr valuePtr, in bool value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in bool value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public bool MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public bool MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}
}

/// <summary>
/// Handles marshalling of boolean objects implemented as bitfields.
/// This only occurs on object UPROPERTY.
/// </summary>
public static class BitfieldBoolMarshaller
{
	public static void ToNative(IntPtr valuePtr, byte fieldMask, bool value)
	{
		unsafe
		{
			byte* byteValue = (byte*)valuePtr;
			byte mask = value ? fieldMask : (byte)0;
			*byteValue = (byte)((*byteValue & ~fieldMask) | mask);
		}
	}

	public static bool FromNative(IntPtr valuePtr, byte fieldMask)
	{
		unsafe
		{
			byte* byteValue = (byte*)valuePtr;
			return (*byteValue & fieldMask) != 0;
		}
	}
}

/// <summary>
/// Handles marshalling of enum types.
/// </summary>
public class EnumMarshaller<T> : BlittableMarshaller<T>
	where T : unmanaged, System.Enum
{
}

/// <summary>
/// Handles marshalling of strings.
/// </summary>
public class StringMarshaller : IMarshaller<string>
{
	public static readonly StringMarshaller Instance;

	static StringMarshaller()
	{
		Instance = new StringMarshaller();
	}

	public static void ToNative(IntPtr valuePtr, in string? value)
	{
		unsafe
		{
			string strValue = value ?? string.Empty;
			fixed (char* stringPtr = strValue)
			{
				StringInterop.CopyToNative(valuePtr, stringPtr);
			}
		}
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in string? value)
	{
		unsafe
		{
			ToNative(arrayPtr + arrayIndex * sizeof(NativeString), value);
		}
	}

	public static string FromNative(IntPtr valuePtr)
	{
		NativeString nativeString = BlittableMarshaller<NativeString>.FromNative(valuePtr);
		return nativeString.ToString();
	}

	public static string FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		unsafe
		{
			return FromNative(arrayPtr + arrayIndex * sizeof(NativeString));
		}
	}

	public void MarshallToNative(IntPtr valuePtr, in string value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in string value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public string MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public string MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}
}

/// <summary>
/// Handles marshalling of FName.
// </summary>
public class NameMarshaller : BlittableMarshaller<Name>
{
}

/// <summary>
/// Handles marshalling of FText.
/// </summary>
public class TextMarshaller : IMarshaller<Text>
{
	private static readonly int SizeOfText;

	public static readonly TextMarshaller Instance;

	static TextMarshaller()
	{
		unsafe
		{
			SizeOfText = TextInterop.SizeOfText();
		}

		Instance = new TextMarshaller();
	}

	public static void ToNative(IntPtr valuePtr, in Text value)
	{
		unsafe
		{
			fixed (char* stringPtr = value.Data)
			{
				TextInterop.StringToText(valuePtr, stringPtr);
			}
		}
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in Text value)
	{
		ToNative(arrayPtr + arrayIndex * SizeOfText, value);
	}

	public static Text FromNative(IntPtr valuePtr)
	{
		unsafe
		{
			NativeString buffer = new NativeString();
			try
			{
				TextInterop.TextToString(valuePtr, ref buffer);
				return new Text(buffer.ToString());
			}
			finally
			{
				ArrayInterop.Destroy(ref buffer.Array);
			}
		}
	}

	public static Text FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr + arrayIndex * SizeOfText);
	}

	public void MarshallToNative(IntPtr valuePtr, in Text value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in Text value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public Text MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public Text MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}
}

/// <summary>
/// Handles marshalling of FFieldPath.
/// </summary>
public class FieldPathMarshaller : IMarshaller<FieldPath>
{
	public static readonly FieldPathMarshaller Instance;

	static FieldPathMarshaller()
	{
		Instance = new FieldPathMarshaller();
	}

	public static void ToNative(IntPtr valuePtr, in FieldPath value)
	{
		unsafe
		{
			fixed (char* bytes = value.Path)
			{
				FieldPathInterop.FieldPathFromString(valuePtr, bytes);
			}
		}
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in FieldPath value)
	{
		ToNative(arrayPtr + arrayIndex * FieldPath.NativeDataSize, value);
	}

	public static FieldPath FromNative(IntPtr valuePtr)
	{
		unsafe
		{
			NativeString buffer = new NativeString();
			try
			{
				FieldPathInterop.FieldPathToString(valuePtr, ref buffer);
				FieldPath fieldPath = new(buffer.ToString());
				return fieldPath;
			}
			finally
			{
				ArrayInterop.Destroy(ref buffer.Array);
			}
		}
	}

	public static FieldPath FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr + arrayIndex * FieldPath.NativeDataSize);
	}

	public void MarshallToNative(IntPtr valuePtr, in FieldPath value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in FieldPath value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public FieldPath MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public FieldPath MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}
}

/// <summary>
/// Handles marshalling of structs.
/// </summary>
public class StructMarshaller<T> : IMarshaller<T>
	where T : struct, IStructMarshallerHelper<T>
{
	public static readonly StructMarshaller<T> Instance;

	static StructMarshaller()
	{
		Instance = new StructMarshaller<T>();
	}

	public static void ToNative(IntPtr valuePtr, in T value)
	{
		T.CreateStructNativeRef(valuePtr).FromManaged(value);
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in T value)
	{
		int dataSize = T.GetNativeDataSize();
		ToNative(arrayPtr + arrayIndex * dataSize, value);
	}

	public static T FromNative(IntPtr valuePtr)
	{
		return T.CreateStructNativeRef(valuePtr).ToManaged();
	}

	public static T FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		int dataSize = T.GetNativeDataSize();
		return FromNative(arrayPtr + arrayIndex * dataSize);
	}

	public void MarshallToNative(IntPtr valuePtr, in T value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in T value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public T MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public T MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}

	public IStructNativeRef<T> MarshallToNativeRef(IntPtr valuePtr)
	{
		return T.CreateStructNativeRef(valuePtr);
	}

	public IStructNativeRef<T> MarshallToNativeRef(IntPtr arrayPtr, int arrayIndex)
	{
		int dataSize = T.GetNativeDataSize();
		return MarshallToNativeRef(arrayPtr + arrayIndex * dataSize);
	}
}

/// <summary>
/// Handles marshalling of UObjects.
/// </summary>
public class ObjectMarshaller<T> : IMarshaller<T?>
	where T : Object
{
	public static readonly ObjectMarshaller<T> Instance;

	static ObjectMarshaller()
	{
		Instance = new ObjectMarshaller<T>();
	}

	public static void ToNative(IntPtr valuePtr, in T? value)
	{
		unsafe
		{
			*(IntPtr*)valuePtr = value?.NativeObject ?? IntPtr.Zero;
		}
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in T? value)
	{
		ToNative(arrayPtr + arrayIndex * IntPtr.Size, value);
	}

	public static T? FromNative(IntPtr valuePtr)
	{
		unsafe
		{
			IntPtr objectPtr = *(IntPtr*)valuePtr;
			if (objectPtr == IntPtr.Zero)
			{
				return null;
			}

			return HouseKeeper.GetManagedObject<T>(objectPtr);
		}
	}

	public static T? FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr + arrayIndex * IntPtr.Size);
	}

	public void MarshallToNative(IntPtr valuePtr, in T? value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in T? value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public T? MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public T? MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}
}

/// <summary>
/// Handles marshalling of TSoftObjectPtr.
/// </summary>
public class SoftObjectPtrMarshaller<T> : IMarshaller<SoftObjectPtr<T>>
	where T : Object
{
	public static readonly SoftObjectPtrMarshaller<T> Instance;

	// ReSharper disable StaticMemberInGenericType
	private static readonly int AssetPathOffset;
	private static readonly int SubPathStringOffset;
	private static readonly int NativeDataSize;
	// ReSharper restore StaticMemberInGenericType

	static SoftObjectPtrMarshaller()
	{
		Instance = new SoftObjectPtrMarshaller<T>();

		unsafe
		{
			AssetPathOffset = sizeof(WeakObjectPtr<T>);
			SubPathStringOffset = AssetPathOffset + sizeof(TopLevelAssetPath);
			NativeDataSize = SubPathStringOffset + sizeof(NativeString);
		}
	}

	public static void ToNative(IntPtr valuePtr, in SoftObjectPtr<T> value)
	{
		BlittableMarshaller<WeakObjectPtr<Object>>.ToNative(valuePtr, value.Data.WeakPtr);
		BlittableMarshaller<TopLevelAssetPath>.ToNative(valuePtr + AssetPathOffset, value.Data.ObjectId.AssetPath);
		StringMarshaller.ToNative(valuePtr + SubPathStringOffset, value.Data.ObjectId.SubPathString);
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in SoftObjectPtr<T> value)
	{
		ToNative(arrayPtr + arrayIndex * NativeDataSize, value);
	}

	public static SoftObjectPtr<T> FromNative(IntPtr valuePtr)
	{
		return new SoftObjectPtr<T>
		{
			Data = new SoftObjectPtrData
			{
				WeakPtr = BlittableMarshaller<WeakObjectPtr<Object>>.FromNative(valuePtr),
				ObjectId = new SoftObjectPath
				{
					AssetPath = BlittableMarshaller<TopLevelAssetPath>.FromNative(valuePtr + AssetPathOffset),
					SubPathString = StringMarshaller.FromNative(valuePtr + SubPathStringOffset)
				}
			}
		};
	}

	public static SoftObjectPtr<T> FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr + arrayIndex * NativeDataSize);
	}

	public void MarshallToNative(IntPtr valuePtr, in SoftObjectPtr<T> value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in SoftObjectPtr<T> value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public SoftObjectPtr<T> MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public SoftObjectPtr<T> MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}
}

/// <summary>
/// Handles marshalling of TLazyObjectPtr.
/// </summary>
public class LazyObjectPtrMarshaller<T> : BlittableMarshaller<LazyObjectPtr<T>>
	where T : Object
{
	public new static readonly LazyObjectPtrMarshaller<T> Instance;

	static LazyObjectPtrMarshaller()
	{
		Instance = new LazyObjectPtrMarshaller<T>();
	}
}

/// <summary>
/// Handles marshalling of TSubclassOf.
/// </summary>
public class SubclassOfMarshaller<T> : IMarshaller<SubclassOf<T>>
	where T : Object
{
	public static readonly SubclassOfMarshaller<T> Instance;

	static SubclassOfMarshaller()
	{
		Instance = new SubclassOfMarshaller<T>();
	}

	public static void ToNative(IntPtr valuePtr, in SubclassOf<T> value)
	{
		unsafe
		{
			*(IntPtr*)valuePtr = value.NativeClass;
		}
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in SubclassOf<T> value)
	{
		ToNative(arrayPtr + arrayIndex * IntPtr.Size, value);
	}

	public static SubclassOf<T> FromNative(IntPtr valuePtr)
	{
		unsafe
		{
			IntPtr classPtr = *(IntPtr*)valuePtr;
			if (classPtr == IntPtr.Zero)
			{
				return default;
			}

			return new SubclassOf<T>(classPtr);
		}
	}

	public static SubclassOf<T> FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr + arrayIndex * IntPtr.Size);
	}

	public void MarshallToNative(IntPtr valuePtr, in SubclassOf<T> value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in SubclassOf<T> value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public SubclassOf<T> MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public SubclassOf<T> MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}
}

/// <summary>
/// Handles marshalling of TSoftClassPtr.
/// </summary>
public class SoftClassPtrMarshaller<T> : IMarshaller<SoftClassPtr<T>>
	where T : Object, IStaticClass<T>
{
	public static readonly SoftClassPtrMarshaller<T> Instance;

	// ReSharper disable StaticMemberInGenericType
	private static readonly int AssetPathOffset;
	private static readonly int SubPathStringOffset;
	private static readonly int NativeDataSize;
	// ReSharper restore StaticMemberInGenericType

	static SoftClassPtrMarshaller()
	{
		Instance = new SoftClassPtrMarshaller<T>();

		unsafe
		{
			AssetPathOffset = sizeof(WeakObjectPtr<T>);
			SubPathStringOffset = AssetPathOffset + sizeof(TopLevelAssetPath);
			NativeDataSize = SubPathStringOffset + sizeof(NativeString);
		}
	}

	public static void ToNative(IntPtr valuePtr, in SoftClassPtr<T> value)
	{
		BlittableMarshaller<WeakObjectPtr<Object>>.ToNative(valuePtr, value.Data.WeakPtr);
		BlittableMarshaller<TopLevelAssetPath>.ToNative(valuePtr + AssetPathOffset, value.Data.ObjectId.AssetPath);
		StringMarshaller.ToNative(valuePtr + SubPathStringOffset, value.Data.ObjectId.SubPathString);
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in SoftClassPtr<T> value)
	{
		ToNative(arrayPtr + arrayIndex * NativeDataSize, value);
	}

	public static SoftClassPtr<T> FromNative(IntPtr valuePtr)
	{
		return new SoftClassPtr<T>
		{
			Data = new SoftObjectPtrData
			{
				WeakPtr = BlittableMarshaller<WeakObjectPtr<Object>>.FromNative(valuePtr),
				ObjectId = new SoftObjectPath
				{
					AssetPath = BlittableMarshaller<TopLevelAssetPath>.FromNative(valuePtr + AssetPathOffset),
					SubPathString = StringMarshaller.FromNative(valuePtr + SubPathStringOffset)
				}
			}
		};
	}

	public static SoftClassPtr<T> FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr + arrayIndex * NativeDataSize);
	}

	public void MarshallToNative(IntPtr valuePtr, in SoftClassPtr<T> value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in SoftClassPtr<T> value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public SoftClassPtr<T> MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public SoftClassPtr<T> MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}
}

/// <summary>
/// Handles marshalling of UInterface.
/// </summary>
public class InterfaceMarshaller<T> : IMarshaller<T?>
	where T : class, IInterface
{
	public static readonly InterfaceMarshaller<T> Instance;

	static InterfaceMarshaller()
	{
		Instance = new InterfaceMarshaller<T>();
	}

	public static void ToNative(IntPtr valuePtr, in T? value)
	{
		unsafe
		{
			IntPtr objectPtr = IntPtr.Zero;
			IntPtr interfacePtr = IntPtr.Zero;
			if (value is Object obj)
			{
				objectPtr = obj.NativeObject;
				interfacePtr = T.InterfaceClass.NativeObject;
			}

			InterfaceInterop.SetObjectAndInterface(valuePtr, objectPtr, interfacePtr);
		}
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in T? value)
	{
		unsafe
		{
			ToNative(arrayPtr + arrayIndex * sizeof(ScriptInterface), value);
		}
	}

	public static T? FromNative(IntPtr valuePtr)
	{
		unsafe
		{
			IntPtr managedHandlePtr = InterfaceInterop.GetObject(valuePtr);
			if (managedHandlePtr == IntPtr.Zero)
			{
				return null;
			}

			GCHandle managedHandle = GCHandle.FromIntPtr(managedHandlePtr);
			return managedHandle.Target as T;
		}
	}

	public static T? FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		unsafe
		{
			return FromNative(arrayPtr + arrayIndex * sizeof(ScriptInterface));
		}
	}

	public void MarshallToNative(IntPtr valuePtr, in T? value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in T? value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public T? MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public T? MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}
}

/// <summary>
/// Handles marshalling of delegates.
/// </summary>
public class DelegateMarshaller<T> : IMarshaller<T>
	where T : Delegate
{
	public static readonly DelegateMarshaller<T> Instance;

	static DelegateMarshaller()
	{
		Instance = new DelegateMarshaller<T>();
	}

	public static void ToNative(IntPtr valuePtr, in T? value)
	{
		unsafe
		{
			ScriptDelegate* scriptDelegatePtr = (ScriptDelegate*)valuePtr;
			if (value == null)
			{
				scriptDelegatePtr->Unbind();
			}
			else
			{
				if (value.Target is not Object targetObject
					|| TypeInterop.FindFunction(targetObject.GetClass().NativeObject, value.Method.Name) == IntPtr.Zero)
				{
					throw new ArgumentException($"the callback for delegate must be a valid UFunction. {nameof(value)}");
				}

				scriptDelegatePtr->BindUFunction(targetObject, value.Method.Name);
			}
		}
	}

	public static void ToNative(IntPtr arrayPtr, int arrayIndex, in T? value)
	{
		unsafe
		{
			ToNative(arrayPtr + arrayIndex * sizeof(ScriptDelegate), value);
		}
	}

	public static T FromNative(IntPtr valuePtr)
	{
		unsafe
		{
			ScriptDelegate* scriptDelegatePtr = (ScriptDelegate*)valuePtr;
			Object? obj = scriptDelegatePtr->GetUObject();
			if (obj == null)
			{
				return null!;
			}

			Name funcName = scriptDelegatePtr->GetFunctionName();
			if (funcName == Name.None)
			{
				return null!;
			}

			Type type = obj.GetType();
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			MethodInfo? methodInfo = type.GetMethod(funcName, bindingFlags);
			if (methodInfo == null)
			{
				return null!;
			}

			return (T)Delegate.CreateDelegate(typeof(T), obj, methodInfo);
		}
	}

	public static T FromNative(IntPtr arrayPtr, int arrayIndex)
	{
		unsafe
		{
			return FromNative(arrayPtr + arrayIndex * sizeof(ScriptDelegate));
		}
	}

	public void MarshallToNative(IntPtr valuePtr, in T value)
	{
		ToNative(valuePtr, value);
	}

	public void MarshallToNative(IntPtr arrayPtr, int arrayIndex, in T value)
	{
		ToNative(arrayPtr, arrayIndex, value);
	}

	public T MarshallFromNative(IntPtr valuePtr)
	{
		return FromNative(valuePtr);
	}

	public T MarshallFromNative(IntPtr arrayPtr, int arrayIndex)
	{
		return FromNative(arrayPtr, arrayIndex);
	}

	public Delegate<T> MarshallToNativeRef(IntPtr valuePtr)
	{
		return new Delegate<T>(valuePtr);
	}

	public Delegate<T> MarshallToNativeRef(IntPtr arrayPtr, int arrayIndex)
	{
		unsafe
		{
			return MarshallToNativeRef(arrayPtr + arrayIndex * sizeof(ScriptDelegate));
		}
	}
}

