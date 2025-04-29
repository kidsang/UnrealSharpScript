using System.Collections;
using SharpScript.Interop;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// Base class for delegate array container, which can access UE delegate memory by reference.
/// </summary>
/// <param name="nativeBuffer">C++ array pointer</param>
/// <param name="arrayProp">FArrayProperty pointer</param>
/// <param name="itemMarshaller">Container internal element wrapper</param>
/// <typeparam name="T">Array element type</typeparam>
public abstract unsafe class DelegateArrayBase<T, U>(IntPtr nativeBuffer, IntPtr arrayProp, DelegateMarshaller<T> itemMarshaller) : IEnumerable<T>
	where U : Delegate<T> where T : Delegate
{
	/// <summary>
	/// C++ array pointer.
	/// </summary>
	protected internal readonly NativeArray* NativeBuffer = (NativeArray*)nativeBuffer;

	/// <summary>
	/// FArrayProperty pointer.
	/// </summary>
	protected readonly IntPtr ArrayProp = arrayProp;

	/// <summary>
	/// Container internal element wrapper.
	/// </summary>
	protected readonly DelegateMarshaller<T> ItemMarshaller = itemMarshaller;

	/// <summary>
	/// Returns number of elements in array.
	/// </summary>
	public int Count => NativeBuffer->Num;

	/// <summary>
	/// Returns maximum number of elements in array.
	/// </summary>
	public int Capacity => NativeBuffer->Max;

	/// <summary>
	/// Return element at given index.
	/// </summary>
	public U Get(int index)
	{
		if (index < 0 || index >= Count)
		{
			throw new IndexOutOfRangeException($"Index {index} out of bounds. Array is size {Count}");
		}

		return (U)ItemMarshaller.MarshallToNativeRef(NativeBuffer->Data, index);
	}

	/// <summary>
	/// Determines the index of a specific item in the Array
	/// </summary>
	public int IndexOf(T item)
	{
		int max = Count;
		for (int index = 0; index < max; ++index)
		{
			U elem = Get(index);
			if (elem.ToManaged().Equals(item))
			{
				return index;
			}
		}

		return -1;
	}

	/// <summary>
	/// Checks if this array contains the element.
	/// </summary>
	/// <returns>True if found. False otherwise.</returns>
	public bool Contains(T item)
	{
		return IndexOf(item) >= 0;
	}

	public bool SequenceEqual(IEnumerable<T> other)
	{
		if (other is DelegateArrayBase<T, U> otherArray)
		{
			if (NativeBuffer == otherArray.NativeBuffer)
			{
				return true;
			}

			return TypeInterop.PropertyIdentical(ArrayProp, (IntPtr)NativeBuffer, (IntPtr)otherArray.NativeBuffer);
		}

		return other.SequenceEqual(this);
	}

	/// <summary>
	/// Convert TArray to C# list.
	/// </summary>
	public List<T> ToList()
	{
		return [..this];
	}

	/// <summary>
	/// Implicit converter from <see cref="DelegateArrayBase{T,U}"/> to <see cref="List{T}"/>
	/// </summary>
	public static implicit operator List<T>(DelegateArrayBase<T, U> array)
	{
		return array.ToList();
	}

	/// <summary>
	/// Array enumerator.
	/// </summary>
	public struct Enumerator(DelegateArrayBase<T, U> array) : IEnumerator<T>
	{
		private int _index = -1;

		public bool MoveNext()
		{
			++_index;
			return _index < array.Count;
		}

		public void Reset()
		{
			_index = -1;
		}

		public T Current => array.Get(_index).ToManaged();

		object IEnumerator.Current => Current;

		public void Dispose()
		{
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

/// <summary>
/// Read-only wrapper for TArray.
/// </summary>
/// <inheritdoc cref="DelegateArrayBase{T,U}"/>
public class DelegateArrayReadOnly<T, U>(IntPtr nativeBuffer, IntPtr arrayProp, DelegateMarshaller<T> itemMarshaller)
	: DelegateArrayBase<T, U>(nativeBuffer, arrayProp, itemMarshaller)
	where U : Delegate<T> where T : Delegate
{
	public U this[int index] => Get(index);
}

/// <summary>
/// Wrapper for TArray.
/// </summary>
/// <inheritdoc cref="DelegateArrayBase{T,U}"/>
public unsafe class DelegateArray<T, U>(IntPtr nativeBuffer, IntPtr arrayProp, DelegateMarshaller<T> itemMarshaller)
	: DelegateArrayBase<T, U>(nativeBuffer, arrayProp, itemMarshaller), ICollection<T>
	where U : Delegate<T> where T : Delegate
{
	public bool IsReadOnly => false;

	public void Add(T item)
	{
		int newIndex = ArrayInterop.AddValue(ArrayProp, NativeBuffer);
		Set(newIndex, item);
	}

	public void Clear()
	{
		ArrayInterop.EmptyValues(ArrayProp, NativeBuffer);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		int max = Count;
		for (int i = 0; i < max; ++i)
		{
			array[arrayIndex + i] = Get(i).ToManaged();
		}
	}

	public bool Remove(T item)
	{
		int index = IndexOf(item);
		if (index != -1)
		{
			ArrayInterop.RemoveValue(ArrayProp, NativeBuffer, index);
		}

		return index != -1;
	}

	public void Insert(int index, T item)
	{
		ArrayInterop.InsertValue(ArrayProp, NativeBuffer, index);
		Get(index).FromManaged(item);
	}

	public void RemoveAt(int index)
	{
		ArrayInterop.RemoveValue(ArrayProp, NativeBuffer, index);
	}

	/// <summary>
	/// Set element at given index.
	/// </summary>
	public void Set(int index, T item)
	{
		Get(index).FromManaged(item);
	}

	public U this[int index] => Get(index);

	/// <summary>
	/// Fill in TArray from C# list.
	/// </summary>
	/// <param name="other">C# list</param>
	public void CopyFrom(IEnumerable<T>? other)
	{
		if (other == null)
		{
			ArrayInterop.EmptyValues(ArrayProp, NativeBuffer);
			return;
		}

		if (other is DelegateArrayBase<T, U> otherArray)
		{
			if (!SequenceEqual(otherArray))
			{
				TypeInterop.CopyPropertyValue(ArrayProp, (IntPtr)NativeBuffer, (IntPtr)otherArray.NativeBuffer);
			}
		}
		else if (other is List<T> otherList)
		{
			int max = otherList.Count;
			ArrayInterop.EmptyAndAddValues(ArrayProp, NativeBuffer, max);
			for (int i = 0; i < max; ++i)
			{
				Set(i, otherList[i]);
			}
		}
		else
		{
			ArrayInterop.EmptyValues(ArrayProp, NativeBuffer);
			foreach (T item in other)
			{
				Add(item);
			}
		}
	}
}
