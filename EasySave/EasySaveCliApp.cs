using System.Globalization;
using EasyCli;

public sealed class EasySaveCliApp : CliApplication
{
    protected override void ConfigureCommands(CommandRegistry registry, IServiceProvider sp)
    {
        registry.Register(new HelpCommand(registry));
        registry.Register(new ExitCommand());
        registry.Register(new SettingsCommand());
    }

    protected override IText BuildText(CultureInfo culture)
    {
        var isFr = culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.OrdinalIgnoreCase);

        var strings = isFr // Pour debug il est possible de remplacer isFr par !isFr le temps d'implémenter le changement de langage
            ? new Dictionary<string, string>
            {
                ["help.title"] = "Aide - commandes disponibles",
                ["shell.welcome"] = "Bienvenue sur EasySave CLI version !",
                ["shell.hint"] = "Entrez 'help' pour voir les commandes disponibles.",
                ["exit.bye"] = "Au revoir !",
                ["error.unknown_command"] = "Commande inconnue",
                ["help.desc"] = "Affiche le menu d'aide",
                ["exit.desc"] = "Quitte l'application",
                ["settings.desc"] = "Ouvre le menu de paramètres WIP",
            }
            : new Dictionary<string, string>
            {
                ["help.title"] = "Help - available commands",
                ["shell.welcome"] = "Welcome to EasySave CLI version !",
                ["shell.hint"] = "Enter 'help' to see available commands.",
                ["exit.bye"] = "See you !",
                ["error.unknown_command"] = "Unknown command",
                ["help.desc"] = "Shows the help menu",
                ["exit.desc"] = "Exits the application",
                ["settings.desc"] = "Opens the settings menu (WIP)",
            };

        return new DictionaryText(strings, culture);
    }
}