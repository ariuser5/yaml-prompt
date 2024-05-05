using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace YamlPrompt.Cli;

public static class Parser {
	public static Dictionary<string, string[]> ParseYaml(string filePath)
	{
		var input = File.ReadAllText(filePath);
		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();

		try {
			var yamlObject = deserializer.Deserialize<Dictionary<string, string[]>>(input);
			return yamlObject;
		} catch (YamlException yamlException) {
			string wrappingExceptionMessage = $"Error parsing YAML: {yamlException.Message}";
			Console.WriteLine(wrappingExceptionMessage);
			throw new Exception(wrappingExceptionMessage, yamlException);
		}
	}	
}
