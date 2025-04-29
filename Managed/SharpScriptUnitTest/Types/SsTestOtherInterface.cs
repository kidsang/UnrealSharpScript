using SharpScript.Interop;
using UnrealEngine.CoreUObject;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public class SsTestOtherInterface : Interface, IStaticClass<SsTestOtherInterface>
{
	public new static SubclassOf<SsTestOtherInterface> StaticClass { get; }

	public new static readonly IntPtr NativeType;

	static SsTestOtherInterface()
	{
		NativeType = TypeInterop.FindClass("SsTestOtherInterface");
		StaticClass = new SubclassOf<SsTestOtherInterface>(NativeType);
	}
}

public interface ISsTestOtherInterface : IInterface
{
	static Class IGetInterfaceClass.InterfaceClass => SsTestOtherInterface.StaticClass.Class!;

	public int FuncInterfaceOther(int InValue);
}
