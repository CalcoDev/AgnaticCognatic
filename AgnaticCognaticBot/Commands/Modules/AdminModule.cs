using System.ComponentModel;
using AgnaticCognaticBot.Database.Models;
using Discord.Commands;
using NLog;

namespace AgnaticCognaticBot.Commands.Modules;

[Summary("Commands for bot management. Only acccessible to those deemed worthy.")]
public class AdminModule : ModuleBase<SocketCommandContext>
{
    public static int MinimumRequiredRank { get; } = 2;
    
    private readonly Bot _bot;

    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public AdminModule(Bot bot)
    {
        _bot = bot;
    }

    [Command("stop", RunMode = RunMode.Async)]
    [Summary("Stops the bot")]
    public async Task Stop()
    {
        await Context.Channel.SendMessageAsync("Shutting down...");
        _bot.StopClient();
    }
}