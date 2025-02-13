using Microsoft.Extensions.DependencyInjection;
using Speck.DurableMessaging.Inbox;

namespace Speck.DurableMessaging;

public class DurableMessagingConfiguration
{
    public IServiceCollection Services { get; }
    
    internal InboxMessageRegistry InboxMessageRegistry { get; } = new();

    internal List<InboxConfiguration> InboxConfigurations { get; } = [];

    internal DurableMessagingConfiguration(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Adds an inbox. Multiple inboxes can be configured if each one has a uniquely configured inbox message table.
    /// </summary>
    /// <param name="configure">Configures the inbox configuration.</param>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddInbox()
    {
        return AddInbox(_ => { });
    }
    
    /// <summary>
    /// Adds an inbox. Multiple inboxes can be configured if each one has a uniquely configured inbox message table.
    /// </summary>
    /// <param name="configure">Configures the inbox configuration.</param>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddInbox(Action<InboxConfiguration> configure)
    {
        var configuration = new InboxConfiguration();
        
        configure(configuration);
        
        InboxConfigurations.Add(configuration);
        
        return this;
    }
    
    /// <summary>
    /// Configures an <see cref="IInboxMessageHandler{TMessage}"/>. The handler will be registered as a transient
    /// service.
    /// </summary>
    /// <typeparam name="THandler">The type of the inbox message handler to configure.</typeparam>
    /// <typeparam name="TMessage">The type of the inbox message.</typeparam>
    /// <returns>This.</returns>
    public DurableMessagingConfiguration AddInboxMessageHandler<THandler, TMessage>(string messageType)
        where THandler : class, IInboxMessageHandler<TMessage>
    {
        InboxMessageRegistry.Add<TMessage>(messageType);
        
        Services.AddTransient<IInboxMessageHandler<TMessage>, THandler>();
        
        return this;
    }
}
