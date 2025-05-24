using System.Diagnostics;
using YamlPrompt.Model;

namespace YamlPrompt.Tasks.Builtins.Tests;

[Trait("TestCategory", "Unit")]
public class DelayTaskTests
{
    [Theory]
    [InlineData(100)]
    [InlineData("100")]
    public void InterpretPayload_ReturnsValue_WhenValid(object value)
    {
        var task = new DelayTask();
        var fields = new Dictionary<string, object?> { [DelayTask.Template.TypeKey] = value };
        var result = task.InterpretPayload(fields);
        Assert.Equal("100", result);
    }

    [Theory]
    [InlineData(100)]
    [InlineData("100")]
    [InlineData("{{100}}")]
    public void InterpretPayload_DoesNotThrow_WhenValidInput(object value)
    {
        var task = new DelayTask();
        var fields = new Dictionary<string, object?> { [DelayTask.Template.TypeKey] = value };
        var ex = Record.Exception(() => task.InterpretPayload(fields));
        Assert.Null(ex);
    }

    [Fact]
    public void InterpretPayload_Throws_WhenMissingValue()
    {
        var task = new DelayTask();
        var fields = new Dictionary<string, object?>()
        {
            [DelayTask.Template.TypeKey] = null
        };
        Assert.Throws<ArgumentException>(() => task.InterpretPayload(fields));
    }

    [Theory]
    [Trait("Note", "Flaky")]
    [InlineData("200")]
    // TODO: The script evaluation takes time, so this test is not reliable.
    // Implementation will be changed such that the script will get compiled 
    // in InterpretPayload and executed in in Execute method.
    // This way we ensure that the delay is not affected by the script evaluation time.
    // [InlineData("{{200}}")]
    public void Execute_WaitsApproximatelyCorrectTime(string input)
    {
        int delayMs = 200;
        var task = new DelayTask();
        var context = new AutomationContext();
        var flowController = new TestFlowController();
        var sw = Stopwatch.StartNew();
        task.Execute(flowController, context, input, null);
        sw.Stop();
        Assert.InRange(sw.ElapsedMilliseconds, delayMs - 30, delayMs + 150);
    }
}

