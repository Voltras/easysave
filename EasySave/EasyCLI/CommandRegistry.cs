/* Fichier qui contient le dictionnaire d'association de commandes => task
 * Garder le dictionnaire en readonly private. On utilise la méthode TryResolve pour récupérer une commande par nom / alias
 */

namespace EasyCli;

public sealed class CommandRegistry
{
    private readonly Dictionary<string, ICliCommand> _byName = new(StringComparer.OrdinalIgnoreCase);
    public void Register(ICliCommand command)
    {
        _byName[command.Name] = command;

        foreach (var a in command.Aliases)
        {
            _byName[a] = command;
        }
    }

    public bool TryResolve(string name, out ICliCommand? command) => _byName.TryGetValue(name, out command);
    public IEnumerable<ICliCommand> AllUnique() => _byName.Values.DistinctBy(c => c.Name);
}