
// NOTE:
// Don't remove explicit using statements, they are needed for the code to compile
using System;
using System.Collections.Generic;
using YamlPrompt.Model;

namespace YamlPrompt.Cli.Tests;
public class EchoRef : TaskDefinitionBase
{
	public override string TypeKey => nameof(EchoRef).ToLowerInvariant();

	public override IReadOnlyDictionary<string, object?> InterpretPayload(
		IReadOnlyDictionary<string, object?> fields)
	{
		return fields;
	}

	protected override string? Invoke(
		AutomationContext context,
		IReadOnlyDictionary<string, object?> payload,
		string? previousResult)
	{
		var suffix = payload["suffix"];
		Console.WriteLine(suffix + previousResult);
		return null;
	}
}