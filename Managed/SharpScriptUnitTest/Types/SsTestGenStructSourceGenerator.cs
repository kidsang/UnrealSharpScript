using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using static SharpScript.Subclassing.PropSpecs;

namespace SharpScriptUnitTest.Types;

[USTRUCT]
public partial struct FSsTestGenStructSourceGenerator
{
	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public bool Bool;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public int Int;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public double Float;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public string String;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public FName Name;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public FText Text;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public List<string> StringArray;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public HashSet<string> StringSet;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public Dictionary<string, int> StringIntMap;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public FSsArrayTestInnerGenStructManual Struct;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public UObject? Object;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public TSoftObjectPtr<UObject> SoftObjectPtr;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public TLazyObjectPtr<UObject> LazyObjectPtr;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public TSubclassOf<UObject> Class;

	[UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "CSharp|Internal")]
	public TSoftClassPtr<UObject> SoftClassPtr;
}
