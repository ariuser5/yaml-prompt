using YamlPrompt.Model;

namespace YamlPrompt.Api;

public interface IGenericTaskDefinitionFactory
{
	IGenericTaskDefinition CreateFrom<T>(TaskDefinition<T> definition);
}

public class GenericTaskDefinitionFactory : IGenericTaskDefinitionFactory
{
	public IGenericTaskDefinition CreateFrom<T>(TaskDefinition<T> definition)
	{
		return new GenericTaskDefinition()
		{
			ConcreteTaskDefinition = definition,
			TypeKey = definition.TypeKey,
			ExecuteImplementation = (next, context, payload, previousResult) =>
			{
				definition.Execute(next, context, (T)payload!, previousResult);
			},
			MapPayloadImplementation = fields => definition.MapPayload(fields)
		};
	}
}
