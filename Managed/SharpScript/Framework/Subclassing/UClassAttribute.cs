namespace SharpScript.Subclassing;

[Flags]
public enum ClassSpecs : uint
{
	BlueprintType = 1 << 0,
}

[AttributeUsage(AttributeTargets.Class)]
// ReSharper disable once InconsistentNaming
public class UCLASSAttribute : Attribute
{
	public UCLASSAttribute(params ClassSpecs[] specifiers)
	{
		foreach (var specifier in specifiers)
		{
			Specifiers |= specifier;
		}
	}

	public ClassSpecs Specifiers { get; }

	public string? DisplayName;
}
