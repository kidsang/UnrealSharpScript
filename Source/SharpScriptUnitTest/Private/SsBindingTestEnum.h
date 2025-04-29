#pragma once
#include "SsBindingTestEnum.generated.h"

UENUM(BlueprintType)
enum ESsBindingTestEnum : uint8
{
	One,
	Two,
	Three,
	Four,
};

UENUM(Flags)
enum class ESsBindingTestLongEnum : uint32
{
	One = 0,
	Two = 1,
	Three = 2,
	Four = 4,
};
