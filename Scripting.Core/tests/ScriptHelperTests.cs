namespace YamlPrompt.Scripting.Core.Tests;

[Trait("TestCategory", "Unit")]
public class ScriptHelperTests
{
	[Theory]
	[InlineData(null, false)]
	[InlineData("", false)]
	[InlineData("123", false)]
	[InlineData("{{ 1 + 2 }}", true)]
	[InlineData("  {{ 1 + 2 }}  ", true)]
	[InlineData("{{\n1 + 2\n}}", true)]
	[InlineData("{{1 + 2}}", true)]
	[InlineData("{ 1 + 2 }", false)]
	public void IsScript_DetectsCorrectly(string? input, bool expected)
	{
		Assert.Equal(expected, ScriptHelper.IsScript(input));
	}

	[Theory]
	[InlineData("{{ 1 + 2 }}", "1 + 2")]
	[InlineData("  {{ 1 + 2 }}  ", "1 + 2")]
	[InlineData("{{\n1 + 2\n}}", "1 + 2")]
	[InlineData("{{1 + 2}}", "1 + 2")]
	[InlineData("123", "123")]
	public void ExtractScript_ExtractsCorrectly(string input, string expected)
	{
		Assert.Equal(expected, ScriptHelper.ExtractScript(input));
	}
}
