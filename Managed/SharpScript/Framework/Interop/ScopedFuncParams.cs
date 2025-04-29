namespace SharpScript.Interop;

/// <summary>
/// Automatically initializes C++ memory for function parameters during construction and automatically destroys C++ memory during Dispose.
/// Works best when used with 'using' statement.
/// </summary>
public unsafe struct ScopedFuncParams() : IDisposable
{
	private readonly IntPtr _nativeFunc;

	private IntPtr _buffer;

	public IntPtr Buffer => _buffer;

	public ScopedFuncParams(IntPtr nativeFunc, byte* buffer) : this()
	{
		_nativeFunc = nativeFunc;
		_buffer = (IntPtr)buffer;
		TypeInterop.InitializeStruct(nativeFunc, _buffer);
	}

	public void Dispose()
	{
		if (_buffer != IntPtr.Zero)
		{
			TypeInterop.UninitializeStruct(_nativeFunc, _buffer);
			_buffer = IntPtr.Zero;
		}
	}
}
