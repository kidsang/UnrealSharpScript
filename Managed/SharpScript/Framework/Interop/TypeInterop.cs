using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for native types.
/// </summary>
[NativeCallbacks]
public static unsafe class TypeInterop
{
	/// <summary>
	/// 根据类名查找原生UClass。
	/// </summary>
	/// <param name="className">UClass名称</param>
	/// <returns>UClass指针</returns>
	public static IntPtr FindClass(string className)
	{
		fixed (char* bytes = className)
		{
			return NativeFindClass(bytes);
		}
	}

	/// <summary>
	/// 根据类名查找原生UScriptStruct。
	/// </summary>
	/// <param name="structName">UScriptStruct名称</param>
	/// <returns>UScriptStruct指针</returns>
	public static IntPtr FindStruct(string structName)
	{
		fixed (char* bytes = structName)
		{
			return NativeFindStruct(bytes);
		}
	}

	/// <summary>
	/// 创建UE结构体C++实例
	/// </summary>
	/// <param name="nativeStruct">UScriptStruct指针</param>
	/// <returns>新创建的结构体实例指针</returns>
	public static IntPtr CreateStructInstance(IntPtr nativeStruct)
	{
		return NativeCreateStructInstance(nativeStruct);
	}

	/// <summary>
	/// 克隆UE结构体C++实例
	/// </summary>
	/// <param name="nativeStruct">UScriptStruct指针</param>
	/// <param name="nativeSrcInstance">要克隆的结构体实例指针</param>
	/// <returns>新创建的结构体实例指针</returns>
	public static IntPtr CloneStructInstance(IntPtr nativeStruct, IntPtr nativeSrcInstance)
	{
		return NativeCloneStructInstance(nativeStruct, nativeSrcInstance);
	}

	/// <summary>
	/// 销毁UE结构体C++实例
	/// </summary>
	/// <param name="nativeStruct">UScriptStruct指针</param>
	/// <param name="nativeInstance">要销毁的结构体实例指针</param>
	public static void DestroyStructInstance(IntPtr nativeStruct, ref IntPtr nativeInstance)
	{
		NativeDestroyStructInstance(nativeStruct, nativeInstance);
		nativeInstance = 0;
	}

	/// <summary>
	/// 获取类型或结构体尺寸
	/// </summary>
	/// <param name="nativeType"></param>
	/// <returns>返回类型或结构体实际内存占用</returns>
	public static int GetStructureSize(IntPtr nativeType)
	{
		return NativeGetStructureSize(nativeType);
	}

	/// <summary>
	/// 使用已分配的内存初始化类、结构体或函数参数内存
	/// </summary>
	/// <param name="nativeType">UClass、UScriptStruct或UFunction指针</param>
	/// <param name="bufferPtr">要初始化的内存</param>
	public static void InitializeStruct(IntPtr nativeType, IntPtr bufferPtr)
	{
		NativeInitializeStruct(nativeType, bufferPtr);
	}

	/// <summary>
	/// 使用类、结构体或函数参数信息反初始化对应内存
	/// </summary>
	/// <param name="nativeType">UClass、UScriptStruct或UFunction指针</param>
	/// <param name="bufferPtr">要反初始化的内存</param>
	public static void UninitializeStruct(IntPtr nativeType, IntPtr bufferPtr)
	{
		NativeUninitializeStruct(nativeType, bufferPtr);
	}

	/// <summary>
	/// 获取类、结构体或方法的名称
	/// </summary>
	/// <param name="nativeType">类、结构体或方法的指针</param>
	public static string GetTypeName(IntPtr nativeType)
	{
		return NativeGetTypeName(nativeType).ToString();
	}

	/// <summary>
	/// 根据类名查找原生UEnum。
	/// </summary>
	/// <param name="enumName">UEnum名称</param>
	/// <returns>UEnum指针</returns>
	public static IntPtr FindEnum(string enumName)
	{
		fixed (char* bytes = enumName)
		{
			return NativeFindEnum(bytes);
		}
	}

	/// <summary>
	/// 在UClass中查找指定名称的UFunction。
	/// </summary>
	/// <param name="nativeClass">UClass指针</param>
	/// <param name="funcName">方法名</param>
	/// <returns>UFunction指针</returns>
	public static IntPtr FindFunction(IntPtr nativeClass, Name funcName)
	{
		return NativeFindFunction(nativeClass, funcName);
	}

