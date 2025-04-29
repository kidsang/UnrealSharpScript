using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public delegate int SsTestDelegate(int InValue);

public static class SsTestDelegateInvoker
{
	// ReSharper disable InconsistentNaming
	private static IntPtr SsTestDelegate_NativeFunc;
	private static int SsTestDelegate_ParamsSize;
	private static int SsTestDelegate_InValue_Offset;
	private static int SsTestDelegate_ReturnValue_Offset;
	// ReSharper restore InconsistentNaming

	public static void Initialize(IntPtr nativeDelegateProp)
	{
		SsTestDelegate_NativeFunc = TypeInterop.GetDelegateSignatureFunction(nativeDelegateProp);
		SsTestDelegate_ParamsSize = TypeInterop.GetFunctionParamsSize(SsTestDelegate_NativeFunc);
		SsTestDelegate_InValue_Offset = TypeInterop.GetPropertyOffsetFromName(SsTestDelegate_NativeFunc, "InValue");
		SsTestDelegate_ReturnValue_Offset = TypeInterop.GetPropertyOffsetFromName(SsTestDelegate_NativeFunc, "ReturnValue");
	}

	public static unsafe int Execute(this Delegate<SsTestDelegate> instance, int InValue)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[SsTestDelegate_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(SsTestDelegate_NativeFunc, _paramsBuffer);

		BlittableMarshaller<int>.ToNative(_params.Buffer + SsTestDelegate_InValue_Offset, InValue);

		instance.ProcessDelegate(_params.Buffer);

		int returnValue = BlittableMarshaller<int>.FromNative(_params.Buffer + SsTestDelegate_ReturnValue_Offset);
		return returnValue;
		// ReSharper restore InconsistentNaming
	}
}
