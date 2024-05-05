using YamlPrompt.Cli;

namespace cli.tests;

public class ParserTests
{
    [Fact]
    public void ParseYaml_WhenInputIsValid_Returns()
    {
        // Arrange
        var yaml = @"
            command1:
                - param1
                - param2
            command2:
                - param1
                - param2
        ";
        var filePath = "test.yaml";
        File.WriteAllText(filePath, yaml);

        // Act
        var result = Parser.ParseYaml(filePath);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(["param1", "param2"], result["command1"]);
        Assert.Equal(["param1", "param2"], result["command2"]);
    }
    
    [Fact]
    public void ParseYaml_WhenInputIsInvalid_Throws()
    {
        // Arrange
        var yaml = @"
            command1:
                param1
                - param2
            command2:
                - param1
                - param2
        ";
        var filePath = "test.yaml";
        File.WriteAllText(filePath, yaml);

        // Act
        var exception = Record.Exception(() => Parser.ParseYaml(filePath));

        // Assert
        Assert.NotNull(exception);
    }
}