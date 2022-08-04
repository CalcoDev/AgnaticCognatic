using AgnaticCognaticBot.Commands;
using AgnaticCognaticBot.Interactions;
using AgnaticCognaticBot.Services;
using Discord;
using Discord.WebSocket;
using NLog;

namespace AgnaticCognaticBot;

public class Bot
{
    public readonly DiscordSocketClient Client;
    
    public readonly ServiceHandler ServiceHandler;
    public readonly CommandHandler CommandHandler;
    // public readonly InteractionHandler InteractionHandler;

    public const int RefreshInterval = 1000;

    private readonly Logger _logger;
    
    private readonly string _token;
    private bool _running;

    public Bot(string token)
    {
        LogManager.LoadConfiguration("nlog.config.xml");
        _logger = LogManager.GetCurrentClassLogger();
        
        _running = true;
        _token = token;
        
        Client = new DiscordSocketClient(new DiscordSocketConfig()
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 50
        });
        Client.Log += Log;

        ServiceHandler = new ServiceHandler(this);
        CommandHandler = new CommandHandler(this, ">>");
        // InteractionHandler = new InteractionHandler(this);
    }

    public async Task StartClient()
    {
        await CommandHandler.InitCommands();
        // await InteractionHandler.InitInteractions();
        
        await Client.LoginAsync(TokenType.Bot, _token);
        await Client.StartAsync();
        
        await Update();

        await Task.Run(Update);

        // await Task.Delay(-1);
    }

    private async Task Update()
    {
        while (_running)
        {
            await Task.Delay(RefreshInterval);
        }

        await Disconnect();
    }
    
    private async Task Disconnect()
    {
        _logger.Info("Shutting down logger...");
        LogManager.Shutdown();
        
        await Client.LogoutAsync();
        await Client.StopAsync();
    }

    public void StopClient()
    {
        _running = false;
    }

    private Task Log(LogMessage logMessage)
    {
        string message = $"{logMessage.Source}: {logMessage.Message} {logMessage.Exception}";
        
        switch (logMessage.Severity)
        {
            case LogSeverity.Critical:
                _logger.Fatal(message);
                break;
            case LogSeverity.Error:
                _logger.Error(message);
                break;
            case LogSeverity.Warning:
                _logger.Warn(message);
                break;
            case LogSeverity.Info:
                _logger.Info(message);
                break;
            case LogSeverity.Verbose:
                break;
            case LogSeverity.Debug:
                _logger.Debug(message);
                break;
            default:
                _logger.Warn("Unknown log severity! " + message);
                break;
        }
        
        return Task.CompletedTask;
    }
}