namespace YamlPrompt.Scripting.Core.Functions;

public static class DateTime
{
    public static System.DateTime Now() => System.DateTime.Now;
    public static System.DateTime UtcNow() => System.DateTime.UtcNow;
    public static int Compare(System.DateTime a, System.DateTime b) => System.DateTime.Compare(a, b);
    public static int DaysBetween(System.DateTime a, System.DateTime b) => (a - b).Days;
    public static bool IsBefore(this System.DateTime a, System.DateTime b) => a < b;
    public static bool IsAfter(this System.DateTime a, System.DateTime b) => a > b;
}
