using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using static SharpScript.Subclassing.ClassSpecs;
using static SharpScript.Subclassing.PropSpecs;

namespace SharpScriptUnitTest.Types;

[UCLASS(BlueprintType)]
public partial class USsTestGenClassSourceGenerator : UObject
{
	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial bool Bool { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial int Int { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial double Float { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial ESsTestGenEnumSourceGenerator Enum { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial string String { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial FName Name { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial FText Text { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TArray<string> StringArray { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TSet<string> StringSet { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TMap<string, int> StringIntMap { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial FSsTestGenStructManualNativeRef Struct { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TArray<FSsTestGenStructManual, FSsTestGenStructManualNativeRef> StructArray { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial ref FSsTestBlittableGenStructManual BlittableStruct { get; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial UObject? Object { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TSoftObjectPtr<UObject> SoftObjectPtr { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TLazyObjectPtr<UObject> LazyObjectPtr { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TSubclassOf<UObject> Class { get; set; }

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public partial TSoftClassPtr<UObject> SoftClassPtr { get; set; }
}
