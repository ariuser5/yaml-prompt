using YamlPrompt.Model;

namespace defs1;

public class Class1 : TaskDefinitionBase<IReadOnlyDictionary<string, object?>>
{
	public override string TypeKey => "def1";

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
		Console.WriteLine(((object[])payload["def1"])[0]);
		Console.WriteLine("Previous result: " + previousResult);
		return "abc from def1";
	}
}
