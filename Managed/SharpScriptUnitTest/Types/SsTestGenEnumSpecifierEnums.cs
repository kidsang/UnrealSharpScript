using SharpScript.Subclassing;

namespace SharpScriptUnitTest.Types;

[UENUM(EnumSpecs.BlueprintType)]
public enum ESsTestGenEnumSpecBlueprintType : byte
{
	One = 0,
	Two,
}

[UENUM]
public enum ESsTestGenEnumSpecPlain : byte
{
	One = 0,
	Two,
}

[UENUM(DisplayName = "Enum DisplayName Test")]
public enum ESsTestGenEnumSpecDisplayName : byte
{
	One = 0,
	Two,
}

[UENUM(Category = "CSharp|Internal")]
public enum ESsTestGenEnumSpecCategory : byte
{
	One = 0,
	Two,
}

[UENUM(Meta = ["ToolTip=Generated for SubclassingSpecifierTest", "CustomFlag"])]
public enum ESsTestGenEnumSpecMeta : byte
{
	One = 0,
	Two,
}

[UENUM(EnumSpecs.BlueprintType,
	DisplayName = "Enum Specifier Metadata Test",
	Category = "CSharp|Internal",
	Meta = ["ToolTip=Generated for SubclassingSpecifierTest", "CustomFlag"])]
public enum ESsTestGenEnumSpecMetadata : byte
{
	One = 0,
	Two,
}
