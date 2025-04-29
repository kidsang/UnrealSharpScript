using System.Collections;
using SharpScript.Interop;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// Base class for set containers.
/// </summary>
/// <param name="setPtr">C++ set pointer.</param>
/// <param name="setProp">FSetProperty pointer.</param>
/// <param name="elemMarshaller">Container element marshaller.</param>
/// <typeparam name="T">Set element type</typeparam>
public abstract unsafe class SetBase<T>(IntPtr setPtr, IntPtr setProp, IMarshaller<T> elemMarshaller) : IEnumerable<T>
{
	/// <summary>
	/// C++ set pointer.
	/// </summary>
	protected internal readonly IntPtr SetPtr = setPtr;

	/// <summary>
	/// FSetProperty pointer.
	/// </summary>
	protected readonly IntPtr SetProp = setProp;

	/// <summary>
	/// FSetProperty->ElementProp.
	/// </summary>
	protected readonly IntPtr ElemProp = TypeInterop.GetSetElementProperty(setProp);

	/// <summary>
	/// Container element marshaller.
	/// </summary>
	protected readonly IMarshaller<T> ElemMarshaller = elemMarshaller;

	/// <summary>
	/// Returns number of elements in the set.
	/// </summary>
	public int Count => SetInterop.GetNum(SetProp, SetPtr);

	/// <summary>
	/// Returns max valid index of the elements in the sparse storage.
	/// </summary>
	protected int GetMaxIndex()
	{
		return SetInterop.GetMaxIndex(SetProp, SetPtr);
	}

	/// <summary>
	/// Return if index is valid.
	/// </summary>
	protected bool IsValidIndex(int index)
	{
		return SetInterop.IsValidIndex(SetProp, SetPtr, index) != 0;
	}

	protected T GetAt(int index)
	{
		if (!IsValidIndex(index))
		{
			throw new IndexOutOfRangeException($"Index {index} is invalid.");
		}

		IntPtr elemPtr = SetInterop.GetElementPtr(SetProp, SetPtr, index);
		return ElemMarshaller.MarshallFromNative(elemPtr);
	}

	protected int IndexOf(T item)
	{
		int elemSize = TypeInterop.GetPropertySize(ElemProp);
		byte* elemBuffer = stackalloc byte[elemSize];
		IntPtr elemPtr = new IntPtr(elemBuffer);

		TypeInterop.InitializePropertyValue(ElemProp, elemPtr);
		ElemMarshaller.MarshallToNative(elemPtr, item);

		int index = SetInterop.FindElementIndexFromHash(SetProp, SetPtr, elemPtr);

		TypeInterop.DestroyPropertyValue(ElemProp, elemPtr);
		return index;
	}

	/// <summary>
	/// Checks if the et contains an element with the given key.
	/// </summary>
	public bool Contains(T item)
	{
		return IndexOf(item) >= 0;
	}

	protected HashSet<T> ToHashSet(IEnumerable<T> other)
	{
		if (other is HashSet<T> otherSet)
		{
			return otherSet;
		}

		return [..other];
	}

	protected bool IsProperSubsetOfInternal(IEnumerable<T> other)
	{
		HashSet<T> otherSet = ToHashSet(other);
		if (Count == 0)
		{
			return otherSet.Count > 0;
		}

		if (Count >= otherSet.Count)
		{
			return false;
		}

		foreach (T item in this)
		{
			if (!otherSet.Contains(item))
			{
				return false;
			}
		}

		return true;
	}

	protected bool IsProperSupersetOfInternal(IEnumerable<T> other)
	{
		if (Count == 0)
		{
			return false;
		}

		int otherCount = 0;
		foreach (T item in other)
		{
			otherCount += 1;
			if (!Contains(item))
			{
				return false;
			}
		}

		return Count > otherCount;
	}

