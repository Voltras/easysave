using System.Globalization;

namespace EasyCli;

public sealed class CommandContext
{
    public IConsole Console { get; }
    public IText Text { get; }
    public IServiceProvider Services { get; }
    public CultureInfo Culture { get; }

    public CommandContext(IConsole console, IText text,  IServiceProvider services, CultureInfo culture)
    {
        Console = console;
        Text = text;
        Services = services;
        Culture = culture;
    }
}