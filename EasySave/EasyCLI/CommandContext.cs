using System.Globalization;
using Spectre.Console;

namespace EasyCli;

public sealed class CommandContext
{
    public IAnsiConsole Console { get; }
    public IText Text { get; }
    public IServiceProvider Services { get; }
    public CultureInfo Culture { get; }

    public CommandContext(IAnsiConsole console, IText text,  IServiceProvider services, CultureInfo culture)
    {
        Console = console;
        Text = text;
        Services = services;
        Culture = culture;
    }
}