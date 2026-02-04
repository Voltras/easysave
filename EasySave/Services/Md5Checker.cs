using System.Security.Cryptography;

namespace EasySave.Services
{
    class Md5Checker
    {
        public bool ShouldCopy(string sourceFilePath, string targetFilePath)
        {
            if (!File.Exists(sourceFilePath))
                return false;

            if (!File.Exists(targetFilePath))
                return true;

            string sourceHash = CalculateHash(sourceFilePath);
            string targetHash = CalculateHash(targetFilePath);

            return !string.Equals(sourceHash, targetHash, StringComparison.OrdinalIgnoreCase);
        }

        public string CalculateHash(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File not found, needed to calcul the hash.", path);

            var attribute = File.GetAttributes(path);
            if ((attribute & FileAttributes.Directory) == FileAttributes.Directory)
                throw new ArgumentException("the following pash is a directory not a file: " + path);

            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
