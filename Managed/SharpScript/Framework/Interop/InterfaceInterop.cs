namespace SharpScript.Interop;

/// <summary>
/// Provides interop methods for UInterface.
/// </summary>
[NativeCallbacks]
public unsafe class InterfaceInterop
{
#pragma warning disable CS0649
	internal static delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> SetObjectAndInterface;
	internal static delegate* unmanaged<IntPtr, IntPtr> GetObject;
#pragma warning restore CS0649
}
