namespace UnrealEngine.Intrinsic;

public static unsafe class Logger
{
	/// <summary>
	/// Prints a message to a log file (does not print to console)
	/// </summary>
	public static void Log(string message)
	{
		fixed (char* bytes = message)
		{
			NativeLog(bytes);
		}
	}

	/// <summary>
	/// Prints a message to console (and log file)
	/// </summary>
	public static void Display(string message)
	{
		fixed (char* bytes = message)
		{
			NativeDisplay(bytes);
		}
	}

	/// <summary>
	/// Prints a warning to console (and log file).
	/// Commandlets and the editor collect and report warnings. Warnings can be treated as an error.
	/// </summary>
	public static void Warning(string message)
	{
		fixed (char* bytes = message)
		{
			NativeWarning(bytes);
		}
	}

	/// <summary>
	/// Prints an error to console (and log file).
	/// Commandlets and the editor collect and report errors. Error messages result in commandlet failure.
	/// </summary>
	public static void Error(string message)
	{
		fixed (char* bytes = message)
		{
			NativeError(bytes);
		}
	}

	internal static delegate* unmanaged[Cdecl]<void*, void> NativeLog;
	internal static delegate* unmanaged[Cdecl]<void*, void> NativeDisplay;
	internal static delegate* unmanaged[Cdecl]<void*, void> NativeWarning;
	internal static delegate* unmanaged[Cdecl]<void*, void> NativeError;
}
