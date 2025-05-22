using YamlPrompt.Model;
using YamlPrompt.ExtensionSdk;

namespace defs2;
public class EchoRef : TaskDefinitionBase
{
	public override string TypeKey => nameof(EchoRef).ToLowerInvariant();

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
		var suffix = payload["suffix"];
		Console.WriteLine(suffix + previousResult);
		return null;
	}
}