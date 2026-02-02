namespace EasyCli;

public interface ICliCommand
{
    string Name { get; }
    IReadOnlyList<string> Aliases { get; }
    string Description { get; }

    Task<int> ExecuteAsync(CommandContext context, string[] args, CancellationToken cancellationToken);
}