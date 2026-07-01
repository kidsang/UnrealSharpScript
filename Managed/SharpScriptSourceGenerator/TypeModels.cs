namespace SharpScriptSourceGenerator;

/// <summary>
/// The kind of UE property a C# property maps to. Drives both the
/// <c>PropertyDef</c> emitted into the static constructor and the
/// accessor body emitted for the partial property.
/// </summary>
internal enum PropertyKind
{
	/// <summary>bool. Uses <c>BoolMarshaller</c>.</summary>
	Bool,

	/// <summary>Blittable numeric (int/float/double/...). Uses <c>BlittableMarshaller&lt;T&gt;</c>.</summary>
	Blittable,

	/// <summary>A byte-backed UENUM. Uses <c>EnumMarshaller&lt;T&gt;</c> and maps to <c>UByteProperty</c>
	/// with the generated <c>&lt;Enum&gt;NativeRef.NativeType</c> as the underlying <c>UEnum</c>.</summary>
	Enum,

	/// <summary>string. Uses <c>StringMarshaller</c>.</summary>
	String,

	/// <summary>FName. Uses <c>NameMarshaller</c>.</summary>
	Name,

	/// <summary>FText. Uses <c>TextMarshaller</c>.</summary>
	Text,

	/// <summary>UObject reference. Uses <c>ObjectMarshaller&lt;T&gt;</c>.</summary>
	Object,

	/// <summary>TSoftObjectPtr&lt;T&gt;. Uses <c>SoftObjectPtrMarshaller&lt;T&gt;</c>.</summary>
	SoftObjectPtr,

	/// <summary>TLazyObjectPtr&lt;T&gt;. Uses <c>LazyObjectPtrMarshaller&lt;T&gt;</c>.</summary>
	LazyObjectPtr,

	/// <summary>TSubclassOf&lt;T&gt;. Uses <c>SubclassOfMarshaller&lt;T&gt;</c>.</summary>
	SubclassOf,

	/// <summary>TSoftClassPtr&lt;T&gt;. Uses <c>SoftClassPtrMarshaller&lt;T&gt;</c>.</summary>
	SoftClassPtr,

	/// <summary>TArray&lt;T&gt; of a simple element. Lazy cached wrapper.</summary>
	Array,

	/// <summary>TArray&lt;TElem, TNativeRef&gt; of a struct element. Lazy cached wrapper.</summary>
	StructArray,

	/// <summary>TSet&lt;T&gt;. Lazy cached wrapper.</summary>
	Set,

	/// <summary>TMap&lt;K, V&gt;. Lazy cached wrapper.</summary>
	Map,

	/// <summary>A struct native-ref property (FXxxNativeRef). Lazy cached wrapper.</summary>
	StructNativeRef,

	/// <summary>A blittable struct exposed by reference (ref FXxx).</summary>
	BlittableStructRef,
}

/// <summary>
/// Describes how a single inner/key element of a container or struct property
/// maps onto the native side. Used to fill <c>PropertyDef</c> sub-type fields
/// and the marshaller instance arguments of container wrappers.
/// </summary>
internal sealed class ElementInfo
{
	/// <summary>Fully qualified C# type used in the wrapper generic args (e.g. "string", "int").</summary>
	public string ManagedType = "";

	/// <summary>UProperty StaticClass name, e.g. "UStrProperty".</summary>
	public string PropTypeClass = "";

	/// <summary>
	/// Expression giving the underlying native type for object/struct elements,
	/// e.g. "UObject.StaticClass.NativeClass" or "FFooNativeRef.NativeType".
	/// Null when not applicable (simple value types).
	/// </summary>
	public string? UnderlyingTypeExpr;

	/// <summary>
	/// Expression giving the marshaller instance used by container wrappers,
	/// e.g. "StringMarshaller.Instance" or "BlittableMarshaller&lt;int&gt;.Instance".
	/// Null for struct elements (TArray struct variant takes no marshaller instance).
	/// </summary>
	public string? MarshallerInstanceExpr;
}

/// <summary>
/// A fully analysed UPROPERTY ready for code emission.
/// </summary>
internal sealed class PropertyModel
{
	public string Name = "";

	public PropertyKind Kind;

	/// <summary>The declared C# type as written by the user (without trailing '?').</summary>
	public string ManagedType = "";

	/// <summary>True when the user declared the property type as nullable (e.g. UObject?).</summary>
	public bool IsNullable;

	/// <summary>UProperty StaticClass name for this property, e.g. "UIntProperty".</summary>
	public string PropTypeClass = "";

