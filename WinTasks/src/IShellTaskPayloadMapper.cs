namespace WinTasks;

public interface IShellTaskPayloadMapper<T>
{
    T Map(ShellTaskPayload payload, IReadOnlyDictionary<string, object?> fields);
}
