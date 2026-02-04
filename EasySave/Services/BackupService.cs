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
        public void ExecuteBackup(BackupJob backupJob)
        {
            Console.WriteLine($"demarrage du job  {backupJob.Name}");

            // On vérifie que la source existe, sinon on arrête tout
            if (!Directory.Exists(backupJob.SourcePath))
            {
                Console.WriteLine(" le dossier source n existe pas");
                return;
            }
        

            // On lance la boucle récursive
            CopyDirectory(backupJob.SourcePath, backupJob.TargetPath, backupJob.Type, backupJob);

            Console.WriteLine($"job {backupJob.Name} fini");
        }

        public void CopyDirectory(string sourceDir, string targetDir, BackupType type, BackupJob backupJob)
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            string[] files = Directory.GetFiles(sourceDir);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);

                CopyFile(file, destFile, type,backupJob);
            }

            string[] subDirs = Directory.GetDirectories(sourceDir);

            foreach (string subDir in subDirs)
            {
                string dirName = new DirectoryInfo(subDir).Name;

                string nextTargetDir = Path.Combine(targetDir, dirName);

                CopyDirectory(subDir, nextTargetDir, type, backupJob);
            }
        }

        public void CopyFile(string sourceFile, string destFile, BackupType type, BackupJob backupJob)
        {
            try
            {
                bool copy = false;

                if (type == BackupType.Full)
                {
                    copy = true;
                }
                else if (type == BackupType.Differential)
                {
                    if (!File.Exists(destFile))
                    {
                        copy = true;
                    }
                    else
                    {
                        bool isDifferent = new Md5Checker().ShouldCopy(sourceFile, destFile);

                        if (isDifferent)
                        {
                            copy = true;
                        }
                    }
                }

                if (copy)
                {
                    File.Copy(sourceFile, destFile, true);
                    Console.WriteLine($"[COPIE] {sourceFile} -> {destFile}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur sur le fichier {sourceFile} : {ex.Message}");
            }
        }
    }
}
