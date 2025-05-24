using YamlPrompt.Model;
using YamlPrompt.Scripting.Core;
using YamlPrompt.Tasks.Sdk;

namespace YamlPrompt.Tasks.Builtins;

public class ContextTask : TaskDefinitionBase<Dictionary<string, object?>>
{
	public static class Template
	{
		public const string TypeKey = "context";
		public const string VariablesFieldName = "variables";
	}
	
	public override string TypeKey => Template.TypeKey;

	public override Dictionary<string, object?> InterpretPayload(
		IReadOnlyDictionary<string, object?> fields)
	{
		var vars = fields[Template.VariablesFieldName] as Dictionary<object, object?>
			?? throw new ArgumentException($"Field '{Template.VariablesFieldName}' is required.");
		
		return vars.ToDictionary(
			x => x.Key as string ?? throw new ArgumentException($"Key '{x.Key}' is not a string."),
			x => x.Value);
	}

	protected override string? Invoke(
		AutomationContext context,
		Dictionary<string, object?> payload,
		string? previousResult)
	{
		foreach (var (key, rawValue) in payload)
		{
			var value = rawValue;
			
			if (rawValue is string stringValue && ScriptHelper.IsScript(stringValue))
			{
				var script = ScriptHelper.ExtractScript(stringValue);
				value = ScriptEvaluator.Evaluate<object>(script, context, previousResult);
			}
				
			context.Items[key] = value;
		}
			
		return null;
	}
}
