using YamlPrompt.Model;

namespace YamlPrompt.Tasks.Builtins.Tests;

public class TestFlowController : IFlowController
{
	public int ExitCode { get; set; }
	public bool AbortRequested { get; set; }
	public string? ReturnValue { get; set; }
	public bool AllowContinuationOnFailure { get; set; }
	public Action<Exception>? ExceptionHandling { get; set; }
}
