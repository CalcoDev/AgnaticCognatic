using AgnaticCognaticBot.Commands;
using AgnaticCognaticBot.Database;
using AgnaticCognaticBot.Helpers;
using AgnaticCognaticBot.Interactions;
using AgnaticCognaticBot.Services;
using Discord;
using Discord.WebSocket;
using NLog;

namespace AgnaticCognaticBot;

public class Bot
{
    public const ulong TestGuildId = 1000484275818352660;
    
    public readonly DiscordSocketClient Client;
    public readonly DatabaseClient DatabaseClient;
    
    public readonly ServiceHandler ServiceHandler;
    public readonly CommandHandler CommandHandler;
    public readonly InteractionHandler InteractionHandler;

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

        DatabaseClient = new DatabaseClient();

        Client = new DiscordSocketClient(new DiscordSocketConfig()
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 50,
            UseInteractionSnowflakeDate = false
        });
        Client.Log += message => Logging.Log(message, _logger);

        ServiceHandler = new ServiceHandler(this);
        CommandHandler = new CommandHandler(this, ">>");
        InteractionHandler = new InteractionHandler(this);
    }

    public async Task StartClient()
    {
        await Client.LoginAsync(TokenType.Bot, _token);
        await Client.StartAsync();
        
        Client.Ready += ClientReady;

        await Update();

        await Task.Run(Update);
    }
    
    private async Task ClientReady()
    {
        await DatabaseClient.InitClient();
        await CommandHandler.InitCommands();
        await InteractionHandler.InitInteractions();
        
        await Client.SetGameAsync("For the grace for the might of our Lord.");
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
        _logger.Info("Shutting down ...");
        LogManager.Shutdown();
        
        await Client.LogoutAsync();
        await Client.StopAsync();
    }

    public void StopClient()
    {
        _running = false;
    }
}