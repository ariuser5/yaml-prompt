namespace WinTasks.Internals;

internal sealed class ShellTaskPayloadMapper : IShellTaskPayloadMapper<ShellTaskPayload>
{
	private static readonly Lazy<ShellTaskPayloadMapper> instance =
		new(() => new ShellTaskPayloadMapper());

	public static ShellTaskPayloadMapper Instance => instance.Value;

	private ShellTaskPayloadMapper()
	{
		// Private constructor to prevent instantiation outside the class
	}

	public ShellTaskPayload Map(
		ShellTaskPayload payload,
		IReadOnlyDictionary<string, object?> fields
	) => payload;
}