namespace UnrealEngine.EnhancedInput;

/// <summary>
/// Trigger events are the Action's interpretation of all Trigger State transitions that occurred for the action in the last tick
/// </summary>
[Flags]
public enum ETriggerEvent : byte
{
	/// <summary>
	/// No significant trigger state changes occurred and there are no active device inputs
	/// </summary>
	None = 0,

	/// <summary>
	/// Triggering occurred after one or more processing ticks
	/// </summary>
	Triggered = 1 << 0,

	/// <summary>
	/// An event has occurred that has begun Trigger evaluation. Note: Triggered may also occur this frame, but this event will always be fired first.
	/// </summary>
	Started = 1 << 1,

	/// <summary>
	/// Triggering is still being processed. For example, an action with a "Press and Hold" trigger
	/// will be "Ongoing" while the user is holding down the key but the time threshold has not been met yet.
	/// </summary>
	Ongoing = 1 << 2,

	/// <summary>
	/// Triggering has been canceled. For example,  the user has let go of a key before the "Press and Hold" time threshold.
	/// The action has started to be evaluated, but never completed.
	/// </summary>
	Canceled = 1 << 3,

	/// <summary>
	/// The trigger state has transitioned from Triggered to None this frame, i.e. Triggering has finished.
	/// Note: Using this event restricts you to one set of triggers for Started/Completed events. You may prefer two actions, each with its own trigger rules.
	/// Completed will not fire if any trigger reports Ongoing on the same frame, but both should fire. e.g. Tick 2 of Hold (= Ongoing) + Pressed (= None) combo will raise Ongoing event only.
	/// </summary>
	Completed = 1 << 4,
}
