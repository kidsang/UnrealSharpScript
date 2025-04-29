#pragma once
#include "SsTestEnum.generated.h"

UENUM(BlueprintType)
enum ESsTestEnum : uint8
{
	One,
	Two,
	Three,
	Four,
};

UENUM(Flags)
enum class ESsTestLongEnum : uint32
{
	One = 0,
	Two = 1,
	Three = 2,
	Four = 4,
};
