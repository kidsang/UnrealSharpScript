using SharpScript;
using SharpScript.Interop;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// Base class for all UObjects bound to C#.
/// </summary>
public unsafe class ObjectBase : IComparable<ObjectBase>
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

	public int CompareTo(ObjectBase? other)
	{
		IntPtr otherPtr = other?.NativeObject ?? IntPtr.Zero;
		return NativeObject.CompareTo(otherPtr);
	}
}
