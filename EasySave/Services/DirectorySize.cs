namespace EasySave.Services;

public static class DirectorySize
{
    public static long SizeInfo(this DirectoryInfo directory)
    {
        // On récupère la taille de tous les fichiers dans le dossier et les sous-dossiers
        return directory.EnumerateFiles("*", SearchOption.AllDirectories)
                        .Sum(file => file.Length);
    }
}

   

