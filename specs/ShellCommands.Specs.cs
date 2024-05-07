using YamlPrompt.Api;

namespace Specs;

public class ShellCommands
{
    private readonly Executor _executor;
    
    public ShellCommands()
    {
        _executor = new Executor();
    }
    
    [Fact]
    public void OneCommand_WhenYamlIsValid_RunsTheCommand()
    {
        // Arrange
        
        // Act
        
        // Assert
        Assert.Fail("Not implemented.");
    }
    
    [Fact]
    public void OneCommand_WhenYamlIsInvalid_ThrowsException()
    {
        // Arrange
        
        // Act
        
        // Assert
        Assert.Fail("Not implemented.");
    }
    
    [Fact]
    public void MultipleCommands_WhenYamlIsValid_RunsAllCommandsSequentially()
    {
        // Arrange
        
        // Act
        
        // Assert
        Assert.Fail("Not implemented.");
    }
    
    [Fact]
    public void MultipleCommands_WhenYamlIsInvalid_ThrowsExceptionBeforeExecutingAnyCommand()
    {
        // Arrange
        
        // Act
        
        // Assert
        Assert.Fail("Not implemented.");
    }
    
    [Fact]
    public void MultipleCommands_WhenOneCommandWithDefaultBehaviorFails_ThrowsExceptionAndStopsExecution()
    {
        // Arrange
        
        // Act
        
        // Assert
        Assert.Fail("Not implemented.");
    }
    
    [Fact]
    public void MultipleCommands_WhenOneCommandWithRecoveryConfigurationFails_ContinuesExecution()
    {
        // Arrange
        
        // Act
        
        // Assert
        Assert.Fail("Not implemented.");
    }
}