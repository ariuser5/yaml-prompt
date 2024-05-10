using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlPrompt.Model;

namespace YamlPrompt.Api.Serialization;

public interface IAutomationScriptDeserializer
{
	AutomationScript DeserializeYaml(string yaml, string[] knownTypes);
}

public class AutomationScriptDeserializer : IAutomationScriptDeserializer
{
	public AutomationScript DeserializeYaml(string yaml, string[] knownTypes)
	{
		if (string.IsNullOrEmpty(yaml))
			throw new ArgumentException("YAML is empty", nameof(yaml));
		
		var knownTypesSnapshot = knownTypes.ToArray();
		
		if (knownTypesSnapshot.Length != knownTypesSnapshot.Distinct().Count())
			throw new ArgumentException("Duplicate known types", nameof(knownTypes));
		
		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();
			
		var deserialized = DeserializeYaml(deserializer, yaml);
		var headers = ExtractHeaders(deserialized)!;
		var behavior = ExtractBehavior(deserialized);
		var steps = ExtractSteps(deserialized, knownTypesSnapshot);
		
		return new AutomationScript()
		{
			Headers = headers,
			Behavior = behavior,
			Steps = steps
		};
	}
	
	private static Dictionary<string, object?> DeserializeYaml(
		IDeserializer deserializer, 
		string yaml)
	{
		bool isSequence = yaml.Trim().StartsWith('-');
		
		return isSequence
			? DeserializeFromSequence(deserializer, yaml)
			: deserializer.Deserialize<Dictionary<string, object?>>(yaml);
	}
	
	private static Dictionary<string, object?> DeserializeFromSequence(
		IDeserializer deserializer,
		string yaml)
	{
		var tasks = deserializer.Deserialize<List<object>>(yaml);

		return new Dictionary<string, object?>()
		{
			[AutomationScript.StepsFieldName] = tasks
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
	
	private static List<AutomationStep> ExtractSteps(
		Dictionary<string, object?> deserialized,
		string[] knownTypes)
	{
		var hasSteps = deserialized.TryGetValue(AutomationScript.StepsFieldName, out object? steps);
		
		if (!hasSteps)
			throw new InvalidDataException("Automation script is missing steps");
		
		if (steps is not List<object>)
			throw new InvalidDataException("Steps is not a list");
		
		var stepsList = (List<object>)steps;
		
		var automationSteps = new List<AutomationStep>();
		
		foreach (var step in stepsList)
		{
			var automationStep = ExtractStep((Dictionary<object, object?>)step, knownTypes);	
			automationSteps.Add(automationStep);
		}
		
		return automationSteps;
	}
	
	private static AutomationStep ExtractStep(Dictionary<object, object?> step, string[] knownTypes)
	{
		var hasType = step.TryGetValue(AutomationScript.StepTypeFieldName, out object? type);
		
		if (!hasType) {
			if (step.Count == 1) {
				var stepAlias = (string)step.Keys.First();
				if (knownTypes.Contains(stepAlias)) {
					return ExtractAliasedStep(stepAlias, step[stepAlias]);
				}
			}
			
			throw new InvalidDataException("Step is missing the 'type' field");
		}
		
		if (type is not string)
			throw new InvalidDataException("Unknown step data type");
		
		var mapping = step.ToDictionary(
			entry => (string)entry.Key,
			entry => entry.Value
		);
		
		return new AutomationStep((string)type, mapping);
	}
	
	private static AutomationStep ExtractAliasedStep(string alias, object? payload)
	{
		if (payload is Dictionary<object, object?> mapping) {
			return new AutomationStep(
				Type: alias, 
				Payload: mapping.ToDictionary(w => (string)w.Key, w => w.Value)
			);
		} else if (payload is List<object?> items) {
			return new AutomationStep(
				Type: alias, 
				Payload: new Dictionary<string, object?>() {{ alias, items.ToArray() }}
			);
		} else if (IsScalar(payload)) {
			return new AutomationStep(
				Type: alias, 
				Payload: new Dictionary<string, object?>() {{ alias, payload }}
			);
		} else {
			throw new InvalidDataException("Aliased step is not a mapping");
		}
	}
	
	private static bool IsScalar(object? obj)
	{
		return obj is string || obj is bool || obj is int || obj is float || obj is double;
	}
}
