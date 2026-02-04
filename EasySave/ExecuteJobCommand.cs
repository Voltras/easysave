using EasyCli;
using EasySave.Models;
using EasySave.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Spectre.Console;
using System.Runtime.CompilerServices;

public sealed class ExecuteJobCommand : ICliCommand
{
    private readonly BackupManager manager;
    public ExecuteJobCommand(BackupManager _manager) => manager = _manager;
    private static List<string> SequentialAliases => ["sequential", "sequenciel", "sequentiel", "sequencial", "seq"];
    public string Name => "execute-job";
    public IReadOnlyList<string> Aliases => ["executeJob", "exec-job"];
    public string Description => "executeJob.desc";

    public Task<int> ExecuteAsync(CommandContext context, string[] args, CancellationToken ct)
    {
        List<BackupJob> jobs = manager.GetList();
        if (jobs.Count == 0)
        {
            context.Console.MarkupLine("[red]Aucun job disponible.[/]");
            return Task.FromResult(0);
        }
        bool sequential = args.Length > 0 && SequentialAliases.Contains(args[0]);
        if (!sequential)
        {
            RunSingle(context.Console, jobs);
            return Task.FromResult(0);
        }

        RunSequentialRange(context.Console, jobs);

        return Task.FromResult(0);
    }

    private void RunSingle(IAnsiConsole console, List<BackupJob> jobs)
    {
        RenderJobsTable(console, jobs, startInclusive: 0);

        int choice = console.Prompt(
            new SelectionPrompt<int>()
                .Title("Choisir un job à exécuter")
                .AddChoices(Enumerable.Range(1, jobs.Count))
                .UseConverter(i => $"{i} - {jobs[i - 1].Name}")
        );

        int index0 = choice;
        manager.RunJob(index0);
    }
    private void RunSequentialRange(IAnsiConsole console, List<BackupJob> jobs)
    {
        RenderJobsTable(console, jobs, startInclusive: 0);

        int startChoice = console.Prompt(
            new SelectionPrompt<int>()
                .Title("Index de départ (séquentiel)")
                .AddChoices(Enumerable.Range(1, jobs.Count))
                .UseConverter(i => $"{i} - {jobs[i - 1].Name}")
        );

        int start0 = startChoice;

        RenderJobsTable(console, jobs, startInclusive: start0);

        int endChoice = console.Prompt(
            new SelectionPrompt<int>()
                .Title("Index de fin (séquentiel)")
                .AddChoices(Enumerable.Range(startChoice, jobs.Count - startChoice + 1))
                .UseConverter(i => $"{i} - {jobs[i - 1].Name}")
        );

        int end0 = endChoice;
        manager.RunSequential(start0, end0);
    }
    private static void RenderJobsTable(IAnsiConsole console, List<BackupJob> jobs, int startInclusive)
    {
        var table = new Spectre.Console.Table()
            .AddColumn("Index")
            .AddColumn("Job");

        for (int i = startInclusive; i < jobs.Count; i++)
            table.AddRow((i + 1).ToString(), jobs[i].Name);

        console.Write(table);
    }

    private bool ParseArgument(string argument)
    {
        return (SequentialAliases.Contains(argument.ToLowerInvariant()));
    }
}