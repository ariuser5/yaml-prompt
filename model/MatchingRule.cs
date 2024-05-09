namespace YamlPrompt.Model;

public abstract class MatchingRule
{
	public abstract MatchingRuleType RuleType { get; } 
    internal MatchingRule() { }
}

public enum MatchingRuleType
{
	Empty,
	ParamArray,
	Template
}

public class MatchingRuleBuilder
{
    public MatchingRule Empty() => EmptyMatchingRule.Instance;
	
	public MatchingRule ParamArray() => ParamArrayMatchingRule.Instance;
	
	public MatchingRule Template(IEnumerable<TemplateFieldDefinition> ruleSet)
	{
		return new TemplateMatchingRule(ruleSet);
	}
}

internal class EmptyMatchingRule : MatchingRule
{
    private static readonly Lazy<EmptyMatchingRule> _lazyInstance = new(() => new EmptyMatchingRule());

    private EmptyMatchingRule() { }

    public static EmptyMatchingRule Instance => _lazyInstance.Value;

    public override MatchingRuleType RuleType => MatchingRuleType.Empty;
}

internal class ParamArrayMatchingRule : MatchingRule
{
	private static readonly Lazy<ParamArrayMatchingRule> _lazyInstance = new(() => new ParamArrayMatchingRule());

	private ParamArrayMatchingRule() { }

	public static ParamArrayMatchingRule Instance => _lazyInstance.Value;

	public override MatchingRuleType RuleType => MatchingRuleType.ParamArray;
}

internal class TemplateMatchingRule : MatchingRule
{
	public TemplateMatchingRule(IEnumerable<TemplateFieldDefinition> ruleSet)
	{
		RuleSet = ruleSet.ToArray();
	}

	public TemplateFieldDefinition[] RuleSet { get; }

	public override MatchingRuleType RuleType => MatchingRuleType.Template;
}