	/// <summary>
	/// For object/class/struct properties: expression for PropertyDef.UnderlyingType,
	/// e.g. "UObject.StaticClass.NativeClass" or "FFooNativeRef.NativeType".
	/// </summary>
	public string? UnderlyingTypeExpr;

	/// <summary>For Object/Soft/Lazy/Subclass/SoftClass: the element type T (e.g. "UObject").</summary>
	public string? TargetType;

	/// <summary>Inner element info for Array/Set/Map(value)/StructArray.</summary>
	public ElementInfo? Inner;

	/// <summary>Key element info for Map.</summary>
	public ElementInfo? Key;

	/// <summary>For StructNativeRef / BlittableStructRef: the native ref type name (e.g. "FFooNativeRef").</summary>
	public string? NativeRefType;

	/// <summary>For BlittableStructRef: the value struct type name (e.g. "FFoo").</summary>
	public string? ValueStructType;

	/// <summary>
	/// True when this property maps to a single blittable value (numeric). Used by the
	/// struct emitter to decide ToManaged/FromManaged form and whether a struct is blittable.
	/// </summary>
	public bool IsBlittable;

	/// <summary>
	/// True when this property is a lazily-cached wrapper (Array/Set/Map/StructArray/StructNativeRef),
	/// i.e. exposes a cached reference object rather than a value via marshaller.
	/// Drives ToManaged/FromManaged emission for structs.
	/// </summary>
	public bool IsWrapper;
}

/// <summary>
/// A fully analysed [UCLASS] declaration ready for code emission.
/// </summary>
internal sealed class ClassModel
{
	public string Namespace = "";

	/// <summary>C# class name including the leading 'U' prefix, e.g. "USsTestGenClassManual".</summary>
	public string ClassName = "";

	/// <summary>UE class name (C# name without the leading 'U'), e.g. "SsTestGenClassManual".</summary>
	public string UnrealName = "";

	/// <summary>The C# base type name, e.g. "UObject".</summary>
	public string SuperClass = "";

	public readonly List<PropertyModel> Properties = new();

	/// <summary>Hint name used for the generated source file.</summary>
	public string HintName => $"{ClassName}.generated.cs";
}

/// <summary>
/// A single value of a [UENUM] ready for code emission.
/// </summary>
internal sealed class EnumValueModel
{
	/// <summary>The C# member name, e.g. "One".</summary>
	public string Name = "";
}

/// <summary>
/// A fully analysed [UENUM] declaration ready for code emission.
/// </summary>
internal sealed class EnumModel
{
	public string Namespace = "";

	/// <summary>C# enum name including the leading 'E' prefix, e.g. "ESsTestGenEnumManual".</summary>
	public string EnumName = "";

	/// <summary>The generated native-ref class name, e.g. "ESsTestGenEnumManualNativeRef".</summary>
	public string NativeRefName => $"{EnumName}NativeRef";

	/// <summary>UE enum name (C# name without the leading 'E'), e.g. "SsTestGenEnumManual".</summary>
	public string UnrealName = "";

	/// <summary>
	/// True when the C# enum carries <c>[Flags]</c>. The generated UEnum is then created with
	/// <c>EEnumFlags::Flags</c> so UE treats it as a bitmask enum.
	/// </summary>
	public bool IsFlags;

	public readonly List<EnumValueModel> Values = new();

	/// <summary>Hint name used for the generated source file.</summary>
	public string HintName => $"{EnumName}.generated.cs";
}

/// <summary>
/// A fully analysed [USTRUCT] declaration ready for code emission.
/// </summary>
internal sealed class StructModel
{
	public string Namespace = "";

	/// <summary>C# struct name including the leading 'F' prefix, e.g. "FSsTestGenStructManual".</summary>
	public string StructName = "";

	/// <summary>The generated native-ref class name, e.g. "FSsTestGenStructManualNativeRef".</summary>
	public string NativeRefName => $"{StructName}NativeRef";

	/// <summary>UE struct name (C# name without the leading 'F'), e.g. "SsTestGenStructManual".</summary>
	public string UnrealName = "";

	/// <summary>
	/// True when every field is blittable (numeric). Blittable structs use a simpler
	/// NativeRef form based on BlittableMarshaller and direct field overlay access.
	/// </summary>
	public bool IsBlittable;

	public readonly List<PropertyModel> Properties = new();

	/// <summary>Hint name used for the generated source file.</summary>
	public string HintName => $"{StructName}.generated.cs";
}
