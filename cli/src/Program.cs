using System.CommandLine;
using YamlPrompt.Api;
using YamlPrompt.Api.Serialization;
using YamlPrompt.Model;

var yamlFileArgument = new Argument<string>("yamlFile", description: "The YAML input string");

var rootCommand = new RootCommand
{
    yamlFileArgument
};

rootCommand.Description = "YAML Prompt CLI";
rootCommand.SetHandler(static (string yamlFile) =>
{
    var yamlInput = File.ReadAllText(yamlFile);
    var deserializer = new AutomationScriptDeserializer();
    var definitions = new[] { new FakeShellTaskDefinition() };
    var automationScript = deserializer.DeserializeYaml(yamlInput, definitions.Select(d => d.TypeKey).ToArray());

    AutomationScript.Run(automationScript, definitions);
}, yamlFileArgument);

return await rootCommand.InvokeAsync(args);

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
        Console.WriteLine(((object[])payload["shell"])[0]);
		    return "ABC";
    }
}