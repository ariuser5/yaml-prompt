using System.Diagnostics;
using YamlPrompt.Model;

namespace YamlPrompt.Tasks.Tests;

[Trait("TestCategory", "Unit")]
public class DelayTaskTests
{
    [Fact]
    public void InterpretPayload_ReturnsValue_WhenValid()
    {
        var task = new DelayTask();
        var fields = new Dictionary<string, object?> { { DelayTask.Template.TypeKey, 100 } };
        var result = task.InterpretPayload(fields);
        Assert.Equal(100, result);
    }

    [Fact]
    public void InterpretPayload_Throws_WhenMissing()
    {
        var task = new DelayTask();
        var fields = new Dictionary<string, object?>()
        {
            [DelayTask.Template.TypeKey] = null
        };
        Assert.Throws<ArgumentException>(() => task.InterpretPayload(fields));
    }

    [Fact]
    public void Execute_WaitsApproximatelyCorrectTime()
    {
        var task = new DelayTask();
        var context = new AutomationContext();
        int delayMs = 200;
        var sw = Stopwatch.StartNew();
        var flowController = new TestFlowController();
        task.Execute(flowController, context, delayMs, null);
        sw.Stop();
        Assert.InRange(sw.ElapsedMilliseconds, delayMs - 30, delayMs + 150);
    }
}

