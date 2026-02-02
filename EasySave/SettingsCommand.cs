using EasyCli;
using Microsoft.Extensions.DependencyInjection;
/*
 * TODO :
 * J'avais pensé à une commande Settings pour anticiper peut-être d'autres paramètres que la langue
 * Ça implique l'utilisation d'un menu (Spectre.Console pour affichage)
 * -> Lib rendu ?? Ou intégration directe dans les commandes ? (Répétitions donc nan ça baisse la quali du code)
 * Settings ou juste commande Lang / Culture ? Pour changer la langue directement puisque c'est le seul besoin auquel répond le menu settings 
 */
public sealed class SettingsCommand : ICliCommand
{
    public string Name => "settings";
    public IReadOnlyList<string> Aliases => ["paramètres", "param", "sttgs"];
    public string Description => "settings.desc";

    public Task<int> ExecuteAsync(CommandContext context, string[] args, CancellationToken ct)
    {
        context.Console.WriteLine("Menu settings WIP !");
        return Task.FromResult(0);
    }
}