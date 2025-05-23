namespace SharpScript.Subclassing;

[Flags]
public enum PropSpecs : uint
{
	EditAnywhere = 1 << 0,

	BlueprintReadWrite = 1 << 1,
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
// ReSharper disable once InconsistentNaming
public class UPROPERTYAttribute : Attribute
{
	public UPROPERTYAttribute(params PropSpecs[] specifiers)
	{
		foreach (var specifier in specifiers)
		{
			Specifiers |= specifier;
		}
	}

	public PropSpecs Specifiers { get; }

	public string? Category;
}
