using System.Diagnostics;
using System.Text.RegularExpressions;
using YamlPrompt.ExtensionSdk;
using YamlPrompt.Model;

namespace WinTasks.Internals;

public class ShellTaskDefinitionBaseImpl<T>(
    string taskAlias,
    string shellExecutorFilePath,
    IShellTaskPayloadMapper<T> payloadMapper
) : TaskDefinitionBase<T>
    where T: ShellTaskPayload
{
    public override string TypeKey { get; } = taskAlias;
    public string ShellExecutorFilePath { get; } = shellExecutorFilePath;
    public IShellTaskPayloadMapper<T> PayloadMapper { get; } = payloadMapper;

    protected override string? Invoke(
        AutomationContext context,
        T payload,
        string? previousResult)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = ShellExecutorFilePath,
            Arguments = "/c " + payload.Command,
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
            string pattern = $@"^{ShellTaskConstants.ResultCaptureVarName}=(.*)$";
            Match match = Regex.Match(output, pattern, RegexOptions.Multiline);
            if (match.Success) {
                result = match.Groups[1].Value;
            }
        };
        
        process.Start();
        process.WaitForExit();
        
        string error = process.StandardError.ReadToEnd();
        int exitCode = process.ExitCode;
        
        error = (string.IsNullOrWhiteSpace(error) && exitCode != 0) 
            ? $"Shell command exited with code {exitCode}."
            : error;
        
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
    
    public override T InterpretPayload(IReadOnlyDictionary<string, object?> fields)
    {
        var command = ReadRequiredValueAsString(
            fields, 
            fieldName: ShellTaskPayload.Fields.CommandFieldName, 
            defaultFieldName: this.TypeKey);
        
        ValidateCommand(command);
        
        var continueOnError 
            = ReadOptionalValueAsString(fields, ShellTaskPayload.Fields.ErrorHandlingFieldName)
                is string stringErrorHandling
            && bool.Parse(stringErrorHandling);
        
        var shellPayload = new ShellTaskPayload(command, continueOnError);
        
        return PayloadMapper.Map(shellPayload, fields);
    }
    
    private static void ValidateCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Command cannot be empty.");
    }
    
    private static string ReadRequiredValueAsString(
        IReadOnlyDictionary<string, object?> fields,
        string fieldName,
        string defaultFieldName)
    {
        if (fields.TryGetValue(fieldName, out object? value)) {
            return CastFieldValueToStringOrThrow(fieldName, value);
        } else if (fields.TryGetValue(defaultFieldName, out object? defaultValue)) {
            if (defaultValue is string defaultValueAsString){
                return defaultValueAsString;
            } else {
                throw new FormatException(
                    $"Unsupported syntax for shell task '{defaultFieldName}' default value. " + 
                    $"Expected 'string' but got '{defaultValue?.GetType().Name}'.");
            }
        }
        
        throw new ArgumentException($"Missing required field '{fieldName}' for task '{defaultFieldName}'");
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
