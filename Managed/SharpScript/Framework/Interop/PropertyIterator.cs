using UnrealEngine.Intrinsic;

namespace SharpScript.Interop;

/// <summary>
/// Helper class that can iterate through all properties of a class in order.
/// </summary>
/// <param name="nativeType">UClass, UStruct or UFunction pointer</param>
public ref struct PropertyIterator(IntPtr nativeType)
{
	/// <summary>
	/// The property pointer currently being pointed to.
	/// </summary>
	private IntPtr _currProp = TypeInterop.GetFirstProperty(nativeType);

	/// <summary>
	/// Searches for a property with the corresponding name starting from the current position, returns null if not found.
	/// </summary>
	/// <param name="propName">The property name to search for</param>
	/// <returns>Corresponding property pointer, or null pointer</returns>
	/// <remarks>
	/// If not found by the end of the property list, it will start searching from the beginning until it returns to the starting position.
	/// Calling this function in the order of property definition can find all properties on the property list with linear complexity.
	/// </remarks>
	public IntPtr FindNext(Name propName)
	{
		if (_currProp == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}

		// If searching for properties in the order they are defined, it can be found and returned immediately.
		IntPtr nextProp = GetNextLoopBack(_currProp);
		if (TypeInterop.GetPropertyFName(_currProp) == propName)
		{
			IntPtr currProp = _currProp;
			_currProp = nextProp;
			return currProp;
		}

		// Otherwise, the entire property list needs to be traversed. Worst case will be O(n^2) complexity.
		while (nextProp != _currProp)
		{
			IntPtr currProp = nextProp;
			nextProp = GetNextLoopBack(nextProp);
			if (TypeInterop.GetPropertyFName(currProp) == propName)
			{
				_currProp = nextProp;
				return currProp;
			}
		}

		return IntPtr.Zero;
	}

	/// <summary>
	/// Finds the next property with the corresponding name and returns the property offset.
	/// </summary>
	/// <param name="propName">The property name to search for</param>
	/// <returns>Returns the property offset.</returns>
	/// <seealso cref="FindNext"/>
	public int FindNextAndGetOffset(Name propName)
	{
		IntPtr nativeProp = FindNext(propName);
		return TypeInterop.GetPropertyOffset(nativeProp);
	}

	/// <summary>
	/// Returns the next item in the property list. If it reaches the end of the list, it automatically returns to the beginning.
	/// </summary>
	private IntPtr GetNextLoopBack(IntPtr currProp)
	{
		IntPtr nextProp = TypeInterop.GetNextProperty(currProp);
		if (nextProp == IntPtr.Zero)
		{
			nextProp = TypeInterop.GetFirstProperty(nativeType);
		}

		return nextProp;
	}
}

