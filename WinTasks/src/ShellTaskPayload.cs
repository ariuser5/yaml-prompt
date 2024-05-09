namespace WinTasks;

public record ShellTaskPayload(
    string Input,
    ShellCommandType InputType,
    bool ContinueOnError = false
);
