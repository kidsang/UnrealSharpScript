#pragma once
#include "CoreMinimal.h"
#include "UObject/Interface.h"
#include "SsTestInterface.generated.h"

/**
 * Interface to allow testing of the various UInterface features that are exposed to CSharp wrapped types.
 */
UINTERFACE(BlueprintType, meta=(CannotImplementInterfaceInBlueprint))
class USsTestInterface : public UInterface
{
	GENERATED_BODY()
};

class ISsTestInterface
{
	GENERATED_IINTERFACE_BODY()

public:
	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	virtual int32 FuncInterface(const int32 InValue) const = 0;
};

/**
 * Interface to allow testing of inheritance on CSharp wrapped types.
 */
UINTERFACE(BlueprintType, meta=(CannotImplementInterfaceInBlueprint))
class USsTestChildInterface : public USsTestInterface
{
	GENERATED_BODY()
};

class ISsTestChildInterface : public ISsTestInterface
{
	GENERATED_IINTERFACE_BODY()

public:
	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	virtual int32 FuncInterfaceChild(const int32 InValue) const = 0;
};

/**
 * Interface to allow testing of multiple-inheritance on CSharp wrapped types.
 */
UINTERFACE(BlueprintType, meta=(CannotImplementInterfaceInBlueprint))
class USsTestOtherInterface : public UInterface
{
	GENERATED_BODY()
};

class ISsTestOtherInterface
{
	GENERATED_IINTERFACE_BODY()

public:
	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	virtual int32 FuncInterfaceOther(const int32 InValue) const = 0;
};
