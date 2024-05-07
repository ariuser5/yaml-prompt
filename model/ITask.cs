namespace YamlPrompt.Model;

public interface ITask
{
    string TypeKey { get; }
    object Execute(params string[] parameters);
}
