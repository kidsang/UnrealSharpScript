namespace SharpScript;

/// <summary>
/// Interface for UE struct reference.
/// </summary>
public interface IStructNativeRef<T> where T : struct
{
	/// <summary>
	/// Convert UE struct reference to C# struct.
	/// </summary>
	public T ToManaged();

	/// <summary>
	/// Fill UE struct with C# struct value.
	/// </summary>
	public void FromManaged(in T value);

	/// <summary>
	/// Create UE struct reference.
	/// </summary>
	/// <param name="valuePtr"></param>
	public static abstract IStructNativeRef<T> CreateInstance(IntPtr valuePtr);

	/// <summary>
	/// Return memory size of the native struct.
	/// </summary>
	public static abstract int GetNativeDataSize();
}

/// <summary>
/// Various interfaces to assist struct marshalling.
/// </summary>
public interface IStructMarshallerHelper<T> where T : struct
{
	public static abstract int GetNativeDataSize();

	public static abstract IStructNativeRef<T> CreateStructNativeRef(IntPtr valuePtr);
}

