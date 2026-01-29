using System.Globalization;
using Microsoft.Extensions.DependencyInjection;

namespace EasyCli;

public abstract class CliApplication
{
    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConsole, SystemConsole>();
        services.AddSingleton<CommandRegistry>();
        services.AddSingleton<CommandRouter>();
        services.AddSingleton<ShellState>();

        ConfigureServices(services);

        using var sp = services.BuildServiceProvider();

        var console = sp.GetRequiredService<IConsole>();
        var culture = ResolveCulture(args);
        var text = BuildText(culture);

        var ctx = new CommandContext(console, text, sp, culture);

        var registry = sp.GetRequiredService<CommandRegistry>();
        ConfigureCommands(registry, sp);

        var router = sp.GetRequiredService<CommandRouter>();
        /*
         * TODO : Retour sous conditions (argument > 0)
         * Ajouter un mode interactif. Le statut du shell est déjà ajouté aux services (GetRequiredService<ShellState>(); pour récupérer le statut booléen)
         * Utiliser un console.readline pour le mode interactif bouclé sur while shell.running et CTRL+C.
         */
        return await router.RouteAsync(ctx, args, ct);
    }

    protected virtual void ConfigureServices(IServiceCollection services) { }
    protected abstract void ConfigureCommands(CommandRegistry registry, IServiceProvider sp);
    protected abstract IText BuildText(CultureInfo culture);
    protected virtual CultureInfo ResolveCulture(string[] args)
    {
        var idx = Array.FindIndex(args, a => a.Equals("--lang", StringComparison.OrdinalIgnoreCase));
        if (idx >= 0 && idx + 1 < args.Length)
        {
            var lang = args[idx + 1].Trim().ToLowerInvariant();
            if (lang is "fr" or "fr-fr") return CultureInfo.GetCultureInfo("fr-FR");
            if (lang is "en" or "en-us") return CultureInfo.GetCultureInfo("en-US");
        }

        return CultureInfo.CurrentUICulture;
    }
}