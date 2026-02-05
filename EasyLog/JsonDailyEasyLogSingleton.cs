using System;

namespace EasyLog;

public sealed class JsonDailyEasyLogSingleton 
{
    private JsonDailyEasyLogSingleton() { }

    private static JsonDailyEasyLog? _instance;
    private static readonly object _sync = new();

    /// <summary>
    /// Retourne l'instance unique de `JsonDailyEasyLog`.
    /// Si l'instance n'existe pas, elle est créée avec les paramètres fournis.
    /// Les appels ultérieurs ignorent `jobName` et `options`.
    /// </summary>
    public static JsonDailyEasyLog GetInstance(string? jobName = null, EasyLogOptions? options = null)
    {
        if (_instance is not null) return _instance;

        lock (_sync)
        {
            if (_instance is null)
            {
                var name = string.IsNullOrWhiteSpace(jobName) ? "UnamedJob" : jobName!;
                _instance = new JsonDailyEasyLog(name, options);
            }
        }

        return _instance;
    }

    /// <summary>
    /// Dispose et réinitialise l'instance unique (utile en fin d'application / tests).
    /// </summary>

}