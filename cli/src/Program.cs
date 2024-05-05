
// See https://aka.ms/new-console-template for more information

using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlPrompt.Cli;

// Load all command classes
var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
	.Where(t => t.GetInterfaces().Contains(typeof(ICommand)));

// Instantiate all command classes and store them in a dictionary
var commands = new Dictionary<string, ICommand>();
foreach (var type in commandTypes)
{
	var command = Activator.CreateInstance(type) as ICommand;
	if (command != null)
		commands.Add(command.Key, command);
}

// Parse the YAML file and execute the commands
var yamlCommands = ParseYaml("commands.yaml");
ExecuteCommands(yamlCommands, commands);

Dictionary<string, string[]> ParseYaml(string filePath)
{
	var input = File.ReadAllText(filePath);
	var deserializer = new DeserializerBuilder()
		.WithNamingConvention(CamelCaseNamingConvention.Instance)
		.Build();

	var yamlObject = deserializer.Deserialize<Dictionary<string, string[]>>(input);
	return yamlObject;
}

void ExecuteCommands(Dictionary<string, string[]> yamlCommands, Dictionary<string, ICommand> commands)
{
	foreach (var yamlCommand in yamlCommands)
	{
		if (commands.TryGetValue(yamlCommand.Key, out var command))
		{
			command.Execute(yamlCommand.Value);
		}
		else
		{
			Console.WriteLine($"Unknown command: {yamlCommand.Key}");
		}
	}
}
