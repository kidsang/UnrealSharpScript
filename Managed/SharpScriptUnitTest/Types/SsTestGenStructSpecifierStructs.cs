using SharpScript.Subclassing;

namespace SharpScriptUnitTest.Types;

[USTRUCT(StructSpecs.BlueprintType)]
public partial struct FSsTestGenStructSpecBlueprintType
{
	[UPROPERTY]
	public int Value;
}

[USTRUCT]
public partial struct FSsTestGenStructSpecPlain
{
	[UPROPERTY]
	public int Value;
}

[USTRUCT(StructSpecs.BlueprintType,
	DisplayName = "Struct Specifier Metadata Test",
	Category = "CSharp|Internal",
	Meta = ["ToolTip=Generated for SubclassingSpecifierTest", "CustomFlag"])]
public partial struct FSsTestGenStructSpecMetadata
{
	[UPROPERTY]
	public int Value;
}