	protected bool IsSubsetOfInternal(IEnumerable<T> other)
	{
		if (Count == 0)
		{
			return true;
		}

		HashSet<T> otherSet = ToHashSet(other);
		if (Count > otherSet.Count)
		{
			return false;
		}

		foreach (T item in this)
		{
			if (!otherSet.Contains(item))
			{
				return false;
			}
		}

		return true;
	}

	protected bool IsSupersetOfInternal(IEnumerable<T> other)
	{
		foreach (T item in other)
		{
			if (!Contains(item))
			{
				return false;
			}
		}

		return true;
	}

	protected bool OverlapsInternal(IEnumerable<T> other)
	{
		if (Count == 0)
		{
			return false;
		}

		foreach (T item in other)
		{
			if (Contains(item))
			{
				return true;
			}
		}

		return false;
	}

	protected bool SetEqualsInternal(IEnumerable<T> other)
	{
		if (other is SetBase<T> otherSet)
		{
			if (SetPtr == otherSet.SetPtr)
			{
				return true;
			}

			return TypeInterop.PropertyIdentical(SetProp, SetPtr, otherSet.SetPtr);
		}

		int otherCount = 0;
		foreach (T item in other)
		{
			otherCount += 1;
			if (!Contains(item))
			{
				return false;
			}
		}

		return Count == otherCount;
	}

	/// <summary>
	/// Convert TSet to C# HashSet.
	/// </summary>
	public HashSet<T> ToHashSet()
	{
		return [..this];
	}

	/// <summary>
	/// Implicit converter from <see cref="SetBase{T}"/> to <see cref="HashSet{T}"/>
	/// </summary>
	public static implicit operator HashSet<T>(SetBase<T> set)
	{
		return set.ToHashSet();
	}

	public struct Enumerator(SetBase<T> set) : IEnumerator<T>
	{
		private int _index = -1;

		public bool MoveNext()
		{
			int maxIndex = set.GetMaxIndex();
			while (++_index < maxIndex && !set.IsValidIndex(_index))
			{
			}

			return _index < maxIndex;
		}

		public void Reset()
		{
			_index = -1;
		}

		public T Current => set.GetAt(_index);

		object? IEnumerator.Current => Current;

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
/// Read-only wrapper for TSet.
/// </summary>
/// <inheritdoc cref="SetBase{T}"/>
public class SetReadOnly<T>(IntPtr setPtr, IntPtr setProp, IMarshaller<T> elemMarshaller) : SetBase<T>(setPtr, setProp, elemMarshaller), IReadOnlySet<T>
{
	public bool IsProperSubsetOf(IEnumerable<T> other)
	{
		return IsProperSubsetOfInternal(other);
	}

	public bool IsProperSupersetOf(IEnumerable<T> other)
	{
		return IsProperSupersetOfInternal(other);
	}

	public bool IsSubsetOf(IEnumerable<T> other)
	{
		return IsSubsetOfInternal(other);
	}

	public bool IsSupersetOf(IEnumerable<T> other)
	{
		return IsSupersetOfInternal(other);
	}

	public bool Overlaps(IEnumerable<T> other)
	{
		return OverlapsInternal(other);
	}

	public bool SetEquals(IEnumerable<T> other)
	{
		return SetEqualsInternal(other);
	}
}

/// <summary>
/// Wrapper for TSet.
/// </summary>
/// <inheritdoc cref="SetBase{T}"/>
public unsafe class Set<T>(IntPtr setPtr, IntPtr setProp, IMarshaller<T> elemMarshaller) : SetBase<T>(setPtr, setProp, elemMarshaller), ISet<T>
{
	public bool IsReadOnly => false;

