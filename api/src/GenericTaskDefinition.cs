using YamlPrompt.Model;

namespace YamlPrompt.Api;

public interface IGenericTaskDefinition
{
	object ConcreteTaskDefinition { get; }
	
	string TypeKey { get; }
	object? MapPayload(IReadOnlyDictionary<string, object?> fields);
	void Execute(
		TaskDelegate next,
		AutomationContext context,
		object? payload,
		string? previousResult);
}

internal class GenericTaskDefinition : IGenericTaskDefinition
{
	internal GenericTaskDefinition() { }
	
	public required object ConcreteTaskDefinition { get; init; }

    public required string TypeKey { get; init;}
	
	public required Action<TaskDelegate, AutomationContext, object?, string?> ExecuteImplementation { get; init; }
	
	public required Func<IReadOnlyDictionary<string, object?>, object?> MapPayloadImplementation { get; init; }

    void IGenericTaskDefinition.Execute(
		TaskDelegate next,
		AutomationContext context,
		object? payload,
		string? previousResult)
    {
        ExecuteImplementation.Invoke(next, context, payload, previousResult);
    }

    object? IGenericTaskDefinition.MapPayload(IReadOnlyDictionary<string, object?> fields)
    {
		return MapPayloadImplementation.Invoke(fields);
    }
}
