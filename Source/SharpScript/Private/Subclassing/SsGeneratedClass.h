#pragma once
#include "CoreMinimal.h"
#include "UObject/Package.h"
#include "Engine/BlueprintGeneratedClass.h"
#include "SsGeneratedClass.generated.h"

struct FSsPropertyDef;

UCLASS()
class USsGeneratedClass : public UBlueprintGeneratedClass
{
	GENERATED_BODY()

public:
	/**
	 * Generate a new unreal class from given infos.
	 * @param ClassName Name of the new class.
	 * @param SuperClass Base class of the new class.
	 * @param PropertyDefines Array of property defines.
	 * @param PropertyCount Count of property array.
	 * @return Newly generated class if success, otherwise nullptr.
	 */
	static USsGeneratedClass* GenerateClass(const FName& ClassName, UClass* SuperClass, const FSsPropertyDef* PropertyDefines, int PropertyCount);

private:
	/** UObject constructor. */
	static void StaticObjectConstructor(const FObjectInitializer& Initializer);

	/** Traverse the inheritance chain and return the first generated class. */
	static const USsGeneratedClass* GetFirstGeneratedClass(const UClass* InClass);

private:
	/** The most derived native super class in the inheritance chain. */
	UClass* NativeSuperClass = nullptr;

	friend class FSsGeneratedClassBuilder;
};
