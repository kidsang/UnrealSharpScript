namespace UnrealEngine.Intrinsic;

/// <summary>
/// Encapsulation of FText.
/// </summary>
public readonly struct FText(string? data) : IEquatable<FText>
{
	internal string Data => data ?? string.Empty;

	public static readonly FText None = default;

	public bool Equals(FText other)
	{
		return Data == other.Data;
	}

	public override bool Equals(object? obj)
	{
		return obj is FText other && Equals(other);
	}

	public override int GetHashCode()
	{
		return Data.GetHashCode();
	}

	public override string ToString()
	{
		return Data;
	}

	public static bool operator ==(FText left, FText right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(FText left, FText right)
	{
		return !(left == right);
	}

	public static implicit operator string(FText text)
	{
		return text.Data;
	}
}
