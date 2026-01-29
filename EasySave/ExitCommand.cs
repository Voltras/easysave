using EasyCli;
using Microsoft.Extensions.DependencyInjection;

public sealed class ExitCommand : ICliCommand
{
    public string Name => "exit";
    public IReadOnlyList<string> Aliases => ["quit", "quitter", "q"];
    public string Description => "Quitte l'application";

    public Task<int> ExecuteAsync(CommandContext context, string[] args, CancellationToken ct)
    {
        var shell = context.Services.GetRequiredService<ShellState>();
        shell.Running = false;
        return Task.FromResult(0);
    }
}