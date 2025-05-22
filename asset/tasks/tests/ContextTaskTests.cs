using YamlPrompt.Model;

namespace YamlPrompt.Tasks.Tests;

[Trait("TestCategory", "Unit")]
public class ContextTaskTests
{
    [Fact]
    public void InterpretPayload_ReturnsDictionary_WhenValid()
    {
        var task = new ContextTask();
		var fields = new Dictionary<string, object?>
		{
			[ContextTask.Template.VariablesFieldName] = new Dictionary<object, object?>
			{
				["foo"] = 123,
				["bar"] = "baz"
			}
		};
		var result = task.InterpretPayload(fields);
        Assert.Equal(2, result.Count);
        Assert.Equal(123, result["foo"]);
        Assert.Equal("baz", result["bar"]);
    }

    [Fact]
    public void InterpretPayload_Throws_WhenMissingVariables()
    {
        var task = new ContextTask();
        var fields = new Dictionary<string, object?>
		{
			[ContextTask.Template.VariablesFieldName] = null
		};
		Assert.Throws<ArgumentException>(() => task.InterpretPayload(fields));
	}

	[Fact]
	public void InterpretPayload_Throws_WhenKeyNotString()
	{
		var task = new ContextTask();
		var fields = new Dictionary<string, object?>
		{
			[ContextTask.Template.VariablesFieldName] = new Dictionary<object, object?>
			{
				[1] = 123,
				["bar"] = "baz"
			}
		};
        Assert.Throws<ArgumentException>(() => task.InterpretPayload(fields));
    }

    [Fact]
    public void Invoke_SetsContextItems()
    {
        var task = new ContextTask();
        var context = new AutomationContext();
        var payload = new Dictionary<string, object?> { { "foo", 42 }, { "bar", "baz" } };
        task.Execute(new TestFlowController(), context, payload, null);
        Assert.Equal(42, context.Items["foo"]);
        Assert.Equal("baz", context.Items["bar"]);
    }
}
