using System.Text;
using Discord;
using Discord.Commands;

namespace AgnaticCognaticBot.Commands.Modules;

[Summary("Miscellaneous commands for fun and profit.")]
public class FunModule : ModuleBase<SocketCommandContext>
{
    private readonly Random _random = new Random(42);
    
    [Command("roll")]
    [Summary("Roll X dice with Y sides.\nFormat: `roll XdY`")]
    public async Task RollDice(string dice)
    {
        var split = dice.Split("d");
        int x = int.Parse(split[0]);
        int y = int.Parse(split[1]);
        
        var rolls = new StringBuilder();
        var sum = 0;
        
        for (var i = 0; i < x; i++)
        {
            var roll = _random.Next(1, y + 1);
            rolls.Append($"{roll}, ");
            sum += roll;
        }

        var embed = new EmbedBuilder();

        // embed.WithTitle(Context.User.Username);
        embed.WithColor(Bot.EmbedColour);
        embed.WithAuthor("Calcopod", Context.User.GetAvatarUrl());
        embed.AddField($"Rolled {x}d{y}: {sum}", rolls.ToString()[..^2]);

        await ReplyAsync("", false, embed.Build());
    }
}