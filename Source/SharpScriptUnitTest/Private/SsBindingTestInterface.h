#pragma once
#include "CoreMinimal.h"
#include "UObject/Interface.h"
#include "SsBindingTestInterface.generated.h"

/**
 * Interface to allow testing of the various UInterface features that are exposed to CSharp wrapped types.
 */
UINTERFACE(BlueprintType, meta=(CannotImplementInterfaceInBlueprint))
class USsBindingTestInterface : public UInterface
{
	GENERATED_BODY()
};

class ISsBindingTestInterface
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
class USsBindingTestChildInterface : public USsBindingTestInterface
{
	GENERATED_BODY()
};

class ISsBindingTestChildInterface : public ISsBindingTestInterface
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
class USsBindingTestOtherInterface : public UInterface
{
	GENERATED_BODY()
};

class ISsBindingTestOtherInterface
{
	GENERATED_IINTERFACE_BODY()

public:
	UFUNCTION(BlueprintCallable, Category = "CSharp|Internal")
	virtual int32 FuncInterfaceOther(const int32 InValue) const = 0;
};