	public bool Add(T item)
	{
		int elemSize = TypeInterop.GetPropertySize(ElemProp);
		byte* elemBuffer = stackalloc byte[elemSize];
		IntPtr elemPtr = new IntPtr(elemBuffer);

		TypeInterop.InitializePropertyValue(ElemProp, elemPtr);
		ElemMarshaller.MarshallToNative(elemPtr, item);

		int index = SetInterop.FindElementIndexFromHash(SetProp, SetPtr, elemPtr);
		bool added = false;
		if (index < 0)
		{
			SetInterop.AddElement(SetProp, SetPtr, elemPtr);
			added = true;
		}

		TypeInterop.DestroyPropertyValue(ElemProp, elemPtr);
		return added;
	}

	void ICollection<T>.Add(T item)
	{
		Add(item);
	}

	public void ExceptWith(IEnumerable<T> other)
	{
		if (Count == 0)
		{
			return;
		}

		foreach (var item in other)
		{
			Remove(item);
		}
	}

	public void IntersectWith(IEnumerable<T> other)
	{
		HashSet<T> otherSet = ToHashSet(other);
		if (otherSet.Count == 0)
		{
			Clear();
			return;
		}

		int maxIndex = GetMaxIndex();
		for (int i = maxIndex - 1; i >= 0; --i)
		{
			if (!IsValidIndex(i))
			{
				continue;
			}

			T item = GetAt(i);
			if (!otherSet.Contains(item))
			{
				SetInterop.RemoveAt(SetProp, SetPtr, i);
			}
		}
	}

	public bool IsProperSubsetOf(IEnumerable<T> other)
	{
		return IsProperSubsetOfInternal(other);
	}

	public bool IsProperSupersetOf(IEnumerable<T> other)
	{
		return IsProperSupersetOfInternal(other);
	}

	public bool IsSubsetOf(IEnumerable<T> other)
	{
		return IsSubsetOfInternal(other);
	}

	public bool IsSupersetOf(IEnumerable<T> other)
	{
		return IsSupersetOfInternal(other);
	}

	public bool Overlaps(IEnumerable<T> other)
	{
		return OverlapsInternal(other);
	}

	public bool SetEquals(IEnumerable<T> other)
	{
		return SetEqualsInternal(other);
	}

	public void SymmetricExceptWith(IEnumerable<T> other)
	{
		if (Count == 0)
		{
			UnionWith(other);
			return;
		}

		foreach (T item in other)
		{
			if (!Remove(item))
			{
				Add(item);
			}
		}
	}

	public void UnionWith(IEnumerable<T> other)
	{
		foreach (T item in other)
		{
			if (!Contains(item))
			{
				Add(item);
			}
		}
	}

	public void Clear()
	{
		SetInterop.EmptyElements(SetProp, SetPtr, 0);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		int maxIndex = GetMaxIndex();
		int index = arrayIndex;
		for (int i = 0; i < maxIndex; ++i)
		{
			if (IsValidIndex(i))
			{
				array[index++] = GetAt(i);
			}
		}
	}

	public bool Remove(T item)
	{
		int index = IndexOf(item);
		if (index >= 0)
		{
			SetInterop.RemoveAt(SetProp, SetPtr, index);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Fill in TSet from C# HashSet.
	/// </summary>
	/// <param name="other">C# HashSet</param>
	public void CopyFrom(IEnumerable<T>? other)
	{
		if (other == null)
		{
			SetInterop.EmptyElements(SetProp, SetPtr, 0);
			return;
		}

		if (other is SetBase<T> otherSet)
		{
			if (SetPtr != otherSet.SetPtr)
			{
				TypeInterop.CopyPropertyValue(SetProp, SetPtr, otherSet.SetPtr);
			}
		}
		else
		{
			int slack = 0;
			if (other is HashSet<T> otherHashSet)
			{
				slack = otherHashSet.Count;
			}

			SetInterop.EmptyElements(SetProp, SetPtr, slack);
			foreach (var item in other)
			{
				IntPtr elemPtr = SetInterop.AddDefaultValueAndGetPtr(SetProp, SetPtr);
				ElemMarshaller.MarshallToNative(elemPtr, item);
			}

			SetInterop.Rehash(SetProp, SetPtr);
		}
	}
}
