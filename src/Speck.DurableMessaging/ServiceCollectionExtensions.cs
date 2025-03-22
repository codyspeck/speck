using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Speck.DurableMessaging.Mailbox;

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
            .AddSingleton(configuration.MailboxMessageTypeCollection)
            .AddSingleton<MailboxMessageFactory>()
            .AddSingleton<MessageSerializer>()
            .AddSingleton<MailboxSignals>()
            .AddSingleton<MailboxMessageTables>()
            .AddScoped<MailboxSignalScope>()
            .AddTransient<IMailbox, Mailbox.Mailbox>();

        foreach (var mailboxConfiguration in configuration.MailboxConfigurations)
        {
            services.AddSingleton(mailboxConfiguration);
            
            services.AddSingleton<IHostedService>(provider => new MailboxPollingService(
                provider,
                mailboxConfiguration,
                provider.GetRequiredService<MailboxSignals>(),
                provider.GetService<ILogger<MailboxPollingService>>()));
        }
        
        return services;
    }
}
