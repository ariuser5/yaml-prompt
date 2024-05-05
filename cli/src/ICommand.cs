namespace YamlPrompt.Cli;
public interface ICommand
{
    string Key { get; }
    object Execute(params string[] parameters);
}
