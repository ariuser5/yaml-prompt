using YamlPrompt.Specs.AppInterface;

namespace YamlPrompt.Specs.InputFormat;

[Trait("TestCategory", "Functional")]
public class InputFormatTests
{
	private readonly AppTestingClient _app;
    
    public InputFormatTests()
    {
        _app = new AppTestingClient();
    }
	
	[Theory]
	[InlineData("""
		date: 2022-01-01
		author: John Doe
		title: Hello, World!
		steps:
		  - type: TestShell
		    input: "{0}"
		    inputType: batch
		  - TestShell:
		      input: "{1}"
		      inputType: batch
		  - TestShell: "{2}"
		""")]
	[InlineData("""
		- type: TestShell
		  input: "{0}"
		  inputType: batch
		- TestShell:
		  - "{1}"
		  - arg1
		  - arg2
		  - argN
		- TestShell: "{2}"
		""")]
	public void InputFormat_WhenValid_Executes(string inputSource)
	{
		// Arrange
		string[] expectedLines = ["hello", "world", "!"];
		
		var yamlCommand = string.Format(inputSource, expectedLines[0], expectedLines[1], expectedLines[2]);
		
		var actualLines = new List<string>();
		var fakeShellTask = new FakeTaskDefinition(actualLines.Add);
		
		_app.TaskDefinitions.Add(fakeShellTask);
		
		// Act
		_app.Execute(yamlCommand);
		
		// Assert
		Assert.Equal(expectedLines, actualLines);	
	}

	[Theory]
	[InlineData("""
		steps:
		  - type: TestShell
		    input: |
		      {{
		        == 1 &&
		        2 > 1 &&
		        "hello".ToUpper() == "HELLO"
		      }}
		""")]
	[InlineData("""
		steps:
		  - type: TestShell
		    input: |
		      {{
		        var a = 5;
		        var b = 10;
		        a < b && b - a == 5
		      }}
		""")]
	public void InputFormat_MultilineScriptExpression_Executes(string inputSource)
	{
		var yamlCommand = inputSource;
		var actualLines = new List<string>();
		var fakeShellTask = new FakeTaskDefinition(actualLines.Add);
		_app.TaskDefinitions.Add(fakeShellTask);
		var exitCode = _app.Execute(yamlCommand);
		Assert.Equal(0, exitCode);
    }
}
