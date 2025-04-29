using Object = UnrealEngine.CoreUObject.Object;

namespace UnrealEngine.Intrinsic;

public interface IStaticClass<T> where T : Object
{
	static abstract SubclassOf<T> StaticClass { get; }
}
