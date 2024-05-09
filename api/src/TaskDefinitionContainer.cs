using YamlPrompt.Model;

namespace YamlPrompt.Api;

public class TaskDefinitionContainer(
	IGenericTaskDefinitionFactory genericTaskDefinitionFactory
) : ITaskDefinitionProvider
{
	private readonly Dictionary<string, IGenericTaskDefinition> _definitions = [];
	
	private readonly IGenericTaskDefinitionFactory _genericTaskDefinitionFactory = genericTaskDefinitionFactory;


    public IReadOnlyCollection<string> TypeKeys => _definitions.Keys;
	
	public IReadOnlyCollection<IGenericTaskDefinition> Definitions => _definitions.Values;
	
	public void Register<T>(TaskDefinition<T> definition)
	{
		_definitions[definition.TypeKey] = _genericTaskDefinitionFactory.CreateFrom(definition);
	}
	
	public void Unregister(string typeKey)
	{
		_definitions.Remove(typeKey);
	}
	
	public bool TryGetDefinition(string typeKey, out IGenericTaskDefinition? definition)
	{
		return _definitions.TryGetValue(typeKey, out definition);
	}
	
	public bool ContainsDefinition(string typeKey)
	{
		return _definitions.ContainsKey(typeKey);
	}
}
