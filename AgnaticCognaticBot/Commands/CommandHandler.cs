using System.Reflection;
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
    
    public readonly Dictionary<string, string> CommandToModule = new Dictionary<string, string>();

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
        if (!message.HasStringPrefix(Prefix, ref pos, StringComparison.InvariantCultureIgnoreCase))
            return;
         
        string command = message.Content[2..];
        if (CommandToModule.TryGetValue(command.ToLower(), out var moduleName))
        {
            // TODO(CALCO): finish this. (Get user form database and check permissions.
            var user = await _bot.DatabaseClient.Users.Select($"id = {message.Author.Id}").Single();
            
            _logger.Debug(user.Rank);
            
            // if (moduleName == "AdminModule" )
            
            var context = new SocketCommandContext(_bot.Client, message);
            var result = await CommandService.ExecuteAsync(context, pos, _serviceProvider);

            _logger.Info("Received command: {0}", command);
        
            if (!result.IsSuccess)
            {
                _logger.Warn("Running command failed: {0}", result.ErrorReason);
            }

            return;
        }
         
        _logger.Info("Received unknown command: {0}", message.ToString()[2..]);
    }

    public async Task InitCommands()
    {
        await CommandService.AddModuleAsync<InfoModule>(_serviceProvider);
        await CommandService.AddModuleAsync<AdminModule>(_serviceProvider);

        BuildCommandsToModules();

        _bot.Client.MessageReceived += HandleCommandAsync;
        
        _logger.Info("Initialised commands.");
    }

    private void BuildCommandsToModules()
    {
        var assembly = Assembly.GetAssembly(typeof(InfoModule));
        if (assembly == null) return;
        
        var types = assembly.GetTypes()
            .Where(type => type.IsClass && type.IsSubclassOf(typeof(ModuleBase<SocketCommandContext>)));
            
        foreach (var type in types)
        {
            foreach (var method in type.GetMethods())
            {
                if (method.Name == "get_Context")
                    break;
                    
                CommandToModule.Add(method.Name.ToLower(), type.Name.ToLower());
            }
        }
    }
}