namespace UnrealEngine.Intrinsic;

/// <summary>
/// Encapsulation of FText.
/// </summary>
public readonly struct Text(string? data) : IEquatable<Text>
{
	internal string Data => data ?? string.Empty;

	public static readonly Text None = default;

	public bool Equals(Text other)
	{
		return Data == other.Data;
	}

	public override bool Equals(object? obj)
	{
		return obj is Text other && Equals(other);
	}

	public override int GetHashCode()
	{
		return Data.GetHashCode();
	}

	public override string ToString()
	{
		return Data;
	}

	public static bool operator ==(Text left, Text right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Text left, Text right)
	{
		return !(left == right);
	}

	public static implicit operator string(Text text)
	{
		return text.Data;
	}
}
