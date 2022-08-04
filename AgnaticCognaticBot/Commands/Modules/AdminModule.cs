using System.ComponentModel;
using AgnaticCognaticBot.Database.Models;
using Discord.Commands;

namespace AgnaticCognaticBot.Commands.Modules;

public class AdminModule : ModuleBase<SocketCommandContext>
{
    private readonly Bot _bot;

    public AdminModule(Bot bot)
    {
        _bot = bot;
    }

    [Command("stop", RunMode = RunMode.Async)]
    [Description("Stops the bot")]
    public async Task StopBot()
    {
        await Context.Channel.SendMessageAsync("Shutting down...");
        _bot.StopClient();
    }
}