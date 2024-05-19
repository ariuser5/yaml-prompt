namespace WinTasks;

public record ShellTaskPayload(
    string Command,
    bool ContinueOnError = false
) {
    public static class Fields
    {
        public const string CommandFieldName = "command";
        public const string ErrorHandlingFieldName = "continueOnError";
    }
}
