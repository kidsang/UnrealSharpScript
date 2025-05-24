using SharpScript.Subclassing;
using static SharpScript.Subclassing.PropSpecs;

namespace SharpScriptUnitTest.Types;

[USTRUCT]
public struct FSsArrayTestInnerGenStructManual : IEquatable<FSsArrayTestInnerGenStructManual>
{
	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public List<int> IntArray;

	public bool Equals(FSsArrayTestInnerGenStructManual other)
	{
		return IntArray.SequenceEqual(other.IntArray);
	}

	public override bool Equals(object? obj)
	{
		return obj is FSsArrayTestInnerGenStructManual other && Equals(other);
	}

	public override int GetHashCode()
	{
		return IntArray.GetHashCode();
	}
}
