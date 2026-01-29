using EasyCli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var app = new EasySaveCliApp();
        return await app.RunAsync(args);
    }
}