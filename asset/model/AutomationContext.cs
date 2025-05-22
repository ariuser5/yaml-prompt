namespace YamlPrompt.Model;

public class AutomationContext
{
    public AutomationBehavior Behavior { get; init; } = new AutomationBehavior();
    public IReadOnlyDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
    public AddOnlyDictionary<string, object> Items { get; init; } = new AddOnlyDictionary<string, object>();
}
