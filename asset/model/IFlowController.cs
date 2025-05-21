namespace YamlPrompt.Model;

public interface IFlowController
{
	// TODO: Implement this
	// void Yield();
	
	int ExitCode { get; set;}
	string? ReturnValue { get; set; }
	bool AllowContinuationOnFailure { get; set; }
	Action<Exception>? ExceptionHandling { get; set; }
}
