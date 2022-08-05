using Discord.Interactions;

namespace AgnaticCognaticBot.Interactions.Modules;

public class InfoModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("lmao", "says lmao")]
    public async Task Lmao()
    {
        await RespondAsync("lmao");
    }
}