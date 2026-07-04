using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;
using static SharpScript.Subclassing.ClassSpecs;

namespace SharpScriptUnitTest.Types;

[UCLASS(BlueprintType)]
public partial class USsTestGenFunctionManual : UObject
{
	[UFUNCTION]
	public int FuncInt32(int InValue, out int OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public long FuncInt64(long InValue, out long OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public float FuncFloat(float InValue, out float OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public double FuncDouble(double InValue, out double OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public bool FuncBool(bool InValue, out bool OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public string FuncString(string InValue, out string OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public FName FuncName(FName InValue, out FName OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public FText FuncText(FText InValue, out FText OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public ESsTestGenEnumManual FuncEnum(ESsTestGenEnumManual InValue, out ESsTestGenEnumManual OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public List<string> FuncStringArray(List<string> InValue, out List<string> OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public HashSet<string> FuncStringSet(HashSet<string> InValue, out HashSet<string> OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public Dictionary<string, int> FuncStringIntMap(Dictionary<string, int> InValue, out Dictionary<string, int> OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public FSsTestGenStructManual FuncStruct(FSsTestGenStructManual InValue, out FSsTestGenStructManual OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public FSsTestBlittableGenStructManual FuncBlittableStruct(FSsTestBlittableGenStructManual InValue, out FSsTestBlittableGenStructManual OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public UObject? FuncObject(UObject? InValue, out UObject? OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public TSoftObjectPtr<UObject> FuncSoftObjectPtr(TSoftObjectPtr<UObject> InValue, out TSoftObjectPtr<UObject> OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public TSubclassOf<UObject> FuncClass(TSubclassOf<UObject> InValue, out TSubclassOf<UObject> OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public TSoftClassPtr<UObject> FuncSoftClassPtr(TSoftClassPtr<UObject> InValue, out TSoftClassPtr<UObject> OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public static int FuncStaticInt32(int InValue, out int OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}

	[UFUNCTION]
	public static string FuncStaticString(string InValue, out string OutValue)
	{
		OutValue = InValue;
		return OutValue;
	}
}
