namespace AgnaticCognaticBot;

public static class Program
{
    private static readonly string? Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

    public static Task Main(string[] args)
    {
        if (Token == null)
        {
            Console.Error.WriteLine("[Error]: No token found. Please check environment variables!");
            return Task.CompletedTask;
        }
        
        var bot = new Bot(Token);
        return bot.StartClient();
    }
}