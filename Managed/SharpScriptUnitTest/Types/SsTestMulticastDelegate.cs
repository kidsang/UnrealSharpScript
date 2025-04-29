using SharpScript.Interop;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public delegate void SsTestMulticastDelegate(string InStr);

public static class SsTestMulticastDelegateInvoker
{
	// ReSharper disable InconsistentNaming
	private static IntPtr SsTestMulticastDelegate_NativeFunc;
	private static int SsTestMulticastDelegate_ParamsSize;
	private static int SsTestMulticastDelegate_InStr_Offset;
	// ReSharper restore InconsistentNaming

	public static void Initialize(IntPtr nativeDelegateProp)
	{
		SsTestMulticastDelegate_NativeFunc = TypeInterop.GetDelegateSignatureFunction(nativeDelegateProp);
		SsTestMulticastDelegate_ParamsSize = TypeInterop.GetFunctionParamsSize(SsTestMulticastDelegate_NativeFunc);
		SsTestMulticastDelegate_InStr_Offset = TypeInterop.GetPropertyOffsetFromName(SsTestMulticastDelegate_NativeFunc, "InStr");
	}

	public static unsafe void Broadcast(this MulticastDelegate<SsTestMulticastDelegate> instance, string InStr)
	{
		// ReSharper disable InconsistentNaming
		byte* _paramsBuffer = stackalloc byte[SsTestMulticastDelegate_ParamsSize];
		using ScopedFuncParams _params = new ScopedFuncParams(SsTestMulticastDelegate_NativeFunc, _paramsBuffer);

		StringMarshaller.ToNative(_params.Buffer + SsTestMulticastDelegate_InStr_Offset, InStr);

		instance.ProcessMulticastDelegate(_params.Buffer);
		// ReSharper restore InconsistentNaming
	}
}
