namespace SharpScriptBuildTool.Actions;

public abstract class BuildToolAction
{ 
	public static bool InitializeAction()
	{
		BuildToolAction buildToolAction = Program.BuildToolOptions.Action switch
		{
			BuildAction.GenerateProject => new GenerateProject(),
			_ => throw new Exception($"Can't find build action with name \"{Program.BuildToolOptions.Action}\"")
		};

		return buildToolAction.RunAction();
	}

	public abstract bool RunAction();
}