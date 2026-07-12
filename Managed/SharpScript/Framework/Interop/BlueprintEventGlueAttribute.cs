namespace SharpScript.Interop;

/// <summary>
/// Marks a binding-generated method as the managed glue for a C++ <c>BlueprintEvent</c> (BlueprintNativeEvent
/// / BlueprintImplementableEvent). It is emitted by the SharpScriptBindingGenerator onto the exported
/// <c>public virtual</c> event method so the <c>EventInterceptorGenerator</c> can recognise the call site and
/// redirect it to the generated <c>Invoke_&lt;Name&gt;</c> virtual-dispatch entry.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class BlueprintEventGlueAttribute(string engineName) : Attribute
{
	public string EngineName => engineName;
}
