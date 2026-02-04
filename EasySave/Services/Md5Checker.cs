using EasySave.Models;
using System;
using System.IO;
using System.Security.Cryptography;

namespace EasySave.Services
{
    class Md5Checker
    {
        public bool ShouldCopy(string sourceFilePath, string targetFilePath)
        {
            if (!File.Exists(sourceFilePath))
                return false; // source introuvable -> rien à copier

            if (!File.Exists(targetFilePath))
                return true; // cible absente -> copier

            string sourceHash = CalculateHash(sourceFilePath);
            string targetHash = CalculateHash(targetFilePath);

            return !string.Equals(sourceHash, targetHash, StringComparison.OrdinalIgnoreCase);
        }

        public string CalculateHash(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Fichier introuvable pour le calcul du hash.", path);

            var attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                throw new ArgumentException("Le chemin fourni est un répertoire, pas un fichier : " + path);

            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
