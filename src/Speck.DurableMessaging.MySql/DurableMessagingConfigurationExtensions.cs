using Microsoft.Extensions.DependencyInjection;

namespace Speck.DurableMessaging.MySql;

public static class DurableMessagingConfigurationExtensions
{
    /// <summary>
    /// Adds services required for using MySQL for durable messaging.  
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>This.</returns>
    public static DurableMessagingConfiguration UseMySql(this DurableMessagingConfiguration configuration)
    {
        configuration.Services.AddTransient<IInbox, MySqlInbox>();
        
        return configuration;
    }
}