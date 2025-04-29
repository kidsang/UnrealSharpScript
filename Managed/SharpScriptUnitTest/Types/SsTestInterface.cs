using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class SsTestInterface : Interface, IStaticClass<SsTestInterface>
{
	public new static SubclassOf<SsTestInterface> StaticClass { get; }

	public new static readonly IntPtr NativeType;

	static SsTestInterface()
	{
		NativeType = TypeInterop.FindClass("SsTestInterface");
		StaticClass = new SubclassOf<SsTestInterface>(NativeType);
	}
}

public interface ISsTestInterface : IInterface
{
	static Class IGetInterfaceClass.InterfaceClass => SsTestInterface.StaticClass.Class!;

	public int FuncInterface(int InValue);
}
