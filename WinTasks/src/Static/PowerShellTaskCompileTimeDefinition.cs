namespace WinTasks.Static;

public sealed class PowerShellTaskCompileTimeDefinition : IPowerShellTaskCompileTimeDefinition
{
	public const string TaskKey = "powershell";
	public const string ExecutorFilePath = "powershell";

	string IPowerShellTaskCompileTimeDefinition.TaskKey => TaskKey;
	string IPowerShellTaskCompileTimeDefinition.ExecutorFilePath => ExecutorFilePath;
	
	private static readonly Lazy<PowerShellTaskCompileTimeDefinition> _singleton =
		new(() => new PowerShellTaskCompileTimeDefinition());
		
	public static PowerShellTaskCompileTimeDefinition Instance => _singleton.Value;
	
	private PowerShellTaskCompileTimeDefinition() 
	{
		// Private constructor to prevent instantiation outside the class
	}
}
