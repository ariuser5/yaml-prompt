// See https://aka.ms/new-console-template for more information

using YamlPrompt.Api;
using YamlPrompt.Api.Serialization;
using YamlPrompt.Model;

const string yaml = @"
date: today
author: John doe
steps:
  - shell: 
    - ""echo Hello, World!""
";


var deserializer = new AutomationScriptDeserializer();
var definitions = new[] { new FakeShellTaskDefinition() };
var automationScript = deserializer.DeserializeYaml(yaml, definitions.Select(d => d.TypeKey).ToArray());

AutomationScript.Run(automationScript, definitions);

class FakeShellTaskDefinition : TaskDefinitionBase<IReadOnlyDictionary<string, object?>>
{
    public override string TypeKey => "shell";

    public override IReadOnlyDictionary<string, object?> InterpretPayload(
		IReadOnlyDictionary<string, object?> fields)
    {
        return fields;
    }

    protected override string? Invoke(
		AutomationContext context,
		IReadOnlyDictionary<string,
		object?> payload,
		string? previousResult)
    {
        Console.WriteLine(payload["input"]);
		    return "ABC";
    }
}