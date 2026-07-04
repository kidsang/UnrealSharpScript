using Microsoft.CodeAnalysis;

namespace SharpScriptSourceGenerator;

/// <summary>
/// Classifies a property's C# type into a <see cref="PropertyModel"/> describing
/// how it maps onto the native UE side. Mirrors the C++ PropertyTranslators used
/// by the UHT-based binding generator, but works against Roslyn semantic symbols.
/// </summary>
internal static class PropertyClassifier
{
	/// <summary>
	/// Blittable numeric C# primitive type → UProperty StaticClass name.
	/// Matches SharpScriptBindingGenerator.PropertyTranslators.PropertyTranslatorManager.
	/// </summary>
	private static readonly Dictionary<string, string> BlittableNumericToProperty = new()
	{
		{ "sbyte", "UInt8Property" },
		{ "short", "UInt16Property" },
		{ "int", "UIntProperty" },
		{ "long", "UInt64Property" },
		{ "byte", "UByteProperty" },
		{ "ushort", "UUint16Property" },
		{ "uint", "UUint32Property" },
		{ "ulong", "UUint64Property" },
		{ "float", "UFloatProperty" },
		{ "double", "UDoubleProperty" },
	};

	/// <summary>
	/// Attempts to classify the given property symbol. Returns null (with a diagnostic
	/// reported via <paramref name="report"/>) when the type is not supported in this phase.
	/// </summary>
	public static PropertyModel? Classify(IPropertySymbol propertySymbol, Action<Diagnostic> report)
	{
		// Detect ref-returning properties: 'ref FFoo BlittableStruct { get; }'.
		bool isRefReturn = propertySymbol.ReturnsByRef || propertySymbol.ReturnsByRefReadonly;

		PropertyModel? model = ClassifyType(propertySymbol.Type, isRefReturn);
		if (model == null)
		{
			report(Diagnostic.Create(
				Diagnostics.UnsupportedPropertyType,
				propertySymbol.Locations.FirstOrDefault(),
				propertySymbol.Name,
				propertySymbol.Type.ToDisplayString()));
			return null;
		}

		model.Name = propertySymbol.Name;
		return model;
	}

	/// <summary>
	/// Classifies a USTRUCT member field. Struct fields use the managed collection types
	/// (<c>List&lt;T&gt;</c> / <c>HashSet&lt;T&gt;</c> / <c>Dictionary&lt;K,V&gt;</c>) and nested
	/// USTRUCTs by value (<c>FXxx</c>), which the generated native-ref exposes as
	/// <c>TArray</c>/<c>TSet</c>/<c>TMap</c> and <c>FXxxNativeRef</c> respectively.
	/// </summary>
	public static PropertyModel? ClassifyField(IFieldSymbol fieldSymbol, Action<Diagnostic> report)
	{
		PropertyModel? model = ClassifyFieldType(fieldSymbol.Type);
		if (model == null)
		{
			report(Diagnostic.Create(
				Diagnostics.UnsupportedPropertyType,
				fieldSymbol.Locations.FirstOrDefault(),
				fieldSymbol.Name,
				fieldSymbol.Type.ToDisplayString()));
			return null;
		}

		model.Name = fieldSymbol.Name;
		return model;
	}