	/// <summary>
	/// 返回UFunction参数返回值结构体大小。
	/// </summary>
	/// <param name="nativeFunc">UFunction指针</param>
	/// <returns>参数返回值结构体大小</returns>
	public static int GetFunctionParamsSize(IntPtr nativeFunc)
	{
		return NativeGetFunctionParamsSize(nativeFunc);
	}

	/// <summary>
	/// 返回类型的第一个属性。
	/// </summary>
	/// <param name="nativeStruct">UClass、UScriptStruct或UFunction指针</param>
	/// <returns>类型的第一个属性，若不存在则返回空。</returns>
	public static IntPtr GetFirstProperty(IntPtr nativeStruct)
	{
		return NativeGetFirstProperty(nativeStruct);
	}

	/// <summary>
	/// 返回属性链表的下一个属性。
	/// </summary>
	/// <param name="nativeProp">FProperty指针</param>
	/// <returns>下一个属性，若不存在则返回空。</returns>
	public static IntPtr GetNextProperty(IntPtr nativeProp)
	{
		return NativeGetNextProperty(nativeProp);
	}

	/// <summary>
	/// 查找UClass、UScriptStruct或UFunction中指定名称的属性。
	/// </summary>
	/// <param name="nativeStruct">UClass、UScriptStruct或UFunction指针</param>
	/// <param name="propName">属性名</param>
	/// <returns>FProperty指针</returns>
	public static IntPtr FindProperty(IntPtr nativeStruct, Name propName)
	{
		return NativeFindProerty(nativeStruct, propName);
	}

	/// <summary>
	/// 获取属性名称。
	/// </summary>
	/// <param name="nativeProp">FProperty指针</param>
	/// <returns>属性名称</returns>
	public static string GetPropertyName(IntPtr nativeProp)
	{
		return NativeGetPropertyName(nativeProp).ToString();
	}

	/// <summary>
	/// 获取属性名称。
	/// </summary>
	/// <param name="nativeProp">FProperty指针</param>
	/// <returns>属性名称</returns>
	public static Name GetPropertyFName(IntPtr nativeProp)
	{
		return NativeGetPropertyName(nativeProp);
	}

	/// <summary>
	/// 返回属性相对于类型的偏移量。
	/// </summary>
	/// <param name="nativeProp">FProperty指针</param>
	/// <returns>属性偏移量</returns>
	public static int GetPropertyOffset(IntPtr nativeProp)
	{
		return NativeGetPropertyOffset(nativeProp);
	}

	/// <summary>
	/// 根据名字查找属性，并返回属性相对于类型的偏移量。
	/// </summary>
	/// <param name="nativeStruct">UClass、UScriptStruct或UFunction指针</param>
	/// <param name="propName">属性名</param>
	/// <returns>属性偏移量</returns>
	public static int GetPropertyOffsetFromName(IntPtr nativeStruct, Name propName)
	{
		return NativeGetPropertyOffsetFromName(nativeStruct, propName);
	}

	/// <summary>
	/// 返回属性的大小。
	/// </summary>
	/// <param name="nativeProp">FProperty指针</param>
	/// <returns>属性偏移量</returns>
	public static int GetPropertySize(IntPtr nativeProp)
	{
		return NativeGetPropertySize(nativeProp);
	}

	/// <summary>
	/// 获取布尔型属性的bitfield mask。
	/// </summary>
	/// <param name="nativeBoolProp">FBoolProperty指针</param>
	/// <returns>bitfield mask</returns>
	public static byte GetBoolPropertyFieldMask(IntPtr nativeBoolProp)
	{
		return NativeGetBoolPropertyFieldMask(nativeBoolProp);
	}

	/// <summary>
	/// 返回FMapProperty中的KeyProp。
	/// </summary>
	/// <param name="nativeMapProp">FMapProperty指针</param>
	public static IntPtr GetMapKeyProperty(IntPtr nativeMapProp)
	{
		return NativeGetMapKeyProperty(nativeMapProp);
	}

	/// <summary>
	/// 返回FMapProperty中的ValueProp。
	/// </summary>
	/// <param name="nativeMapProp">FMapProperty指针</param>
	public static IntPtr GetMapValueProperty(IntPtr nativeMapProp)
	{
		return NativeGetMapValueProperty(nativeMapProp);
	}

