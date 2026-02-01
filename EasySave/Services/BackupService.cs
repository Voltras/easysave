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
        public void ExecuteBackup(EasySave.Models.BackupJob job)
        {
            Console.WriteLine($"demarrage du job  {job.Name}");

            // On vérifie que la source existe, sinon on arrête tout
            if (!Directory.Exists(job.SourcePath))
            {
                Console.WriteLine(" le dossier source n existe pas");
                return;
            }
        

            // On lance la boucle récursive
            CopyDirectory(job.SourcePath, job.TargetPath, job.Type);

            Console.WriteLine($"job {job.Name} fini");
        }

        public void CopyDirectory(string sourceDir, string targetDir, BackupType type)
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

                CopyFile(file, destFile, type);
            }

            string[] subDirs = Directory.GetDirectories(sourceDir);

            foreach (string subDir in subDirs)
            {
                string dirName = new DirectoryInfo(subDir).Name;

                string nextTargetDir = Path.Combine(targetDir, dirName);

                CopyDirectory(subDir, nextTargetDir, type);
            }
        }

        public void CopyFile(string sourceFile, string destFile, BackupType type)
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
                        DateTime sourceTime = File.GetLastWriteTime(sourceFile);
                        DateTime destTime = File.GetLastWriteTime(destFile);

                        if (sourceTime > destTime)
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
