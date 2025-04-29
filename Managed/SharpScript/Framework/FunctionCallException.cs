namespace SharpScript;

/// <summary>
/// Exception thrown when calling UFunction fails.
/// </summary>
public class FunctionCallException(string message) : Exception(message);