	/// <summary>
	/// 返回FSetProperty中的ElementProp。
	/// </summary>
	/// <param name="nativeSetProp">FSetProperty指针</param>
	public static IntPtr GetSetElementProperty(IntPtr nativeSetProp)
	{
		return NativeGetSetElementProperty(nativeSetProp);
	}

	/// <summary>
	/// 返回FDelegateProp中的UFunction指针
	/// </summary>
	/// <param name="nativeDelegateProp">FDelegateProp指针</param>
	public static IntPtr GetDelegateSignatureFunction(IntPtr nativeDelegateProp)
	{
		return NativeGetDelegateSignatureFunction(nativeDelegateProp);
	}

	/// <summary>
	/// 比较两个属性是否相等。
	/// </summary>
	/// <param name="nativeProp">FProperty指针</param>
	/// <param name="nativeValueA">属性A的指针</param>
	/// <param name="nativeValueB">属性B的指针</param>
	/// <returns></returns>
	public static bool PropertyIdentical(IntPtr nativeProp, IntPtr nativeValueA, IntPtr nativeValueB)
	{
		int result = NativePropertyIdentical(nativeProp, nativeValueA, nativeValueB);
		return result != 0;
	}

	/// <summary>
	/// 初始化C++属性值。
	/// </summary>
	/// <param name="nativeProp">FProperty指针</param>
	/// <param name="value">C++值地址</param>
	public static void InitializePropertyValue(IntPtr nativeProp, IntPtr value)
	{
		NativeInitializePropertyValue(nativeProp, value);
	}

	/// <summary>
	/// 将属性拷贝到目标位置。
	/// </summary>
	/// <param name="nativeProp">FProperty指针</param>
	/// <param name="dest">拷贝目标</param>
	/// <param name="src">拷贝源</param>
	public static void CopyPropertyValue(IntPtr nativeProp, IntPtr dest, IntPtr src)
	{
		NativeCopyPropertyValue(nativeProp, dest, src);
	}

	/// <summary>
	/// 销毁C++属性值。
	/// </summary>
	/// <param name="nativeProp">FProperty指针</param>
	/// <param name="value">C++值地址</param>
	public static void DestroyPropertyValue(IntPtr nativeProp, IntPtr value)
	{
		NativeDestroyPropertyValue(nativeProp, value);
	}

#pragma warning disable CS0649
	internal static delegate* unmanaged[Cdecl]<char*, IntPtr> NativeFindClass;
	internal static delegate* unmanaged[Cdecl]<char*, IntPtr> NativeFindStruct;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> NativeCreateStructInstance;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr> NativeCloneStructInstance;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> NativeDestroyStructInstance;
	internal static delegate* unmanaged[Cdecl]<IntPtr, int> NativeGetStructureSize;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> NativeInitializeStruct;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> NativeUninitializeStruct;
	internal static delegate* unmanaged[Cdecl]<IntPtr, Name> NativeGetTypeName;
	internal static delegate* unmanaged[Cdecl]<char*, IntPtr> NativeFindEnum;
	internal static delegate* unmanaged[Cdecl]<IntPtr, Name, IntPtr> NativeFindFunction;
	internal static delegate* unmanaged[Cdecl]<IntPtr, int> NativeGetFunctionParamsSize;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> NativeGetFirstProperty;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> NativeGetNextProperty;
	internal static delegate* unmanaged[Cdecl]<IntPtr, Name, IntPtr> NativeFindProerty;
	internal static delegate* unmanaged[Cdecl]<IntPtr, Name> NativeGetPropertyName;
	internal static delegate* unmanaged[Cdecl]<IntPtr, int> NativeGetPropertyOffset;
	internal static delegate* unmanaged[Cdecl]<IntPtr, Name, int> NativeGetPropertyOffsetFromName;
	internal static delegate* unmanaged[Cdecl]<IntPtr, int> NativeGetPropertySize;
	internal static delegate* unmanaged[Cdecl]<IntPtr, byte> NativeGetBoolPropertyFieldMask;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> NativeGetMapKeyProperty;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> NativeGetMapValueProperty;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> NativeGetSetElementProperty;
	internal static delegate* unmanaged<IntPtr, IntPtr> NativeGetDelegateSignatureFunction;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, int> NativePropertyIdentical;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> NativeInitializePropertyValue;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, IntPtr, void> NativeCopyPropertyValue;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> NativeDestroyPropertyValue;
#pragma warning restore CS0649
}
