using EasyCli;
using EasySave.Models;
using EasySave.Services;
using Spectre.Console;

public sealed class RemoveJobCommand : ICliCommand
{
    private readonly BackupManager manager;
    private IReadOnlyList<string> ArgumentAliases => ["all", "tout", "every", "everything"];
    public RemoveJobCommand(BackupManager _manager) => manager = _manager;
    public string Name => "remove-job";
    public IReadOnlyList<string> Aliases => ["suppressiontravaillage", "rmj", "removeJob", "supprimer-job", "delete-job", "suppr-job", "del-job", "rm-job"];
    public string Description => "removeJob.desc";

    public Task<int> ExecuteAsync(CommandContext context, string[] args, CancellationToken ct)
    {
        List<BackupJob> jobs = manager.GetList();
        if (args.Length > 0)
        {
            if (ParseArgument(args[0])) manager.ClearBackupJobsList();
            else context.Console.MarkupLine($"[red]{context.Text["removeJob.argumentError"]}[/]");
        }
        else
        {
            if (jobs.Count > 0)
            {

                context.Console.WriteLine("Suppression d'un job :");
                int choice = context.Console.Prompt(
                    new SelectionPrompt<int>()
                        .Title(context.Text["table.chooseJob"])
                        .AddChoices(Enumerable.Range(1, jobs.Count))
                        .UseConverter(i => $"{i} - {jobs[i - 1].Name}")
                );
                manager.RemoveBackupJobInList(jobs[choice-1]);
            }
            else
            {
                context.Console.MarkupLine($"[red]{context.Text["removeJob.emptyList"]}[/]");
            }
        }
        return Task.FromResult(0);
    }

    private bool ParseArgument(string argument)
    {
        if (ArgumentAliases.Contains(argument)) return true;
        return false;
    }
}