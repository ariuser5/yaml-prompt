# yaml-prompt
Reads and executes commands taking YAML input

To run using cli support, the cli project needs to be built first.
The output executable is called `ysap.exe`.

`ysap.exe` requires a file path as an argument. The file should contain a YAML string with a `steps` key. The value of the `steps` key should be a list of task definitions.
Each task definition is the declarative representation of its corresponding c# implementation.

The following is an example of a YAML file that can be used as input to `ysap.exe`:

```yaml
steps:
  - type: "PrintHello"
    message: "Hello World!"
  - type: "PrintBye"
    message: "Have a good day!"
```

The implementation of each task must be a class that implements the `YamlPrompt.Model.ITaskDefinition` interface and the class must then be assembled into a dll file that is placed in the "registry" subfolder where `ysap.exe` is located.
(this is a temporary solution until a more sophisticated way of loading assemblies is implemented)

For future, one better approach would be that the ysap tool would be installed on the system and the installation would assign an environment variable that would point to the folder where the assemblies are located. This way, the tool would be able to load the assemblies from that folder without the need to copy them to the same folder where the ysap tool is located.

E.g.: `YSAP_REGISTRY_PATH=C:\Program Files\YamlPrompt\Registry`

# Solution Notes
This solution contains custom code analyzers that are used to analyze the code and provide suggestions for improvements. The analyzers are implemented as Roslyn analyzers and can be used in any C# project.
The analyzers are implemented in the `development/code-analyzer` project and can be used in any C# project by adding a reference to the analyzer package.

The analyzers enforce the following rules:
- All tests should have a category attribute. This is done by annotating the test class or the test method with the xUnit 'Trait' attribute, where the first parameter must be "Category" and the second must be one of the available test categories (See `./development/dev-rules/TestCategoryRules/cs`).

To use the analyzer in your project, add a NuGet reference to the analyzer package.

If this is the first time you are using the analyzer, you need to build the package from the source code and push it to a nuget local/public feed.

## Push the analyzer nuget package to a local nuget feed
1. Create a local nuget feed folder. This can be any folder on your machine. For example, `C:\nuget-feed`.
2. Open a command prompt (or powershell) and add the local nuget feed to the nuget sources:
```cmd / pwsh
dotnet nuget add source C:\nuget-feed
```
3. Once local feed is created, you can build the nuget package from the source code and push it to the local feed. (see README.md in the `development/code-analyzer` project for more details)