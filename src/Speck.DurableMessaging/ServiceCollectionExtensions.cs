using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Speck.DurableMessaging.Inbox;
using Speck.DurableMessaging.Outbox;

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

        services
            .AddSingleton(configuration.InboxMessageTypeCollection)
            .AddSingleton<InboxMessageFactory>()
            .AddSingleton<MessageSerializer>()
            .AddSingleton<InboxSignals>()
            .AddSingleton<InboxMessageTables>()
            .AddScoped<InboxSignalScope>()
            .AddTransient<IInbox, Inbox.Inbox>();
        
        services
            .AddSingleton(configuration.OutboxMessageTypeCollection)
            .AddSingleton<OutboxMessageFactory>()
            .AddSingleton<MessageSerializer>()
            .AddSingleton<OutboxSignals>()
            .AddSingleton<OutboxMessageTables>()
            .AddScoped<OutboxSignalScope>()
            .AddTransient<IOutbox, Outbox.Outbox>();

        foreach (var inboxConfiguration in configuration.InboxConfigurations)
        {
            services.AddSingleton(inboxConfiguration);
            
            services.AddSingleton<IHostedService>(provider => new InboxPollingService(
                provider,
                inboxConfiguration,
                provider.GetRequiredService<InboxSignals>(),
                provider.GetService<ILogger<InboxPollingService>>()));
        }
        
        foreach (var outboxConfiguration in configuration.OutboxConfigurations)
        {
            services.AddSingleton(outboxConfiguration);
            
            services.AddSingleton<IHostedService>(provider => new OutboxPollingService(
                provider,
                outboxConfiguration,
                provider.GetRequiredService<OutboxSignals>(),
                provider.GetService<ILogger<OutboxPollingService>>()));
        }
        
        return services;
    }
}
