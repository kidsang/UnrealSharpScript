using UnrealEngine.Intrinsic;

namespace SharpScript;

/// <summary>
/// Exception thrown when the underlying C++ object becomes invalid (e.g., garbage collected by Unreal Engine).
/// </summary>
public class NativeObjectInvalidException(ObjectBase obj) : Exception($"'{obj.GetType().Name}' underlying UObject is invalid");
