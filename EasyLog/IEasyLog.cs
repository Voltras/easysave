namespace EasyLog;

public interface IEasyLog : IDisposable // IDisposable sert à ouvrir / fermer le fichier automatiquement.
{
    void CreateLog(
        LogAction action,
        string sourcepath,
        string destinationpath,
        long fileSizeBytes,
        long transferTimeMs,
        string? error = null
    );
}