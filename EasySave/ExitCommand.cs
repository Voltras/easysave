using EasyCli;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

public sealed class ExitCommand : ICliCommand
{
    public string Name => "exit";
    public IReadOnlyList<string> Aliases => ["quit", "quitter", "q"];
    public string Description => "exit.desc";

    public Task<int> ExecuteAsync(CommandContext context, string[] args, CancellationToken ct)
    {
        context.Console.WriteLine(context.Text["exit.bye"]);
        var shell = context.Services.GetRequiredService<ShellState>();
        shell.Running = false;
        return Task.FromResult(0);
    }
}