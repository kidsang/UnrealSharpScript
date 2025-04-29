#pragma once
#include "CoreMinimal.h"

/**
* @brief Records whether a corresponding C# object exists for each UObject.
* @note
* The maximum capacity of the array is consistent with `GUObjectArray` and will not be expanded.
* To ensure thread safety, TArray<bool> must be used instead of TBitArray.
* @note
* See `MaxObjectsInGame`, estimated memory usage for PC is 2MB, for IOS and Android is 128KB.
* See `MaxObjectsInEditor`, estimated memory usage for editor is 24MB.
*/
class FSsManagedObjectArray
{
public:
	/** Pre-allocate size for the array, no further expansion afterwards. */
	void Initialize(int32 InMaxElements);

	/** Called when a C# object is created, marking the UObject as having a C# object. */
	inline void AddManagedObject(int32 InObjectIndex)
	{
		check(InObjectIndex < MaxElements);
		ManagedObjects[InObjectIndex] = true;
	}

	/** Called when a UObject is destroyed, clearing the C# object flag for the UObject. */
	inline void FreeManagedObject(int32 InObjectIndex)
	{
		check(InObjectIndex < MaxElements);
		ManagedObjects[InObjectIndex] = false;
	}

	/** Determines if a UObject has a corresponding C# object. */
	inline bool HasManagedObject(int32 InObjectIndex) const
	{
		check(InObjectIndex < MaxElements);
		return ManagedObjects[InObjectIndex];
	}

private:
	// Maximum capacity of the array, consistent with `GUObjectArray->Capacity()`.
	int32 MaxElements = 0;

	// Each bit corresponds to a UObject, true means this object has a C# instance.
	TArray<bool> ManagedObjects;
};
