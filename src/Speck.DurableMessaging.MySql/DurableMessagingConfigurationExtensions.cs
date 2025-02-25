using Microsoft.Extensions.DependencyInjection;
using Speck.DurableMessaging.Common;
using Speck.DurableMessaging.Inbox;

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
        configuration.Services.AddTransient<IInboxMessageRepository, MySqlInboxMessageRepository>();
        configuration.Services.AddTransient<IUnitOfWork, MySqlUnitOfWork>();
        
        return configuration;
    }
}