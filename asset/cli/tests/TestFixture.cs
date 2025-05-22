using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using YamlPrompt.ExtensionSdk;
using YamlPrompt.Model;

namespace YamlPrompt.Cli.Tests;

[CollectionDefinition("Test Collection")]
public class TestCollection : ICollectionFixture<TestFixture> { }

public class TestFixture : IAsyncLifetime
{
	public readonly string TestDefinitionsDir = Path.Combine(Directory.GetCurrentDirectory(), "resources", "defs");
	public readonly string RegistryDir = Path.Combine(Directory.GetCurrentDirectory(), "registry");
	
	public Task InitializeAsync()
	{
		CreateTaskDefinitionsRegistry();
		return Task.CompletedTask;
	}
 
	public Task DisposeAsync()
	{ 
		if (Directory.Exists(RegistryDir))
		{
			Directory.Delete(RegistryDir, true);
		}
		
		return Task.CompletedTask;
	}
	
	private void CreateTaskDefinitionsRegistry()
	{
		if (Directory.Exists(RegistryDir) == false)
		{
			Directory.CreateDirectory(RegistryDir);
		}
	
		var sourceFiles = Directory.GetFiles(TestDefinitionsDir, "*.cs");
		foreach (var file in sourceFiles)
		{
			var outputDll = Path.Combine(RegistryDir, Path.GetFileNameWithoutExtension(file)) + ".dll";
			CompileAssemblyFromSourceFile(file, outputDll);
		}
	}
	
	private static void CompileAssemblyFromSourceFile(
		string sourceFile,
		string outputDll)
	{
		var code = File.ReadAllText(sourceFile);
		var syntaxTree = CSharpSyntaxTree.ParseText(code);

		var runtimeDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
		var references = new List<MetadataReference>
		{
			MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(ITaskDefinition).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(TaskDefinitionBase).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(IReadOnlyDictionary<,>).Assembly.Location),
			MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Runtime.dll")),
        	MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "netstandard.dll")),
		};

		var assemblyName = Path.GetFileNameWithoutExtension(outputDll);
		var compilation = CSharpCompilation.Create(
			assemblyName,
			[syntaxTree],
			references,
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

		var emitResult = compilation.Emit(outputDll);

		if (!emitResult.Success)
		{
			var errors = string.Join(Environment.NewLine, emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
			throw new Exception($"Error compiling {sourceFile}:{Environment.NewLine}{errors}");
		}
	}
}