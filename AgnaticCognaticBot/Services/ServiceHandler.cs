using Microsoft.Extensions.DependencyInjection;

namespace AgnaticCognaticBot.Services;

public class ServiceHandler
{
    public readonly IServiceProvider ServiceProvider;

    public ServiceHandler(Bot bot)
    {
        var map = new ServiceCollection()
            .AddSingleton(bot);

        ServiceProvider = map.BuildServiceProvider();
    }
}