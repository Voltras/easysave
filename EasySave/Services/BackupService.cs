using EasySave.Models;
using EasyLog;

namespace EasySave.Services
{
    class BackupService
    {
        private string _status = "";
        public required JsonDailyEasyLog _logger;

        public string Status
        {
            get { return _status; }

        }
        public void ExecuteBackup(BackupJob backupJob)
        {
            Console.WriteLine($"demarrage du job  {backupJob.Name}");

            // On vérifie que la source existe, sinon on arrête tout
            if (!Directory.Exists(backupJob.SourcePath))
            {
                throw new DirectoryNotFoundException("source directory not found");

            }


            // On lance la boucle récursive
            CopyDirectory(backupJob.SourcePath, backupJob.TargetPath, backupJob.Type, backupJob);

            Console.WriteLine($"job {backupJob.Name} fini");
        }

        private void CopyDirectory(string sourceDir, string targetDir, BackupType type, BackupJob backupJob)
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

                (long time,long bytes) = CopyFile(file, destFile, type, backupJob);

                    if (time > 0){_logger.CreateLog(LogAction.Skip, file, destFile, time, bytes);}

                    else if (time == -1){_logger.CreateLog(LogAction.Error, file, destFile, time, bytes);}

                    else { _logger.CreateLog(LogAction.Transfer, file, destFile, time, bytes);}

            }

            string[] subDirs = Directory.GetDirectories(sourceDir);

            foreach (string subDir in subDirs)
            {
                string dirName = new DirectoryInfo(subDir).Name;

                string nextTargetDir = Path.Combine(targetDir, dirName);

                CopyDirectory(subDir, nextTargetDir, type, backupJob);
            }
        }

        private (long,long) CopyFile(string sourceFile, string destFile, BackupType type, BackupJob backupJob)
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

                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    File.Copy(sourceFile, destFile, true);
                    long bytes = new FileInfo(destFile).Length;
                    Console.WriteLine($"[COPIE] {sourceFile} -> {destFile}");
                    watch.Stop();
                    long time = watch.ElapsedMilliseconds;
                    return (time,bytes);

                }
            }
            catch (Exception ex)
            {
                return (-1,-1);
                throw new Exception($"Error copying file {sourceFile} to {destFile}: {ex.Message}");


            }
            return (0,0);
        }
    }
}

// vvariable status take a s string initialiser a rien et des qu on met a jour 