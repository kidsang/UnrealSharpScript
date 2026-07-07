using System.Runtime.InteropServices;
using UnrealEngine.Intrinsic;

namespace SharpScript.Subclassing;

/// <summary>
/// A single metadata key/value pair applied to a generated type.
/// <br/> See: SsSubclassingUtils.h FSsMetaDataEntry
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct MetaDataEntry
{
	/// <summary>
	/// Metadata key.
	/// </summary>
	public FName Key;

	/// <summary>
	/// Pointer to a null-terminated UTF-16 (wchar_t / TCHAR) metadata value string. The caller must
	/// keep the underlying string pinned/alive for the duration of the GenerateClass call.
	/// </summary>
	public char* Value;
}

/// <summary>
/// Meta info collected from csharp property definitions to create unreal property.
/// <br/> See: SsSubclassingUtils.h FSsPropertyDef
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct PropertyDef
{
	/// <summary>
	/// Name of property.
	/// </summary>
	public FName PropName;

	/// <summary>
	/// Class of UProperty, eg. UObjectProperty::StaticClass.
	/// </summary>
	public IntPtr PropType;

	/// <summary>
	/// Underlying type of this property, e.g. UObjectProperty::PropertyClass.
	/// </summary>
	public IntPtr UnderlyingType;

	/// <summary>
	/// For UArrayProperty or USetProperty, this represents the type of inner property.
	/// </summary>
	public IntPtr InnerPropType;

	/// <summary>
	/// For UArrayProperty or USetProperty, this represents the underlying type of inner property.
	/// </summary>
	public IntPtr InnerUnderlyingType;

	/// <summary>
	/// For UMapProperty, this represents the type of map key property.
	/// </summary>
	public IntPtr KeyPropType;

	/// <summary>
	/// For UMapProperty, this represents the underlying type of map key property.
	/// </summary>
	public IntPtr KeyUnderlyingType;
}

/// <summary>
/// Meta info collected from csharp enum value definitions to create unreal enum.
/// <br/> See: SsSubclassingUtils.h FSsEnumValueDef
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct EnumValueDef
{
	/// <summary>
	/// Name of enum value.
	/// </summary>
	public FName Name;

	/// <summary>
	/// Underlying integer value of enum value.
	/// </summary>
	public long Value;
}

/// <summary>
/// Engine-agnostic UFUNCTION parameter role flags. These are stable values defined by SharpScript,
/// NOT UE EPropertyFlags (CPF_*) — the C++ layer (USsSubclassingUtils::TranslateParamFlags) translates
/// them to the correct CPF_* for whatever engine version is in use, so the C# side stays version-proof.
/// <br/> Must be kept in sync with C++ ESsFunctionParamFlags.
/// </summary>
[Flags]
public enum ParamFlags : uint
{
	None = 0,

	/// <summary> Normal input parameter. </summary>
	InParam = 1 << 0,

	/// <summary> Value is copied out after the call (out parameter). </summary>
	OutParam = 1 << 1,

	/// <summary> The return value of the function. </summary>
	ReturnParam = 1 << 2,
}

/// <summary>
/// Meta info collected from a csharp UFUNCTION parameter to create a UFunction parameter property.
/// Shares the same shape as <see cref="PropertyDef"/> plus a parameter flags field.
/// <br/> See: SsSubclassingUtils.h FSsFunctionParamDef
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FunctionParamDef
{
	/// <summary>
	/// Name of the parameter.
	/// </summary>
	public FName ParamName;

	/// <summary>
	/// Class of UProperty, eg. UIntProperty::StaticClass.
	/// </summary>
	public IntPtr PropType;

	/// <summary>
	/// Underlying type of this parameter (object property class / struct / enum).
	/// </summary>
	public IntPtr UnderlyingType;

	/// <summary>
	/// For array/set inner or map value: the inner property class.
	/// </summary>
	public IntPtr InnerPropType;

	/// <summary>
	/// For array/set inner or map value: the inner underlying type.
	/// </summary>
	public IntPtr InnerUnderlyingType;

	/// <summary>
	/// For map key: the key property class.
	/// </summary>
	public IntPtr KeyPropType;

	/// <summary>
	/// For map key: the key underlying type.
	/// </summary>
	public IntPtr KeyUnderlyingType;

	/// <summary>
	/// Engine-agnostic parameter role flags. <see cref="ParamFlags.Parm"/> is always implied.
	/// </summary>
	public ParamFlags ParamFlags;
}

