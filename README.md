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

E.g.: YSAP_REGISTRY_PATH=C:\Program Files\YamlPrompt\Registry
