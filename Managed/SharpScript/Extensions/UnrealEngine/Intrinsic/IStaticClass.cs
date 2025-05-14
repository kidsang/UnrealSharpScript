using UnrealEngine.CoreUObject;

namespace UnrealEngine.Intrinsic;

public interface IStaticClass<T> where T : UObject
{
	/// <summary>
	/// Return unreal class of this type.
	/// </summary>
	static abstract TSubclassOf<T> StaticClass { get; }
}
