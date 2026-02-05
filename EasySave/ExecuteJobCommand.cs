using EasyCli;
using EasySave.Models;
using EasySave.Services;
using Spectre.Console;

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
            context.Console.MarkupLine($"[red]{context.Text["error.NoAvailableJobs"]}[/]");
            return Task.FromResult(0);
        }
        bool sequential = args.Length > 0 && SequentialAliases.Contains(args[0]);
        if (!sequential)
        {
            RunSingle(context, jobs);
            context.Console.MarkupLine(context.Text["executeJob.success"]);
            return Task.FromResult(0);
        }

        RunSequentialRange(context, jobs);

        context.Console.MarkupLine(context.Text["executeJob.success"]);
        return Task.FromResult(0);
    }

    private void RunSingle(CommandContext context, List<BackupJob> jobs)
    {
        RenderJobsTable(context, jobs, startInclusive: 0);

        int choice = context.Console.Prompt(
            new SelectionPrompt<int>()
                .Title(context.Text["table.chooseJob"])
                .AddChoices(Enumerable.Range(1, jobs.Count))
                .UseConverter(i => $"{i} - {jobs[i - 1].Name}")
        );
        RunWithStatus(
            context,
            $"{context.Text["status.runningJob"]} {jobs[choice - 1].Name}",
            () => manager.RunJob(choice)
        );
    }
    private void RunSequentialRange(CommandContext context, List<BackupJob> jobs)
    {
        context.Console.WriteLine(context.Text["table.executeJobDesc"]);
        RenderJobsTable(context, jobs, startInclusive: 0);

        string mode = context.Console.Prompt(
            new SelectionPrompt<string>()
                .Title(context.Text["executeJob.chooseSeqMode"])
                .AddChoices(context.Text["executeJob.rangeChoice"], context.Text["executeJob.individualChoice"])
            );

        if(mode == context.Text["executeJob.rangeChoice"])
        {
            int startChoice = context.Console.Prompt(
                new SelectionPrompt<int>()
                    .Title(context.Text["table.startSequentialTitle"])
                    .AddChoices(Enumerable.Range(1, jobs.Count))
                    .UseConverter(i => $"{i} - {jobs[i - 1].Name}")
                );

            int endChoice = context.Console.Prompt(
                new SelectionPrompt<int>()
                    .Title(context.Text["table.endSequentialTitle"])
                    .AddChoices(Enumerable.Range(startChoice, jobs.Count - startChoice + 1))
                    .UseConverter(i => $"{i} - {jobs[i - 1].Name}")
                );

            RunWithStatus(
                context,
                $"{context.Text["status.runningSeq"]} {startChoice} -> {endChoice}",
                () => manager.RunSequential(startChoice, endChoice)
            );
        }
        else
        {

        }
    }
    private static void RenderJobsTable(CommandContext context, List<BackupJob> jobs, int startInclusive)
    {
        var table = new Spectre.Console.Table()
            .AddColumn("Index")
            .AddColumn("Job");

        for (int i = startInclusive; i < jobs.Count; i++)
            table.AddRow((i + 1).ToString(), jobs[i].Name);

        context.Console.Write(table);
    }

    private bool ParseArgument(string argument)
    {
        return (SequentialAliases.Contains(argument.ToLowerInvariant()));
    }

    private static void RunWithStatus(CommandContext context, string message, Action work)
    {
        context.Console.Status()
            .Spinner(Spinner.Known.Aesthetic)
            .SpinnerStyle(Color.Blue)
            .Start(message, _ => work());
    }
}