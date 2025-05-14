using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

[UnrealTypeName("SsTestChildInterface")]
public class USsTestChildInterface : USsTestInterface, IStaticClass<USsTestChildInterface>
{
	public new static TSubclassOf<USsTestChildInterface> StaticClass { get; }

	public new static readonly IntPtr NativeType;

	static USsTestChildInterface()
	{
		NativeType = TypeInterop.FindClass("SsTestChildInterface");
		StaticClass = new TSubclassOf<USsTestChildInterface>(NativeType);
	}
}

public interface ISsTestChildInterface : ISsTestInterface
{
	static UClass IGetInterfaceClass.InterfaceClass => USsTestChildInterface.StaticClass.Class!;

	public int FuncInterfaceChild(int InValue);
}
