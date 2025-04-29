using SharpScript.Interop;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// Encapsulation of FieldPath.
/// </summary>
public readonly struct FieldPath(string? path) : IEquatable<FieldPath>
{
	/// <summary>
	/// Path to the FField object from the innermost FField to the outermost UObject (UPackage)
	/// </summary>
	public string Path => path ?? string.Empty;

	public static readonly int NativeDataSize;

	static FieldPath()
	{
		unsafe
		{
			NativeDataSize = FieldPathInterop.GetNativeDataSize();
		}
	}

	public FieldPath() : this(String.Empty)
	{
	}

	public bool Equals(FieldPath other)
	{
		return Path == other.Path;
	}

	public override bool Equals(object? obj)
	{
		return obj is FieldPath other && Equals(other);
	}

	public override int GetHashCode()
	{
		return Path.GetHashCode();
	}

	public override string ToString()
	{
		return Path;
	}
}
