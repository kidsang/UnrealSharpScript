using SharpScript.Interop;
using Object = UnrealEngine.CoreUObject.Object;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// Wrapper for MulticastScriptDelegate.
/// </summary>
/// <param name="delegatePtr">C++ MulticastScriptDelegate pointer</param>
/// <typeparam name="T">Bindable C# delegate type</typeparam>
public unsafe class MulticastDelegate<T>(IntPtr delegatePtr)
	where T : Delegate
{
	/// <summary>
	/// Checks to see if any functions are bound to this multi-cast delegate
	/// </summary>
	/// <returns>True if any functions are bound</returns>
	public bool IsBound()
	{
		return MulticastDelegateInterop.IsBound(delegatePtr) != 0;
	}

	/// <summary>
	/// Checks whether a function delegate is already a member of this multi-cast delegate's invocation list
	/// </summary>
	/// <param name="value">Delegate to check</param>
	/// <returns>True if the delegate is already in the list.</returns>
	public bool Contains(T value)
	{
		if (value.Target is not Object obj)
		{
			return false;
		}

		return ContainsUFunction(obj, value.Method.Name);
	}

	/// <summary>
	/// Checks whether a function delegate is already a member of this multi-cast delegate's invocation list
	/// </summary>
	/// <param name="obj">delegate binding object</param>
	/// <param name="functionName">delegate function name</param>
	/// <returns>True if the delegate is already in the list.</returns>
	public bool ContainsUFunction(Object obj, Name functionName)
	{
		return MulticastDelegateInterop.ContainsUFunction(delegatePtr, obj.NativeObject, functionName) != 0;
	}

	/// <summary>
	/// Adds a function delegate to this multi-cast delegate's invocation list
	/// </summary>
	/// <param name="value">Delegate to add</param>
	public void Add(T value)
	{
		if (value.Target is not Object obj
			|| TypeInterop.FindFunction(obj.GetClass().NativeObject, value.Method.Name) == IntPtr.Zero)
		{
			throw new ArgumentException($"the callback for delegate must be a valid UFunction. {nameof(value)}");
		}

		AddUFunction(obj, value.Method.Name);
	}

	/// <summary>
	/// Adds a function delegate to this multi-cast delegate's invocation list
	/// </summary>
	/// <param name="obj">delegate binding object</param>
	/// <param name="functionName">delegate function name</param>
	public void AddUFunction(Object obj, Name functionName)
	{
		MulticastDelegateInterop.AddUFunction(delegatePtr, obj.NativeObject, functionName);
	}

	/// <summary>
	/// Adds a function delegate to this multi-cast delegate's invocation list if a delegate with the same signature
	/// doesn't already exist in the invocation list
	/// </summary>
	/// <param name="value">Delegate to add</param>
	public void AddUnique(T value)
	{
		if (value.Target is not Object obj
			|| TypeInterop.FindFunction(obj.GetClass().NativeObject, value.Method.Name) == IntPtr.Zero)
		{
			throw new ArgumentException($"the callback for delegate must be a valid UFunction. {nameof(value)}");
		}

		AddUniqueUFunction(obj, value.Method.Name);
	}

	/// <summary>
	/// Adds a function delegate to this multi-cast delegate's invocation list if a delegate with the same signature
	/// doesn't already exist in the invocation list
	/// </summary>
	/// <param name="obj">delegate binding object</param>
	/// <param name="functionName">delegate function name</param>
	public void AddUniqueUFunction(Object obj, Name functionName)
	{
		MulticastDelegateInterop.AddUniqueUFunction(delegatePtr, obj.NativeObject, functionName);
	}

	/// <summary>
	/// Removes a function from this multi-cast delegate's invocation list (performance is O(N)).  Note that the
	/// order of the delegates may not be preserved!
	/// </summary>
	/// <param name="value">Delegate to remove</param>
	public void Remove(T value)
	{
		if (value.Target is not Object obj)
		{
			return;
		}

		RemoveUFunction(obj, value.Method.Name);
	}

	/// <summary>
	/// Removes a function from this multi-cast delegate's invocation list (performance is O(N)).  Note that the
	/// order of the delegates may not be preserved!
	/// </summary>
	/// <param name="obj">delegate binding object</param>
	/// <param name="functionName">delegate function name</param>
	public void RemoveUFunction(Object obj, Name functionName)
	{
		MulticastDelegateInterop.RemoveUFunction(delegatePtr, obj.NativeObject, functionName);
	}

	/// <summary>
	/// Removes all delegate bindings from this multicast delegate's
	/// invocation list that are bound to the specified object.
	///
	/// This method also compacts the invocation list.
	/// </summary>
	/// <param name="obj">The object to remove bindings for.</param>
	public void RemoveAll(Object obj)
	{
		MulticastDelegateInterop.RemoveAll(delegatePtr, obj.NativeObject);
	}

	/// <summary>
	/// Removes all functions from this delegate's invocation list
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void Clear()
	{
		MulticastDelegateInterop.Clear(delegatePtr);
	}

	/// <summary>
	/// Executes a multi-cast delegate by calling all functions on objects bound to the delegate.  Always
	/// safe to call, even if when no objects are bound, or if objects have expired.  In general, you should
	/// never call this function directly.  Instead, call Broadcast() on a derived class.
	/// </summary>
	/// <param name="paramsBuffer">Parameter structure</param>
	public void ProcessMulticastDelegate(IntPtr paramsBuffer)
	{
		MulticastDelegateInterop.ProcessMulticastDelegate(delegatePtr, paramsBuffer);
	}

	public override string ToString()
	{
		NativeString buffer = new NativeString();
		try
		{
			MulticastDelegateInterop.MulticastDelegateToString(delegatePtr, ref buffer);
			return buffer.ToString();
		}
		finally
		{
			ArrayInterop.Destroy(ref buffer.Array);
		}
	}
}
