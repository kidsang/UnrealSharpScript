#include "SsManagedObjectArray.h"

void FSsManagedObjectArray::Initialize(int32 InMaxElements)
{
	MaxElements = InMaxElements;
	ManagedObjects.SetNumZeroed(InMaxElements);
}
