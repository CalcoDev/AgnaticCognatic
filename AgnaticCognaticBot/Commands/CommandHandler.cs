using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NLog;

namespace AgnaticCognaticBot.Commands;

public class CommandHandler
{
    private readonly Bot _bot;
    
    private readonly CommandService _commandService;
    private readonly IServiceProvider _serviceProvider;

    private readonly Logger _logger;
    
    public string Prefix { get; set; }

    public CommandHandler(Bot bot)
    {
        _logger = LogManager.GetCurrentClassLogger();
        
        _bot = bot;
        _serviceProvider = bot.ServiceHandler.ServiceProvider;
        
        _commandService = new CommandService(new CommandServiceConfig()
        {
            LogLevel = LogSeverity.Info,
            CaseSensitiveCommands = false,
            IgnoreExtraArgs = true
        });

        Prefix = ">>>";
    }
    
    public CommandHandler(Bot bot, string prefix) : this(bot)
    {
        Prefix = prefix;
    }

    private async Task HandleCommandAsync(SocketMessage socketMessage)
    {
        // Ignore messages from bots
        var message = socketMessage as SocketUserMessage;
        
        if (message == null)
            return;

        if (message.Author.Id == _bot.Client.CurrentUser.Id || message.Author.IsBot)
            return;

        int pos = 0;
        if (message.HasStringPrefix(Prefix, ref pos, StringComparison.InvariantCultureIgnoreCase))
        {
            var context = new SocketCommandContext(_bot.Client, message);
            var result = await _commandService.ExecuteAsync(context, pos, _serviceProvider);
            if (!result.IsSuccess)
            {
                _logger.Warn("Running command failed: {0}", result.ErrorReason);
            }
        }
    }

    public async Task InitCommands()
    {
        await _commandService.AddModuleAsync<InfoModule>(_serviceProvider);
        await _commandService.AddModuleAsync<AdminModule>(_serviceProvider);
        
        _bot.Client.MessageReceived += HandleCommandAsync;
    }
}