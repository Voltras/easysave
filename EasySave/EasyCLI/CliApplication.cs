using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.Globalization;

namespace EasyCli;

public abstract class CliApplication
{
    public async Task<int> RunAsync(string[] args, CancellationToken ct = default)
    {
        var services = new ServiceCollection();

        // services.AddSingleton<IConsole, SystemConsole>();
        services.AddSingleton<IAnsiConsole>(_ =>
        AnsiConsole.Create(new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.Detect,
            ColorSystem = ColorSystemSupport.Detect,
            Interactive = InteractionSupport.Detect,
        }));
        services.AddSingleton<CommandRegistry>();
        services.AddSingleton<CommandRouter>();
        services.AddSingleton<ShellState>();

        ConfigureServices(services);

        using var sp = services.BuildServiceProvider();

        var console = sp.GetRequiredService<IAnsiConsole>();
        var shell = sp.GetRequiredService<ShellState>();

        // Récup des arguments pour mode non-interactif (utile surtout pour --lang)
        var parsed = ParseGlobalOptions(args);

        // redéfinition de la culture si on l'appelle dans le mode non interactif
        var culture = parsed.CultureOverride ?? CultureInfo.CurrentUICulture;

        var text = BuildText(culture);
        var ctx = new CommandContext(console, text, sp, culture);

        var registry = sp.GetRequiredService<CommandRegistry>();
        ConfigureCommands(registry, sp);

        var router = sp.GetRequiredService<CommandRouter>();

        // Mode non-interactif
        if (parsed.RemainingArgs.Length > 0)
        {
            return await router.RouteAsync(ctx, parsed.RemainingArgs, ct);
        }

        // Mode interactif
        var panel = new Panel($"""
            
            [Cyan]
            
             /$$$$$$$$                                /$$$$$$                               
            | $$_____/                               /$$__  $$                              
            | $$        /$$$$$$   /$$$$$$$ /$$   /$$| $$  \__/  /$$$$$$  /$$    /$$ /$$$$$$ 
            | $$$$$    |____  $$ /$$_____/| $$  | $$|  $$$$$$  |____  $$|  $$  /$$//$$__  $$
            | $$__/     /$$$$$$$|  $$$$$$ | $$  | $$ \____  $$  /$$$$$$$ \  $$/$$/| $$$$$$$$
            | $$       /$$__  $$ \____  $$| $$  | $$ /$$  \ $$ /$$__  $$  \  $$$/ | $$_____/
            | $$$$$$$$|  $$$$$$$ /$$$$$$$/|  $$$$$$$|  $$$$$$/|  $$$$$$$   \  $/  |  $$$$$$$
            |________/ \_______/|_______/  \____  $$ \______/  \_______/    \_/    \_______/
                                           /$$  | $$                                        
                                          |  $$$$$$/                                        
                                           \______/                                         
            
            {text["shell.welcome"]}
            {text["shell.hint"]}
            [/]
            """).DoubleBorder().Expand().BorderColor(Color.Cyan);
        console.Write(panel);

        int lastCode = 0;
        while (shell.Running && !ct.IsCancellationRequested)
        {
            Console.Write("> ");
            var userInput = Console.ReadLine();
            if (userInput is null)
                break;

            var tokens = CommandLineTokenizer.Tokenize(userInput);
            if (tokens.Length == 0)
                continue;

            lastCode = await router.RouteAsync(ctx, tokens, ct);

            // Hot-reload language when requested by a command
            if (shell.RequestedCulture is not null)
            {
                culture = shell.RequestedCulture;
                shell.RequestedCulture = null;

                text = BuildText(culture);
                ctx = new CommandContext(console, text, sp, culture);
            }
        }

        return lastCode;
    }

    protected virtual void ConfigureServices(IServiceCollection services) { }
    protected abstract void ConfigureCommands(CommandRegistry registry, IServiceProvider sp);
    protected abstract IText BuildText(CultureInfo culture);

    protected virtual GlobalOptions ParseGlobalOptions(string[] args)
    {
        var remaining = new List<string>();
        CultureInfo? overrideCulture = null;

        for (int i = 0; i < args.Length; i++)
        {
            var a = args[i];

            // --lang fr ou -l fr
            if (a.Equals("--lang", StringComparison.OrdinalIgnoreCase) ||
                a.Equals("-l", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 < args.Length && CultureHelper.TryParse(args[i + 1], out var c))
                {
                    overrideCulture = c;
                    i++;
                    continue;
                }

                // Si lang ne possède aucun argument (juste --lang)
                if (i + 1 < args.Length)
                    i++;
                continue;
            }

            // support pour synthaxe --lang=fr
            if (a.StartsWith("--lang=", StringComparison.OrdinalIgnoreCase))
            {
                var v = a.Substring("--lang=".Length);
                if (CultureHelper.TryParse(v, out var c))
                    overrideCulture = c;
                continue;
            }

            remaining.Add(a);
        }

        return new GlobalOptions(overrideCulture, remaining.ToArray());
    }

    protected readonly record struct GlobalOptions(CultureInfo? CultureOverride, string[] RemainingArgs);
}
