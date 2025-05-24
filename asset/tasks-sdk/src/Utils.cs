
namespace YamlPrompt.Tasks.Sdk;
public class Utils
{
	public static bool IsNumber(object? value)
	{
		return value is sbyte
			|| value is byte
			|| value is short
			|| value is ushort
			|| value is int
			|| value is uint
			|| value is long
			|| value is ulong
			|| value is float
			|| value is double
			|| value is decimal;
	}
}