namespace WinTasks;

public interface IPowerShellTaskCompileTimeDefinition
{
	string TaskKey { get; }
	string ExecutorFilePath { get; }
}
