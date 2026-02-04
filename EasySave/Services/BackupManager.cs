using System.Collections.Generic;
using EasyCli;
using EasySave.Models;

namespace EasySave.Services
{
    public class BackupManager
    {
        public List<BackupJob> BackupJobs { get; set; }
        private BackupService _backupService;

        private static IReadOnlyList<string> AliasesBackupTypeFull => ["full", "complète", "complet", "complete"];
        private static IReadOnlyList<string> AliasesBackupTypeDifferential => ["différentiel", "differential", "diff"];

        public BackupManager()
        {
            _backupService = new BackupService();
            BackupJobs = new List<BackupJob>();

            // simu :2 travaux de test Hardcodé pour test
            
            //BackupJobs.Add(new BackupJob("Travail 1", @"C:\Users\laara\OneDrive\Bureau\Seminaire science fondamentale de l'ingénieur\prosit 2", @"C:\Users\laara\OneDrive\Bureau\repo_de_copie", BackupType.Full));
            //BackupJobs.Add(new BackupJob("Travail 2", @"C:\Users\laara\OneDrive\Bureau\english", @"C:\Users\laara\OneDrive\Bureau\Bloc_Csharp\Projet\TEST", BackupType.Full));
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
                throw new ArgumentException("Invalid job listing.");
            }

            if (end - start + 1 > 5)
            {
                throw new ArgumentException ("Too many jobs to execute, limit -> 5.");
            }
            for (int i = start; i <= end; i++)
            {
                RunJob(i);
            }
        }

        public bool AddBackupJobInList(string name , string sourcePath, string targetPath, string type)
        {
            if (AliasesBackupTypeFull.Contains(type.ToLowerInvariant()))
            {
                BackupType objecttype = BackupType.Full;
                BackupJobs.Add(new BackupJob(name, sourcePath, targetPath, objecttype));
                return true;
            }


            if (AliasesBackupTypeDifferential.Contains(type.ToLowerInvariant()))
            {
                BackupType objecttype = BackupType.Full;
                BackupJobs.Add(new BackupJob(name, sourcePath, targetPath, objecttype));
                return true;
            }

            else
            {
                throw new ArgumentException("Le type de copie n'est pas valide !");
            }    

        }

        public void ListAllJobs(List<BackupJob>) 
        {
            foreach (var backupjob in BackupJobs)
            {
                Console.WriteLine($"Job Name: {backupjob.Name}, Source: {backupjob.SourcePath}, Target: {backupjob.TargetPath}, Type: {backupjob.Type}");
            }

        }
        public List<BackupJob> GetList()
        {
            return BackupJobs;
        }

        public void DeleteBackupJob(int index)
        {
            foreach (var backupjob in BackupJobs)
            {
                if (BackupJobs.IndexOf(backupjob) == index - 1)
                {
                    BackupJobs.Remove(backupjob);
                }
            }
        }
    }
}

// add singleton constructor for the lsit of backup jobs 