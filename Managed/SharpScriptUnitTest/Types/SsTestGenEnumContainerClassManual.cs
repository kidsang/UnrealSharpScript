using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using static SharpScript.Subclassing.PropSpecs;

namespace SharpScriptUnitTest.Types;

/// <summary>
/// A [UCLASS] with hand-written bindings (see SsTestGenEnumContainerClassManual.generated.cs),
/// used to exercise byte-backed UENUM values as container elements: TArray/TSet elements and
/// TMap key/value. Paired with <see cref="USsTestGenEnumContainerClassSourceGenerator"/> to
/// prove the source-generator output is equivalent.
/// </summary>
[UCLASS]
public partial class USsTestGenEnumContainerClassManual : UObject
{
	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TArray<ESsTestGenEnumManual> EnumArray { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TSet<ESsTestGenEnumManual> EnumSet { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TMap<ESsTestGenEnumManual, int> EnumKeyMap { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TMap<int, ESsTestGenEnumManual> EnumValueMap { get; }
}
