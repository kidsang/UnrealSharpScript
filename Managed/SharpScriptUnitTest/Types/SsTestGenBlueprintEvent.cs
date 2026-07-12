using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using UnrealEngine.SharpScriptUnitTest;

namespace SharpScriptUnitTest.Types;

[UCLASS]
public partial class USsTestGenBlueprintEvent : USsBindingTestGenBlueprintEventBase
{
	/// <summary>
	/// Records whether the default implementation of <see cref="DoubleScoreEvent"/> actually ran.
	/// </summary>
	public bool DoubleScoreBodyRan;

	/// <summary>
	/// Result stored by the void event <see cref="RecordNotifyEvent"/> (amount * 3).
	/// </summary>
	public int RecordedNotifyValue;

	/// <summary>When true, <see cref="BaseNativeScore"/> calls base and returns its result + 1.</summary>
	public bool CallBaseNativeScore;

	/// <summary>The value observed from base.BaseNativeScore() (the C++ _Implementation), if called.</summary>
	public int LastBaseNativeResult;

	/// <summary>Records whether the C# override of <see cref="BaseNativeScore"/> ran.</summary>
	public bool BaseNativeScoreBodyRan;

	/// <summary>Records whether the C# override of <see cref="BaseImplScore"/> ran.</summary>
	public bool BaseImplScoreBodyRan;

	/// <summary>
	/// A blueprint-overridable event with a native (C#) default implementation that doubles its input.
	/// </summary>
	[UFUNCTION(FuncSpecs.BlueprintEvent)]
	public int DoubleScoreEvent(int score)
	{
		DoubleScoreBodyRan = true;
		return score * 2;
	}

	/// <summary>
	/// A BlueprintEvent with an out parameter and a non-blittable (string) type, exercising in/out/return
	/// marshalling across the ProcessEvent boundary.
	/// </summary>
	[UFUNCTION(FuncSpecs.BlueprintEvent)]
	public string FormatGreetingEvent(string name, out int length)
	{
		string greeting = $"Hello, {name}!";
		length = greeting.Length;
		return greeting;
	}

	/// <summary>
	/// A void BlueprintEvent, exercising the no-return dispatch path. Stores the tripled value into
	/// </summary>
	[UFUNCTION(FuncSpecs.BlueprintEvent)]
	public void RecordNotifyEvent(int amount)
	{
		RecordedNotifyValue = amount * 3;
	}

	/// <summary>
	/// C# override of the C++ base <c>BlueprintNativeEvent</c> <see cref="USsBindingTestGenBlueprintEventBase.BaseNativeScore"/>.
	/// </summary>
	[UFUNCTION(FuncSpecs.BlueprintEvent)]
	public override int BaseNativeScore(int InValue)
	{
		BaseNativeScoreBodyRan = true;
		if (CallBaseNativeScore)
		{
			LastBaseNativeResult = base.BaseNativeScore(InValue);
			return LastBaseNativeResult + 1;
		}

		return InValue * 2;
	}

	/// <summary>
	/// C# override of the C++ base <c>BlueprintImplementableEvent</c>
	/// </summary>
	[UFUNCTION(FuncSpecs.BlueprintEvent)]
	public override int BaseImplScore(int InValue)
	{
		BaseImplScoreBodyRan = true;
		return InValue * 10;
	}
}
