using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for native types.
/// </summary>
[NativeCallbacks]
public static unsafe class TypeInterop
{
	/// <summary>
	/// Find native UClass by class name.
	/// </summary>
	/// <param name="className">UClass name</param>
	/// <returns>UClass pointer</returns>
	public static IntPtr FindClass(string className)
	{
		fixed (char* bytes = className)
		{
			return NativeFindClass(bytes);
		}
	}

	/// <summary>
	/// Find native UScriptStruct by struct name.
	/// </summary>
	/// <param name="structName">UScriptStruct name</param>
	/// <returns>UScriptStruct pointer</returns>
	public static IntPtr FindStruct(string structName)
	{
		fixed (char* bytes = structName)
		{
			return NativeFindStruct(bytes);
		}
	}

	/// <summary>
	/// Create UE struct C++ instance
	/// </summary>
	/// <param name="nativeStruct">UScriptStruct pointer</param>
	/// <returns>Pointer to the newly created struct instance</returns>
	public static IntPtr CreateStructInstance(IntPtr nativeStruct)
	{
		return NativeCreateStructInstance(nativeStruct);
	}

	/// <summary>
	/// Clone UE struct C++ instance
	/// </summary>
	/// <param name="nativeStruct">UScriptStruct pointer</param>
	/// <param name="nativeSrcInstance">Pointer to the struct instance to be cloned</param>
	/// <returns>Pointer to the newly created struct instance</returns>
	public static IntPtr CloneStructInstance(IntPtr nativeStruct, IntPtr nativeSrcInstance)
	{
		return NativeCloneStructInstance(nativeStruct, nativeSrcInstance);
	}

	/// <summary>
	/// Destroy UE struct C++ instance
	/// </summary>
	/// <param name="nativeStruct">UScriptStruct pointer</param>
	/// <param name="nativeInstance">Pointer to the struct instance to be destroyed</param>
	public static void DestroyStructInstance(IntPtr nativeStruct, ref IntPtr nativeInstance)
	{
		NativeDestroyStructInstance(nativeStruct, nativeInstance);
		nativeInstance = 0;
	}

	/// <summary>
	/// Get the size of a type or struct
	/// </summary>
	/// <param name="nativeType"></param>
	/// <returns>Returns the actual memory usage of the type or struct</returns>
	public static int GetStructureSize(IntPtr nativeType)
	{
		return NativeGetStructureSize(nativeType);
	}

	/// <summary>
	/// Initialize memory for class, struct, or function parameters using allocated memory
	/// </summary>
	/// <param name="nativeType">UClass, UScriptStruct, or UFunction pointer</param>
	/// <param name="bufferPtr">Memory to be initialized</param>
	public static void InitializeStruct(IntPtr nativeType, IntPtr bufferPtr)
	{
		NativeInitializeStruct(nativeType, bufferPtr);
	}

	/// <summary>
	/// Deinitialize memory corresponding to class, struct, or function parameter information
	/// </summary>
	/// <param name="nativeType">UClass, UScriptStruct, or UFunction pointer</param>
	/// <param name="bufferPtr">Memory to be deinitialized</param>
	public static void DeinitializeStruct(IntPtr nativeType, IntPtr bufferPtr)
	{
		NativeDeinitializeStruct(nativeType, bufferPtr);
	}

	/// <summary>
	/// Get the name of a class, struct, or method
	/// </summary>
	/// <param name="nativeType">Pointer to the class, struct, or method</param>
	public static string GetTypeName(IntPtr nativeType)
	{
		return NativeGetTypeName(nativeType).ToString();
	}

	/// <summary>
	/// Find native UEnum by enum name.
	/// </summary>
	/// <param name="enumName">UEnum name</param>
	/// <returns>UEnum pointer</returns>
	public static IntPtr FindEnum(string enumName)
	{
		fixed (char* bytes = enumName)
		{
			return NativeFindEnum(bytes);
		}
	}

