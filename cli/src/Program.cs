
// See https://aka.ms/new-console-template for more information

using System.Reflection;
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
var yamlCommands = Parser.ParseYaml("commands.yaml");
ExecuteCommands(yamlCommands, commands);

static void ExecuteCommands(Dictionary<string, string[]> yamlCommands, Dictionary<string, ICommand> commands)
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
