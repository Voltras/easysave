namespace EasyLog;

public sealed record EasyLogOptions(
    string? BaseDirectory = null, // Null revient à dire ProgramData\EasySave\Logs mais c'est personnalisable
    bool UseUtc = true,
    bool FlushEachWrite = true
    );