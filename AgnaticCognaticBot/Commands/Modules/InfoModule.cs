using System.ComponentModel;
using System.Text;
using Discord;
using Discord.Commands;
using RunMode = Discord.Commands.RunMode;

namespace AgnaticCognaticBot.Commands.Modules;

[Summary("Give information about stuff lol.")]
public class InfoModule : ModuleBase<SocketCommandContext>
{
    private readonly CommandHandler _commandHandler;
    
    private readonly Color _embedColour = new(57, 147, 237);

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
        embed.WithColor(_embedColour);
        embed.WithTitle("Agnatic Cognatic Bot");
        embed.WithDescription("A bot for the Agnatic Cognatic Discord server");
        
        embed.WithThumbnailUrl("https://i.imgur.com/AW6H3FH.png");
        
        embed.AddField("Author", "Calcopod");
        embed.AddField("Version", "1.0.0");
        
        List<CommandInfo> commandInfos = _commandHandler.CommandService.Commands.ToList();

        embed.AddField("Commands", commandInfos.Count);

        foreach (var module in _commandHandler.CommandService.Modules)
        {
            embed.AddField(module.Name, $"{module.Summary ?? "No description provided"}");
            
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
            x.IconUrl = "https://static.wikia.nocookie.net/warriors/images/a/ab/Crowfeather.SE-11-FC.png/revision/latest?cb=20190128001440";
        });
        
        embed.WithCurrentTimestamp();

        await ReplyAsync("", false, embed.Build());
    }
}