namespace UnrealEngine.Engine;

/// <summary>
/// Streaming Source Priority
/// </summary>
public enum EStreamingSourcePriority : byte
{
	Highest = 0,

	High = 64,

	Normal = 128,

	Low = 192,

	Lowest = 255,

	Default = Normal,
}
