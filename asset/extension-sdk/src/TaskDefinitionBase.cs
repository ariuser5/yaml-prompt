using YamlPrompt.Model;

namespace YamlPrompt.ExtensionSdk;

public abstract class TaskDefinitionBase : TaskDefinitionBase<IReadOnlyDictionary<string, object?>>
{
    override public IReadOnlyDictionary<string, object?> InterpretPayload(
        IReadOnlyDictionary<string, object?> fields
    ) => fields;
}

public abstract class TaskDefinitionBase<T> : ITaskDefinition
{
    public abstract string TypeKey { get; }

    protected abstract string? Invoke(AutomationContext context, T payload, string? previousResult);

    public abstract T InterpretPayload(IReadOnlyDictionary<string, object?> fields);

    public virtual void Execute(
        IFlowController flowController,
        AutomationContext context,
        T payload,
        string? previousResult)
    {
        try
        {
            flowController.ReturnValue = Invoke(context, payload, previousResult);
        }
        catch (Exception)
        {
            flowController.ExitCode = 1;

            if (context.Behavior.ContinueOnException)
            {
                flowController.AllowContinuationOnFailure = true;
            }
            else
            {
                throw;
            }
        }
    }

    object? ITaskDefinition.InterpretPayload(
        IReadOnlyDictionary<string, object?> fields
    ) => this.InterpretPayload(fields);

    void ITaskDefinition.Execute(
        IFlowController flowController,
        AutomationContext context,
        object? payload,
        string? previousResult
    ) => this.Execute(flowController, context, (T)payload!, previousResult);
}
