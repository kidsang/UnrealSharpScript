using UnrealEngine.Intrinsic;

namespace SharpScript;

/// <summary>
/// Capture global unhandled exceptions and redirect the exception stack to UE logs
/// </summary>
public static class UnhandledExceptionHandler
{
	internal static void Initialize()
	{
		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
	}

	internal static void Uninitialize()
	{
		AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
	}

	private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		Exception ex = (e.ExceptionObject as Exception)!;
		Logger.Error(ex.ToString());
	}
}
