using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

[UnrealTypeName("SsTestInterface")]
public class USsTestInterface : UInterface, IStaticClass<USsTestInterface>
{
	public new static TSubclassOf<USsTestInterface> StaticClass { get; }

	public new static readonly IntPtr NativeType;

	static USsTestInterface()
	{
		NativeType = TypeInterop.FindClass("SsTestInterface");
		StaticClass = new TSubclassOf<USsTestInterface>(NativeType);
	}
}

public interface ISsTestInterface : IInterface
{
	static UClass IGetInterfaceClass.InterfaceClass => USsTestInterface.StaticClass.Class!;

	public int FuncInterface(int InValue);
}