	/// <summary>
	/// Find UFunction with the specified name in UClass.
	/// </summary>
	/// <param name="nativeClass">UClass pointer</param>
	/// <param name="funcName">Method name</param>
	/// <returns>UFunction pointer</returns>
	public static IntPtr FindFunction(IntPtr nativeClass, FName funcName)
	{
		return NativeFindFunction(nativeClass, funcName);
	}

	/// <summary>
	/// Return the size of UFunction parameter return value struct.
	/// </summary>
	/// <param name="nativeFunc">UFunction pointer</param>
	/// <returns>Size of parameter return value struct</returns>
	public static int GetFunctionParamsSize(IntPtr nativeFunc)
	{
		return NativeGetFunctionParamsSize(nativeFunc);
	}

	/// <summary>
	/// Return the EFunctionFlags (FUNC_*) bit set of a UFunction.
	/// </summary>
	/// <param name="nativeFunc">UFunction pointer</param>
	/// <returns>The raw FUNC_* flags as a uint.</returns>
	public static uint GetFunctionFlags(IntPtr nativeFunc)
	{
		return NativeGetFunctionFlags(nativeFunc);
	}


	/// <summary>
	/// Initialize function parameter buffer.
	/// </summary>
	/// <param name="nativeType">UFunction pointer</param>
	/// <param name="bufferPtr">Buffer to be initialized</param>
	/// <remarks>Buffer *must* be zero initialized before calling this function.</remarks>
	public static void InitializeFunctionParams(IntPtr nativeType, IntPtr bufferPtr)
	{
		NativeInitializeFunctionParams(nativeType, bufferPtr);
	}

	/// <summary>
	/// Deinitialize function parameter buffer.
	/// </summary>
	/// <param name="nativeType">UFunction pointer</param>
	/// <param name="bufferPtr">Buffer to be deinitialized</param>
	public static void DeinitializeFunctionParams(IntPtr nativeType, IntPtr bufferPtr)
	{
		NativeDeinitializeFunctionParams(nativeType, bufferPtr);
	}

	/// <summary>
	/// Return the first property of the type.
	/// </summary>
	/// <param name="nativeStruct">UClass, UScriptStruct, or UFunction pointer</param>
	/// <returns>The first property of the type, or null if it doesn't exist.</returns>
	public static IntPtr GetFirstProperty(IntPtr nativeStruct)
	{
		return NativeGetFirstProperty(nativeStruct);
	}

	/// <summary>
	/// Return the next property in the property chain.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <returns>The next property, or null if it doesn't exist.</returns>
	public static IntPtr GetNextProperty(IntPtr nativeProp)
	{
		return NativeGetNextProperty(nativeProp);
	}

	/// <summary>
	/// Find property with the specified name in UClass, UScriptStruct, or UFunction.
	/// </summary>
	/// <param name="nativeStruct">UClass, UScriptStruct, or UFunction pointer</param>
	/// <param name="propName">Property name</param>
	/// <returns>FProperty pointer</returns>
	public static IntPtr FindProperty(IntPtr nativeStruct, FName propName)
	{
		return NativeFindProperty(nativeStruct, propName);
	}

	/// <summary>
	/// Get property name.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <returns>Property name</returns>
	public static string GetPropertyName(IntPtr nativeProp)
	{
		return NativeGetPropertyName(nativeProp).ToString();
	}

	/// <summary>
	/// Get property name.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <returns>Property name</returns>
	public static FName GetPropertyFName(IntPtr nativeProp)
	{
		return NativeGetPropertyName(nativeProp);
	}

	/// <summary>
	/// Return the offset of the property relative to the type.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <returns>Property offset</returns>
	public static int GetPropertyOffset(IntPtr nativeProp)
	{
		return NativeGetPropertyOffset(nativeProp);
	}

	/// <summary>
	/// Find property by name and return its offset relative to the type.
	/// </summary>
	/// <param name="nativeStruct">UClass, UScriptStruct, or UFunction pointer</param>
	/// <param name="propName">Property name</param>
	/// <returns>Property offset</returns>
	public static int GetPropertyOffsetFromName(IntPtr nativeStruct, FName propName)
	{
		return NativeGetPropertyOffsetFromName(nativeStruct, propName);
	}

