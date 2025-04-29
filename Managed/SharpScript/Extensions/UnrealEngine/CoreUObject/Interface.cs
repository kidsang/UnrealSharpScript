using System.Runtime.InteropServices;

namespace UnrealEngine.CoreUObject;

/// <summary>
/// Base class for all interfaces
/// </summary>
public partial class Interface
{
}

/// <summary>
/// Base class for all interfaces
/// </summary>
public interface IInterface : IGetInterfaceClass
{
	static Class IGetInterfaceClass.InterfaceClass => Interface.StaticClass!;
}

/// <summary>
/// Helper to get UClass of interface
/// </summary>
public interface IGetInterfaceClass
{
	public static abstract Class InterfaceClass { get; }
}

/// <summary>
/// This utility class stores the FProperty data for a native interface property.  ObjectPointer and InterfacePointer point to different locations in the same UObject.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct ScriptInterface
{
	public IntPtr ObjectPointer;
	public IntPtr InterfacePointer;
}
