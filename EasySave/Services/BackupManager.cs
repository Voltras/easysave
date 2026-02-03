using System.Collections.Generic;
using EasyCli;
using EasySave.Models;

namespace EasySave.Services
{
    public class BackupManager
    {
        public List<BackupJob> BackupJobs { get; set; }
        private BackupService _backupService;

        public BackupManager()
        {
            _backupService = new BackupService();
            BackupJobs = new List<BackupJob>();

            // simu :2 travaux de test Hardcodé pour test
            
            BackupJobs.Add(new BackupJob("Travail 1", @"C:\Users\laara\OneDrive\Bureau\apprentissage react", @"C:\Users\laara\OneDrive\Bureau\repo_de_copie", BackupType.Full));
            BackupJobs.Add(new BackupJob("Travail 2", @"C:\Users\laara\OneDrive\Bureau\repo_de_prelevement", @"C:\Users\laara\OneDrive\Bureau\repo_de_copie", BackupType.Differential));
        }

        public void RunJob(int index) 
        {
            // On vérifie que le numéro demandé est valide
            if (index >= 1 && index <= BackupJobs.Count)
            {
                // On récupère le job 
                BackupJob jobToRun = BackupJobs[index - 1];

                // On le donne au service
                _backupService.ExecuteBackup(jobToRun);
            }
            else
            {
                Console.WriteLine($"num de job  pas bon{index}");
            }
        }

        // ex lancer une plage (1-3)
        public void RunSequential(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                RunJob(i);
            }
        }
    }
}