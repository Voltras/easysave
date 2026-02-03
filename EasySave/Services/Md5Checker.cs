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
            Console.WriteLine("Source Hash: " + sourceHash);
            string targetHash = CalculateHash(targetFilePath);
            Console.WriteLine("Target Hash: " + targetHash);

            if (sourceHash != targetHash)
            {
                return true;
            }

            return false;
        }

        private string CalculateHash(string path)
        {

            using var md5 = System.Security.Cryptography.MD5.Create();
            Console.WriteLine("Calculating MD5 for: " + path);
            using var stream = File.OpenRead(path);
            Console.WriteLine("File opened successfully for hashing."+ stream);
            var hash = md5.ComputeHash(stream);
            Console.WriteLine("Hash computed: " + hash);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
