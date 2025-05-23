namespace SharpScript.Subclassing;

[AttributeUsage(AttributeTargets.Struct)]
// ReSharper disable once InconsistentNaming
public class USTRUCTAttribute : Attribute
{
	public string? DisplayName;
}
