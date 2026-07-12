using SharpScript;
using SharpScript.Interop;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// Base class for all UObjects bound to C#.
/// </summary>
public unsafe class UObjectBase : IComparable<UObjectBase>
{
	/// <summary>
	/// Underlying UObject
	/// </summary>
	public IntPtr NativeObject { get; internal set; }

	/// <summary>
	/// Determines if the held UObject exists, returns false if it doesn't.
	/// </summary>
	public bool IsValid()
	{
		return NativeObject != IntPtr.Zero && ObjectInterop.IsValid(NativeObject) != 0;
	}

	/// <summary>
	/// Throws an exception when the underlying UObject is invalid.
	/// </summary>
	protected void ThrowIfNotValid()
	{
		if (!IsValid())
		{
			throw new NativeObjectInvalidException(this);
		}
	}

	/// <summary>
	/// Calls UFunction
	/// </summary>
	/// <param name="nativeFunc">UFunction pointer</param>
	/// <param name="paramsPtr">Pointer to the structure of parameters and return values</param>
	protected void InvokeFunctionCall(IntPtr nativeFunc, IntPtr paramsPtr)
	{
		ThrowIfNotValid();

		int result = ObjectInterop.InvokeFunctionCall(NativeObject, nativeFunc, paramsPtr);
		if (result == 0)
		{
			string objName = ObjectInterop.GetName(NativeObject);
			string funcName = TypeInterop.GetTypeName(nativeFunc);
			string message = $"error calling function {objName}.{funcName}";
			throw new FunctionCallException(message);
		}
	}

	/// <summary>
	/// Calls a blueprint event UFunction (BlueprintNativeEvent) through virtual dispatch: the function is
	/// looked up by name on this object's ACTUAL class (native FindFunctionChecked), then invoked via
	/// ProcessEvent. This is what lets a base-class C# reference reach a blueprint subclass's override — a
	/// fixed UFunction pointer resolved from a static class (see <see cref="InvokeFunctionCall"/>) cannot.
	/// Generated event call-site interceptors route source-level calls here; the native dispatch thunk that
	/// runs the C# default implementation is a separate physical path, so no recursion occurs.
	/// </summary>
	/// <param name="funcName">Name of the event UFunction to dispatch.</param>
	/// <param name="paramsPtr">Pointer to the params/return-value buffer laid out per the UFunction.</param>
	protected void InvokeVirtualFunctionCall(FName funcName, IntPtr paramsPtr)
	{
		ThrowIfNotValid();

		int result = ObjectInterop.InvokeVirtualFunctionCall(NativeObject, funcName, paramsPtr);
		if (result == 0)
		{
			string objName = ObjectInterop.GetName(NativeObject);
			string message = $"error calling event function {objName}.{funcName}";
			throw new FunctionCallException(message);
		}
	}

	/// <summary>
	/// Calls static UFunction.
	/// </summary>
	/// <param name="nativeClass">Pointer to the UClass that the static method belongs to</param>
	/// <param name="nativeFunc">UFunction pointer</param>
	/// <param name="paramsPtr">Pointer to the structure of parameters and return values</param>
	protected static void InvokeStaticFunctionCall(IntPtr nativeClass, IntPtr nativeFunc, IntPtr paramsPtr)
	{
		int result = ObjectInterop.InvokeStaticFunctionCall(nativeClass, nativeFunc, paramsPtr);
		if (result == 0)
		{
			string objName = ObjectInterop.GetName(nativeClass);
			string funcName = TypeInterop.GetTypeName(nativeFunc);
			string message = $"error calling function {objName}.{funcName}";
			throw new FunctionCallException(message);
		}
	}

	public int CompareTo(UObjectBase? other)
	{
		IntPtr otherPtr = other?.NativeObject ?? IntPtr.Zero;
		return NativeObject.CompareTo(otherPtr);
	}
}
