using AgnaticCognaticBot.Helpers;
using AgnaticCognaticBot.Interactions.Modules;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using NLog;
using IResult = Discord.Interactions.IResult;

namespace AgnaticCognaticBot.Interactions;

public class InteractionHandler
{
    public readonly InteractionService InteractionService;

    private readonly Logger _logger;
    
    private readonly Bot _bot;
    private readonly IServiceProvider _serviceProvider;

    private const ulong TestGuildId = 1000484275818352660;

    public InteractionHandler(Bot bot)
    {
        _logger = NLog.LogManager.GetCurrentClassLogger();
        
        _bot = bot;
        _serviceProvider = bot.ServiceHandler.ServiceProvider;
        
        InteractionService = new InteractionService(bot.Client, new InteractionServiceConfig()
        {
            LogLevel = LogSeverity.Info,
            ThrowOnError = true
        });

        InteractionService.Log += message => Logging.Log(message, _logger);
    }

    public async Task InitInteractions()
    {
        await InteractionService.AddModuleAsync<InfoModule>(_serviceProvider);

        _bot.Client.InteractionCreated += HandleInteraction;
        
        InteractionService.SlashCommandExecuted += SlashCommandExecuted;
        InteractionService.ContextCommandExecuted += ContextCommandExecuted;
        InteractionService.ComponentCommandExecuted += ComponentCommandExecuted;

        bool isDebug = Debugging.IsDebugMode();
        
        if (isDebug)
            await InteractionService.RegisterCommandsToGuildAsync(TestGuildId, true);
        else
            await InteractionService.RegisterCommandsGloballyAsync(true);

        _logger.Info("Initialised interactions {0}.", isDebug ? $"on guild with ID: {TestGuildId}" : "globally");
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_bot.Client, interaction);
            await InteractionService.ExecuteCommandAsync(context, _serviceProvider);
            
            _logger.Info("Recieved interaction: {0}", interaction.Data);
        }
        catch (Exception ex)
        {
            _logger.Error("Running interaction failed: {0}", ex);

            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
    
    private Task SlashCommandExecuted(SlashCommandInfo info, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
        {
            _logger.Warn("Running slash command interaction failed: {0}", result.ErrorReason);
        }

        return Task.CompletedTask;
    }
    
    private Task ContextCommandExecuted(ContextCommandInfo info, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
        {
            _logger.Warn("Running context interaction failed: {0}", result.ErrorReason);
        }
        
        return Task.CompletedTask;
    }

    
    private Task ComponentCommandExecuted(ComponentCommandInfo info, IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
        {
            _logger.Warn("Running component command interaction failed: {0}", result.ErrorReason);
        }
        
        return Task.CompletedTask;
    }
}