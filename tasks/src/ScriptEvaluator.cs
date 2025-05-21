
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using YamlPrompt.Conditionals.Model;
using YamlPrompt.Model;

namespace YamlPrompt.Tasks;

public class ScriptEvaluator
{
	public static T Evaluate<T>(
		AutomationContext context,
		string script,
		string? previousResult)
	{
		var globals = new ConditionalScritGlobals(context, previousResult);
        return CSharpScript.EvaluateAsync<T>(
            script,
            globals: globals,
            options: ScriptOptions.Default
                .WithReferences(
                    typeof(ConditionalScritGlobals).Assembly,
                    typeof(AutomationContext).Assembly
                )
                .WithImports(
                    "YamlPrompt.Model",
                    "YamlPrompt.Conditionals.Model")
        ).Result;
	}
}