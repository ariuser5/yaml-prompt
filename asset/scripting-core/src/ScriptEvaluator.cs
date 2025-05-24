// TODO:
// Ideally, we want to only expose some methods for EvaluateCondition while for
// EvaluateScript we may want to expose all.
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using YamlPrompt.Model;

namespace YamlPrompt.Scripting.Core;

public class ScriptEvaluator
{
	public static bool EvaluateCondition(
		string conditionScript,
		AutomationContext context,
		string? previousResult)
	{
		var globals = new ScritGlobals(context, previousResult);
		var contextualizedScript = ContextualizeScript(conditionScript, context);
		return CSharpScript.EvaluateAsync<bool>(
			contextualizedScript,
			globals: globals,
			options: ScriptOptions.Default
				.WithReferences(
					typeof(ScritGlobals).Assembly,
					typeof(AutomationContext).Assembly
				)
				.WithImports(
					"YamlPrompt.Model",
					"YamlPrompt.Scripting.Core.Functions")
		).Result;
	}

	public static T Evaluate<T>(
		string script,
		AutomationContext context,
		string? previousResult)
	{
		var globals = new ScritGlobals(context, previousResult);
		var contextualizedScript = ContextualizeScript(script, context);
		return CSharpScript.EvaluateAsync<T>(
			contextualizedScript,
			globals: globals,
			options: ScriptOptions.Default
				.WithReferences(
					globals.GetType().Assembly,
					typeof(AutomationContext).Assembly
				)
				.WithImports(
					"YamlPrompt.Model",
					"YamlPrompt.Scripting.Core.Functions")
		).Result;
	}
	
	private static string ContextualizeScript(
		string script,
		AutomationContext context)
	{
		var declarations = new StringBuilder();
		foreach (var kvp in context.Items)
		{
			if (kvp.Key is string name && kvp.Value != null)
			{
				var type = kvp.Value.GetType();
				declarations.AppendLine($"var {name} = ({type.FullName})Context.Items[\"{name}\"];");
			}
		}
		return declarations.ToString() + script;
	}
}