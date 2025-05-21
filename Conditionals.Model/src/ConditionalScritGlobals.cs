using YamlPrompt.Model;

namespace YamlPrompt.Conditionals.Model;

public class ConditionalScritGlobals
{
    public ConditionalScritGlobals(AutomationContext context, string? previousResult)
    {
        Context = context;
        PreviousResult = previousResult;
    }

    public AutomationContext Context { get; }
    public string? PreviousResult { get; }
}
