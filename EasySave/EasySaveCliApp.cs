using System.Globalization;
using EasyCli;

public sealed class EasySaveCliApp : CliApplication
{
    protected override void ConfigureCommands(CommandRegistry registry, IServiceProvider sp)
    {
        registry.Register(new HelpCommand(registry));
        registry.Register(new ExitCommand());
    }

    protected override IText BuildText(CultureInfo culture)
    {
        var isFr = culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.OrdinalIgnoreCase);

        var strings = isFr
            ? new Dictionary<string, string>
            {
                ["help.title"] = "Aide - commandes disponibles",
                ["error.unknown_command"] = "Commande inconnue",
            }
            : new Dictionary<string, string>
            {
                ["help.title"] = "Help - available commands",
                ["error.unknown_command"] = "Unknown command",
            };

        return new DictionaryText(strings, culture);
    }
}