namespace SharpScript;

/// <summary>
/// When a class is marked as NativeCallbacks, static unmanaged delegate class members will attempt to bind with C++ function pointers.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class NativeCallbacksAttribute : Attribute;
