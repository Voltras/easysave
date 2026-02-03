using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO; 
using static EasySave.Models.BackupJob;
using EasySave.Models;

namespace EasySave.Services
{
    class BackupService
    {
        public void ExecuteBackup(EasySave.Models.BackupJob backupJob)
        {
            Console.WriteLine($"demarrage du job  {backupJob.Name}");

            // On vérifie que la source existe, sinon on arrête tout
            if (!Directory.Exists(backupJob.SourcePath))
            {
                Console.WriteLine(" le dossier source n'existe pas");
                return;
            }
        
            // On lance la boucle récursive
            CopyDirectory(backupJob);

            Console.WriteLine($"job {backupJob.Name} fini");
        }

        public void CopyDirectory(BackupJob backupjob)
        {
            BackupJob _backupjob = backupjob;
            string sourceDir = backupjob.SourcePath;
            string targetDir= backupjob.TargetPath;
            BackupType type= backupjob.Type;

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            string[] files = Directory.GetFiles(sourceDir);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);

                CopyFile(backupjob);
            }

            string[] subDirs = Directory.GetDirectories(sourceDir);

            foreach (string subDir in subDirs)
            {
                string dirName = new DirectoryInfo(subDir).Name;

                string nextTargetDir = Path.Combine(targetDir, dirName);

                CopyDirectory(backupjob);
            }
        }

        public void CopyFile(BackupJob backupJob)
        {
            string destfile = backupJob.TargetPath;
            string sourcefile = backupJob.SourcePath;
            BackupType type = backupJob.Type;

            try
            {
                
                // si condition respecter on peiu copier 
                bool copy = false;

                if (type == BackupType.Full)
                {
                    copy = true;
                }
                else if (type == BackupType.Differential)
                {
                    if (!File.Exists(destfile))
                    {
                        copy = true; 
                    }
                    else
                    {
                        bool isdifferent = new Md5Checker().ShouldCopy(backupJob);

                        if (isdifferent)
                        {
                            copy = true;
                        }
                    }
                }

                if (copy)
                {
                    File.Copy(sourcefile, destfile, true);


                    Console.WriteLine($"[COPIE] {sourcefile} -> {destfile}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur sur le fichier {sourcefile} : {ex.Message}");
            }
        }
    }
}
