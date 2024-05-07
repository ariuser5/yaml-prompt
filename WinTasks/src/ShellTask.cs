using YamlPrompt.Model;

namespace WinTasks;

public class ShellTask : ITask
{
    public const string TaskKey = "shell";
    
    
    public string TypeKey => TaskKey;

    public object Execute(params string[] parameters)
    {
        throw new NotImplementedException();
    }
}
