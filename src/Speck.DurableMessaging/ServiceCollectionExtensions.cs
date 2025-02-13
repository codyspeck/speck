using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Speck.DurableMessaging.Inbox;

namespace Speck.DurableMessaging;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds durable messaging services.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">The action used to configure the services.</param>
    /// <returns>This.</returns>
    public static IServiceCollection AddDurableMessaging(
        this IServiceCollection services,
        Action<DurableMessagingConfiguration> configure)
    {
        var configuration = new DurableMessagingConfiguration(services);
        
        configure(configuration);

        services.AddSingleton(configuration.InboxMessageRegistry);

        foreach (var inboxConfiguration in configuration.InboxConfigurations)
        {
            services.AddSingleton<IHostedService>(provider => new InboxPollingService(provider, inboxConfiguration));
        }
        
        return services;
    }
}
