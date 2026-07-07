using SharpScript.Subclassing;
using UnrealEngine.CoreUObject;

namespace SharpScriptUnitTest.Types;

[UCLASS(ClassSpecs.BlueprintType)]
public partial class USsTestGenSpecBlueprintType : UObject;

[UCLASS(ClassSpecs.NotBlueprintType)]
public partial class USsTestGenSpecNotBlueprintType : USsTestGenSpecBlueprintType;

[UCLASS(ClassSpecs.Blueprintable)]
public partial class USsTestGenSpecBlueprintable : UObject;

[UCLASS(ClassSpecs.NotBlueprintable)]
public partial class USsTestGenSpecNotBlueprintable : USsTestGenSpecBlueprintable;

[UCLASS(ClassSpecs.NotPlaceable)]
public partial class USsTestGenSpecNotPlaceable : UObject;

[UCLASS(ClassSpecs.DefaultToInstanced)]
public partial class USsTestGenSpecDefaultToInstanced : UObject;

[UCLASS(ClassSpecs.Const)]
public partial class USsTestGenSpecConst : UObject;

[UCLASS(ClassSpecs.Abstract)]
public partial class USsTestGenSpecAbstract : UObject;

[UCLASS(ClassSpecs.Deprecated)]
public partial class USsTestGenSpecDeprecated : UObject;

[UCLASS(ClassSpecs.Transient)]
public partial class USsTestGenSpecTransient : UObject;

[UCLASS(ClassSpecs.NonTransient)]
public partial class USsTestGenSpecNonTransient : USsTestGenSpecTransient { }

[UCLASS(ClassSpecs.PerObjectConfig)]
public partial class USsTestGenSpecPerObjectConfig : UObject;

[UCLASS(ClassSpecs.DefaultConfig)]
public partial class USsTestGenSpecDefaultConfig : UObject;

[UCLASS(ClassSpecs.GlobalUserConfig)]
public partial class USsTestGenSpecGlobalUserConfig : UObject;

[UCLASS(ClassSpecs.ProjectUserConfig)]
public partial class USsTestGenSpecProjectUserConfig : UObject;

[UCLASS(ClassSpecs.ConfigDoNotCheckDefaults)]
public partial class USsTestGenSpecConfigDoNotCheckDefaults : UObject;

[UCLASS(ClassSpecs.EditInlineNew)]
public partial class USsTestGenSpecEditInlineNew : UObject;

[UCLASS(ClassSpecs.NotEditInlineNew)]
public partial class USsTestGenSpecNotEditInlineNew : USsTestGenSpecEditInlineNew;

[UCLASS(ClassSpecs.HideDropdown)]
public partial class USsTestGenSpecHideDropdown : UObject;

[UCLASS(Config = "Game")]
public partial class USsTestGenSpecConfig : UObject;

[UCLASS(ClassSpecs.NotPlaceable, ClassSpecs.Const, ClassSpecs.Abstract, ClassSpecs.HideDropdown)]
public partial class USsTestGenSpecCombined : UObject;

[UCLASS(ClassSpecs.BlueprintType, ClassSpecs.Blueprintable,
	DisplayName = "Specifier Metadata Test",
	Category = "CSharp|Internal",
	Meta = ["ToolTip=Generated for SubclassingSpecifierTest", "CustomFlag"])]
public partial class USsTestGenSpecMetadata : UObject { }
