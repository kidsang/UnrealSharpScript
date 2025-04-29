#pragma once
#include "CoreMinimal.h"
#include "SsBindingTestDelegate.generated.h"

/**
 * Delegate to allow testing of the various script delegate features that are exposed to CSharp wrapped types.
 */
DECLARE_DYNAMIC_DELEGATE_RetVal_OneParam(int32, FSsBindingTestDelegate, int32, InValue);

/**
 * Multicast delegate to allow testing of the various script delegate features that are exposed to CSharp wrapped types.
 */
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FSsBindingTestMulticastDelegate, FString, InStr);

/**
 * Delegate for slate pre/post tick event.
 */
DECLARE_DYNAMIC_DELEGATE_OneParam(FSsBindingTestSlateTickDelegate, float, InDeltaTime);


/** 此结构体仅用于触发UBT生成代码，无实际用途。 */
USTRUCT()
struct FSsBindingTestDelegateDummy
{
	GENERATED_BODY()
};
