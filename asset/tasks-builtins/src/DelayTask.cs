using YamlPrompt.Model;
using YamlPrompt.Scripting.Core;
using YamlPrompt.Tasks.Sdk;

namespace YamlPrompt.Tasks.Builtins;

public class DelayTask : TaskDefinitionBase<string>
{
	public static class Template
	{
		public const string TypeKey = "delay";
	}

	public override string TypeKey => Template.TypeKey;

	public override string InterpretPayload(IReadOnlyDictionary<string, object?> fields)
	{
		var delay = fields[Template.TypeKey];
		if (delay is int delayMs) return delayMs.ToString(); 
		 
		return fields[Template.TypeKey] as string
			?? throw new ArgumentException($"Field '{Template.TypeKey}' is required.");
	}

	protected override string? Invoke(
		AutomationContext context,
		string payload,
		string? previousResult)
	{
		var delay = ParseDelay(payload, context, previousResult);
		Task.Delay(delay).Wait();
		return null;
	}
	
	private static int ParseDelay(string value, AutomationContext context, string? previousResult)
	{
		if (int.TryParse(value, out var delayMs))
			return delayMs;

		if (ScriptHelper.IsScript(value))
			value = ScriptHelper.ExtractScript(value);

		var objectValue = ScriptEvaluator.Evaluate<object>(value, context, previousResult);
		return int.Parse(objectValue.ToString() ?? "");
	}
}