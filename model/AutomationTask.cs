namespace YamlPrompt.Model;

public abstract class AutomationTask<TIn, TOut>
{
	public static AutomationTask<TIn, TOut> Create(
		Func<AutomationContext, TIn, string[], TOut> handler)
	{
		return new AutomationTaskImpl(handler);
	}

    public abstract TOut Execute(AutomationContext context, TIn persistedValue, string[] parameters);
	
	public static implicit operator AutomationTask<TIn, TOut>(Func<AutomationContext, TIn, string[], TOut> handler)
	{
		return Create(handler);
	}
	
	private class AutomationTaskImpl : AutomationTask<TIn, TOut>
	{	
		private readonly Func<AutomationContext, TIn, string[], TOut> _handler;

		public AutomationTaskImpl(Func<AutomationContext, TIn, string[], TOut> handler)
		{
			_handler = handler;
		}

		public override TOut Execute(AutomationContext context, TIn persistedValue, string[] parameters)
		{
			return _handler(context, persistedValue, parameters);
		}	
	}
}
