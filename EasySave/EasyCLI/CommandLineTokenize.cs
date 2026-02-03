using System.Runtime.CompilerServices;
using System.Text;

namespace EasySave.EasyCLI;

public static class CommandLineTokenizer
{
    public static string[] Tokenize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Array.Empty<string>();
        }

        var tokens = new List<string>();
        var sb = new StringBuilder();

        bool inQuotes = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }
            if (!inQuotes && char.IsWhiteSpace(c))
            {
                if (sb.Length > 0)
                {
                    tokens.Add(sb.ToString());
                    sb.Clear();
                }
                continue;
            }
            sb.Append(c);
        }
        if (sb.Length > 0)
        {
            tokens.Add(sb.ToString());
        }
        return tokens.ToArray();
    }
}