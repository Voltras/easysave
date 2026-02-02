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
        if (args.Length > 0)
        {
            return await router.RouteAsync(ctx, args, ct);
        }

        var shell = sp.GetRequiredService<ShellState>();
        console.WriteLine(text["shell.welcome"]);
        console.WriteLine(text["shell.hint"]);

        int lastCode = 0;

        while (shell.Running && !ct.IsCancellationRequested)
        {
            console.Write("> ");
            var userInput = console.ReadLine();
            if (userInput is null)
            {
                break;
            }
            var tokens = CommandLineTokenizer.Tokenize(userInput);
            if (tokens.Length == 0) continue;
            lastCode = await router.RouteAsync(ctx, tokens, ct);
        }
        return lastCode;
    }

    protected virtual void ConfigureServices(IServiceCollection services) { }
    protected abstract void ConfigureCommands(CommandRegistry registry, IServiceProvider sp);
    protected abstract IText BuildText(CultureInfo culture);
    /*
     * TODO :
     * Implémenter une commande de settings pour changer la langue de la console
     */
    protected virtual CultureInfo ResolveCulture(string[] args) // Je voulais l'implémenter en commande mais on fera autrement ça marche pas avec le système actuel
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