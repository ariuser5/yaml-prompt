using Moq;
using YamlPrompt.Specs.AppInterface;

namespace YamlPrompt.Specs.ShellCommands;

public class ShellCommandsTests : FileSystemDependentTestFixture
{
    private readonly Mock<AppTestingController> _app;
    
    public ShellCommandsTests()
    {
        _app = new Mock<AppTestingController>
        {
            CallBase = true
        };
    }
    
    [Theory]
    [InlineData("- shell: \"echo '{0}' >> '{1}'\"")]
    [InlineData(@"
        - shell:
            inputType: command
            input: ""echo '{0}' >> '{1}""'
    ")]
    [InlineData(@"
        - type: shell
          inputType: command
          input: ""echo '{0}' >> '{1}""'
    ")]
    [InlineData(@"
        date: 2021-09-01
        author: John Doe
        tasks:
          - shell:
              inputType: command
              input: ""echo '{0}' >> '{1}""'
    ")]
    [InlineData(@"
        date: 2021-09-01
        author: John Doe
        tasks:
          - type: shell
            inputType: command
            input: ""echo '{0}' >> '{1}""'
    ")]
    public void OneCommand_WithInlineShellCommand_RunsTheCommand(string yamlInput)
    {
        // Arrange
        string expectedKey = Guid.NewGuid().ToString();
        string expectedFile = base.CreateFile("output.txt");
        string yamlCommand = string.Format(yamlInput, expectedKey, expectedFile);
        
        // Act
        _app.Object.Execute(yamlCommand);
        
        // Assert
        Assert.True(File.Exists(expectedFile));
        Assert.Equal(expectedKey, File.ReadAllText(expectedFile));
    }
    
    [Theory]
    [InlineData("- shell: {script-file}")]
    [InlineData(@"
        - shell:
            inputType: script
            input: '{script-file}'
    ")]
    [InlineData(@"
        - type: shell
          inputType: script
          input: '{script-file}'
    ")]
    [InlineData(@"
        date: 2021-09-01
        author: John Doe
        tasks:
          - shell:
              inputType: script
              input: '{script-file}'
    ")]
    [InlineData(@"
        date: 2021-09-01
        author: John Doe
        tasks:
          - type: shell
            inputType: script
            input: '{script-file}'
    ")]
    public void OneCommand_WithScript_RunsTheCommand(string yamlInput)
    {
        // Arrange
        const string scriptTemplate = "\"echo '{0}' >> '{1}'\"";
        
        string expectedKey = Guid.NewGuid().ToString();
        string expectedFile = base.CreateFile("output.txt");
        string script = string.Format(scriptTemplate, expectedKey, expectedFile);
        string scriptFile = base.CreateFile("script.sh", script);
        string yamlCommand = yamlInput.Replace("{script-file}", scriptFile);
        
        // Act
        _app.Object.Execute(yamlCommand);
        
        // Assert
        Assert.True(File.Exists(expectedFile));
        Assert.Equal(expectedKey, File.ReadAllText(expectedFile));
    }
    
    [Theory]
    [InlineData(@"
        - shell: ""echo '1' >> '{0}'""
        - shell:
            input: ""echo '2' >> '{0}'""
        - type: shell
          inputType: command
          input: ""echo '3' >> '{0}'""
    ")]
    [InlineData(@"
        date: 2021-09-01
        author: John Doe
        tasks:
          - shell: ""echo '1' >> '{0}'""
          - shell:
              inputType: command
              input: ""echo '2' >> '{0}'""
          - type: shell
            input: ""echo '3' >> '{0}'""
    ")]
    public void MultipleCommands_AsInlineShellCommands_RunsAllCommandsSequentially(string yamlInput)
    {
        // Arrange
        string outputFile = base.CreateFile("output.txt");
        string yamlCommand = string.Format(yamlInput, outputFile);
        
        File.WriteAllText(outputFile, "");
        
        // Act
        _app.Object.Execute(yamlCommand);
        
        // Assert
        string[] actualValues = File.ReadAllLines(outputFile);
        
        Assert.Equal(3, actualValues.Length);
        Assert.Equal(["1", "2", "3"], actualValues);
    }
    
    [Theory]
    [InlineData(@"
        - shell: ""script1.sh""
        - shell:
            inputType: script
            input: ""script2.sh""
        - type: shell
          inputType: script
          input: ""script3.sh""
    ")]
    [InlineData(@"
        date: 2021-09-01
        author: John Doe
        tasks:
          - shell: ""script1.sh""
          - shell:
              inputType: script
              input: ""script2.sh""
          - type: shell
            inputType: script
            input: ""script3.sh""
    ")]
    public void MultipleCommands_AsShellScripts_RunsAllCommandsSequentially(string yamlInput)
    {
        // Arrange
        string outputFile = base.CreateFile("output.txt");
        
        base.CreateFile(
            fileName: "script1.sh", 
            content: $"echo '1' >> '{outputFile}'");
            
        base.CreateFile(
            fileName: "script2.sh", 
            content: $"echo '2' >> '{outputFile}'");
            
        base.CreateFile(
            fileName: "script3.sh", 
            content: $"echo '3' >> '{outputFile}'");
        
        // Act
        _app.Object.Execute(yamlInput);
        
        // Assert
        string[] actualValues = File.ReadAllLines(outputFile);
        
        Assert.Equal(3, actualValues.Length);
        Assert.Equal(["1", "2", "3"], actualValues);
    }
    
    [Theory]
    [InlineData(@"
        - shell: ""script1.sh""
        - shell:
            inputType: command
            input: ""echo '2' >> '{0}'""
        - type: shell
          inputType: script
          input: ""script3.sh""
    ")]
    [InlineData(@"
        date: 2021-09-01
        author: John Doe
        tasks:
          - shell: ""script1.sh""
          - shell:
              input: ""echo '2' >> '{0}'""
          - type: shell
            inputType: script
            input: ""script3.sh""
    ")]
    public void MultipleCommands_AsMixOfShellScriptsAndInlineCommands_RunsAllCommandsSequentially(string yamlInput)
    {
        // Arrange
        string outputFile = base.CreateFile("output.txt");
        
        base.CreateFile(
            fileName: "script1.sh", 
            content: $"echo '1' >> '{outputFile}'");
            
        base.CreateFile(
            fileName: "script3.sh", 
            content: $"echo '3' >> '{outputFile}'");
        
        string yamlCommand = string.Format(yamlInput, outputFile);
        
        // Act
        _app.Object.Execute(yamlInput);
        
        // Assert
        string[] actualValues = File.ReadAllLines(yamlCommand);
        
        Assert.Equal(3, actualValues.Length);
        Assert.Equal(["1", "2", "3"], actualValues);
    }
    
    [Theory]
    [InlineData(@"
        - shell: ""echo '1' >> '{0}'""
        - shell:
            input: ""exit 1""
            behavior: strict
        - type: shell
          inputType: command
          input: ""echo '3' >> '{0}'""
    ")]
    [InlineData(@"
        - shell: ""echo '1' >> '{0}'""
        - shell:
            inputType: command
            input: ""cd /non-existent-random-folder_9204""
            behavior: strict
        - type: shell
          inputType: command
          input: ""echo '3' >> '{0}'""
    ")]
    public void MultipleCommands_WhenOneCommandWithDefaultBehaviorFails_ThrowsExceptionAndStopsExecution(
        string yamlInput)
    {
        // Arrange 
        string outputFile = base.CreateFile("output.txt");
        string yamlCommand = string.Format(yamlInput, outputFile);
        
        // Act
        _app.Object.Execute(yamlCommand);
        
        // Assert
        string[] actualValues = File.ReadAllLines(outputFile);
        
        Assert.Single(actualValues, "1");
    }
    
    [Theory]
    [InlineData(@"
        - shell: ""echo '1' >> '{0}'""
        - shell:
            inputType: command
            input: ""exit 1""
            behavior: forgiving
        - type: shell
          inputType: command
          input: ""echo '3' >> '{0}'""
    ")]
    [InlineData(@"
        - shell: ""echo '1' >> '{0}'""
        - shell:
            inputType: command
            input: ""cd /non-existent-random-folder_9204""
            behavior: forgiving
        - type: shell
          inputType: command
          input: ""echo '3' >> '{0}'""
    ")]
    public void MultipleCommands_WhenOneCommandWithRecoveryConfigurationFails_ContinuesExecution(
        string yamlInput)
    {
        // Arrange
        string outputFile = base.CreateFile("output.txt");
        string yamlCommand = string.Format(yamlInput, outputFile);
        
        // Act
        _app.Object.Execute(yamlCommand);
        
        // Assert
        string[] actualValues = File.ReadAllLines(outputFile);
        
        Assert.Equal(2, actualValues.Length);
        Assert.Equal(["1", "3"], actualValues);
    }
    
    [Fact]
    public void Command_CanPassValueToTheNextCommand()
    {
        // Arrange
        string expectedValue = Guid.NewGuid().ToString();
        string outputFile = base.CreateFile("output.txt");
        string yamlCommand = @"
            - shell: ""echo '1' >> '{output-file}'""
            - shell:
                inputType: command
                input: ""set SH_PERSISTED='{expected-key}'""
            - type: shell
              inputType: command
              input: ""echo '{expected-key}' >> '{output-file}'""
        ".Replace("{output-file}", outputFile).Replace("{expected-key}", expectedValue);
        
        // Act
        _app.Object.Execute(yamlCommand);
        
        // Assert
        string[] actualValues = File.ReadAllLines(outputFile);
        
        Assert.Equal(2, actualValues.Length);
        Assert.Equal(["1", expectedValue], actualValues);
    }
}