using Microsoft.Extensions.DependencyInjection;

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
        configure(new DurableMessagingConfiguration(services));
        
        return services;
    }
}
