using YamlPrompt.Model;

namespace YamlPrompt.Api;

public record AutomationScript
{
	public const string BehaviorFieldName = "behavior";
	public const string StepsFieldName = "steps";
	public const string StepTypeFieldName = "type";
	
	public Dictionary<string, object?> Headers { get; init; } = [];
	public AutomationBehavior? Behavior { get; init; } = null;
	public List<AutomationStep> Steps { get; init; } = [];
	
	public void Run(ITaskDefinitionProvider definitionProvider)
	{
		var headersSnapshot = Headers.ToDictionary(entry => entry.Key, entry => entry.Value);
		var behaviorSnapshot = Behavior ?? new AutomationBehavior();
		var stepsSnapshot = Steps.ToArray();
		
		var pipelineNodes = CreatePipeline(definitionProvider, stepsSnapshot);
        RunPipeline(pipelineNodes, headersSnapshot, behaviorSnapshot);
	}
	
	private static IEnumerable<(IGenericTaskDefinition task, object? payload)> CreatePipeline(
		ITaskDefinitionProvider definitionProvider,
		AutomationStep[] steps)
	{
		foreach (var step in steps) {
			if (definitionProvider.TryGetDefinition(step.Type, out IGenericTaskDefinition? definition) &&
				definition is not null)
			{
				object? mappedPayload = MapTaskPayload(definition, step);
				yield return (definition, mappedPayload);
			}
			else {
				throw new InvalidOperationException($"No definition found for task type '{step.Type}'");
			}
		}
	}
	
	private static void RunPipeline(
		IEnumerable<(
			IGenericTaskDefinition task, 
			object? payload
		)> pipelineNodes,
		IReadOnlyDictionary<string, object?> headers,
		AutomationBehavior behavior)
	{
		var context = new AutomationContext()
		{
			Headers = headers.ToDictionary(entry => entry.Key, entry => entry.Value?.ToString() ?? ""),
			Behavior = behavior,
		};
		
		string? lastResult = null;
		bool? isNextCalled = null;
		
		void Next(AutomationContext context, string? currentResult)
		{
			lastResult = currentResult;
			isNextCalled = true;
		}
		
		foreach (var (task, payload) in pipelineNodes) {
			isNextCalled = false;
			task.Execute(Next, context, payload, lastResult);
			if (isNextCalled is false) break; 
		}
	}
	
	private static object? MapTaskPayload(IGenericTaskDefinition definition, AutomationStep step)
	{
		try {
			return definition.MapPayload(step.Payload);
		} catch(Exception ex) {
			throw new InvalidOperationException($"Failed to map payload for task type '{step.Type}'", ex);
		}
	}
}
