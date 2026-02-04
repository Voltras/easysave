using System.Text;

namespace EasyCli;
/*
 * Normalisation des entrées.
 * Grâce aux tokens, chaque séparation d'arguments par espace finit dans une liste de strings
 * Chaque string est tokenizé pour la normalisation et l'échappement des caractères.
 */
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