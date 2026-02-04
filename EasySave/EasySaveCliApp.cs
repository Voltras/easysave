using EasyCli;
using EasySave.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

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
        registry.Register(new RemoveJobCommand(manager));
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
                ["createJob.desc"] = "Ajoute un job à la liste des jobs de copie. Usage : add-job <nom> <source> <destination> <type (différentiel / complet)>",
                ["createJob.success"] = "Le job de sauvegarde a été enregistrée dans la liste d'attente !",
                ["createJob.error"] = "Le type de sauvegarde renseigné est incorrect ou la liste d'attente a atteint le nombre maximum de sauvegardes ! (5 sauvegardes max)",
                ["createJob.notEnoughArgs"] = "Vous n'avez pas entré assez d'arguments. Usage : add-job <nom> <source> <destination> <type (différentiel / complet)>",
                ["executeJob.desc"] = "Exécute un ou des jobs dans la liste des jobs de copie. Usage: execute-job <Optionnel : sequentiel>",
                ["error.NoAvailableJobs"] = "Aucun job disponible. Ajoutez en avec add-job. Usage : add-job <nom> <source> <destination> <type (différentiel / complet)>",
                ["table.chooseJob"] = "Choisissez un job parmi la liste :",
                ["table.startSequentialTitle"] = "Choisissez le premier job (séquentiel plage) :",
                ["table.endSequentialTitle"] = "Choisissez le dernier job (séquentiel plage) :",
                ["table.executeJobDesc"] = "Jobs disponibles :",
                ["executeJob.chooseSeqMode"] = "Choisissez le mode d'exécution séquentielle :",
                ["executeJob.rangeChoice"] = "Plage",
                ["executeJob.individualChoice"] = "Individuel",
                ["removeJob.desc"] = "Supprime un job de la liste. Usage : remove-job <Argument optionnel pour nettoyer la liste entière : nettoyer>",
                ["removeJob.argumentError"] = "L'argument renseigné est invalide. Usage : remove-job <Argument optionnel pour nettoyer la liste entière : nettoyer>",
                ["removeJob.emptyList"] = "La liste est déjà vide !",
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
                ["createJob.error"] = "The type of save you entered is incorrect or the pending job list is already full ! (5 saves max)",
                ["createJob.notEnoughArgs"] = "You did not enter enough arguments. Usage : add-job <name> <source> <destination> <type (differential / full)>",
                ["executeJob.desc"] = "Executes jobs from the pending list. Usage: execute-job <Optional : sequential>",
                ["error.NoAvailableJobs"] = "No available jobs. Add one with the add-job command. Usage : add-job <name> <source> <destination> <type (differential / full)>",
                ["table.chooseJob"] = "Choose a job within the list :",
                ["table.startSequentialTitle"] = "First job index (sequential) :",
                ["table.endSequentialTitle"] = "Last job index (sequential) :",
                ["table.executeJobDesc"] = "Available jobs :",
                ["executeJob.chooseSeqMode"] = "Choose the sequential execution mode :",
                ["executeJob.rangeChoice"] = "Range",
                ["executeJob.individualChoice"] = "Individual",
                ["removeJob.desc"] = "Deletes a job from the pending list. Usage : remove-job <Optional argument to clear the whole list : clear>",
                ["removeJob.argumentError"] = "The given argument is invalid. Usage : remove-job <Optional argument to clear the whole list : clear>",
                ["removeJob.emptyList"] = "The list is already empty !",
            };

        return new DictionaryText(strings, culture);
    }
}
