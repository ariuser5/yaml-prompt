using System.Diagnostics;
using System.Text.RegularExpressions;
using YamlPrompt.Model;

namespace WinTasks;

public class ShellTaskDefinition : TaskDefinition<ShellTaskPayload>
{
    public const string TaskKey = "shell";
    public const string InputFieldName = "input";
    public const string InputTypeFieldName = "inputType";
    public const string ErrorHandlingFieldName = "continueOnError";
    public const string ResultCaptureVarName = "SH_PASS_NEXT";
    
    public override string TypeKey => TaskKey;
    
    protected override string? Invoke(
        AutomationContext context,
        ShellTaskPayload payload,
        string? previousResult)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = payload.InputType switch
            {
                ShellCommandType.Batch => "cmd.exe",
                ShellCommandType.PowerShell => "powershell.exe",
                ShellCommandType.Bash => "bash",
                _ => throw new ArgumentException("Invalid input type.")
            },
            Arguments = payload.Input,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        
        Process process = new()
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };
        
        string? result = null;
        string output = "";
        process.Exited += (sender, e) =>
        {
            // Read the output of the process
            output = process.StandardOutput.ReadToEnd();

            // Parse the output to extract the value of the environment variable
            // This assumes that the process writes the value of the environment variable
            // to its output in a specific format
            string pattern = $@"^{ResultCaptureVarName}=(.*)$";
            Match match = Regex.Match(output, pattern, RegexOptions.Multiline);
            if (match.Success) {
                result = match.Groups[1].Value;
            }
        };
        
        process.Start();
        process.WaitForExit();
        
        string error = process.StandardError.ReadToEnd();
        
        if (string.IsNullOrWhiteSpace(error) == false)
        {
            if (payload.ContinueOnError) {
                Console.WriteLine("Shell ignore error enabled. Continuing..."); // TODO: replace this with proper logging
            } else {
                throw new Exception(error);
            }
        }
        
        Console.WriteLine(output);  // TODO: replace this with proper logging
        return result;
    }
    
    public override ShellTaskPayload MapPayload( IReadOnlyDictionary<string, object?> fields)
    {
        var input = ReadRequiredValueAsString(fields, InputFieldName);
        
        ValidateInput(input);
        
        var inputType = ReadOptionalValueAsString(fields, InputTypeFieldName) is string stringInputType
            ? Enum.Parse<ShellCommandType>(stringInputType, ignoreCase: true)
            : default;
        
        var continueOnError = ReadOptionalValueAsString(fields, ErrorHandlingFieldName) is string stringErrorHandling
            && bool.Parse(stringErrorHandling);
        
        return new ShellTaskPayload(input, inputType, continueOnError);
    }
    
    private static void ValidateInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be empty.");
    }
    
    private static string ReadRequiredValueAsString(
        IReadOnlyDictionary<string, object?> fields,
        string fieldName)
    {
        if (fields.TryGetValue(fieldName, out object? value))
            return CastFieldValueToStringOrThrow(fieldName, value);
        
        throw new ArgumentException($"Missing required field '{fieldName}'");
    }
    
    private static string? ReadOptionalValueAsString(
        IReadOnlyDictionary<string, object?> fields,
        string fieldName)
    {
        if (fields.TryGetValue(fieldName, out object? value))
            return CastFieldValueToStringOrThrow(fieldName, value);
        
        return null;
    }
    
    private static string CastFieldValueToStringOrThrow(string fieldName, object? value)
    {
        if (value is not string str)
            throw new ArgumentException(
                $"Invalid data type for field '{fieldName}'. " + 
                $"Expected 'string' but got '{value?.GetType().Name}'.");
        
        return str;
    }
}
