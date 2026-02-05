using System.Text;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        var app = new EasySaveCliApp();
        return await app.RunAsync(args);
    }
}