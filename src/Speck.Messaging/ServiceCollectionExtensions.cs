using Microsoft.Extensions.DependencyInjection;

namespace Speck.Messaging;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Messaging services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>This.</returns>
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        return services;
    }
}
