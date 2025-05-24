using System.Dynamic;
using YamlPrompt.Model;

namespace YamlPrompt.Scripting.Core;

public class ScritGlobals : DynamicObject
{
    private readonly ExpandoObject _expando = new();

    public ScritGlobals(AutomationContext context, string? previousResult)
    {
        Context = context;
        PreviousResult = previousResult;
        // Assign all context items as dynamic properties
        var dict = (IDictionary<string, object?>)_expando;
        foreach (var kvp in context.Items)
        {
            dict[kvp.Key] = kvp.Value;
        }
    }

    public AutomationContext Context { get; }
    public string? PreviousResult { get; }
    public IDictionary<string, object?> Variables => _expando;

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
        => ((IDictionary<string, object?>)_expando).TryGetValue(binder.Name, out result);

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        ((IDictionary<string, object?>)_expando)[binder.Name] = value;
        return true;
    }
}
