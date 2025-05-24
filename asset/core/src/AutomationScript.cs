using YamlPrompt.Model;

namespace YamlPrompt.Core;

public record AutomationScript
{
	public const string BehaviorFieldName = "behavior";
	public const string StepsFieldName = "steps";
	public const string StepTypeFieldName = "type";
	
	public Dictionary<string, object?> Headers { get; init; } = [];
	public AutomationBehavior? Behavior { get; init; } = null;
	public List<AutomationStep> Steps { get; init; } = [];
	
	public static int Run(AutomationScript instance, IEnumerable<ITaskDefinition> definitions)
	{
		var definitionsSnapshot = definitions.ToArray();
		var headersSnapshot = instance.Headers.ToDictionary(entry => entry.Key, entry => entry.Value);
		var behaviorSnapshot = instance.Behavior ?? new AutomationBehavior();
		var stepsSnapshot = instance.Steps.ToArray();
		
		var pipelineNodes = CreatePipeline(definitionsSnapshot, stepsSnapshot);
        return RunPipeline(pipelineNodes, headersSnapshot, behaviorSnapshot);
	}
	
	private static (ITaskDefinition task, object? payload)[] CreatePipeline(
		ITaskDefinition[] definitions,
		AutomationStep[] steps)
	{
		return steps.Select(step => {
			ITaskDefinition? definition = FindTaskDefinition(definitions, step.Type);
			if (definition is not null) {
				object? mappedPayload = step.ContextualizePayload(definition);
				return (definition, mappedPayload);
			} else {
				throw new InvalidOperationException($"No definition found for task type '{step.Type}'");
			}
		}).ToArray();
	}
	
	private static ITaskDefinition? FindTaskDefinition(
		IEnumerable<ITaskDefinition> definitions,
		string type)
	{
		try {
			return definitions.SingleOrDefault(def => def.TypeKey == type);
		} catch (InvalidOperationException) {
			throw new InvalidOperationException($"Multiple task definitions found for type '{type}'");
		}
	}

	private static int RunPipeline(
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
		int exitCode = 0;

		foreach (var (task, payload) in pipelineNodes)
		{
			var flowController = new FlowController()
			{
				ExceptionHandling = HandleException,
				ExitCode = exitCode,
			};

			try
			{
				task.Execute(flowController, context, payload, lastResult);
			}
			catch (Exception ex)
			{
				exitCode = 1;
				flowController.ExceptionHandling.Invoke(ex);
			}

			if (flowController.ExitCode != 0 &&
				flowController.AllowContinuationOnFailure is false)
			{
				break;
			}

			if (flowController.AbortRequested) break;

			lastResult = flowController.ReturnValue;
		}
		
		return exitCode;
	}

	private static void HandleException(Exception ex)
	{
		Console.WriteLine(ex.Message);
		Console.WriteLine(ex.StackTrace);
	}
	
    private class FlowController : IFlowController
	{
		public int ExitCode { get; set; } = 0;
		public bool AbortRequested { get; set; }
		public string? ReturnValue { get; set; } = null;
		public bool AllowContinuationOnFailure { get; set; } = false;
		public Action<Exception>? ExceptionHandling { get; set; }
	}
}
