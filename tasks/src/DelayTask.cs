using YamlPrompt.Model;

namespace YamlPrompt.Tasks;

public class DelayTask : TaskDefinitionBase<string>
{
	public static class Template
	{
		public const string TypeKey = "delay";
	}
	
	public override string TypeKey => Template.TypeKey;

	public override string InterpretPayload(IReadOnlyDictionary<string, object?> fields)
	{
		var input = fields[Template.TypeKey] as string
			?? throw new ArgumentException($"Field '{Template.TypeKey}' is required.");
		
		return !int.TryParse(input, out var _)
			? ScriptHelper
			: input;
	}

	protected override string? Invoke(
		AutomationContext context,
		string payload,
		string? previousResult)
	{
		if (int.TryParse(payload, out var delay))
		{
			Task.Delay(delay).Wait();
			return null;
		}
		
		var delayMs = ScriptEvaluator.Evaluate<int>(
			context,
			payload,
			previousResult);
			
		Task.Delay(delayMs).Wait();
		return null;
	}
}