using EasyCli;
using EasySave.Services;
using Spectre.Console;

public sealed class CreateJobCommand : ICliCommand
{
    private readonly BackupManager manager;
    public CreateJobCommand(BackupManager _manager) => manager = _manager;
    public string Name => "add-job";
    public IReadOnlyList<string> Aliases => ["CreationTravaillage", "cj", "addJob", "ajouter-job", "create-job"];
    public string Description => "createJob.desc";

    public Task<int> ExecuteAsync(CommandContext context, string[] args, CancellationToken ct)
    {
        if (args.Length < 4)
        {
            context.Console.MarkupLine($"[red]{context.Text["createJob.notEnoughArgs"]}[/]");
        }
        else if (manager.AddBackupJobInList(args[0], args[1], args[2], args[3]))
        {
            context.Console.MarkupLine($"[green]{context.Text["createJob.success"]}[/]");
        }
        else
        {
            context.Console.MarkupLine($"[red]{context.Text["createJob.error"]}[/]");
        }
        return Task.FromResult(0);
    }
}