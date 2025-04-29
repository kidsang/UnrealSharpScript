using UnrealBuildTool;

public class SharpScriptUnitTest : ModuleRules
{
	public SharpScriptUnitTest(ReadOnlyTargetRules Target) : base(Target)
	{
		// todo: twx convenient for debugging during development
		bUseUnity = false;
		PCHUsage = PCHUsageMode.NoPCHs;
		OptimizeCode = CodeOptimization.Never;

		PublicDependencyModuleNames.AddRange(
			new string[]
			{
				"Core",
			}
		);

		PrivateDependencyModuleNames.AddRange(
			new string[]
			{
				"CoreUObject",
				"Engine",
				"Slate",
				"SlateCore",
				"Projects",
				"SharpScriptHost",
				"SharpScript",
			}
		);
	}
}
