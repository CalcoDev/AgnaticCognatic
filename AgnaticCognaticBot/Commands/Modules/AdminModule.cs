using System.ComponentModel;
using AgnaticCognaticBot.Database.Models;
using Discord.Commands;
using NLog;

namespace AgnaticCognaticBot.Commands.Modules;

public class AdminModule : CommandModuleBase
{
    public override int MinimumRequiredRank { get; } = 2;
    
    private readonly Bot _bot;

    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public AdminModule(Bot bot)
    {
        _bot = bot;
    }

    [Command("stop", RunMode = RunMode.Async)]
    [Description("Stops the bot")]
    public async Task Stop()
    {
        await Context.Channel.SendMessageAsync("Shutting down...");
        _bot.StopClient();
    }
    
    [Command("db", RunMode = RunMode.Async)]
    [Description("Testing db.")]
    public async Task TestDb()
    {
        var guild = new Guild()
        {
            DiscordUid = Bot.TestGuildId
        };

        try
        {
            await _bot.DatabaseClient.Guilds.Insert(guild);
        }
        catch (Exception e)
        {
            _logger.Error("Error inserting guild into database: " + e.Message);
        }

        await Context.Channel.SendMessageAsync("Added thing to database. (Hopefully ....)");
    }

    [Command("registerSelf", RunMode = RunMode.Async)]
    [Description("Register yourself as an admin.")]
    public async Task RegisterSelf()
    {
        var user = new User()
        {
            Rank = 2,
            AdminGuilds = new[] { (decimal)Bot.TestGuildId },
            DiscordUid = Context.User.Id
        };
        
        try
        {
            await _bot.DatabaseClient.Users.Insert(user);
        }
        catch (Exception e)
        {
            _logger.Error("Error inserting user into database: " + e.Message);
        }
    }
}