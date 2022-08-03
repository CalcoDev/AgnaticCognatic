using Discord.Commands;

namespace AgnaticCognaticBot.Commands;

public class InfoModule : ModuleBase<SocketCommandContext>
{
    [Command("ping", RunMode = RunMode.Sync)]
    [Summary("Says pong!")]
    public Task PingPong()
    {
        return Context.Channel.SendMessageAsync("Pong!");
    }
    
    [Command("the last stand", RunMode = RunMode.Sync)]
    [Summary("FOR THE GRACE FOR THE MIGHT OF OUR LORD")]
    public Task TheLastStand()
    {
        return Context.Channel.SendMessageAsync("FOR THE GRACE FOR THE MIGHT OF OUR LORD");
    }
}