using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;

namespace SharpScriptUnitTest.Types;

[UCLASS]
public partial class USsTestGenFuncSpec : UObject
{
	[UFUNCTION(FuncSpecs.Exec)]
	public void FuncExec(int InValue)
	{
	}

	[UFUNCTION(FuncSpecs.BlueprintCallable)]
	public int FuncBlueprintCallable(int InValue)
	{
		return InValue;
	}

	[UFUNCTION(FuncSpecs.BlueprintPure)]
	public int FuncBlueprintPure(int InValue)
	{
		return InValue;
	}

	[UFUNCTION(FuncSpecs.CallInEditor)]
	public void FuncCallInEditor()
	{
	}

	[UFUNCTION(FuncSpecs.Exec, FuncSpecs.BlueprintCallable)]
	public int FuncCombined(int InValue)
	{
		return InValue;
	}

	[UFUNCTION(FuncSpecs.BlueprintCallable,
		DisplayName = "Function Metadata Test",
		Category = "CSharp|Internal",
		Meta = ["ToolTip=Generated for SubclassingSpecifierTest", "CustomFlag"])]
	public int FuncMetadata(int InValue)
	{
		return InValue;
	}

	[UFUNCTION]
	public int FuncPlain(int InValue)
	{
		return InValue;
	}
}
