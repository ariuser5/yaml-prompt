using System.CommandLine;
using System.Runtime.CompilerServices;
using YamlPrompt.Api;
using YamlPrompt.Api.Serialization;
using YamlPrompt.Cli.Registry;

var yamlFileArgument = new Argument<string>("yamlFile", description: "The YAML input string");

var rootCommand = new RootCommand
{
    yamlFileArgument
};

rootCommand.Description = "YAML Prompt CLI";
rootCommand.SetHandler(static yamlFile =>
{
    var yamlInput = File.ReadAllText(yamlFile);
    var deserializer = new AutomationScriptDeserializer();
    var definitions = DefinitionsLoader.LoadTaskDefinitions();
    var knownTypes = definitions.Select(d => d.TypeKey).ToArray();
    var automationScript = deserializer.DeserializeYaml(yamlInput, knownTypes);

    AutomationScript.Run(automationScript, definitions);
}, yamlFileArgument);

return await rootCommand.InvokeAsync(args);
