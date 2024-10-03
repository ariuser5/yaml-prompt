using System.Reflection;
using YamlPrompt.Model;

namespace YamlPrompt.Cli.Registry;
public class DefinitionsLoader
{
	private const string registryDirName = "registry";
	
	public static List<ITaskDefinition> LoadTaskDefinitions()
	{
		var workingDir = Directory.GetCurrentDirectory();
		var registryDir = Path.Combine(workingDir, registryDirName);
		
		if (Directory.Exists(registryDir) == false) {
			Directory.CreateDirectory(registryDir);
		}
		
		var dlls = Directory.GetFiles(registryDir, "*.dll");
		
		if (dlls.Length == 0) {
			throw new InvalidOperationException("No task definitions found");
		}
		
		var defsGroupedByKey = dlls.Select(Assembly.LoadFrom)
			.SelectMany(assembly => assembly.GetTypes().Where(IsTaskDefinition)
			.Select(type => (ITaskDefinition)Activator.CreateInstance(type)!))
			.GroupBy(def => def.TypeKey);
		
		foreach (var group in defsGroupedByKey) {
			if (group.Count() > 1) {
				throw new InvalidOperationException($"Multiple task definitions found for key '{group.Key}'");
			}
		}
		
		return defsGroupedByKey.Select(group => group.First()).ToList();
	}
	
	private static bool IsTaskDefinition(Type type)
	{
		return typeof(ITaskDefinition).IsAssignableFrom(type) && !type.IsAbstract;
	}
}