	/// <summary>
	/// Return the size of the property.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <returns>Property size</returns>
	public static int GetPropertySize(IntPtr nativeProp)
	{
		return NativeGetPropertySize(nativeProp);
	}

	/// <summary>
	/// Return the property flags (EPropertyFlags / CPF_*) of the property.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <returns>The raw EPropertyFlags bit set.</returns>
	public static UInt64 GetPropertyFlags(IntPtr nativeProp)
	{
		return NativeGetPropertyFlags(nativeProp);
	}

	/// <summary>
	/// Determines if the property has any metadata associated with the key.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <param name="key">Metadata key</param>
	/// <remarks>
	/// Metadata only exists in editor builds, so this always returns false in non-editor builds.
	/// </remarks>
	public static bool HasPropertyMetaData(IntPtr nativeProp, string key)
	{
		fixed (char* keyPtr = key)
		{
			return NativeHasPropertyMetaData(nativeProp, (IntPtr)keyPtr) != 0;
		}
	}

	/// <summary>
	/// Find the metadata value associated with the key on the property.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <param name="key">Metadata key</param>
	/// <remarks>
	/// Metadata only exists in editor builds, so this always returns an empty string in non-editor builds.
	/// </remarks>
	public static string GetPropertyMetaData(IntPtr nativeProp, string key)
	{
		NativeString buffer = new NativeString();
		try
		{
			fixed (char* keyPtr = key)
			{
				NativeGetPropertyMetaData(nativeProp, (IntPtr)keyPtr, ref buffer);
			}

			return buffer.ToString();
		}
		finally
		{
			ArrayInterop.Destroy(ref buffer.Array);
		}
	}

	/// <summary>
	/// Determines if the type (UClass / UScriptStruct / UEnum ... any UField) has any metadata
	/// associated with the key.
	/// </summary>
	/// <param name="nativeType">UField pointer (e.g. a UClass's NativeObject or a generated struct/enum's NativeType).</param>
	/// <param name="key">Metadata key</param>
	/// <remarks>
	/// Metadata only exists in editor builds, so this always returns false in non-editor builds.
	/// </remarks>
	public static bool HasTypeMetaData(IntPtr nativeType, string key)
	{
		fixed (char* keyPtr = key)
		{
			return NativeHasTypeMetaData(nativeType, (IntPtr)keyPtr) != 0;
		}
	}

	/// <summary>
	/// Find the metadata value associated with the key on the type (UClass / UScriptStruct / UEnum ...).
	/// </summary>
	/// <param name="nativeType">UField pointer (e.g. a UClass's NativeObject or a generated struct/enum's NativeType).</param>
	/// <param name="key">Metadata key</param>
	/// <remarks>
	/// Metadata only exists in editor builds, so this always returns an empty string in non-editor builds.
	/// </remarks>
	public static string GetTypeMetaData(IntPtr nativeType, string key)
	{
		NativeString buffer = new NativeString();
		try
		{
			fixed (char* keyPtr = key)
			{
				NativeGetTypeMetaData(nativeType, (IntPtr)keyPtr, ref buffer);
			}

			return buffer.ToString();
		}
		finally
		{
			ArrayInterop.Destroy(ref buffer.Array);
		}
	}

	/// <summary>
	/// Get the bitfield mask of a boolean property.
	/// </summary>
	/// <param name="nativeBoolProp">FBoolProperty pointer</param>
	/// <returns>Bitfield mask</returns>
	public static byte GetBoolPropertyFieldMask(IntPtr nativeBoolProp)
	{
		return NativeGetBoolPropertyFieldMask(nativeBoolProp);
	}

	/// <summary>
	/// Return the KeyProp of FMapProperty.
	/// </summary>
	/// <param name="nativeMapProp">FMapProperty pointer</param>
	public static IntPtr GetMapKeyProperty(IntPtr nativeMapProp)
	{
		return NativeGetMapKeyProperty(nativeMapProp);
	}

