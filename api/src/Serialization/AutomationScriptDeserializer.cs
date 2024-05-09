
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlPrompt.Model;

namespace YamlPrompt.Api.Serialization;

public interface IAutomationScriptDeserializer
{
	AutomationScript DeserializeYaml(string yaml);
}

public class AutomationScriptDeserializer : IAutomationScriptDeserializer
{
	public AutomationScript DeserializeYaml(string yaml)
	{
		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();
			
		var deserialized = deserializer.Deserialize<Dictionary<string, object?>>(yaml);
		var headers = ExtractHeaders(deserialized)!;
		var behavior = ExtractBehavior(deserialized);
		var steps = ExtractSteps(deserialized);
		
		return new AutomationScript()
		{
			Headers = headers,
			Behavior = behavior,
			Steps = steps
		};
	}
	
	private static Dictionary<string, object?> ExtractHeaders(Dictionary<string, object?> deserialized)
	{
		var headers = new Dictionary<string, object?>();
		
		foreach (var (key, value) in deserialized) {
			if (key != AutomationScript.BehaviorFieldName && 
				key != AutomationScript.StepsFieldName)
			{
				headers[key] = value;
			}
		}
		
		return headers;
	}
	
	private static AutomationBehavior ExtractBehavior(Dictionary<string, object?> deserialized)
	{
		var defaultBehavior = new AutomationBehavior();
		
		if (!deserialized.TryGetValue(AutomationScript.BehaviorFieldName, out object? behaviorDeserialized))
			return defaultBehavior;
		
		if (behaviorDeserialized is not Dictionary<string, object> behaviorMapping)
			throw new InvalidDataException("Behavior is not a mapping");
		
		var continueOnException = defaultBehavior.ContinueOnException;
		
		if (behaviorMapping.TryGetValue(
				key: nameof(AutomationBehavior.ContinueOnException), 
				value: out object? continueOnExceptionDeserialized)
		) {
			if (continueOnExceptionDeserialized is not bool continueOnExceptionAsBool) {
				throw new InvalidDataException($"{nameof(AutomationBehavior.ContinueOnException)} is not a boolean");
			}
			
			continueOnException = continueOnExceptionAsBool;
		}
		
		return defaultBehavior with
		{
			ContinueOnException = continueOnException
		};
	}
	
	private static List<AutomationStep> ExtractSteps(Dictionary<string, object?> deserialized)
	{
		var hasSteps = deserialized.TryGetValue(AutomationScript.StepsFieldName, out object? steps);
		
		if (!hasSteps)
			throw new InvalidDataException("Script is missing steps");
		
		if (steps is not List<object>)
			throw new InvalidDataException("Steps is not a list");
		
		var stepsList = (List<object>)steps;
		
		var automationSteps = new List<AutomationStep>();
		
		foreach (var step in stepsList)
		{
			var automationStep = ExtractStep((Dictionary<string, object?>)step);	
			automationSteps.Add(automationStep);
		}
		
		return automationSteps;
	}
	
	private static AutomationStep ExtractStep(Dictionary<string, object?> step)
	{
		var hasType = step.TryGetValue(AutomationScript.StepTypeFieldName, out object? type);
		
		if (!hasType)
			throw new InvalidDataException("Step is missing the type field");
		
		if (type is not string)
			throw new InvalidDataException("Unknown step data type");
		
		return new AutomationStep((string)type, step);
	}
}
