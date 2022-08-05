using System.Reflection;
using AgnaticCognaticBot.Commands.Modules;
using AgnaticCognaticBot.Helpers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NLog;
using Postgrest;

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
            int rank = 0;
            try
            {
                var user = await _bot.DatabaseClient.Users
                    .Filter("discord_uid", Constants.Operator.Equals, "383567751819558932")
                    .Single();
                
                rank = user.Rank;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error getting user rank.");
            }

            var context = new SocketCommandContext(_bot.Client, message);
            if (moduleName == "adminmodule" && rank != 2)
            {
                await context.Channel.SendMessageAsync("You do not have permission to use this command.");
                return;
            }
            
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
                
                var attribData = method.GetCustomAttributesData();
                foreach (var data in attribData)
                {
                    if (data.AttributeType != typeof(CommandAttribute))
                        continue;
                    
                    var commandName = (string) data.ConstructorArguments[0].Value!;
                    CommandToModule.Add(commandName.ToLower(), type.Name.ToLower());
                }
            }
        }
    }
}