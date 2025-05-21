using YamlPrompt.Model;

namespace defs1;
public class FwdRef : TaskDefinitionBase
{
	public override string TypeKey => nameof(FwdRef).ToLowerInvariant();

	public override IReadOnlyDictionary<string, object?> InterpretPayload(
		IReadOnlyDictionary<string, object?> fields)
	{
		return fields;
	}

	protected override string? Invoke(
		AutomationContext context,
		IReadOnlyDictionary<string, object?> payload,
		string? previousResult)
	{
		var value = ((object[])payload[TypeKey])[0];
		return value?.ToString() ?? throw new ArgumentNullException(nameof(value));
	}
}
