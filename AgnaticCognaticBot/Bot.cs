using AgnaticCognaticBot.Commands;
using AgnaticCognaticBot.Services;
using Discord;
using Discord.WebSocket;

namespace AgnaticCognaticBot;

public class Bot
{
    public readonly DiscordSocketClient Client;
    
    public readonly ServiceHandler ServiceHandler;
    public readonly CommandHandler CommandHandler;

    private readonly string _token;
    private bool _running;

    public Bot(string token)
    {
        _running = true;
        _token = token;
        
        Client = new DiscordSocketClient(new DiscordSocketConfig()
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 50
        });
        Client.Log += Log;

        ServiceHandler = new ServiceHandler(this);
        CommandHandler = new CommandHandler(this);
    }

    public async Task StartClient()
    {
        await CommandHandler.InitCommands();
        
        await Client.LoginAsync(TokenType.Bot, _token);
        await Client.StartAsync();

        await Update();
    }

    private async Task Update()
    {
        while (_running)
        {
            // Do stuff each "frame"
        }

        await Client.LogoutAsync();
        await Client.StopAsync();
    }

    public void StopClient()
    {
        _running = false;
    }

    private Task Log(LogMessage logMessage)
    {
        Console.WriteLine(logMessage.ToString());
        return Task.CompletedTask;
    }
}