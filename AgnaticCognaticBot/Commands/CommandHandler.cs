using AgnaticCognaticBot.Commands.Modules;
using AgnaticCognaticBot.Helpers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NLog;

namespace AgnaticCognaticBot.Commands;

public class CommandHandler
{
    public readonly CommandService CommandService;
    
    private readonly Bot _bot;
    private readonly IServiceProvider _serviceProvider;

    private readonly Logger _logger;
    
    public string Prefix { get; set; }

    public CommandHandler(Bot bot)
    {
        _logger = LogManager.GetCurrentClassLogger();
        
        _bot = bot;
        _serviceProvider = bot.ServiceHandler.ServiceProvider;
        
        CommandService = new CommandService(new CommandServiceConfig()
        {
            LogLevel = LogSeverity.Info,
            CaseSensitiveCommands = false,
            IgnoreExtraArgs = true
        });

        CommandService.Log += message => Logging.Log(message, _logger);

        Prefix = ">>>";
    }
    
    public CommandHandler(Bot bot, string prefix) : this(bot)
    {
        Prefix = prefix;
    }

    private async Task HandleCommandAsync(SocketMessage socketMessage)
    {
        if (socketMessage is not SocketUserMessage message)
            return;

        if (message.Author.Id == _bot.Client.CurrentUser.Id || message.Author.IsBot)
            return;

        int pos = 0;
        if (message.HasStringPrefix(Prefix, ref pos, StringComparison.InvariantCultureIgnoreCase))
        {
            var context = new SocketCommandContext(_bot.Client, message);
            var result = await CommandService.ExecuteAsync(context, pos, _serviceProvider);
            
            _logger.Info("Received command: {0}", message.ToString()[2..]);
            
            if (!result.IsSuccess)
            {
                _logger.Warn("Running command failed: {0}", result.ErrorReason);
            }
        }
    }

    public async Task InitCommands()
    {
        await CommandService.AddModuleAsync<InfoModule>(_serviceProvider);
        await CommandService.AddModuleAsync<AdminModule>(_serviceProvider);
        
        _bot.Client.MessageReceived += HandleCommandAsync;
        
        _logger.Info("Initialised commands.");
    }
}