using YamlPrompt.Model;
using YamlPrompt.ExtensionSdk;

namespace YamlPrompt.Tasks.Conditionals;

public class AssertTask : TaskDefinitionBase<string>
{
    public static class Template
    {
        public const string TypeKey = "assert";
        public const string ConditionField = "condition";
    }

    public override string TypeKey => Template.TypeKey;

    public override string InterpretPayload(IReadOnlyDictionary<string, object?> fields)
    {
        return fields[Template.ConditionField] as string
            ?? throw new ArgumentException($"Field '{Template.ConditionField}' is required.");
    }

    public override void Execute(
        IFlowController flowController,
        AutomationContext context,
        string payload,
        string? previousResult)
    {
        base.Execute(flowController, context, payload, previousResult);

        var returnValue = bool.Parse(flowController.ReturnValue
            ?? throw new InvalidOperationException("Return value is null."));

        flowController.AbortRequested = !returnValue;
    }

    protected override string? Invoke(
        AutomationContext context,
        string payload,
        string? previousResult)
    {
        var result = ScriptEvaluator.Evaluate<bool>(
            context,
            payload,
            previousResult);
            
        return result.ToString();
    }
}