	/// <summary>
	/// Classifies a UFUNCTION parameter (or return value) type into a <see cref="PropertyModel"/>.
	/// Parameters use the same collection forms as struct fields (<c>List&lt;T&gt;</c> /
	/// <c>HashSet&lt;T&gt;</c> / <c>Dictionary&lt;K,V&gt;</c>) and pass USTRUCTs by value. A USTRUCT
	/// whose fields are all blittable is marshalled directly with <c>BlittableMarshaller&lt;T&gt;</c>
	/// (matching the hand-written FuncBlittableStruct); otherwise it goes through its generated
	/// <c>FXxxNativeRef</c> (matching FuncStruct).
	/// </summary>
	public static PropertyModel? ClassifyParam(ITypeSymbol type)
	{
		ITypeSymbol coreType = StripNullable(type).WithNullableAnnotation(NullableAnnotation.None);

		// USTRUCT passed by value. A fully-blittable struct marshals via BlittableMarshaller<T>;
		// a non-blittable one via its generated native-ref.
		if (IsUserStructType(coreType))
		{
			string valueType = coreType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string nativeRef = $"{valueType}NativeRef";
			bool blittable = IsBlittableStruct(coreType);
			return new PropertyModel
			{
				Kind = PropertyKind.StructNativeRef,
				ManagedType = valueType,
				PropTypeClass = "UStructProperty",
				NativeRefType = nativeRef,
				ValueStructType = valueType,
				UnderlyingTypeExpr = $"{nativeRef}.NativeType",
				IsBlittable = blittable,
				IsWrapper = !blittable,
			};
		}

		// Collection parameters use the field-style collection classification (List/HashSet/Dictionary).
		if (coreType is INamedTypeSymbol { IsGenericType: true, Name: "List" or "HashSet" or "Dictionary" })
		{
			return ClassifyFieldType(type);
		}

		// Everything else (bool/numeric/string/FName/FText/enum/object/soft/lazy/subclass/soft-class/
		// TArray/TSet/TMap wrapper forms) behaves identically to the class-property case.
		return ClassifyType(type, isRefReturn: false);
	}

