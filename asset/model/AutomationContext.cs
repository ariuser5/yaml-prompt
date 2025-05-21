namespace YamlPrompt.Model;

public class AutomationContext
{
    public AutomationBehavior Behavior { get; init; } = new AutomationBehavior();
    public IReadOnlyDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
    public AppendOnlyDictionary<string, object?> Items { get; init; } = new AppendOnlyDictionary<string, object?>();
}
