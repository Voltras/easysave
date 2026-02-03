using EasySave.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Services
{
    class Md5Checker
    {
        public bool ShouldCopy(BackupJob backupJob)
        {
            string sourceFilePath = backupJob.SourcePath;
            string targetFilePath = backupJob.TargetPath;

            if (File.Exists(targetFilePath))
            {
                return true;
            }

            string sourceHash = CalculateHash(sourceFilePath);
            string targetHash = CalculateHash(targetFilePath);

            if (sourceHash != targetHash)
            {
                return true;
            }

            return false;
        }

        private string CalculateHash(string path)
        {

            using var md5 = System.Security.Cryptography.MD5.Create();
            using var stream = File.OpenRead(path);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