	/// <summary>
	/// True when a USTRUCT is blittable, i.e. its managed layout matches the UE-generated native
	/// layout so it can be marshalled with <c>BlittableMarshaller&lt;T&gt;</c>. Delegates to the shared
	/// <see cref="IsBlittableStructType"/> so this stays identical to <c>UStructGenerator</c>'s rule
	/// (an unbound instance field or any non-blittable [UPROPERTY] makes the struct non-blittable).
	/// </summary>
	private static bool IsBlittableStruct(ITypeSymbol structType)
	{
		foreach (ISymbol member in structType.GetMembers())
		{
			if (member is not IFieldSymbol fieldSymbol || fieldSymbol.IsStatic || fieldSymbol.IsImplicitlyDeclared)
			{
				continue;
			}

			if (!SymbolUtils.HasUPropertyAttribute(fieldSymbol))
			{
				return false;
			}

			PropertyModel? fieldModel = ClassifyFieldType(fieldSymbol.Type);
			if (fieldModel == null || !fieldModel.IsBlittable)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Maps a struct field's declared type onto a <see cref="PropertyModel"/>. Handles the
	/// collection / nested-struct field forms, then delegates to the shared value/object
	/// classification for everything else.
	/// </summary>
	private static PropertyModel? ClassifyFieldType(ITypeSymbol type)
	{
		ITypeSymbol coreType = StripNullable(type).WithNullableAnnotation(NullableAnnotation.None);

		if (coreType is INamedTypeSymbol { IsGenericType: true } named)
		{
			ImmutableArrayLike<ITypeSymbol> args = new(named.TypeArguments);
			switch (named.Name)
			{
				case "List":
				{
					ElementInfo? inner = ClassifyElement(args[0]);
					if (inner == null) return null;
					return MakeArrayFromElement(inner);
				}
				case "HashSet":
				{
					ElementInfo? inner = ClassifyElement(args[0]);
					if (inner == null) return null;
					// Struct elements have no TSet<T, TRef> wrapper; reject rather than emit
					// an uncompilable set (a struct is not hashable as a UE set element anyway).
					if (inner.NativeRefType != null) return null;
					return new PropertyModel
					{
						Kind = PropertyKind.Set,
						ManagedType = $"TSet<{inner.ManagedType}>",
						PropTypeClass = "USetProperty",
						Inner = inner,
						IsWrapper = true,
					};
				}
				case "Dictionary":
					return MakeMap(args[0], args[1]);
			}
		}

		// Nested USTRUCT by value: 'FXxx Field;' is exposed as a lazy FXxxNativeRef.
		if (IsUserStructType(coreType))
		{
			string valueType = coreType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string nativeRef = $"{valueType}NativeRef";
			return new PropertyModel
			{
				Kind = PropertyKind.StructNativeRef,
				ManagedType = valueType,
				PropTypeClass = "UStructProperty",
				NativeRefType = nativeRef,
				ValueStructType = valueType,
				UnderlyingTypeExpr = $"{nativeRef}.NativeType",
				IsWrapper = true,
			};
		}

		// Everything else (bool/numeric/string/FName/FText/object/soft/lazy/subclass/...)
		// behaves identically to the class-property case.
		return ClassifyType(type, isRefReturn: false);
	}

	private static PropertyModel? ClassifyType(ITypeSymbol type, bool isRefReturn)
	{
		bool isNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
		// Strip both Nullable<T> (value types) and the reference-type '?' annotation so
		// the managed type name never carries a trailing '?'. Nullability is tracked
		// separately via isNullable and re-applied only where appropriate (e.g. UObject?).
		ITypeSymbol coreType = StripNullable(type).WithNullableAnnotation(NullableAnnotation.None);

		// Special predefined value types.
		switch (coreType.SpecialType)
		{
			case SpecialType.System_Boolean:
				return new PropertyModel
				{
					Kind = PropertyKind.Bool,
					ManagedType = "bool",
					PropTypeClass = "UBoolProperty",
				};
			case SpecialType.System_String:
				return new PropertyModel
				{
					Kind = PropertyKind.String,
					ManagedType = "string",
					PropTypeClass = "UStrProperty",
				};
		}

		string typeName = coreType.Name;
		string managed = coreType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

		// Blittable numeric primitives.
		string keyword = ToKeyword(coreType);
		if (BlittableNumericToProperty.TryGetValue(keyword, out string? numericProp))
		{
			return new PropertyModel
			{
				Kind = PropertyKind.Blittable,
				ManagedType = keyword,
				PropTypeClass = numericProp,
				IsBlittable = true,
			};
		}

		switch (typeName)
		{
			case "FName":
				return new PropertyModel { Kind = PropertyKind.Name, ManagedType = "FName", PropTypeClass = "UNameProperty" };
			case "FText":
				return new PropertyModel { Kind = PropertyKind.Text, ManagedType = "FText", PropTypeClass = "UTextProperty" };
		}

		// UENUM: a byte-backed enum maps to a UByteProperty whose underlying UEnum is the generated <Enum>NativeRef.NativeType.
		// Only byte-backed enums are supported by the native subclassing path (FByteProperty).
		if (coreType.TypeKind == TypeKind.Enum && IsByteBackedEnum(coreType))
		{
			return new PropertyModel
			{
				Kind = PropertyKind.Enum,
				ManagedType = managed,
				PropTypeClass = "UByteProperty",
				TargetType = managed,
				UnderlyingTypeExpr = $"{managed}NativeRef.NativeType",
				IsBlittable = true,
			};
		}

		// Generic wrappers.
		if (coreType is INamedTypeSymbol { IsGenericType: true } named)
		{
			ImmutableArrayLike<ITypeSymbol> args = new(named.TypeArguments);
			switch (typeName)
			{
				case "TSoftObjectPtr":
					return MakePointerLike(PropertyKind.SoftObjectPtr, "USoftObjectProperty", args[0]);
				case "TLazyObjectPtr":
					return MakePointerLike(PropertyKind.LazyObjectPtr, "ULazyObjectProperty", args[0]);
				case "TSubclassOf":
					return MakePointerLike(PropertyKind.SubclassOf, "UClassProperty", args[0]);
				case "TSoftClassPtr":
					return MakePointerLike(PropertyKind.SoftClassPtr, "USoftClassProperty", args[0]);
				case "TArray":
					return MakeArray(args);
				case "TSet":
				{
					ElementInfo? inner = ClassifyElement(args[0]);
					if (inner == null) return null;
					// Struct elements have no TSet<T, TRef> wrapper; reject rather than emit
					// an uncompilable set (a struct is not hashable as a UE set element anyway).
					if (inner.NativeRefType != null) return null;
					return new PropertyModel
					{
						Kind = PropertyKind.Set,
						ManagedType = $"TSet<{inner.ManagedType}>",
						PropTypeClass = "USetProperty",
						Inner = inner,
						IsWrapper = true,
					};
				}
				case "TMap":
					return MakeMap(args[0], args[1]);
			}
		}

		// UObject reference (the C# type derives from UObject).
		if (SymbolUtils.IsUObjectDerived(coreType, includeSelf: true))
		{
			return new PropertyModel
			{
				Kind = PropertyKind.Object,
				ManagedType = managed,
				IsNullable = isNullable,
				PropTypeClass = "UObjectProperty",
				TargetType = managed,
				UnderlyingTypeExpr = $"{managed}.StaticClass.NativeClass",
			};
		}

		// Struct native ref (FXxxNativeRef) — exposed as a lazy wrapper.
		if (typeName.EndsWith("NativeRef"))
		{
			return new PropertyModel
			{
				Kind = PropertyKind.StructNativeRef,
				ManagedType = managed,
				PropTypeClass = "UStructProperty",
				NativeRefType = managed,
				UnderlyingTypeExpr = $"{managed}.NativeType",
				IsWrapper = true,
			};
		}

		// Blittable struct exposed by reference: 'ref FFoo Prop { get; }'.
		if (isRefReturn && typeName.StartsWith("F"))
		{
			string nativeRef = $"{managed}NativeRef";
			return new PropertyModel
			{
				Kind = PropertyKind.BlittableStructRef,
				ManagedType = managed,
				PropTypeClass = "UStructProperty",
				ValueStructType = managed,
				NativeRefType = nativeRef,
				UnderlyingTypeExpr = $"{nativeRef}.NativeType",
			};
		}

		return null;
	}

	private static PropertyModel MakePointerLike(PropertyKind kind, string propClass, ITypeSymbol target)
	{
		string targetName = target.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
		string wrapperName = kind switch
		{
			PropertyKind.SoftObjectPtr => $"TSoftObjectPtr<{targetName}>",
			PropertyKind.LazyObjectPtr => $"TLazyObjectPtr<{targetName}>",
			PropertyKind.SubclassOf => $"TSubclassOf<{targetName}>",
			PropertyKind.SoftClassPtr => $"TSoftClassPtr<{targetName}>",
			_ => targetName,
		};
		return new PropertyModel
		{
			Kind = kind,
			ManagedType = wrapperName,
			PropTypeClass = propClass,
			TargetType = targetName,
			UnderlyingTypeExpr = $"{targetName}.StaticClass.NativeClass",
		};
	}

	private static PropertyModel? MakeArray(ImmutableArrayLike<ITypeSymbol> args)
	{
		// TArray<TElem, TNativeRef>: struct array variant.
		if (args.Length == 2)
		{
			string elem = args[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			string nativeRef = args[1].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			ElementInfo inner = new()
			{
				ManagedType = elem,
				PropTypeClass = "UStructProperty",
				UnderlyingTypeExpr = $"{nativeRef}.NativeType",
			};
			return new PropertyModel
			{
				Kind = PropertyKind.StructArray,
				ManagedType = $"TArray<{elem}, {nativeRef}>",
				PropTypeClass = "UArrayProperty",
				Inner = inner,
				IsWrapper = true,
			};
		}

		// TArray<T>: simple element variant (or struct element via native-ref).
		ElementInfo? simple = ClassifyElement(args[0]);
		if (simple == null) return null;
		return MakeArrayFromElement(simple);
	}

	/// <summary>
	/// Builds an array property model from an already-classified element. A struct element
	/// (carrying a native-ref) becomes a StructArray exposed as <c>TArray&lt;T, TRef&gt;</c>;
	/// every other element becomes a simple <c>TArray&lt;T&gt;</c>.
	/// </summary>
	private static PropertyModel MakeArrayFromElement(ElementInfo inner)
	{
		if (inner.NativeRefType != null)
		{
			return new PropertyModel
			{
				Kind = PropertyKind.StructArray,
				ManagedType = $"TArray<{inner.ManagedType}, {inner.NativeRefType}>",
				PropTypeClass = "UArrayProperty",
				Inner = inner,
				IsWrapper = true,
			};
		}

		return new PropertyModel
		{
			Kind = PropertyKind.Array,
			ManagedType = $"TArray<{inner.ManagedType}>",
			PropTypeClass = "UArrayProperty",
			Inner = inner,
			IsWrapper = true,
		};
	}

	/// <summary>
	/// Builds a map property model from key/value element types. A struct value (carrying a
	/// native-ref) becomes a StructMap exposed as <c>TMap&lt;K, V, VRef&gt;</c> (the wrapper takes
	/// only the key marshaller); every other value becomes a simple <c>TMap&lt;K, V&gt;</c>.
	/// Struct keys are unsupported (a struct is not hashable as a UE map key).
	/// </summary>
	private static PropertyModel? MakeMap(ITypeSymbol keyType, ITypeSymbol valueType)
	{
		ElementInfo? key = ClassifyElement(keyType);
		ElementInfo? value = ClassifyElement(valueType);
		if (key == null || value == null) return null;

		// A struct key has no hashable map-key representation; reject it.
		if (key.NativeRefType != null) return null;

		if (value.NativeRefType != null)
		{
			return new PropertyModel
			{
				Kind = PropertyKind.StructMap,
				ManagedType = $"TMap<{key.ManagedType}, {value.ManagedType}, {value.NativeRefType}>",
				PropTypeClass = "UMapProperty",
				Key = key,
				Inner = value,
				IsWrapper = true,
			};
		}

		return new PropertyModel
		{
			Kind = PropertyKind.Map,
			ManagedType = $"TMap<{key.ManagedType}, {value.ManagedType}>",
			PropTypeClass = "UMapProperty",
			Key = key,
			Inner = value,
			IsWrapper = true,
		};
	}

	/// <summary>
	/// Classifies a container inner/key element into its native property class and
	/// container marshaller instance expression.
	/// </summary>
	private static ElementInfo? ClassifyElement(ITypeSymbol type)
	{
		ITypeSymbol coreType = StripNullable(type).WithNullableAnnotation(NullableAnnotation.None);

		switch (coreType.SpecialType)
		{
			case SpecialType.System_Boolean:
				return new ElementInfo { ManagedType = "bool", PropTypeClass = "UBoolProperty", MarshallerInstanceExpr = "BoolMarshaller.Instance" };
			case SpecialType.System_String:
				return new ElementInfo { ManagedType = "string", PropTypeClass = "UStrProperty", MarshallerInstanceExpr = "StringMarshaller.Instance" };
		}

		string keyword = ToKeyword(coreType);
		if (BlittableNumericToProperty.TryGetValue(keyword, out string? numericProp))
		{
			return new ElementInfo
			{
				ManagedType = keyword,
				PropTypeClass = numericProp,
				MarshallerInstanceExpr = $"BlittableMarshaller<{keyword}>.Instance",
			};
		}

		string managed = coreType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

		// UENUM: a byte-backed enum maps to a UByteProperty whose underlying UEnum is the generated <Enum>NativeRef.NativeType.
		// Only byte-backed enums are supported by the native subclassing path (FByteProperty).
		if (coreType.TypeKind == TypeKind.Enum && IsByteBackedEnum(coreType))
		{
			return new ElementInfo
			{
				ManagedType = managed,
				PropTypeClass = "UByteProperty",
				UnderlyingTypeExpr = $"{managed}NativeRef.NativeType",
				MarshallerInstanceExpr = $"EnumMarshaller<{managed}>.Instance",
			};
		}

		switch (coreType.Name)
		{
			case "FName":
				return new ElementInfo { ManagedType = "FName", PropTypeClass = "UNameProperty", MarshallerInstanceExpr = "NameMarshaller.Instance" };
			case "FText":
				return new ElementInfo { ManagedType = "FText", PropTypeClass = "UTextProperty", MarshallerInstanceExpr = "TextMarshaller.Instance" };
		}

		if (SymbolUtils.IsUObjectDerived(coreType, includeSelf: true))
		{
			// ObjectMarshaller<T> implements IMarshaller<T?>, so the container element type is
			// nullable (UObject?) to match the marshaller instance's generic argument.
			return new ElementInfo
			{
				ManagedType = $"{managed}?",
				PropTypeClass = "UObjectProperty",
				UnderlyingTypeExpr = $"{managed}.StaticClass.NativeClass",
				MarshallerInstanceExpr = $"ObjectMarshaller<{managed}>.Instance",
			};
		}

		// User-defined USTRUCT used as a container element (array element / map value).
		// Exposed through its generated FXxxNativeRef; the struct container wrappers
		// (TArray<T, TRef> / TMap<K, V, VRef>) marshal through the native-ref rather than an
		// IMarshaller instance, so MarshallerInstanceExpr stays null.
		if (IsUserStructType(coreType))
		{
			string nativeRef = $"{managed}NativeRef";
			return new ElementInfo
			{
				ManagedType = managed,
				PropTypeClass = "UStructProperty",
				UnderlyingTypeExpr = $"{nativeRef}.NativeType",
				NativeRefType = nativeRef,
			};
		}

		return null;
	}

	private static ITypeSymbol StripNullable(ITypeSymbol type)
	{
		if (type is INamedTypeSymbol { IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T } named)
		{
			return named.TypeArguments[0];
		}
		return type;
	}

	private static string ToKeyword(ITypeSymbol type)
	{
		return type.SpecialType switch
		{
			SpecialType.System_SByte => "sbyte",
			SpecialType.System_Int16 => "short",
			SpecialType.System_Int32 => "int",
			SpecialType.System_Int64 => "long",
			SpecialType.System_Byte => "byte",
			SpecialType.System_UInt16 => "ushort",
			SpecialType.System_UInt32 => "uint",
			SpecialType.System_UInt64 => "ulong",
			SpecialType.System_Single => "float",
			SpecialType.System_Double => "double",
			_ => type.Name,
		};
	}

	/// <summary>
	/// True when the enum's underlying type is <c>byte</c>. The native subclassing path only
	/// supports byte-backed enums (FByteProperty + UEnum); other underlying types are rejected.
	/// </summary>
	private static bool IsByteBackedEnum(ITypeSymbol type)
	{
		return type is INamedTypeSymbol { EnumUnderlyingType.SpecialType: SpecialType.System_Byte };
	}

	/// <summary>
	/// True when the type is a user-defined UE struct (carries [USTRUCT]). Such a struct,
	/// when used as a field by value, is exposed via its generated FXxxNativeRef wrapper.
	/// </summary>
	private static bool IsUserStructType(ITypeSymbol type)
	{
		if (type.TypeKind != TypeKind.Struct)
		{
			return false;
		}

		foreach (AttributeData attr in type.GetAttributes())
		{
			if (attr.AttributeClass?.Name == "USTRUCTAttribute")
			{
				return true;
			}
		}
		return false;
	}
}

/// <summary>
/// Thin index-able wrapper over an immutable array of type arguments to keep
/// the classifier code readable without taking a hard dependency on the exact
/// ImmutableArray API surface in this file.
/// </summary>
internal readonly struct ImmutableArrayLike<T>(System.Collections.Immutable.ImmutableArray<T> items)
{
	public T this[int index] => items[index];

	public int Length => items.Length;
}
