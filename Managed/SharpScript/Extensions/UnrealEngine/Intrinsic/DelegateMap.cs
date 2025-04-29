using System.Collections;
using SharpScript.Interop;

namespace UnrealEngine.Intrinsic;

/// <summary>
/// Base class for delegate dictionary containers, allowing access to UE delegate memory by reference.
/// </summary>
/// <param name="mapPtr">C++ dictionary pointer</param>
/// <param name="mapProp">FMapProperty pointer</param>
/// <param name="keyMarshaller">Dictionary key marshaller</param>
/// <param name="valueMarshaller">Dictionary value marshaller</param>
/// <typeparam name="TKey">Dictionary key type</typeparam>
/// <typeparam name="TValue">Dictionary value type</typeparam>
/// <typeparam name="UValue">Dictionary value reference type</typeparam>
public abstract unsafe class DelegateMapBase<TKey, TValue, UValue>(IntPtr mapPtr, IntPtr mapProp, IMarshaller<TKey> keyMarshaller, DelegateMarshaller<TValue> valueMarshaller)
	: IEnumerable<KeyValuePair<TKey, TValue>>
	where TKey : notnull where UValue : Delegate<TValue> where TValue : Delegate
{
	/// <summary>
	/// C++ dictionary pointer.
	/// </summary>
	protected internal readonly IntPtr MapPtr = mapPtr;

	/// <summary>
	/// FMapProperty pointer.
	/// </summary>
	protected readonly IntPtr MapProp = mapProp;

	/// <summary>
	/// FMapProperty->KeyProp
	/// </summary>
	protected readonly IntPtr KeyProp = TypeInterop.GetMapKeyProperty(mapProp);

	/// <summary>
	/// FMapProperty->ValueProp
	/// </summary>
	protected readonly IntPtr ValueProp = TypeInterop.GetMapValueProperty(mapProp);

	/// <summary>
	/// Dictionary key marshaller.
	/// </summary>
	protected readonly IMarshaller<TKey> KeyMarshaller = keyMarshaller;

	/// <summary>
	/// Dictionary value marshaller.
	/// </summary>
	protected readonly DelegateMarshaller<TValue> ValueMarshaller = valueMarshaller;

	/// <summary>
	/// Returns number of elements in the map.
	/// </summary>
	public int Count => MapInterop.GetNum(MapProp, MapPtr);

	/// <summary>
	/// Returns max valid index of the elements in the sparse storage.
	/// </summary>
	protected int GetMaxIndex()
	{
		return MapInterop.GetMaxIndex(MapProp, MapPtr);
	}

	/// <summary>
	/// Return if index is valid.
	/// </summary>
	protected bool IsValidIndex(int index)
	{
		return MapInterop.IsValidIndex(MapProp, MapPtr, index) != 0;
	}

	/// <summary>
	/// Returns pointers to the pair in the map
	/// </summary>
	protected bool GetPairPtr(int index, out IntPtr keyPtr, out IntPtr valuePtr)
	{
		return MapInterop.GetPairPtr(MapProp, MapPtr, index, out keyPtr, out valuePtr) != 0;
	}

	protected KeyValuePair<TKey, UValue> GetAt(int index)
	{
		if (!IsValidIndex(index))
		{
			throw new IndexOutOfRangeException($"Index {index} is invalid.");
		}

		if (!GetPairPtr(index, out var keyPtr, out var valuePtr))
		{
			throw new InvalidOperationException($"Failed to get pair at index {index}.");
		}

		return new KeyValuePair<TKey, UValue>(KeyMarshaller.MarshallFromNative(keyPtr), (UValue)ValueMarshaller.MarshallToNativeRef(valuePtr));
	}

	protected TKey GetKeyAt(int index)
	{
		if (!IsValidIndex(index))
		{
			throw new IndexOutOfRangeException($"Index {index} is invalid.");
		}

		if (!GetPairPtr(index, out var keyPtr, out _))
		{
			throw new InvalidOperationException($"Failed to get pair at index {index}.");
		}

		return KeyMarshaller.MarshallFromNative(keyPtr);
	}

	protected UValue GetValueAt(int index)
	{
		if (!IsValidIndex(index))
		{
			throw new IndexOutOfRangeException($"Index {index} is invalid.");
		}

		if (!GetPairPtr(index, out _, out var valuePtr))
		{
			throw new InvalidOperationException($"Failed to get pair at index {index}.");
		}

		return (UValue)ValueMarshaller.MarshallToNativeRef(valuePtr);
	}

	protected int IndexOf(TKey value)
	{
		int keySize = TypeInterop.GetPropertySize(KeyProp);
		byte* keyBuffer = stackalloc byte[keySize];
		IntPtr keyPtr = new IntPtr(keyBuffer);

		TypeInterop.InitializePropertyValue(KeyProp, keyPtr);
		KeyMarshaller.MarshallToNative(keyPtr, value);

		int index = MapInterop.FindMapIndexWithKey(MapProp, MapPtr, keyPtr);

		TypeInterop.DestroyPropertyValue(KeyProp, keyPtr);
		return index;
	}

	/// <summary>
	///  Get the value associated with a specified key
	/// </summary>
	public UValue? Get(TKey key)
	{
		int index = IndexOf(key);
		return index >= 0 ? GetValueAt(index) : null;
	}

	/// <summary>
	/// Check if the map contains the key.
	/// </summary>
	public bool ContainsKey(TKey key)
	{
		return IndexOf(key) >= 0;
	}

	/// <summary>
	/// Check if the map contains the value.
	/// </summary>
	public bool ContainsValue(TValue value)
	{
		EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;
		int maxIndex = GetMaxIndex();
		for (int i = 0; i < maxIndex; ++i)
		{
			if (IsValidIndex(i) && comparer.Equals(GetValueAt(i).ToManaged(), value))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Convert TMap to C# Dictionary.
	/// </summary>
	public Dictionary<TKey, TValue> ToDictionary()
	{
		return new(this);
	}

	/// <summary>
	/// Implicit converter from <see cref="DelegateMapBase{TKey,TValue,UValue}"/> to <see cref="Dictionary{TKey,TValue}"/>
	/// </summary>
	public static implicit operator Dictionary<TKey, TValue>(DelegateMapBase<TKey, TValue, UValue> map)
	{
		return map.ToDictionary();
	}

	public struct Enumerator(DelegateMapBase<TKey, TValue, UValue> map) : IEnumerator<KeyValuePair<TKey, TValue>>
	{
		private int _index = -1;

		public bool MoveNext()
		{
			int maxIndex = map.GetMaxIndex();
			while (++_index < maxIndex && !map.IsValidIndex(_index))
			{
			}

			return _index < maxIndex;
		}

		public void Reset()
		{
			_index = -1;
		}

		public KeyValuePair<TKey, TValue> Current
		{
			get
			{
				var pair = map.GetAt(_index);
				return new(pair.Key, pair.Value.ToManaged());
			}
		}

		object IEnumerator.Current => Current;

		public void Dispose()
		{
		}
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public readonly struct KeyCollection(DelegateMapBase<TKey, TValue, UValue> map) : ICollection<TKey>
	{
		public struct KeyEnumerator(DelegateMapBase<TKey, TValue, UValue> map) : IEnumerator<TKey>
		{
			private int _index = -1;

			public bool MoveNext()
			{
				int maxIndex = map.GetMaxIndex();
				while (++_index < maxIndex && !map.IsValidIndex(_index))
				{
				}

				return _index < maxIndex;
			}

			public void Reset()
			{
				_index = -1;
			}

			public TKey Current => map.GetKeyAt(_index);

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}
		}

		public IEnumerator<TKey> GetEnumerator()
		{
			return new KeyEnumerator(map);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(TKey item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(TKey item)
		{
			return map.ContainsKey(item);
		}

		public void CopyTo(TKey[] array, int arrayIndex)
		{
			int maxIndex = map.GetMaxIndex();
			int index = arrayIndex;
			for (int i = 0; i < maxIndex; ++i)
			{
				if (map.IsValidIndex(i))
				{
					array[index++] = map.GetKeyAt(i);
				}
			}
		}

		public bool Remove(TKey item)
		{
			throw new NotSupportedException();
		}

		public int Count => map.Count;

		public bool IsReadOnly => true;
	}

	public readonly struct ValueCollection(DelegateMapBase<TKey, TValue, UValue> map) : ICollection<TValue>
	{
		public struct ValueEnumerator(DelegateMapBase<TKey, TValue, UValue> map) : IEnumerator<TValue>
		{
			private int _index = -1;

			public bool MoveNext()
			{
				int maxIndex = map.GetMaxIndex();
				while (++_index < maxIndex && !map.IsValidIndex(_index))
				{
				}

				return _index < maxIndex;
			}

			public void Reset()
			{
				_index = -1;
			}

			public TValue Current => map.GetValueAt(_index).ToManaged();

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return new ValueEnumerator(map);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(TValue item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(TValue item)
		{
			return map.ContainsValue(item);
		}

		public void CopyTo(TValue[] array, int arrayIndex)
		{
			int maxIndex = map.GetMaxIndex();
			int index = arrayIndex;
			for (int i = 0; i < maxIndex; ++i)
			{
				if (map.IsValidIndex(i))
				{
					array[index++] = map.GetValueAt(i).ToManaged();
				}
			}
		}

		public bool Remove(TValue item)
		{
			throw new NotSupportedException();
		}

		public int Count => map.Count;

		public bool IsReadOnly => true;
	}
}

/// <summary>
/// Read-only wrapper for TMap.
/// </summary>
/// <inheritdoc cref="DelegateMapBase{TKey,TValue,UValue}"/>
public class DelegateMapReadOnly<TKey, TValue, UValue>(IntPtr mapPtr, IntPtr mapProp, IMarshaller<TKey> keyMarshaller, DelegateMarshaller<TValue> valueMarshaller)
	: DelegateMapBase<TKey, TValue, UValue>(mapPtr, mapProp, keyMarshaller, valueMarshaller)
	where TKey : notnull where UValue : Delegate<TValue> where TValue : Delegate
{
	public bool TryGetValue(TKey key, out UValue value)
	{
		int index = IndexOf(key);
		if (index >= 0)
		{
			value = GetAt(index).Value;
			return true;
		}

		value = null!;
		return false;
	}

	public UValue this[TKey key] => Get(key)!;

	public KeyCollection Keys => new(this);

	public ValueCollection Values => new(this);
}

/// <summary>
/// Wrapper for TMap.
/// </summary>
/// <inheritdoc cref="DelegateMapBase{TKey,TValue,UValue}"/>
public unsafe class DelegateMap<TKey, TValue, UValue>(IntPtr mapPtr, IntPtr mapProp, IMarshaller<TKey> keyMarshaller, DelegateMarshaller<TValue> valueMarshaller)
	: DelegateMapBase<TKey, TValue, UValue>(mapPtr, mapProp, keyMarshaller, valueMarshaller)
	where TKey : notnull where UValue : Delegate<TValue> where TValue : Delegate
{
	public void Add(KeyValuePair<TKey, TValue> item)
	{
		Add(item.Key, item.Value);
	}

	public void Clear()
	{
		MapInterop.EmptyValues(MapProp, MapPtr, 0);
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		return ContainsKey(item.Key);
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		int maxIndex = GetMaxIndex();
		int index = arrayIndex;
		for (int i = 0; i < maxIndex; ++i)
		{
			if (IsValidIndex(i))
			{
				var pair = GetAt(i);
				array[index++] = new(pair.Key, pair.Value.ToManaged());
			}
		}
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		return Remove(item.Key);
	}

	public bool IsReadOnly => false;

	public void Add(TKey key, TValue value)
	{
		int keySize = TypeInterop.GetPropertySize(KeyProp);
		int valueSize = TypeInterop.GetPropertySize(ValueProp);
		byte* keyBuffer = stackalloc byte[keySize];
		byte* valueBuffer = stackalloc byte[valueSize];
		IntPtr keyPtr = new IntPtr(keyBuffer);
		IntPtr valuePtr = new IntPtr(valueBuffer);

		TypeInterop.InitializePropertyValue(KeyProp, keyPtr);
		TypeInterop.InitializePropertyValue(ValueProp, valuePtr);

		KeyMarshaller.MarshallToNative(keyPtr, key);
		ValueMarshaller.MarshallToNative(valuePtr, value);

		MapInterop.AddPair(MapProp, MapPtr, keyPtr, valuePtr);

		TypeInterop.DestroyPropertyValue(KeyProp, keyPtr);
		TypeInterop.DestroyPropertyValue(ValueProp, valuePtr);
	}

	public bool Remove(TKey key)
	{
		int index = IndexOf(key);
		if (index < 0)
		{
			return false;
		}

		MapInterop.RemoveAt(MapProp, MapPtr, index);
		return true;
	}

	public bool TryGetValue(TKey key, out UValue? value)
	{
		int index = IndexOf(key);
		if (index >= 0)
		{
			value = GetAt(index).Value;
			return true;
		}

		value = null;
		return false;
	}

	public UValue this[TKey key] => Get(key)!;

	public KeyCollection Keys => new(this);

	public ValueCollection Values => new(this);

	/// <summary>
	/// Fill in TMap from C# dictionary.
	/// </summary>
	/// <param name="other">C# dictionary</param>
	public void CopyFrom(IEnumerable<KeyValuePair<TKey, TValue>>? other)
	{
		if (other == null)
		{
			MapInterop.EmptyValues(MapProp, MapPtr, 0);
			return;
		}

		if (other is DelegateMapBase<TKey, TValue, UValue> otherMap)
		{
			if (MapPtr != otherMap.MapPtr)
			{
				TypeInterop.CopyPropertyValue(MapProp, MapPtr, otherMap.MapPtr);
			}
		}
		else
		{
			int slack = 0;
			if (other is Dictionary<TKey, TValue> otherDict)
			{
				slack = otherDict.Count;
			}

			MapInterop.EmptyValues(MapProp, MapPtr, slack);
			foreach (var pair in other)
			{
				MapInterop.AddDefaultValueAndGetPair(MapProp, MapPtr, out var keyPtr, out var valuePtr);
				KeyMarshaller.MarshallToNative(keyPtr, pair.Key);
				ValueMarshaller.MarshallToNative(valuePtr, pair.Value);
			}

			MapInterop.Rehash(MapProp, MapPtr);
		}
	}
}
