using Discord;
using Discord.WebSocket;

namespace AgnaticCognaticBot;

public class Bot
{
    private readonly DiscordSocketClient _client;
    private readonly string _token;

    private bool running = true;

    public Bot(string token)
    {
        running = true;
        _token = token;
        
        _client = new DiscordSocketClient(new DiscordSocketConfig()
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 50
        });
        _client.Log += Log;
    }

    public async Task StartClient()
    {
        await _client.LoginAsync(TokenType.Bot, _token);
        await _client.StartAsync();

        await Update();
    }

    private async Task Update()
    {
        while (running)
        {
            // Do stuff each "frame"
        }

        await StopClientAsync();
    }

    public void StopClient()
    {
        running = false;
    }

    private async Task StopClientAsync()
    {
        await _client.StopAsync();
    }

    private Task Log(LogMessage logMessage)
    {
        Console.WriteLine(logMessage.ToString());
        return Task.CompletedTask;
    }
}