using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

[UnrealTypeName("SsTestOtherInterface")]
public class USsTestOtherInterface : UInterface, IStaticClass<USsTestOtherInterface>
{
	public new static TSubclassOf<USsTestOtherInterface> StaticClass { get; }

	public new static readonly IntPtr NativeType;

	static USsTestOtherInterface()
	{
		NativeType = TypeInterop.FindClass("SsTestOtherInterface");
		StaticClass = new TSubclassOf<USsTestOtherInterface>(NativeType);
	}
}

public interface ISsTestOtherInterface : IInterface
{
	static UClass IGetInterfaceClass.InterfaceClass => USsTestOtherInterface.StaticClass.Class!;

	public int FuncInterfaceOther(int InValue);
}
