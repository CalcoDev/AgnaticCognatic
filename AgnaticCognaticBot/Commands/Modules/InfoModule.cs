using System.ComponentModel;
using System.Text;
using Discord;
using Discord.Commands;
using RunMode = Discord.Commands.RunMode;

namespace AgnaticCognaticBot.Commands.Modules;

[Summary("Commands related to giving information about the bot, Minecraft server and more!")]
public class InfoModule : ModuleBase<SocketCommandContext>
{
    private readonly CommandHandler _commandHandler;
    
    public InfoModule(Bot bot)
    {
        _commandHandler = bot.CommandHandler;
    }

    [Command("info", RunMode = RunMode.Async)]
    [Alias("help", "commands")]
    [Summary("Displays info about the bot")]
    public async Task Info()
    {
        var embed = new EmbedBuilder();
        embed.WithColor(Bot.EmbedColour);
        embed.WithTitle("Agnatic Cognatic Bot");
        embed.WithDescription("A bot for the Agnatic Cognatic Discord server");
        
        embed.WithThumbnailUrl("https://i.imgur.com/AW6H3FH.png");
        
        embed.AddField("Author", "Calcopod");
        embed.AddField("Version", "1.0.0");
        
        foreach (var module in _commandHandler.CommandService.Modules)
        {
            // Dumb way of leaving empty space.
            embed.AddField("\u200b", "\u200b", false);
            embed.AddField(module.Name, $"{module.Summary ?? "No description provided"}", false);
            
            foreach (var command in module.Commands)
            {
                embed.AddField(command.Name, $"{command.Summary?? "No description provided." }\n" +
                                             $"Aliases: {string.Join(", ", command.Aliases)}", true);
            }
        }

        embed.WithFooter(x =>
        {
            x.Text = "Agnatic Cognatic Bot";
            // TODO(calco): Change this to an actual Calcopod icon, and not a cat.
            x.IconUrl = "https://i.imgur.com/x2JUBsd.png";
        });
        
        embed.WithCurrentTimestamp();

        await ReplyAsync("", false, embed.Build());
    }

    [Command("about")]
    [Summary("Shameless plug on the part of the developer...")]
    public async Task PromoteSelf()
    {
        var embed = new EmbedBuilder();

        embed.WithColor(Bot.EmbedColour);
        
        // It seems the cat has been embraced.
        embed.WithAuthor("Calcopod", "https://i.imgur.com/x2JUBsd.png");
        
        embed.WithTitle("Shameless self promotion");
        embed.WithDescription("How dares the developer do such a thing?\nIf you were looking for info about " +
                              "the bot, use `>>info`");
        
        embed.AddField("\u200b", "\u200b", false);
        
        embed.AddField("\u200b", "\u200b", true);
        embed.AddField("Reddit", "https://www.reddit.com/user/Calcopod", true);
        embed.AddField("\u200b", "\u200b", true);

        embed.AddField("\u200b", "\u200b", false);
        
        embed.AddField("Github", "https://github.com/CalcoDev", true);
        embed.AddField("Own website!", "https:/calcopod.dev", true);
        embed.AddField("Twitter", "https://twitter.com/Calcopod2", true);
        
        embed.AddField("\u200b", "\u200b", false);
        
        embed.AddField("\u200b", "\u200b", true);
        embed.AddField("Youtube", "https://www.youtube.com/channel/UCXIJscEzuyeNP1PQF7NrqCg", true);
        embed.AddField("\u200b", "\u200b", true);

        embed.WithCurrentTimestamp();
        embed.WithFooter(x =>
        {
            x.Text = "Agnatic Cognatic Bot";
            x.IconUrl = "https://i.imgur.com/x2JUBsd.png";
        });

        await ReplyAsync("", false, embed.Build());
    }
}