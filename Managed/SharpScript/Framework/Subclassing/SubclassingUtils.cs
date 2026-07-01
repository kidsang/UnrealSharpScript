using System.Runtime.InteropServices;
using UnrealEngine.Intrinsic;

namespace SharpScript.Subclassing;

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

[NativeCallbacks]
public static unsafe class SubclassingUtils
{
#pragma warning disable CS0649
	internal static delegate* unmanaged[Cdecl]<
		IntPtr, /** ManagedType */
		FName, /** ClassName */
		IntPtr, /** SuperClass */
		IntPtr, /** PropertyDefines */
		int, /** PropertyCount */
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
}
