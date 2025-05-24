using YamlPrompt.Model;
using YamlPrompt.Tasks.Builtins.Conditionals;

namespace YamlPrompt.Tasks.Builtins.Tests.Conditionals;

[Trait("TestCategory", "Unit")]
public class AssertTaskTests
{
    [Fact]
    public void InterpretPayload_ReturnsCondition_WhenValid()
    {
        var task = new AssertTask();
        var fields = new Dictionary<string, object?>
		{
			[AssertTask.Template.ConditionField] = "{{1 == 1}}"
		};
        var result = task.InterpretPayload(fields);
        Assert.Equal("1 == 1", result);
    }

    [Fact]
    public void InterpretPayload_Throws_WhenMissingCondition()
    {
        var task = new AssertTask();
        var fields = new Dictionary<string, object?>
		{
			[AssertTask.Template.ConditionField] = null
		};
		Assert.Throws<ArgumentException>(() => task.InterpretPayload(fields));
	}

	[Theory]
	[InlineData(1)]
	[InlineData(true)]
	public void InterpretPayload_Throws_WhenConditionNotString(object input)
	{
		var task = new AssertTask();
		var fields = new Dictionary<string, object?>
		{
			[AssertTask.Template.ConditionField] = input
		};
        Assert.Throws<ArgumentException>(() => task.InterpretPayload(fields));
    }

	[Theory]
	[InlineData("1 == 1")]
	[InlineData("true")]
	[InlineData("false")]
	[InlineData("1 != 2")]
	[InlineData("1 < 2")]
	[InlineData("1 > 0")]
	[InlineData("1 <= 2")]
    public void Execute_DoesNotThrow_WhenConditionIsValid(string condition)
	{
		var task = new AssertTask();
		var context = new AutomationContext();
        var flowController = new TestFlowController();
		var ex = Record.Exception(() => task.Execute(flowController, context, condition, null));
		Assert.Null(ex);
	}
	
	[Theory]
	[InlineData("1 == 1", true)]
	[InlineData("true", true)]
	[InlineData("false", false)]
	[InlineData("1 != 2", true)]
	[InlineData("1 < 2", true)]
	[InlineData("1 > 0", true)]
	[InlineData("1 > 2", false)]
    public void Execute_RequestsFlowAbort_WhenConditionIsFalse(
		string condition,
		bool isRequestingAbort)
	{
		var task = new AssertTask();
		var context = new AutomationContext();
        var flowController = new TestFlowController();
		task.Execute(flowController, context, condition, null);
		Assert.NotEqual(isRequestingAbort, flowController.AbortRequested);
	}
}
