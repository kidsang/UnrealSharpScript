using System.Runtime.InteropServices;
using SharpScript.Interop;
using Object = UnrealEngine.CoreUObject.Object;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// Wrapper for FScriptDelegate.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ScriptDelegate : IEquatable<ScriptDelegate>
{
	internal ulong _accessDetector;
	internal WeakObjectData _object;
	internal Name _functionName;

	/// <summary>
	/// Binds a UFunction to this delegate.
	/// </summary>
	/// <param name="obj">The object to call the function on.</param>
	/// <param name="functionName">The name of the function to call.</param>
	public void BindUFunction(Object obj, Name functionName)
	{
		DelegateInterop.BindUFunction(ref this, obj.NativeObject, functionName);
	}

	/// <summary>
	///  Checks to see if the user object bound to this delegate is still valid
	/// </summary>
	/// <returns>True if the object is still valid and it's safe to execute the function call</returns>
	public bool IsBound()
	{
		return DelegateInterop.IsBound(ref this) != 0;
	}

	/// <summary>
	/// Checks to see if this delegate is bound to the given user object.
	/// </summary>
	/// <returns>True if this delegate is bound to InUserObject, false otherwise.</returns>
	public bool IsBoundToObject(Object obj)
	{
		return DelegateInterop.IsBoundToObject(ref this, obj.NativeObject) != 0;
	}

	/// <summary>
	/// Unbinds this delegate
	/// </summary>
	public void Unbind()
	{
		DelegateInterop.Unbind(ref this);
	}

	/// <summary>
	///  Gets the object bound to this delegate
	/// </summary>
	public Object? GetUObject()
	{
		IntPtr managedHandlePtr = DelegateInterop.GetUObject(ref this);
		GCHandle managedHandle = GCHandle.FromIntPtr(managedHandlePtr);
		return managedHandle.Target as Object;
	}

	/// <summary>
	/// Gets the name of the function to call on the bound object
	/// </summary>
	public Name GetFunctionName()
	{
		return DelegateInterop.GetFunctionName(ref this);
	}

	/// <summary>
	/// Executes a delegate by calling the named function on the object bound to the delegate.  You should
	/// always first verify that the delegate is safe to execute by calling IsBound() before calling this function.
	/// In general, you should never call this function directly.  Instead, call Execute() on a derived class.
	/// </summary>
	/// <param name="paramsBuffer">Parameter structure</param>
	internal void ProcessDelegate(IntPtr paramsBuffer)
	{
		DelegateInterop.ProcessDelegate(ref this, paramsBuffer);
	}

	public bool Equals(ScriptDelegate other)
	{
		return DelegateInterop.DelegateEquals(ref this, ref other) != 0;
	}

	public override bool Equals(object? obj)
	{
		return obj is ScriptDelegate other && Equals(other);
	}

	public override int GetHashCode()
	{
		return DelegateInterop.DoGetTypeHash(ref this);
	}

	public override string ToString()
	{
		NativeArray buffer = new NativeArray();
		try
		{
			DelegateInterop.DelegateToString(ref this, ref buffer);
			return new string((char*)buffer.Data);
		}
		finally
		{
			ArrayInterop.Destroy(ref buffer);
		}
	}
}
