using System.Runtime.CompilerServices;

namespace SharpScriptUnitTest;

/// <summary>
/// Record the file path and line number where the class is defined
/// </summary>
public class RecordFilePathAttribute([CallerFilePath] string filePath = "", [CallerLineNumber] int lineNo = 0)
	: Attribute
{
	public readonly string FilePath = filePath;
	public readonly int LineNo = lineNo;
}
