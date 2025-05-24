using YamlPrompt.Model;
using YamlPrompt.Scripting.Core;
using YamlPrompt.Tasks.Sdk;

namespace YamlPrompt.Tasks.Builtins.Conditionals;

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
        var input = fields[Template.ConditionField] as string
            ?? throw new ArgumentException($"Field '{Template.ConditionField}' is required.");

        var IsValidCondition = ScriptHelper.IsScript(input);
        if (!IsValidCondition)
        {
            throw new ArgumentException($"Field '{Template.ConditionField}' is not a valid script.");
        }
        
        return ScriptHelper.ExtractScript(input);
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
        string script,
        string? previousResult)
    {
        var result = ScriptEvaluator.EvaluateCondition(
            script,
            context,
            previousResult);
            
        return result.ToString();
    }
}