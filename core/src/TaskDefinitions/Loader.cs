using System.Reflection;
using YamlPrompt.Model;

namespace YamlPrompt.Core.TaskDefinitions;

public interface ILoader
{
	List<ITaskDefinition> Load(string path);
}

public class Loader: ILoader
{
	public Action<string> MultipleDefinitionsFoundHandler { get; set; } 
		= taskKey => throw new InvalidOperationException(
			$"Multiple task definitions found for key '{taskKey}'");
	
	public List<ITaskDefinition> Load(string path)
	{
		// is file or directory?
		if (File.Exists(path)) {
			return InstantiateDefinitions([path]);
		} else if (Directory.Exists(path)) {
			var dlls = Directory.GetFiles(path, "*.dll");
			return InstantiateDefinitions(dlls);
		} else {
			throw new FileNotFoundException($"No task definitions found at '{path}'");
		}
	}
	
	public List<ITaskDefinition> Load(IEnumerable<string> paths)
	{
		return paths.Select(path => Load(path))
			.SelectMany(defs => defs)
			.ToList();
	}
	
	public List<ITaskDefinition> Load(Assembly assembly)
	{
		return InstantiateDefinitions([assembly]);
	}
	
	public List<ITaskDefinition> Load(IEnumerable<Assembly> assemblies)
	{
		return InstantiateDefinitions(assemblies);
	}
	
	private List<ITaskDefinition> InstantiateDefinitions(IEnumerable<string> sourceDlls)
	{
		var assemblies = sourceDlls.Select(Assembly.LoadFrom);
		return InstantiateDefinitions(assemblies);
	}
	
	private List<ITaskDefinition> InstantiateDefinitions(IEnumerable<Assembly> sourceAssemblies)
	{
		var defsGroupedByKey = sourceAssemblies
			.SelectMany(assembly => assembly.GetTypes()
				.Where(IsTaskDefinition)
				.Select(type => (ITaskDefinition)Activator.CreateInstance(type)!))
			.GroupBy(def => def.TypeKey);
		
		foreach (var group in defsGroupedByKey) {
			if (group.Count() > 1) {
				this.MultipleDefinitionsFoundHandler?.Invoke(group.Key);
			}
		}
		
		return defsGroupedByKey.Select(group => group.First()).ToList();
	}
	
	private static bool IsTaskDefinition(Type type)
	{
		return typeof(ITaskDefinition).IsAssignableFrom(type) && !type.IsAbstract;
	}
}