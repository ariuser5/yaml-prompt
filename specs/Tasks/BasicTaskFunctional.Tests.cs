using YamlPrompt.Specs.AppInterface;
using YamlPrompt.Tasks.Builtins;
using YamlPrompt.Tasks.Builtins.Conditionals;

namespace YamlPrompt.Specs.Tasks;

[Trait("TestCategory", "Functional")]
public class BasicTaskFunctionalSpecs
{
	[Fact]
	public void DelayTask_ExecutesInPipeline()
	{
		var client = new AppTestingClient
		{
			TaskDefinitions = [new DelayTask()]
		};
		string yaml = """
			- delay: 100
			""";
		var sw = System.Diagnostics.Stopwatch.StartNew();
		var exitCode = client.Execute(yaml);
		sw.Stop();
		Assert.Equal(0, exitCode);
		Assert.InRange(sw.ElapsedMilliseconds, 80, 300);
	}

	[Fact]
	public void ContextTask_SetsVariablesInPipeline()
	{
		var contextTask = new ContextTask();
		var client = new AppTestingClient
		{
			TaskDefinitions = [contextTask]
		};
		string yaml = """
			- type: context
			  variables:
			    foo: 42
			    bar: baz
			""";
		var exitCode = client.Execute(yaml);
		Assert.Equal(0, exitCode);
	}

	[Theory]
	[InlineData("{{ 1 == 1 }}")]
	[InlineData("{{ true }}")]
	[InlineData("{{ false }}")]
	[InlineData("{{ 1 < 2 }}")]
	[InlineData("{{ 1 > 2 }}")]
	public void AssertTask_DoesntBreak_WhenEvaluatingCondition(string condition)
	{
		var client = new AppTestingClient
		{
			TaskDefinitions = [new AssertTask()]
		};
		string yaml = $"""
			- type: assert
			  condition: "{condition}"
			""";
		var exitCode = client.Execute(yaml);
		Assert.Equal(0, exitCode);
	}
}
