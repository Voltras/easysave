using System.Globalization;

namespace EasyCli;

public sealed class DictionaryText : IText
{
    private readonly IReadOnlyDictionary<string, string> _strings;
    public CultureInfo Culture { get; }


    public DictionaryText(IReadOnlyDictionary<string, string> strings, CultureInfo culture)
    {
        _strings = strings;
        Culture = culture;
    }

    public string this[string key] => _strings.TryGetValue(key, out var v) ? v : key;
}