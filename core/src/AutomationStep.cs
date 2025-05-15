using YamlPrompt.Model;

namespace YamlPrompt.Core;

public record AutomationStep(
	string Type,
	Dictionary<string, object?> Payload
)
{
	public object? ContextualizePayload(ITaskDefinition definition)
	{
		try {
			return definition.InterpretPayload(this.Payload);
		} catch(Exception ex) {
			throw new InvalidOperationException($"Failed to interpret payload for task type '{this.Type}'", ex);
		}
	}
}