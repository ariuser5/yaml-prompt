using YamlPrompt.Model;

namespace YamlPrompt.Api;

public record AutomationTaskDefinitionSnapshot(
	string TypeKey,
	MatchingRule MatchingRule,
	Func<AutomationContext, object?, string[], object?> Implementation
);
