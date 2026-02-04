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
            
            BackupJobs.Add(new BackupJob("Travail 1", @"C:\Users\laara\OneDrive\Bureau\Seminaire science fondamentale de l'ingénieur\prosit 2", @"C:\Users\laara\OneDrive\Bureau\repo_de_copie", BackupType.Full));
            BackupJobs.Add(new BackupJob("Travail 2", @"C:\Users\laara\OneDrive\Bureau\english", @"C:\Users\laara\OneDrive\Bureau\Bloc_Csharp\Projet\TEST", BackupType.Full));
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

        // todo gerer le max de job 5 
        public void RunSequential(int start, int end)
        {

            if (start < 1 || end > BackupJobs.Count || start > end)
            {
                Console.WriteLine("Plage de jobs invalide.");
                return;
            }

            if (end - start + 1 > 5)
            {
                Console.WriteLine("Le nombre maximum de jobs à exécuter est de 5.");
                return;
            }
            for (int i = start; i <= end; i++)
            {
                RunJob(i);
            }
        }
    }
}