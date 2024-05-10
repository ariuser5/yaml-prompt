using WinTasks;
using YamlPrompt.Specs.AppInterface;

namespace YamlPrompt.Specs.ShellBatch;

public class BatchScriptTests : FileSystemDependentTestFixture
{
    private readonly AppTestingClient _app;
    
    public BatchScriptTests()
    {
        _app = new AppTestingClient();
        _app.TaskDefinitions.Add(new BatchTaskDefinition());
    }
    
    [Theory]
    [InlineData("- batch: 'echo {0} >> \"{1}\"'")]
    [InlineData("""
        - batch:
            command: 'echo {0} >> "{1}"'
        """)]
    [InlineData("""
        - type: batch
          command: 'echo {0} >> "{1}"'
        """)]
    [InlineData("""
        date: 2021-09-01
        author: John Doe
        steps:
          - batch:
              command: 'echo {0} >> "{1}"'
        """)]
    [InlineData("""
        date: 2021-09-01
        author: John Doe
        steps:
          - type: batch
            command: 'echo {0} >> "{1}"'
        """)]
    public void OneCommand_Inline_RunsTheCommand(string yamlInput)
    {
        // Arrange
        string expectedKey = Guid.NewGuid().ToString();
        string expectedFile = base.CreateFile("output.txt");
        string yamlCommand = string.Format(yamlInput, expectedKey, expectedFile);
        
        // Act
        _app.Execute(yamlCommand);
        
        // Assert
        Assert.True(File.Exists(expectedFile));
        Assert.Equal(expectedKey, File.ReadAllText(expectedFile).Trim());
    }
    
