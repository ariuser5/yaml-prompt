namespace WinTasks;

public static class ShellCommandTypes
{
    public const string KnownScriptExtensions = ".bat .ps1 .sh";

    public static ShellCommandType FromExtension(string extension)
    {
        return extension switch
        {
            ".bat" => ShellCommandType.Batch,
            ".ps1" => ShellCommandType.PowerShell,
            ".sh" => ShellCommandType.Bash,
            _ => throw new ArgumentException($"Unknown script extension: {extension}")
        };
    }
    
    public static string ToExtension(ShellCommandType type)
    {
        return type switch
        {
            ShellCommandType.Batch => ".bat",
            ShellCommandType.PowerShell => ".ps1",
            ShellCommandType.Bash => ".sh",
            _ => throw new ArgumentException($"Unknown script type: {type}")
        };
    }
}