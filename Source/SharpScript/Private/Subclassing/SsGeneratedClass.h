#pragma once
#include "CoreMinimal.h"
#include "UObject/Package.h"
#include "Engine/BlueprintGeneratedClass.h"
#include "SsGeneratedClass.generated.h"

struct FSsPropertyDef;
struct FSsFunctionDef;

/**
 * The managed dispatch function that is invoked from a generated UFunction's native thunk.
 * @param ManagedObjectHandle GCHandle IntPtr of the C# wrapper object (the "this" of the call).
 * @param ParamsBuffer Pointer to the UFunction params buffer, laid out per the UFunction's properties.
 */
using FSsManagedFunctionDispatch = void (*)(const void* ManagedObjectHandle, void* ParamsBuffer);

/**
 * A UFunction that was generated from a C# [UFUNCTION] method. Its implementation lives in C#; the
 * engine reaches it through the shared native thunk installed by Bind().
 */
UCLASS()
class USsGeneratedFunction : public UFunction
{
	GENERATED_BODY()

public:
	//~ Begin UFunction interface
	virtual void Bind() override;
	//~ End UFunction interface

	/** The managed dispatch invoked by the native thunk. Set when the function is generated. */
	FSsManagedFunctionDispatch ManagedDispatch = nullptr;

	/**
	 * The native thunk shared by every generated UFunction. UE's script VM (blueprint / ProcessEvent)
	 * enters here; it forwards the params buffer to the managed dispatch bound to this function.
	 */
	DECLARE_FUNCTION(execCallManagedFunction);
};

/**
 * An Unreal class that was generated from a C# type.
 */
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
	 * @param FunctionDefines Array of function defines.
	 * @param FunctionCount Count of function array.
	 * @return Newly generated class if success, otherwise nullptr.
	 */
	static USsGeneratedClass* GenerateClass(const FName& ClassName, UClass* SuperClass, const FSsPropertyDef* PropertyDefines, int PropertyCount, const FSsFunctionDef* FunctionDefines, int FunctionCount);

private:
	/** UObject constructor. */
	static void StaticObjectConstructor(const FObjectInitializer& Initializer);

	/** Traverse the inheritance chain and return the first generated class. */
	static const USsGeneratedClass* GetFirstGeneratedClass(const UClass* InClass);

	/**
	 * Move every generated UFunction on this class out to the transient package, freeing their names.
	 * Called before a hot-reload transfer so the newly generated functions can re-take the same names.
	 */
	void MoveGeneratedFunctionsAside();

private:
	/** The most derived native super class in the inheritance chain. */
	UClass* NativeSuperClass = nullptr;

	// ReSharper disable once CppUE4ProbableMemoryIssuesWithUObjectsInContainer
	/** All USsGeneratedFunction objects generated onto this class. Used to move them aside on reload. */
	TArray<USsGeneratedFunction*> GeneratedFunctions;

	friend class FSsGeneratedClassBuilder;
};
