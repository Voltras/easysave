using System.Globalization;

namespace EasyCli;

public static class CultureHelper
{
    public static bool TryParse(string? value, out CultureInfo culture)
    {
        culture = CultureInfo.InvariantCulture;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        // Normalisation de l'entrée utilisateur, on remplace _ avec - pour uniformer le tout
        var v = value.Trim().Replace('_', '-').ToLowerInvariant();

        // Normalisation. Il est possible de rajouter des valeurs ici si nécessaire.
        if (v is "fr" or "fr-fr" or "france" or "fra")
        {
            culture = CultureInfo.GetCultureInfo("fr-FR");
            return true;
        }

        if (v is "en" or "en-us" or "english" or "eng")
        {
            culture = CultureInfo.GetCultureInfo("en-US");
            return true;
        }

        try
        {
            culture = CultureInfo.GetCultureInfo(v);
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }
}
