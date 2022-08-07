using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public readonly Dictionary<string, int> ModuleToRank = new Dictionary<string, int>();

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
            var requiredRank = ModuleToRank[moduleName];
            if (rank < requiredRank)
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
        var assembly = Assembly.GetAssembly(typeof(CommandHandler));
        if (assembly == null) return;

        var moduleBaseType = typeof(ModuleBase<SocketCommandContext>);
        var types = assembly.GetTypes()
            .Where(type => type.IsClass && type.IsSubclassOf(moduleBaseType));
        
        _logger.Info("Searching for command modules...");
        foreach (var type in types)
        {
            var fieldInfo = type.GetProperty("MinimumRequiredRank");
            int minimumRequiredRank = 0;
            if (fieldInfo == null)
                _logger.Warn("Could not find rank field in module {0}. Assuming it to be 0 - Default.", type.Name);
            else
                minimumRequiredRank = (int)(fieldInfo.GetValue(null) ?? 0);
            
            ModuleToRank.Add(type.Name.ToLower(), minimumRequiredRank);
            _logger.Info("Found module {0} with minimum required rank {1}, and the following commands:.", type.Name, minimumRequiredRank);

            foreach (var method in type.GetMethods())
            {
                if (method.Name == "get_Context")
                    break;
                
                var attribData = method.GetCustomAttributesData();

                var commandAttribType = typeof(CommandAttribute);
                var commandDescAttribType = typeof(SummaryAttribute);
                var aliasAttribType = typeof(AliasAttribute);

                string commandName = "";
                string commandDesc = "No description found.";
                string possibleAliases = "";
                bool shouldLog = false;
                // TODO(calco): Should probably do some checks to see if the method is an actual command.
                foreach (var data in attribData)
                {
                    if (data.AttributeType == commandAttribType)
                    {
                        commandName = (string)data.ConstructorArguments[0].Value!; // Name is a required param. 
                        CommandToModule.Add(commandName.ToLower(), type.Name.ToLower());
                        
                        shouldLog = true;
                    }
                    else if (data.AttributeType == commandDescAttribType)
                        commandDesc = (string)data.ConstructorArguments[0].Value!; // Description is a required param.
                    else if (data.AttributeType == aliasAttribType)
                    {
                        if (data.ConstructorArguments[0].Value! is not ReadOnlyCollection<CustomAttributeTypedArgument> aliases) continue;

                        foreach (var alias in aliases)
                        {
                            var actualAliasName = ((string)alias.Value!).ToLower();

                            possibleAliases += $"{actualAliasName} / ";
                            
                            CommandToModule.Add(actualAliasName, type.Name.ToLower());
                        }
                    }
                }

                if (shouldLog)
                {
                    _logger.Info(
                        $"{commandName} {(possibleAliases.Length > 0 ? $"/ {possibleAliases[..^2]}" : "")}- {commandDesc}"
                    );
                }
            }
        }
    }
}