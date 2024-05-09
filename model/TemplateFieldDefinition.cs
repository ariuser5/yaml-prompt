namespace YamlPrompt.Model;

public record TemplateFieldDefinition(
	string Name,
	bool IsRequired = false,
	string? ValueRegexPattern = null
);
