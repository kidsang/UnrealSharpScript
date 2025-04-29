using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class SsTestChildInterface : SsTestInterface, IStaticClass<SsTestChildInterface>
{
	public new static SubclassOf<SsTestChildInterface> StaticClass { get; }

	public new static readonly IntPtr NativeType;

	static SsTestChildInterface()
	{
		NativeType = TypeInterop.FindClass("SsTestChildInterface");
		StaticClass = new SubclassOf<SsTestChildInterface>(NativeType);
	}
}

public interface ISsTestChildInterface : ISsTestInterface
{
	static Class IGetInterfaceClass.InterfaceClass => SsTestChildInterface.StaticClass.Class!;

	public int FuncInterfaceChild(int InValue);
}
