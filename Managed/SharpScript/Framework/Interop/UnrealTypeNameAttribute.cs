namespace SharpScript.Interop;

/// <summary>
/// Holds the type name in unreal engine without prefix (F/U/A).
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
internal class UnrealTypeNameAttribute(string typeName) : Attribute
{
	public string TypeName => typeName;
}
