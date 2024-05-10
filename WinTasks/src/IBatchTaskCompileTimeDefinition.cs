namespace WinTasks;

public interface IBatchTaskCompileTimeDefinition
{
	string TaskKey { get; }
	string ExecutorFilePath { get; }
}
