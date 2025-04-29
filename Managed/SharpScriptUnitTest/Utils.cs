using System.Runtime.CompilerServices;

namespace SharpScriptUnitTest;

/// <summary>
/// Some utility functions used for unit testing
/// </summary>
public static class Utils
{
	public static void Assert(bool condition, [CallerArgumentExpression("condition")] string? expression = null)
	{
		if (!condition)
		{
			throw new Exception($"assertion failed: \"{expression}\"");
		}
	}
}
