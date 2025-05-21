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
#pragma warning restore CS0649
}