/// <summary>
/// Engine-agnostic UFUNCTION role flags. These are stable values defined by SharpScript,
/// NOT UE EFunctionFlags (FUNC_*) — the C++ layer (USsSubclassingUtils::TranslateFunctionFlags)
/// translates them to the correct FUNC_* for whatever engine version is in use, so the C# side
/// stays version-proof.
/// <br/> Must be kept in sync with C++ ESsFunctionFlags.
/// </summary>
[Flags]
public enum SsFunctionFlags : uint
{
	None = 0,

	/// <summary>
	/// The function is a C# <c>static</c> method. The generated UFunction is marked FUNC_Static and
	/// the native thunk does not resolve a "this" instance; the dispatch stub ignores the object handle.
	/// </summary>
	Static = 1 << 0,
}

/// <summary>
/// Meta info collected from a csharp UFUNCTION definition to create a UFunction on a generated class.
/// <br/> See: SsSubclassingUtils.h FSsFunctionDef
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FunctionDef
{
	/// <summary>
	/// Name of the function.
	/// </summary>
	public FName FuncName;

	/// <summary>
	/// Array of parameter defines (return / out params first, then input params in reverse order).
	/// </summary>
	public IntPtr Params;

	/// <summary>
	/// Count of parameter array.
	/// </summary>
	public int ParamCount;

	/// <summary>
	/// The managed dispatch function pointer for this function.
	/// A static [UnmanagedCallersOnly] stub: void(IntPtr managedObjectHandle, void* paramsBuffer).
	/// For static functions the managedObjectHandle argument is unused (IntPtr.Zero).
	/// </summary>
	public IntPtr ManagedDispatch;

	/// <summary>
	/// Engine-agnostic function role flags (e.g. <see cref="SsFunctionFlags.Static"/>).
	/// Translated to FUNC_* by the C++ layer.
	/// </summary>
	public SsFunctionFlags FunctionFlags;
}

/// <summary>
/// Meta info collected from a csharp [UCLASS] definition to create a unreal class.
/// <br/> See: SsSubclassingUtils.h FSsClassDef
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ClassDef
{
	/// <summary>
	/// Name of the class.
	/// </summary>
	public FName ClassName;

	/// <summary>
	/// Native super UClass pointer.
	/// </summary>
	public IntPtr SuperClass;

	/// <summary>
	/// Array of property defines.
	/// </summary>
	public IntPtr PropertyDefines;

	/// <summary>
	/// Count of property array.
	/// </summary>
	public int PropertyCount;

	/// <summary>
	/// Array of function defines (may be IntPtr.Zero when FunctionCount is 0).
	/// </summary>
	public IntPtr FunctionDefines;

	/// <summary>
	/// Count of function array.
	/// </summary>
	public int FunctionCount;

	/// <summary>
	/// The raw <c>ClassSpecs</c> bit set, passed through unchanged. The C++ layer expands it.
	/// </summary>
	public ulong Specifiers;

	/// <summary>
	/// Array of <see cref="MetaDataEntry"/> (may be IntPtr.Zero when MetaCount is 0).
	/// </summary>
	public IntPtr MetaEntries;

	/// <summary>
	/// Count of metadata entry array.
	/// </summary>
	public int MetaCount;

	/// <summary>
	/// Pointer to a null-terminated TCHAR configuration file name (UCLASS(Config=X)).
	/// May be IntPtr.Zero when no Config was specified; in that case the class inherits
	/// ClassConfigName from its super class.
	/// </summary>
	public IntPtr ConfigName;
}

[NativeCallbacks]
public static unsafe class SubclassingUtils
{
#pragma warning disable CS0649
	internal static delegate* unmanaged[Cdecl]<
		IntPtr, /** ManagedType */
		IntPtr, /** ClassDef* */
		IntPtr> GenerateClass;
	internal static delegate* unmanaged[Cdecl]<
		FName, /** StructName */
		IntPtr, /** PropertyDefines */
		int, /** PropertyCount */
		IntPtr> GenerateStruct;
	internal static delegate* unmanaged[Cdecl]<
		FName, /** EnumName */
		IntPtr, /** ValueDefines */
		int, /** ValueCount */
		byte, /** bIsFlags */
		IntPtr> GenerateEnum;
#pragma warning restore CS0649

	/// <summary>
	/// Resolve the C# wrapper object from the GCHandle IntPtr passed by the native UFunction thunk.
	/// Used by generated UFUNCTION dispatch stubs to recover the "this" instance.
	/// </summary>
	/// <typeparam name="T">The generated wrapper type.</typeparam>
	/// <param name="managedObjectHandle">GCHandle IntPtr of the C# wrapper object.</param>
	/// <returns>The C# wrapper instance.</returns>
	public static T ResolveManagedObject<T>(IntPtr managedObjectHandle) where T : class
	{
		GCHandle handle = GCHandle.FromIntPtr(managedObjectHandle);
		return (T)handle.Target!;
	}
}
