using SharpScript.Subclassing;
using static SharpScript.Subclassing.EnumSpecs;

namespace SharpScriptUnitTest.Types;

[UENUM(BlueprintType), Flags]
public enum ESsTestGenEnumSourceGenerator : byte
{
	One = 0,
	Two = 1,
	Three,
	Four,
}
