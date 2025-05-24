using YamlPrompt.Specs.AppInterface;
using YamlPrompt.Tasks.Builtins;
using YamlPrompt.Tasks.Builtins.Conditionals;

namespace YamlPrompt.Specs.Tasks;

[Trait("TestCategory", "Functional")]
public class BasicFlowsSpecs
{
	[Fact]
	public void DelayWithContextAndAssertFlow_Works()
	{
		var client = new AppTestingClient
		{
			TaskDefinitions = [new ContextTask(), new DelayTask(), new AssertTask()]
		};
		string yaml = """
			- type: context
			  variables:
			    start: "{{ DateTime.UtcNow() }}"
			    delay: 150
			- delay: '{{ delay }}'
			- type: assert
			  condition: '{{ DateTime.UtcNow() > start }}'
			""";
		var exitCode = client.Execute(yaml);
		Assert.Equal(0, exitCode);
    }

	[Fact]
	public void DelayWithDynamicDelayFromContext_Works()
	{
		var client = new AppTestingClient
		{
			TaskDefinitions = [new ContextTask(), new DelayTask(), new AssertTask()]
		};
		string yaml = """
			- type: context
			  variables:
			    delay: 200
			- delay: '{{ delay }}'
			- type: assert
			  condition: '{{ delay == 200 }}'
			""";
		var exitCode = client.Execute(yaml);
		Assert.Equal(0, exitCode);
    }
}
