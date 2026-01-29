using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace EasyCli;

public sealed class CommandRouter
{
    private readonly CommandRegistry _registry;
    public CommandRouter(CommandRegistry registry)
    {
        _registry = registry;
    }

    public async Task<int> RouteAsync(CommandContext ctx, string[] args, CancellationToken ct)
    {
        if (args.Length == 0)
        {
            return await ShowHelpAsync(ctx); // Renvoie le menu d'aide lorsqu'on tape une entrée vide.
        }
        if (args[0].Equals("--help", StringComparison.OrdinalIgnoreCase) ||
            args[0].Equals("-h", StringComparison.OrdinalIgnoreCase))
        {
            return await ShowHelpAsync(ctx);
        }
        var cmdName = args[0];
        var cmdArgs = args.Skip(1).ToArray();

        if (!_registry.TryResolve(cmdName, out var cmd))
        {
            ctx.Console.WriteError(ctx.Text["error.unknown_command"] + ": " + cmdName);
            return 2;
        }
        return await cmd!.ExecuteAsync(ctx, cmdArgs, ct);
    }

    private Task<int> ShowHelpAsync(CommandContext ctx) 
    {
        ctx.Console.WriteLine(ctx.Text["help.title"]);
        foreach(var c in _registry.AllUnique().OrderBy(c => c.Name))
        {
            ctx.Console.WriteLine($"    {c.Name} - {c.Description}");
        }
        return Task.FromResult(0);
    }

    private static bool LooksLikeBackupSelection(string token)
    {
        return Regex.IsMatch(token, @"^\d+([;-]\d+)*(\;\d+([;-]\d+)*)*$");
    }
}