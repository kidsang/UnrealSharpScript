using SharpScript.Interop;
using Object = UnrealEngine.CoreUObject.Object;

namespace UnrealEngine.Intrinsic;
/// <summary>
/// Wrapper for ScriptDelegate.
/// </summary>
/// <param name="delegatePtr">C++ ScriptDelegate pointer</param>
/// <typeparam name="T">Bindable C# delegate type</typeparam>
public unsafe class Delegate<T>(IntPtr delegatePtr)
	where T : Delegate
{
	/// <summary>
	/// ScriptDelegate pointer
	/// </summary>
	private readonly ScriptDelegate* _scriptDelegate = (ScriptDelegate*)delegatePtr;

	/// <summary>
	/// Bind a C# callback function, this callback must be a UFunction.
	/// </summary>
	/// <param name="handler">C# callback function.</param>
	public void Bind(T handler)
	{
		if (_scriptDelegate->IsBound())
		{
			throw new InvalidOperationException($"delegate already bound with '{_scriptDelegate->ToString()}'.");
		}

		if (handler.Target is not Object targetObject
			|| TypeInterop.FindFunction(targetObject.GetClass().NativeObject, handler.Method.Name) == IntPtr.Zero)
		{
			throw new ArgumentException($"the callback for delegate must be a valid UFunction. {nameof(handler)}");
		}

		_scriptDelegate->BindUFunction(targetObject, handler.Method.Name);
	}

	/// <inheritdoc cref="ScriptDelegate.BindUFunction"/>
	public void BindUFunction(Object obj, Name functionName)
	{
		if (_scriptDelegate->IsBound())
		{
			throw new InvalidOperationException($"delegate already bound with '{_scriptDelegate->ToString()}'.");
		}

		if (TypeInterop.FindFunction(obj.GetClass().NativeObject, functionName) == IntPtr.Zero)
		{
			throw new ArgumentException($"the callback for delegate must be a valid UFunction. {functionName}");
		}

		_scriptDelegate->BindUFunction(obj, functionName);
	}

	/// <inheritdoc cref="ScriptDelegate.IsBound"/>
	public bool IsBound()
	{
		return _scriptDelegate->IsBound();
	}

	/// <inheritdoc cref="ScriptDelegate.IsBoundToObject"/>
	public bool IsBoundToObject(Object obj)
	{
		return _scriptDelegate->IsBoundToObject(obj);
	}

	/// <inheritdoc cref="ScriptDelegate.Unbind"/>
	public void Unbind()
	{
		_scriptDelegate->Unbind();
	}

	/// <inheritdoc cref="ScriptDelegate.ProcessDelegate"/>
	public void ProcessDelegate(IntPtr paramsBuffer)
	{
		if (!IsBound())
		{
			throw new InvalidOperationException($"calling an unbound delegate is illegal");
		}

		_scriptDelegate->ProcessDelegate(paramsBuffer);
	}

	public override string ToString()
	{
		return _scriptDelegate->ToString();
	}

	public static implicit operator T(Delegate<T> handler)
	{
		return handler.ToManaged();
	}

	public T ToManaged()
	{
		return DelegateMarshaller<T>.FromNative(delegatePtr);
	}

	public void FromManaged(T value)
	{
		DelegateMarshaller<T>.ToNative(delegatePtr, value);
	}
}
