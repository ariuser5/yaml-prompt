using YamlPrompt.Model;

namespace defs2;

public class Class1 : TaskDefinitionBase<IReadOnlyDictionary<string, object?>>
{
	public override string TypeKey => "def2";

	public override IReadOnlyDictionary<string, object?> InterpretPayload(
		IReadOnlyDictionary<string, object?> fields)
	{
		return fields;
	}

	protected override string? Invoke(
		AutomationContext context,
		IReadOnlyDictionary<string, object?> payload,
		string? previousResult)
	{
		Console.WriteLine(payload["input"]);
		Console.WriteLine("Previous result: " + previousResult);
		return "abc from def2";
	}
}
