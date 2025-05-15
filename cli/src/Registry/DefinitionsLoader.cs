using YamlPrompt.Api.TaskDefinitions;
using YamlPrompt.Model;

namespace YamlPrompt.Cli.Registry;

public class DefinitionsLoader
{
	private const string RegistryDirName = "registry";

	public static List<ITaskDefinition> LoadTaskDefinitions()
	{
		var registryDir = GetRegistryDirectory();
		var loader = new Loader()
		{
			MultipleDefinitionsFoundHandler = HandleMultipleDefinitionsFound
		};
		 
		var definitions = loader.Load(registryDir);
		
		if (definitions.Count == 0)
			Console.WriteLine("No task definitions found.");
		
		return definitions;
	}
	
	private static string GetRegistryDirectory()
	{
		var workingDir = Directory.GetCurrentDirectory();
		var registryDir = Path.Combine(workingDir, RegistryDirName);
		
		if (Directory.Exists(registryDir) == false) {
			Directory.CreateDirectory(registryDir);
		}
		return registryDir;
	}
	
	private static void HandleMultipleDefinitionsFound(string taskKey)
	{
		Console.WriteLine($"Multiple task definitions found for key '{taskKey}'");
	}
}