using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using static SharpScript.Subclassing.ClassSpecs;
using static SharpScript.Subclassing.PropSpecs;

namespace SharpScriptUnitTest.Types;

/// <summary>
/// A [UCLASS] whose binding code is produced by the SharpScript source generator, used to
/// exercise byte-backed UENUM values as container elements: TArray/TSet elements and TMap
/// key/value. Paired with <see cref="USsTestGenEnumContainerClassManual"/> (hand-written
/// bindings) to prove the generator output is equivalent.
/// </summary>
[UCLASS(BlueprintType)]
public partial class USsTestGenEnumContainerClassSourceGenerator : UObject
{
	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TArray<ESsTestGenEnumSourceGenerator> EnumArray { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TSet<ESsTestGenEnumSourceGenerator> EnumSet { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TMap<ESsTestGenEnumSourceGenerator, int> EnumKeyMap { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TMap<int, ESsTestGenEnumSourceGenerator> EnumValueMap { get; }
}
