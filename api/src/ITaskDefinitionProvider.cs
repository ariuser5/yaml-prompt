namespace YamlPrompt.Api;

public interface ITaskDefinitionProvider
{
	bool TryGetDefinition(string typeKey, out IGenericTaskDefinition? definition);
}
