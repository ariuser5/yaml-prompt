using YamlPrompt.Tasks.Sdk;
using YamlPrompt.Model;

namespace YamlPrompt.Specs;

class FakeTaskDefinition(
	Action<string> onInvoke
) : TaskDefinitionBase
{
	private readonly Action<string> _onInvoke = onInvoke;
	
	public override string TypeKey => "TestShell";
	
	protected override string? Invoke(
		AutomationContext context,
		IReadOnlyDictionary<string, object?> payload,
		string? previousResult)
	{
        if (payload.TryGetValue("input", out object? input) ||
            payload.TryGetValue(TypeKey, out input)
        ) {
            var inputArg
                = input is string v ? v
                : input is IEnumerable<object> inputAsList ? inputAsList.FirstOrDefault() as string
				: input is object[] inputAsArray ? inputAsArray[0] as string
                : null;

            if (inputArg != null) {
                _onInvoke(inputArg);
                return inputArg;
            }
        }

        throw new InvalidDataException(
			$"Input is missing or has invalid data type." +
			$"Expected '{typeof(string)}' or '{typeof(List<string>)}'.");
	}
}