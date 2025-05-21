using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using static SharpScript.Subclassing.ClassSpecs;
using static SharpScript.Subclassing.PropSpecs;

namespace SharpScriptUnitTest.Types;

[UCLASS(BlueprintType)]
public partial class USsTestGenClassManual : UObject
{
	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial bool Bool { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial int Int { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial double Float { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial string String { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial FName Name { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial FText Text { get; set; }
}
