namespace YamlPrompt.Model;

public interface ITaskDefinition
{
    string TypeKey { get; }
    
    object? InterpretPayload(IReadOnlyDictionary<string, object?> fields);
    
    void Execute(
        IFlowController flowController,
        AutomationContext context,
        object? payload,
        string? previousResult);
}
