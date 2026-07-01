namespace SharpScript.Subclassing;

[Flags]
public enum EnumSpecs : uint
{
	BlueprintType = 1 << 0,
}

[AttributeUsage(AttributeTargets.Enum)]
// ReSharper disable once InconsistentNaming
public class UENUMAttribute : Attribute
{
	public UENUMAttribute(params EnumSpecs[] specifiers)
	{
		foreach (var specifier in specifiers)
		{
			Specifiers |= specifier;
		}
	}

	public EnumSpecs Specifiers { get; }

	public string? DisplayName;
}
