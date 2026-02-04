using System.Globalization;
using EasyCli;
using EasySave.Models;
using Microsoft.Extensions.DependencyInjection;
using EasySave.Services;

public sealed class EasySaveCliApp : CliApplication
{
    private BackupManager manager = new BackupManager();
    protected override void ConfigureServices(IServiceCollection services)
    {
    }

    protected override void ConfigureCommands(CommandRegistry registry, IServiceProvider sp)
    {
        registry.Register(new HelpCommand(registry));
        registry.Register(new CreateJobCommand(manager));
        registry.Register(new ExitCommand());
        registry.Register(new ExecuteJobCommand(manager));
    }

    protected override IText BuildText(CultureInfo culture)
    {
        var isFr = culture.TwoLetterISOLanguageName.Equals("fr", StringComparison.OrdinalIgnoreCase);
        var strings = isFr
            ? new Dictionary<string, string>
            {
                ["help.title"] = "Aide - commandes disponibles",
                ["shell.welcome"] = "Bienvenue sur EasySave CLI version !",
                ["shell.hint"] = "Entrez 'help' pour voir les commandes disponibles.",
                ["exit.bye"] = "Au revoir !",
                ["error.unknown_command"] = "Commande inconnue",
                ["help.desc"] = "Affiche le menu d'aide",
                ["exit.desc"] = "Quitte l'application",
                ["createJob.desc"] = "Ajoute un travail à la liste des travaux de copie. Usage : add-job <nom> <source> <destination> <type (différentiel / complet)>",
                ["createJob.success"] = "Le travail de sauvegarde a été enregistrée dans la liste d'attente !",
                ["createJob.error"] = "La liste d'attente a atteint le nombre maximum de sauvegardes ! (5 sauvegardes max)",
                ["createJob.notEnoughArgs"] = "Vous n'avez pas entré assez d'arguments."
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
                ["createJob.desc"] = "Adds a job to the copying job list. Usage : add-job <name> <source> <destination> <type (differential / full)>",
                ["createJob.success"] = "The job was successfuly added to the pending job list.",
                ["createJob.error"] = "The pending job list is already full ! (5 saves max)",
            };

        return new DictionaryText(strings, culture);
    }
}
