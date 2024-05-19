using YamlPrompt.Api;
using YamlPrompt.Api.Serialization;
using YamlPrompt.Model;

namespace YamlPrompt.Specs.AppInterface;

public class AppTestingClient
{
	public List<ITaskDefinition> TaskDefinitions { get; set; } = [];
	
	public virtual void Execute(string yaml)
	{
		var deserializer = new AutomationScriptDeserializer();
		var taskDefinitions = TaskDefinitions.ToArray();
		var automationScript = deserializer.DeserializeYaml(
			yaml, 
			knownTypes: taskDefinitions.Select(d => d.TypeKey).ToArray());

		AutomationScript.Run(automationScript, taskDefinitions);
	}
}