using YamlPrompt.Model;
using YamlPrompt.ExtensionSdk;

namespace YamlPrompt.Tasks;

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
		foreach (var (key, value) in payload)
			context.Items[key] = value;
			
		return null;
	}
}