    [Theory]
    [InlineData("- batch: \"{script-file}\"")]
    [InlineData("""
        - batch:
            command: "{script-file}"
        """)]
    [InlineData("""
        - type: batch
          command: "{script-file}"
        """)]
    [InlineData("""
        date: 2021-09-01
        author: John Doe
        steps:
          - batch:
              command: "{script-file}"
        """)]
    [InlineData("""
        date: 2021-09-01
        author: John Doe
        steps:
          - type: batch
            command: "{script-file}"
        """)]
    public void OneCommand_AsScript_RunsTheCommand(string yamlInput)
    {
        // Arrange
        const string scriptTemplate = "echo {0} >> \"{1}\"";
        
        string expectedKey = Guid.NewGuid().ToString();
        string expectedFile = base.CreateFile("output.txt");
        string script = string.Format(scriptTemplate, expectedKey, expectedFile);
        string scriptFile = base.CreateFile("script.bat", script);
        string yamlCommand = yamlInput
            .Replace("{script-file}", scriptFile)
            .Replace(@"\", @"\\");                  // YAML required. `\` must be escaped.
        
        // Act
        _app.Execute(yamlCommand);
        
        // Assert
        Assert.True(File.Exists(expectedFile));
        Assert.Equal(expectedKey, File.ReadAllText(expectedFile).Trim());
    }
    
    [Theory]
    [InlineData("""
        - batch: 'echo 1 >> "{0}"'
        - batch:
            command: 'echo 2 >> "{0}"'
        - type: batch
          command: 'echo 3 >> "{0}"'
        """)]
    [InlineData("""
        date: 2021-09-01
        author: John Doe
        steps:
          - batch: 'echo 1 >> "{0}"'
          - batch:
              command: 'echo 2 >> "{0}"'
          - type: batch
            command: 'echo 3 >> "{0}"'
        """)]
    public void MultipleCommands_AsInline_RunsAllCommandsSequentially(string yamlInput)
    {
        // Arrange
        string outputFile = base.CreateFile("output.txt");
        string yamlCommand = string
            .Format(yamlInput, outputFile)
            .Replace(@"\", @"\\");          // YAML required. `\` must be escaped.
        
        // Act
        _app.Execute(yamlCommand);
        
        // Assert
        string[] actualValues = File.ReadAllLines(outputFile).Select(w => w.TrimEnd()).ToArray();
        
        Assert.Equal(3, actualValues.Length);
        Assert.Equal(["1", "2", "3"], actualValues);
    }
    
    [Theory]
    [InlineData("""
        - batch: "{0}"
        - batch:
            command: "{1}"
        - type: batch
          command: "{2}"
        """)]
    [InlineData("""
        date: 2021-09-01
        author: John Doe
        steps:
          - batch: "{0}"
          - batch:
              command: "{1}"
          - type: batch
            command: "{2}"
        """)]
    public void MultipleCommands_AsShellScripts_RunsAllCommandsSequentially(string yamlInput)
    {
        // Arrange
        string outputFile = base.CreateFile("output.txt");
        
        var scriptFile1 = base.CreateFile(
            fileName: "script1.bat", 
            content: $"echo 1 >> \"{outputFile}\"");
            
        var scriptFile2 = base.CreateFile(
            fileName: "script2.bat", 
            content: $"echo 2 >> \"{outputFile}\"");
            
        var scriptFile3 = base.CreateFile(
            fileName: "script3.bat", 
            content: $"echo 3 >> \"{outputFile}\"");
            
        var yamlCommand = string
            .Format(yamlInput, scriptFile1, scriptFile2, scriptFile3)
            .Replace(@"\", @"\\");
        
        // Act
        _app.Execute(yamlCommand);
        
        // Assert
        string[] actualValues = File.ReadAllLines(outputFile).Select(w => w.TrimEnd()).ToArray();
        
        Assert.Equal(3, actualValues.Length);
        Assert.Equal(["1", "2", "3"], actualValues);
    }
    
    [Theory]
    [InlineData("""
        - batch: "{0}"
        - batch:
            command: 'echo 2 >> "{1}"'
        - type: batch
          command: "{2}"
        """)]
    [InlineData("""
        date: 2021-09-01
        author: John Doe
        steps:
          - batch: "{0}"
          - batch:
              command: 'echo 2 >> "{1}"'
          - type: batch
            command: "{2}"
        """)]
    public void MultipleCommands_AsMixOffScriptsAndInlineCommands_RunsAllCommandsSequentially(string yamlInput)
    {
        // Arrange
        string outputFile = base.CreateFile("output.txt");
        
        var scriptFile1 = base.CreateFile(
            fileName: "script1.bat",
            content: $"echo 1 >> \"{outputFile}\"");
            
        var scriptFile2 = base.CreateFile(
            fileName: "script3.bat", 
            content: $"echo 3 >> \"{outputFile}\"");
        
        string yamlCommand = string
            .Format(yamlInput, scriptFile1, outputFile, scriptFile2)
            .Replace(@"\", @"\\");
        
        // Act
        _app.Execute(yamlCommand);
        
        // Assert
        string[] actualValues = File.ReadAllLines(outputFile).Select(w => w.TrimEnd()).ToArray();
        
        Assert.Equal(3, actualValues.Length);
        Assert.Equal(["1", "2", "3"], actualValues);
    }
    
    [Theory]
    [InlineData("""
        - batch: 'echo 1 >> "{0}"'
        - batch:
            command: "exit 1"
            continueOnError: false
        - type: batch
          command: 'echo 3 >> "{0}"'
        """)]
    [InlineData("""
        - batch: 'echo 1 >> "{0}"'
        - batch:
            command: 'cd /non-existent-random-folder_9204'
            continueOnError: false
        - type: batch
          command: 'echo 3 >> "{0}"'
        """)]
    public void MultipleCommands_WhenOneCommandWithDefaultBehaviorFails_ThrowsExceptionAndStopsExecution(
        string yamlInput)
    {
        // Arrange 
        string outputFile = base.CreateFile("output.txt");
        string yamlCommand = string
            .Format(yamlInput, outputFile)
            .Replace(@"\", @"\\");
        
        // Act
        _app.Execute(yamlCommand);
        
        // Assert
        string[] actualValues = File.ReadAllLines(outputFile).Select(w => w.TrimEnd()).ToArray();
        
        Assert.True(actualValues.Length == 1);  // Only the first command should run (and succeed)
        Assert.Equal(actualValues, ["1"]);
    }
    
    [Theory]
    [InlineData("""
        - batch: 'echo 1 >> "{0}"'
        - batch:
            command: "exit 1"
            continueOnError: true
        - type: batch
          command: 'echo 3 >> "{0}"'
        """)]
    [InlineData("""
        - batch: 'echo 1 >> "{0}"'
        - batch:
            command: "cd /non-existent-random-folder_9204"
            continueOnError: true
        - type: batch
          command: 'echo 3 >> "{0}"'
        """)]
    public void MultipleCommands_WhenOneCommandWithRecoveryConfigurationFails_ContinuesExecution(
        string yamlInput)
    {
        // Arrange
        string outputFile = base.CreateFile("output.txt");
        string yamlCommand = string
            .Format(yamlInput, outputFile)
            .Replace(@"\", @"\\");
        
        // Act
        _app.Execute(yamlCommand);
        
        // Assert
        string[] actualValues = File.ReadAllLines(outputFile).Select(w => w.TrimEnd()).ToArray();
        
        Assert.Equal(2, actualValues.Length);
        Assert.Equal(["1", "3"], actualValues);
    }
    
    [Fact]
    public void Command_CanPassValueToTheNextCommand()
    {
        // Arrange
        string expectedValue = Guid.NewGuid().ToString();
        string outputFile = base.CreateFile("output.txt");
        string yamlCommand = $@"
            - batch: 'echo 1 >> ""{{output-file}}""'
            - batch:
                command: 'echo {ShellTaskConstants.ResultCaptureVarName}={{expected-key}}'
            - type: batch
              command: 'echo {{expected-key}} >> ""{{output-file}}""'
        ".Replace("{output-file}", outputFile)
        .Replace("{expected-key}", expectedValue)
        .Replace(@"\", @"\\");
        
        // Act
        _app.Execute(yamlCommand);
        
        // Assert
        string[] actualValues = File.ReadAllLines(outputFile).Select(w => w.TrimEnd()).ToArray();
        
        Assert.Equal(2, actualValues.Length);
        Assert.Equal(["1", expectedValue], actualValues);
    }
}