	/// <summary>
	/// Return the ValueProp of FMapProperty.
	/// </summary>
	/// <param name="nativeMapProp">FMapProperty pointer</param>
	public static IntPtr GetMapValueProperty(IntPtr nativeMapProp)
	{
		return NativeGetMapValueProperty(nativeMapProp);
	}

	/// <summary>
	/// Return the ElementProp of FSetProperty.
	/// </summary>
	/// <param name="nativeSetProp">FSetProperty pointer</param>
	public static IntPtr GetSetElementProperty(IntPtr nativeSetProp)
	{
		return NativeGetSetElementProperty(nativeSetProp);
	}

	/// <summary>
	/// Return the UFunction pointer of FDelegateProp
	/// </summary>
	/// <param name="nativeDelegateProp">FDelegateProp pointer</param>
	public static IntPtr GetDelegateSignatureFunction(IntPtr nativeDelegateProp)
	{
		return NativeGetDelegateSignatureFunction(nativeDelegateProp);
	}

	/// <summary>
	/// Compare if two properties are equal.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <param name="nativeValueA">Pointer to property A</param>
	/// <param name="nativeValueB">Pointer to property B</param>
	/// <returns></returns>
	public static bool PropertyIdentical(IntPtr nativeProp, IntPtr nativeValueA, IntPtr nativeValueB)
	{
		int result = NativePropertyIdentical(nativeProp, nativeValueA, nativeValueB);
		return result != 0;
	}

	/// <summary>
	/// Initialize C++ property value.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <param name="value">C++ value address</param>
	public static void InitializePropertyValue(IntPtr nativeProp, IntPtr value)
	{
		NativeInitializePropertyValue(nativeProp, value);
	}

	/// <summary>
	/// Copy property to the target location.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <param name="dest">Copy destination</param>
	/// <param name="src">Copy source</param>
	public static void CopyPropertyValue(IntPtr nativeProp, IntPtr dest, IntPtr src)
	{
		NativeCopyPropertyValue(nativeProp, dest, src);
	}

	/// <summary>
	/// Destroy C++ property value.
	/// </summary>
	/// <param name="nativeProp">FProperty pointer</param>
	/// <param name="value">C++ value address</param>
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
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> NativeDeinitializeStruct;
	internal static delegate* unmanaged[Cdecl]<IntPtr, FName> NativeGetTypeName;
	internal static delegate* unmanaged[Cdecl]<char*, IntPtr> NativeFindEnum;
	internal static delegate* unmanaged[Cdecl]<IntPtr, FName, IntPtr> NativeFindFunction;
	internal static delegate* unmanaged[Cdecl]<IntPtr, int> NativeGetFunctionParamsSize;
	internal static delegate* unmanaged[Cdecl]<IntPtr, uint> NativeGetFunctionFlags;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> NativeInitializeFunctionParams;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> NativeDeinitializeFunctionParams;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> NativeGetFirstProperty;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr> NativeGetNextProperty;
	internal static delegate* unmanaged[Cdecl]<IntPtr, FName, IntPtr> NativeFindProperty;
	internal static delegate* unmanaged[Cdecl]<IntPtr, FName> NativeGetPropertyName;
	internal static delegate* unmanaged[Cdecl]<IntPtr, int> NativeGetPropertyOffset;
	internal static delegate* unmanaged[Cdecl]<IntPtr, FName, int> NativeGetPropertyOffsetFromName;
	internal static delegate* unmanaged[Cdecl]<IntPtr, int> NativeGetPropertySize;
	internal static delegate* unmanaged[Cdecl]<IntPtr, UInt64> NativeGetPropertyFlags;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int> NativeHasPropertyMetaData;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ref NativeString, void> NativeGetPropertyMetaData;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int> NativeHasTypeMetaData;
	internal static delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ref NativeString, void> NativeGetTypeMetaData;
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
