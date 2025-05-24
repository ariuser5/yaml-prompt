using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace YamlPrompt.Scripting.Core;

public static class ScriptHelper
{
    private static readonly Regex ScriptPattern = new(@"^\s*\{\{[\s\S]*\}\}\s*$", RegexOptions.Compiled);

    /// <summary>
    /// Determines if the input string is a script expression (wrapped in {{ ... }}).
    /// </summary>
    public static bool IsScript([NotNullWhen(true)] string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;
        return ScriptPattern.IsMatch(input);
    }

    /// <summary>
    /// Extracts the script body from a {{ ... }}-wrapped string, or returns the input as-is if not a script.
    /// </summary>
    public static string ExtractScript(string input)
    {
        if (IsScript(input))
        {
            var trimmed = input.Trim();
            return trimmed[2..^2].Trim();
        }
        return input;
    }
}
