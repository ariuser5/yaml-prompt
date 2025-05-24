namespace YamlPrompt.Scripting.Core.Functions;

public static class String
{
    public static bool IsNullOrEmpty(string? value) => string.IsNullOrEmpty(value);
    public static bool IsNullOrWhiteSpace(string? value) => string.IsNullOrWhiteSpace(value);
    public static string ToLower(this string value) => value.ToLowerInvariant();
    public static string ToUpper(this string value) => value.ToUpperInvariant();
    public static bool Contains(this string value, string substring) => value.Contains(substring);
    public static bool StartsWith(this string value, string prefix) => value.StartsWith(prefix);
    public static bool EndsWith(this string value, string suffix) => value.EndsWith(suffix);
}
