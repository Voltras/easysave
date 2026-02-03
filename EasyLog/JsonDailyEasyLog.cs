using System.Text.Json;

namespace EasyLog;

public sealed class JsonDailyEasyLog : IEasyLog
{
    private readonly string _jobName;
    private readonly EasyLogOptions _options;
    private readonly object _lock = new();

    private StreamWriter? _writer;
    private DateOnly _currentDay;
    private string _currentFilePath = string.Empty;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = false
    };

    public JsonDailyEasyLog(string jobName, EasyLogOptions? options = null)
    {
        _jobName = string.IsNullOrWhiteSpace(jobName)? "UnamedJob" : jobName;
        _options = options ?? new EasyLogOptions();
        RotateIfNeeded();
    }

    public void CreateLog(
        LogAction action,
        string sourcePath,
        string destinationPath,
        long fileSizeBytes,
        long transferTimeMs,
        string? error = null)
    {
        var now = _options.UseUtc ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
        var endUnix = now.ToUnixTimeSeconds();

        long? startUnix = null;
        if(transferTimeMs >= 0)
        {
            var deltaSec = transferTimeMs / 1000;
            startUnix = endUnix - deltaSec;
        }
        var entry = new
        {
            timestamp = now.ToString("O"),
            jobName = _jobName,
            action = action.ToString(),
            sourcePath = NormalizePath(sourcePath),
            destinationPath = NormalizePath(destinationPath),
            fileSizeBytes,
            transferTimeMs,
            timestampStartUnix = startUnix,
            timestampEndUnix = endUnix,
            error
        };

        var line = JsonSerializer.Serialize(entry, JsonOpts);

        lock (_lock)
        {
            RotateIfNeeded();
            _writer!.WriteLine(line);
            if (_options.FlushEachWrite)
            {
                _writer.Flush();
            }
        }
    }

    private void RotateIfNeeded() 
    {
        var now = _options.UseUtc ? DateTimeOffset.UtcNow : DateTimeOffset.Now;

        var day = DateOnly.FromDateTime(now.DateTime);

        if (_writer is not null && day == _currentDay) return;

        _writer?.Dispose();
        _currentDay = day;

        var baseDir = _options.BaseDirectory;
        if (string.IsNullOrWhiteSpace(baseDir))
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            baseDir = Path.Combine(programData, "EasySave", "Logs");
        }

        Directory.CreateDirectory(baseDir);

        _currentFilePath = Path.Combine(baseDir, $"{_currentDay:yyyy-MM-dd}.json");

        var fs = new FileStream(
            _currentFilePath,
            FileMode.Append,
            FileAccess.Write,
            FileShare.Read
        );

        _writer = new StreamWriter(fs)
        {
            AutoFlush = _options.FlushEachWrite
        };
    }

    private static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        if(path.StartsWith(@"\\", StringComparison.Ordinal))
        {
            return path;
        }
        return Path.GetFullPath(path);
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _writer?.Dispose();
            _writer = null;
        }
    }
}