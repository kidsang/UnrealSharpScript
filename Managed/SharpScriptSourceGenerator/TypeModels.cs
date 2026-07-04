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

	/// <summary>TMap&lt;K, V&gt; of a simple value element. Lazy cached wrapper.</summary>
	Map,

	/// <summary>TMap&lt;K, V, VNativeRef&gt; whose value is a struct element. Lazy cached wrapper.
	/// The wrapper constructor takes only the key marshaller; struct values are marshalled
	/// through the generated value native-ref.</summary>
	StructMap,

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
	/// Null for struct elements (TArray/TMap struct variants take no value marshaller instance).
	/// </summary>
	public string? MarshallerInstanceExpr;

	/// <summary>
	/// For struct elements: the generated value native-ref type name (e.g. "FFooNativeRef").
	/// Used as the third generic argument of struct container wrappers
	/// (TArray&lt;T, TRef&gt; / TMap&lt;K, V, VRef&gt;). Null for non-struct elements.
	/// </summary>
	public string? NativeRefType;
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
/// The role of a UFUNCTION parameter, mirroring <c>SharpScript.Subclassing.ParamFlags</c>.
/// </summary>
internal enum ParamRole
{
	/// <summary>A normal input parameter.</summary>
	In,

	/// <summary>An <c>out</c> parameter, copied back after the call.</summary>
	Out,

	/// <summary>The function return value.</summary>
	Return,
}

/// <summary>
/// A single analysed UFUNCTION parameter (or the synthetic return value) ready for emission.
/// Wraps the shared <see cref="PropertyModel"/> that describes how the value maps onto the
/// native side, plus the parameter name and its <see cref="ParamRole"/>.
/// </summary>
internal sealed class FunctionParamModel
{
	/// <summary>The parameter name, e.g. "InValue"; the synthetic return uses "ReturnValue".</summary>
	public string Name = "";

	/// <summary>The parameter role (in / out / return).</summary>
	public ParamRole Role;

	/// <summary>Type mapping shared with the property path (Kind / marshaller / native type).</summary>
	public PropertyModel Type = null!;

	/// <summary>
	/// The parameter's declared C# type exactly as written in the user method signature
	/// (e.g. "List&lt;string&gt;", "UObject?", "int"). Used to declare the dispatch-stub local and the
	/// <c>out</c> argument so it matches the user method — the wrapper read expression (e.g.
	/// <c>new TArray&lt;string&gt;(...)</c>) converts into it. The native <see cref="Type"/> mapping
	/// still drives marshalling, native prop class and offsets.
	/// </summary>
	public string DeclaredType = "";

	/// <summary>The <c>ParamFlags</c> enum member name for this role.</summary>
	public string ParamFlagsExpr => Role switch
	{
		ParamRole.Return => "ParamFlags.ReturnParam",
		ParamRole.Out => "ParamFlags.OutParam",
		_ => "ParamFlags.InParam",
	};

	/// <summary>True when a container/wrapper element needs a captured native property pointer.</summary>
	public bool NeedsNativeProp => Type.IsWrapper;
}

/// <summary>
/// A fully analysed [UFUNCTION] method ready for code emission.
/// </summary>
internal sealed class FunctionModel
{
	/// <summary>The C# method name (also the UE function name), e.g. "FuncInt32".</summary>
	public string Name = "";

	/// <summary>True when the method is a C# <c>static</c> method (maps to FUNC_Static).</summary>
	public bool IsStatic;

	/// <summary>The synthetic return-value parameter, or null for a void method.</summary>
	public FunctionParamModel? ReturnParam;

	/// <summary>The declared parameters in source order (in + out).</summary>
	public readonly List<FunctionParamModel> Parameters = new();

	/// <summary>
	/// All params in the order emitted into the native FunctionParamDef[] array: the return
	/// value first (if any), then out params, then in params — matching the hand-written
	/// reference (SsTestGenFunctionManual.generated.cs).
	/// </summary>
	public IEnumerable<FunctionParamModel> NativeParamOrder()
	{
		if (ReturnParam != null)
		{
			yield return ReturnParam;
		}
		foreach (FunctionParamModel p in Parameters)
		{
			if (p.Role == ParamRole.Out)
			{
				yield return p;
			}
		}
		foreach (FunctionParamModel p in Parameters)
		{
			if (p.Role == ParamRole.In)
			{
				yield return p;
			}
		}
	}

	/// <summary>All params (in + out + return) — used to declare offset / native-prop fields.</summary>
	public IEnumerable<FunctionParamModel> AllParams()
	{
		foreach (FunctionParamModel p in Parameters)
		{
			yield return p;
		}
		if (ReturnParam != null)
		{
			yield return ReturnParam;
		}
	}
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

	public readonly List<FunctionModel> Functions = new();

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
