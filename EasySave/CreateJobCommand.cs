//using EasyCli;
//using EasySave.Models;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Win32;
//using System.Runtime.CompilerServices;

//public sealed class CreateJobCommand : ICliCommand
//{
//    public List<BackupJob> _backupJobs;
//    public CreateJobCommand(List<BackupJob> BackupJobs) => _backupJobs = BackupJobs;
//    public string Name => "add-job";
//    public IReadOnlyList<string> Aliases => ["CreationTravaillage", "cj", "addJob", "ajouter-job", "create-job"];
//    public string Description => "createJob.desc";

//    public Task<int> ExecuteAsync(CommandContext context, string[] args, CancellationToken ct)
//    {
//        if(args.Length < 4)
//        {
//            context.Console.WriteError(context.Text["createJob.notEnoughArgs"]);
//        }
//        else if (_backupJobs.Count < 5)
//        {
//            context.Console.WriteLine(context.Text["createJob.success"]);
//        }
//        else context.Console.WriteError(context.Text["createJob.error"]);
//        return Task.FromResult(0);
//    }
//}