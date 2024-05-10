using WinTasks.Internals;
using WinTasks.Static;
using YamlPrompt.Model;

namespace WinTasks;

public class BatchTaskDefinition : ITaskDefinition
{
	private readonly IBatchTaskCompileTimeDefinition _compileTimeDefinition;
	private readonly ShellTaskDefinitionBaseImpl<ShellTaskPayload> _baseImpl;
	
	public BatchTaskDefinition() : this(
		compileTimeDefinition: BatchTaskCompileTimeDefinition.Instance,
		payloadMapper: ShellTaskPayloadMapper.Instance
	) { }
	
	public BatchTaskDefinition(
		IBatchTaskCompileTimeDefinition compileTimeDefinition,
		IShellTaskPayloadMapper<ShellTaskPayload> payloadMapper)
	{
        this.PayloadMapper = payloadMapper;
		_compileTimeDefinition = compileTimeDefinition;
		
		_baseImpl = new ShellTaskDefinitionBaseImpl<ShellTaskPayload>(
			taskAlias: this.TypeKey,
			shellExecutorFilePath: compileTimeDefinition.ExecutorFilePath,
			payloadMapper: payloadMapper);
    }
	
	public string TypeKey => _compileTimeDefinition.TaskKey;
	public IShellTaskPayloadMapper<ShellTaskPayload> PayloadMapper { get; }

    void ITaskDefinition.Execute(
		IFlowController flowController,
		AutomationContext context,
		object? payload,
		string? previousResult
	) => _baseImpl.Execute(flowController, context, (ShellTaskPayload)payload!, previousResult);

    object? ITaskDefinition.InterpretPayload(
		IReadOnlyDictionary<string, object?> fields
	) => _baseImpl.InterpretPayload(fields);
}
