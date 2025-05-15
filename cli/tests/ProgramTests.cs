using System.Diagnostics;

namespace YamlPrompt.Cli.Tests;

[Collection("Test Collection")]
[Trait("TestCategory", "Integration")]
public class ProgramTests : IDisposable
{
	private readonly string _sut = Path.Combine(Directory.GetCurrentDirectory(), "ysap.exe");
	private readonly TestFixture _fixture;
	
	private readonly string _tempYaml;
	private Action? _cleanup;

	public ProgramTests(TestFixture fixture)
	{
		_fixture = fixture;
		
		var yaml = """
			steps:
			  - fwdref:
			    - "Hello, test!"
			  - type: echoref
			    suffix: "test-suffix - "
			""";
			
		_tempYaml = Path.Combine(Directory.GetCurrentDirectory(), "temp.yaml");
        File.WriteAllText(_tempYaml, yaml);
		_cleanup = () => File.Delete(_tempYaml);
	}

	// Teardown
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
	public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
	{
		if (_cleanup != null)
		{
			_cleanup();
			_cleanup = null;
		}
	}
	
    private async Task<(string output, string error)> RunYsapWithYamlAsync(string yamlFile)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _sut,
            Arguments = $"\"{yamlFile}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi) 
			?? throw new Exception("Failed to start ysap.exe process.");
		
		string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        process.WaitForExit();

        return (output, error);
    }

    [Fact]
    public async Task Ysap_PrintsExpectedOutput_ForValidYaml()
    {
        // Arrange & Act
        var (output, error) = await RunYsapWithYamlAsync(_tempYaml);

        // Assert
		Assert.Equal(string.Empty, error);
        Assert.Contains("test-suffix - Hello, test!", output);
    }
	
	[Theory]
	[InlineData("fake-test-file.yaml", "Unhandled exception: System.IO.FileNotFoundException: Could not find file")]
	[InlineData(@"\fake\test-file.yaml", "Unhandled exception: System.IO.DirectoryNotFoundException: Could not find a part of the path")]
	public async Task Ysap_PrintsError_ForMissingYaml(
		string relativeYamlFile,
		string expectedStartWithErrorMessage)
	{
		// Arrange
		var fakeYamlFile = Path.Combine(Directory.GetCurrentDirectory(), relativeYamlFile);
		
		// Act
		var (_, error) = await RunYsapWithYamlAsync(fakeYamlFile);
		
		// Assert
		Assert.StartsWith(expectedStartWithErrorMessage, error);
		Assert.Contains(fakeYamlFile, error);
	}

    [Fact]
    public async Task Ysap_PrintsError_ForInvalidYaml()
    {
		// Arrange
		var invalidYamlStep = "  - invalid: \"invalid\"";
		File.AppendAllLines(_tempYaml, ["", invalidYamlStep]);

		// Act
		var (_, error) = await RunYsapWithYamlAsync(_tempYaml);

		// Assert
		Assert.StartsWith("Unhandled exception: System.IO.InvalidDataException: Step [2] is missing the 'type' field", error);
    }
	
	[Fact]
	public async Task Ysap_PrintsWarning_ForEmptyDefinitions()
	{
		// Arrange
		Directory.Delete(_fixture.RegistryDir, true);

		// Act
		var (output, _) = await RunYsapWithYamlAsync(_tempYaml);

		// Assert
		Assert.Contains("No task definitions found.", output);
	}
}