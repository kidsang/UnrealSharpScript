#pragma once
#include "CoreMinimal.h"
#include "SsTestDelegate.generated.h"

/**
 * Delegate to allow testing of the various script delegate features that are exposed to CSharp wrapped types.
 */
DECLARE_DYNAMIC_DELEGATE_RetVal_OneParam(int32, FSsTestDelegate, int32, InValue);

/**
 * Multicast delegate to allow testing of the various script delegate features that are exposed to CSharp wrapped types.
 */
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FSsTestMulticastDelegate, FString, InStr);

/**
 * Delegate for slate pre/post tick event.
 */
DECLARE_DYNAMIC_DELEGATE_OneParam(FSsTestSlateTickDelegate, float, InDeltaTime);


/** This struct is only used to trigger UBT code generation, and has no practical use. */
USTRUCT()
struct FSsTestDelegateDummy
{
	GENERATED_BODY()
};
