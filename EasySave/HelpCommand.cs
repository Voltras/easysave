using EasyCli;
using Spectre.Console;

public sealed class HelpCommand : ICliCommand
{
    private readonly CommandRegistry _registry;

    public HelpCommand(CommandRegistry registry) => _registry = registry;

    public string Name => "help";
    public IReadOnlyList<string> Aliases => ["-h", "--help"];
    public string Description => "help.desc";

    public Task<int> ExecuteAsync(CommandContext context, string[] args, CancellationToken ct)
    {
        context.Console.WriteLine(context.Text["help.title"]);
        var table = new Table();
        table.AddColumn("Commande");
        table.AddColumn("Description");
        foreach (var c in _registry.AllUnique().OrderBy(c => c.Name))
            table.AddRow(c.Name, context.Text[c.Description]);
        context.Console.Write(table);
        return Task.FromResult(0);
    }
}