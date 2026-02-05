using EasySave.Models;
using EasyLog;

namespace EasySave.Services
{
    class BackupService
    {
        private string _status = "";

        private readonly JsonDailyEasyLog _logger;

        public string Status
        {
            get { return _status; }
        }

        public BackupService()
        {
            // Utilise l'instance singleton partagée pour éviter d'ouvrir plusieurs handles sur le même fichier
            _logger = JsonDailyEasyLogSingleton.GetInstance("backup_log.json");
        }
        public bool ExecuteBackup(BackupJob backupJob)
        {
            if (!Directory.Exists(backupJob.SourcePath))
            {
                return false;
            }

            DirectoryInfo di = new DirectoryInfo(backupJob.SourcePath);
            long totalSize = di.SizeInfo(); 
            long processedSize = 0;

            CopyDirectory(backupJob.SourcePath, backupJob.TargetPath, backupJob.Type, backupJob, totalSize, ref processedSize);

            _status = "Completed";
            return true;
        }

        private void CopyDirectory(string sourceDir, string targetDir, BackupType type, BackupJob backupJob, long totalSize, ref long processedSize)
        {
            // Création du dossier cible s'il n'existe pas
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
                _logger.CreateLog(LogAction.CreateDirectory, sourceDir, targetDir, 0, 0);
            }

            // Traitement des fichiers du dossier actuel
            string[] files = Directory.GetFiles(sourceDir);
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);

                long fileSize = new FileInfo(file).Length;

                (long time, long bytes) = CopyFile(file, destFile, type, backupJob);

                if (time > 0) // Succès du transfert
                {
                    processedSize += bytes;
                    UpdateProgress(processedSize, totalSize);
                    _logger.CreateLog(LogAction.Transfer, file, destFile, time, bytes);
                }
                else if (time == 0) // Fichier ignoré (Skip)
                {

                    processedSize += fileSize;
                    UpdateProgress(processedSize, totalSize);
                    _logger.CreateLog(LogAction.Skip, file, destFile, time, bytes);
                }
                else if (time == -1) // Erreur
                {
                    _logger.CreateLog(LogAction.Error, file, destFile, time, bytes);
                }
            }

            string[] subDirs = Directory.GetDirectories(sourceDir);
            foreach (string subDir in subDirs)
            {
                string dirName = Path.GetFileName(subDir);
                string nextTargetDir = Path.Combine(targetDir, dirName);

                CopyDirectory(subDir, nextTargetDir, type, backupJob, totalSize, ref processedSize);
            }
        }

        private void UpdateProgress(long processed, long total)
        {
            double percentage = (total > 0) ? ((double)processed / total) * 100 : 100;

            _status = $"{Math.Round(percentage, 2)}%|{processed}|{total}";

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
                    watch.Stop();
                    long time = watch.ElapsedMilliseconds;

                    return (time, bytes);
                }
            }
            catch (Exception ex)
            {
                // Retourner -1 indique une erreur 
                return (-1,-1);
            }
            return (0,0);
        }
    }
}

// vvariable status take a s string initialiser a rien et des qu on met a jour 