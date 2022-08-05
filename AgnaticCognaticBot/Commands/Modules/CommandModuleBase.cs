using Discord.Commands;

namespace AgnaticCognaticBot.Commands.Modules;

public abstract class CommandModuleBase : ModuleBase<SocketCommandContext>
{
    public abstract int MinimumRequiredRank { get; }
}