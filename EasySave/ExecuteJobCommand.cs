using EasyCli;
using EasySave.Models;
using EasySave.Services;
using Spectre.Console;
using System.Globalization;

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
            RunSingle(context, jobs, ct);
            context.Console.MarkupLine(context.Text["executeJob.success"]);
            return Task.FromResult(0);
        }

        RunSequentialRange(context, jobs, ct);

        context.Console.MarkupLine(context.Text["executeJob.success"]);
        return Task.FromResult(0);
    }

    private void RunSingle(CommandContext context, List<BackupJob> jobs, CancellationToken ct)
    {
        RenderJobsTable(context, jobs, startInclusive: 0);

        int choice = context.Console.Prompt(
            new SelectionPrompt<int>()
                .Title(context.Text["table.chooseJob"])
                .AddChoices(Enumerable.Range(1, jobs.Count))
                .UseConverter(i => $"{i} - {jobs[i - 1].Name}")
        );
        RunWithProgress(
            context,
            $"{context.Text["status.runningJob"]} {jobs[choice - 1].Name}",
            statusProvider: manager.GetStatus,
            work: () => manager.RunJob(choice),
            ct: ct
        );
    }
    private void RunSequentialRange(CommandContext context, List<BackupJob> jobs, CancellationToken ct)
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

            RunWithProgress(
                context,
                $"{context.Text["status.runningSeq"]} {startChoice} -> {endChoice}",
                statusProvider: manager.GetStatus,
                work: () => manager.RunSequential(startChoice, endChoice),
                ct: ct
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

    private static void RunWithProgress(
    CommandContext context,
    string title,
    Func<string> statusProvider,
    Action work,
    CancellationToken ct = default)
    {
        context.Console.Progress()
            .AutoClear(false)
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn()
            )
            .Start(ctx =>
            {
                var task = ctx.AddTask(title, maxValue: 1);
                var worker = Task.Run(work, ct);
                while (!worker.IsCompleted && !ct.IsCancellationRequested)
                {
                    var status = statusProvider();
                    if (TryParseStatus(status, out _, out var processed, out var total) && total > 0)
                    {
                        task.MaxValue = total;
                        task.Value = Math.Min(processed, total);
                    }
                }

                worker.GetAwaiter().GetResult();

                task.Value = task.MaxValue;
            });
    }
    private static bool TryParseStatus(string? s, out double pct, out long processed, out long total)
    {
        pct = 0; processed = 0; total = 0;
        if (string.IsNullOrWhiteSpace(s)) return false;

        var parts = s.Split('|');
        if (parts.Length < 3) return false;

        var pctPart = parts[0].Trim().TrimEnd('%');
        _ = double.TryParse(pctPart, NumberStyles.Float, CultureInfo.InvariantCulture, out pct);

        if (!long.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out processed))
            return false;
        if (!long.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out total))
            return false;

        return true;
    }
}