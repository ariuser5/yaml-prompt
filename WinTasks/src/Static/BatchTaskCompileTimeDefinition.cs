namespace WinTasks.Static;

public sealed class BatchTaskCompileTimeDefinition : IBatchTaskCompileTimeDefinition
{
	public const string TaskKey = "batch";
	public const string ExecutorFilePath = "cmd";

    string IBatchTaskCompileTimeDefinition.TaskKey => TaskKey;
    string IBatchTaskCompileTimeDefinition.ExecutorFilePath => ExecutorFilePath;
	
	private static readonly Lazy<BatchTaskCompileTimeDefinition> _singleton =
		new(() => new BatchTaskCompileTimeDefinition());
		
	public static BatchTaskCompileTimeDefinition Instance => _singleton.Value;
	
	private BatchTaskCompileTimeDefinition() 
	{
		// Private constructor to prevent instantiation outside the class
	}
}
