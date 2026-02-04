using EasyCli;
using EasySave.Models;
using EasySave.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Spectre.Console;
using System.Runtime.CompilerServices;

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
            context.Console.WriteLine($"[red]{context.Text["createJob.notEnoughArgs"]}");
        }
        if (manager.AddBackupJobInList(args[0], args[1], args[2], args[3]))
        {
            context.Console.WriteLine(context.Text["createJob.success"]);
            List<BackupJob> lalistemdr = manager.GetList();
            foreach (BackupJob BckpJob in lalistemdr)
            {
                context.Console.WriteLine($"Debug : {BckpJob.Name}");
            }
        } else context.Console.WriteLine(context.Text["createJob.error"]);
        return Task.FromResult(0);
    }
}