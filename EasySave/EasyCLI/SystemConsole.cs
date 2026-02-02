namespace EasyCli;

public sealed class SystemConsole : IConsole
{
    public void Write(string message) => Console.Write(message);

    public void WriteLine(string message = "") => Console.WriteLine(message);

    public void WriteError(string message)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(message);
        Console.ForegroundColor = prev;
    }

    public string? ReadLine() => Console.ReadLine();
}