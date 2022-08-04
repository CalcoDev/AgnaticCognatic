﻿using System.ComponentModel;
using AgnaticCognaticBot.Database.Models;
using Discord.Commands;
using NLog;

namespace AgnaticCognaticBot.Commands.Modules;

public class AdminModule : ModuleBase<SocketCommandContext>
{
    private readonly Bot _bot;

    private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

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
    
    [Command("db", RunMode = RunMode.Async)]
    [Description("Testing db.")]
    public async Task TestDb()
    {
        var guild = new Guild()
        {
            CreatedAt = DateTime.Now,
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
}