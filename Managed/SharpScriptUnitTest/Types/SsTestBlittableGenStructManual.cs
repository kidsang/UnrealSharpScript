using SharpScript.Subclassing;
using static SharpScript.Subclassing.PropSpecs;

namespace SharpScriptUnitTest.Types;

[USTRUCT]
public partial struct FSsTestBlittableGenStructManual : IEquatable<FSsTestBlittableGenStructManual>
{
	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public int X;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public int Y;

	public bool Equals(FSsTestBlittableGenStructManual other)
	{
		return X == other.X && Y == other.Y;
	}

	public override bool Equals(object? obj)
	{
		return obj is FSsTestBlittableGenStructManual other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}
}
