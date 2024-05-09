namespace YamlPrompt.Model;

public delegate void TaskDelegate(AutomationContext context, string? currentResult);

public abstract class TaskDefinition<T>
{
    public abstract string TypeKey { get; }
    
    protected abstract string? Invoke(AutomationContext context, T payload, string? previousResult);
    
    public abstract T MapPayload(IReadOnlyDictionary<string, object?> fields);
    
    public virtual void Execute(
        TaskDelegate next,
        AutomationContext context,
        T payload,
        string? previousResult)
    {
        string? nextResult = null;
        try {
            nextResult = Invoke(context, payload, previousResult);
        } catch(Exception ex) {
            Console.WriteLine(ex);  // TODO: replace this with proper logging
            
            if (context.Behavior.ContinueOnException) {
                next(context, null);
                return;
            }
        }
        
        next(context, nextResult);
    }
    
}
