namespace YamlPrompt.Scripting.Core.Functions;

public static class Math
{
    public static int Abs(int value) => System.Math.Abs(value);
    public static double Abs(double value) => System.Math.Abs(value);
    public static int Max(int a, int b) => System.Math.Max(a, b);
    public static double Max(double a, double b) => System.Math.Max(a, b);
    public static int Min(int a, int b) => System.Math.Min(a, b);
    public static double Min(double a, double b) => System.Math.Min(a, b);
    public static double Round(double value, int digits = 0) => System.Math.Round(value, digits);
}
