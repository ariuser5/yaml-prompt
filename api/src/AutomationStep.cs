
namespace YamlPrompt.Api;

public record AutomationStep(
	string Type,
	Dictionary<string, object?> Payload
);