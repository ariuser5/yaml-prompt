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
	
	public static void Run(AutomationScript instance, IEnumerable<ITaskDefinition> definitions)
	{
		var definitionsSnapshot = definitions.ToArray();
		var headersSnapshot = instance.Headers.ToDictionary(entry => entry.Key, entry => entry.Value);
		var behaviorSnapshot = instance.Behavior ?? new AutomationBehavior();
		var stepsSnapshot = instance.Steps.ToArray();
		
		var pipelineNodes = CreatePipeline(definitionsSnapshot, stepsSnapshot);
        RunPipeline(pipelineNodes, headersSnapshot, behaviorSnapshot);
	}
	
	private static (ITaskDefinition task, object? payload)[] CreatePipeline(
		ITaskDefinition[] definitions,
		AutomationStep[] steps)
	{
		return steps.Select(
			step => {
				ITaskDefinition? definition = definitions.FirstOrDefault(def => def.TypeKey == step.Type);
				if (definition is not null) {
					object? mappedPayload = InterpretStepPayload(definition, step);
					return (definition, mappedPayload);
				} else {
					throw new InvalidOperationException($"No definition found for task type '{step.Type}'");
				}
			}
		).ToArray();
	}
	
	private static void RunPipeline(
		IEnumerable<(
			ITaskDefinition task, 
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
		
		foreach (var (task, payload) in pipelineNodes) {
			var flowController = new FlowController()
			{
				ExceptionHandling = (ex) => {
					Console.WriteLine(ex.Message);
					Console.WriteLine(ex.StackTrace);
				}
			};
			
			try {
				task.Execute(flowController, context, payload, lastResult);
			} catch (Exception ex) {
				flowController.ExceptionHandling.Invoke(ex);
			}
			
			if (flowController.ExitCode != 0) {
				if (flowController.AllowContinuationOnFailure is false) break;
			}
			
			lastResult = flowController.ReturnValue;
		}
	}
	
	private static object? InterpretStepPayload(ITaskDefinition definition, AutomationStep step)
	{
		try {
			return definition.InterpretPayload(step.Payload);
		} catch(Exception ex) {
			throw new InvalidOperationException($"Failed to interpret payload for task type '{step.Type}'", ex);
		}
	}

    private class FlowController : IFlowController
    {
        public int ExitCode { get; set; } = 0;
        public string? ReturnValue { get; set; } = null;
        public bool AllowContinuationOnFailure { get; set; } = false;
        public Action<Exception>? ExceptionHandling { get; set; }
    }
}
